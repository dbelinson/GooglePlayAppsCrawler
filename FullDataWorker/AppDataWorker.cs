using MongoDB.Driver.Builders;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.MongoDB;
using SharedLibrary.Reviews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebUtilsLib;

namespace FullDataWorker
{
    class AppDataWorker
    {
        // Control Variables
        private static int _maxRetries = 5;

        // Inner Dictionary of App Data
        private static Dictionary<String, AppStatusModel> _appStatus;

        static void Main (string[] args)
        {
            // Checking for Input Parameters
            if (args == null || args.Length != 1)
            {
                Console.WriteLine ("Incorrect number of arguments received. Expected One");
                System.Environment.Exit (-100);
            }

            // Human Readable Variable
            string inputFile = args[0];

            // Checking if the Input file received exists
            if (!File.Exists (inputFile))
            {
                Console.WriteLine (String.Format("Received input file does not exist : {0}", inputFile));
                System.Environment.Exit (-101);
            }

            // App Status 
            _appStatus = new Dictionary<String, AppStatusModel> ();

            // Creating Instance of Database Manager
            MongoDBWrapper mongoDB   = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            // Creating Instance of Parser
            PlayStoreParser dataParser = new PlayStoreParser ();

            goto PeopleData;

            using (WebRequests httpClient = new WebRequests ())
            {
                // Minor Configuration of the Http Client - Ensures that the requests response will be in english
                // By doing so, we have no problems parsing the dates to their proper formats
                httpClient.Headers.Add (Consts.ACCEPT_LANGUAGE);
                httpClient.Host     = Consts.HOST;
                httpClient.Encoding = "utf-8";
                httpClient.EncodingDetection = WebRequests.CharsetDetection.DefaultCharset;

                // Iterating over File Lines (App Urls) - To Extract Data, Not The Reviews Yet.
                foreach (string appUrl in File.ReadAllLines (inputFile))
                {
                    // Logging Progress
                    Console.WriteLine ("\n => Processing App : " + appUrl);

                    // Executing Http Get Request for the Apps's Data - With max of 5 Retries
                    String appDataResponse = String.Empty;
                    int currentRetry       = 0;

                    do
                    {
                        // Http Get
                        appDataResponse = httpClient.Get (appUrl);

                    } while (String.IsNullOrWhiteSpace(appDataResponse) || ++currentRetry <= _maxRetries);

                    // Sanity Check
                    if (String.IsNullOrWhiteSpace (appDataResponse))
                    {
                        Console.WriteLine ("\t\t.Error - Failed to find page of app : " + appUrl + ". Skipping it");
                        continue;
                    }

                    Console.WriteLine("\t\t.Page Found. Firing Parser");

                    // Parsing App Data
                    AppModel appData = dataParser.ParseAppPage (appDataResponse, appUrl);

                    // Checking If this app is on the database already
                    if (mongoDB.AppProcessed (appUrl))
                    {
                        Console.WriteLine ("\t\t.Previous Version of App Found. Updating It");
                        mongoDB.UpdateRecord (appData, "Url", appData.Url);

                        // Updating App Status
                        _appStatus.Add
                            (
                                appData.Url,
                                new AppStatusModel ()
                                {
                                    appId   = appData.Url.Replace (Consts.PLAY_STORE_PREFIX, String.Empty),
                                    appUrl  = appData.Url,
                                    appName = appData.Name,
                                    status  = "Updated"
                                }
                            );
                    }
                    else
                    {
                        Console.WriteLine ("\t\t.No Previous Version of the App Found. Adding to Database");
                        mongoDB.Insert<AppModel> (appData);

                        // Updating App Status
                        _appStatus.Add
                            (
                                appData.Url,
                                new AppStatusModel ()
                                {
                                    appId   = appData.Url.Replace (Consts.PLAY_STORE_PREFIX, String.Empty),
                                    appUrl  = appData.Url,
                                    appName = appData.Name,
                                    status  = "Inserted"
                                }
                            );
                    }
                }
            }

            Reviews:
            // Next Phase: Parse Reviews of those Apps
            Console.WriteLine ("\n => Parsing Complete. Obtaining Reviews");

            // Iterating again over app urls to parse the reviews from this app
            foreach (string appUrl in File.ReadAllLines (inputFile))
            {
                // Reaching App Id
                string appID = _appStatus[appUrl].appId;

                // Reviews-Break-Parsing Flag
                bool shouldContinueParsing = true;

                // Parsing Review Pages from the apps
                for (int currentPage = 1; /* no stop condition */; currentPage++)
                {
                    // Getting Reviews Data Bundle
                    string reviewsData = ReviewsWrapper.GetAppReviews (appID, currentPage);

                    // Checking for Blocking Situation
                    if (String.IsNullOrEmpty (reviewsData))
                    {
                        Console.WriteLine("Blocked by Play Store. Sleeping process for 10 minutes before retrying.");

                        // Thread Wait for 10 Minutes
                        Thread.Sleep (10 * 60 * 1000);
                    }

                    // Checking for "No Reviews" app
                    if (reviewsData.Length < 50)
                    {
                        Console.WriteLine ("No Reviews left for this app. Skipping");
                        break;
                    }

                    // Normalizing Response to Proper HTML
                    reviewsData = ReviewsWrapper.NormalizeResponse (reviewsData);

                    // Iterating over Parsed Reviews
                    foreach (var review in dataParser.ParseReviews (reviewsData))
                    {
                        // Adding App Data to the review
                        review.appID   = _appStatus[appUrl].appId;
                        review.appName = _appStatus[appUrl].appName;
                        review.appURL  = _appStatus[appUrl].appUrl;

                        // Incrementing Reviews Count for this app
                        _appStatus[appUrl].reviews++;

                        // Adding Review Object to Database
                        review.timestamp = DateTime.Now;

                        // Building Query to check for duplicated review
                        var duplicatedReviewQuery = Query.EQ ("permalink", review.permalink);

                        // Checking for duplicated review before inserting it
                        if (mongoDB.FindMatch<AppReview> (duplicatedReviewQuery, 1, 0, Consts.REVIEWS_COLLECTION).Count () == 0)
                        {
                            // Inserting Review into MongoDB
                            mongoDB.Insert<AppReview> (review, Consts.REVIEWS_COLLECTION);
                        }
                        else
                        {
                            Console.WriteLine ("Duplicated Review. Skipping App");

                            // When this happens, there are no more reviews to be parsed
                            shouldContinueParsing = false; // Skipping this apps processing
                        }
                    }

                    // Hiccup to avoid Blocking problems
                    Console.WriteLine ("Parsed Reviews: " + _appStatus[appUrl].reviews);
                    Thread.Sleep (new Random ().Next (14000, 21000));

                    if (!shouldContinueParsing)
                    {
                        break;
                    }
                }
            }

            PeopleData:

            Console.WriteLine ("\n\n => Processing People Data");

            Console.WriteLine ("\nSimulating Google Login Using Selenium.");
            using (var firefoxDriver = new FirefoxDriver ())
            {
                // Navigating to Dummy Url - One that I Know that well be asked for a login
                firefoxDriver.Navigate ().GoToUrl ("https://play.google.com/store/people/details?id=101242565951396343093");

                // Reaching Login Fields
                var loginField    = firefoxDriver.FindElementById ("Email");
                var passwordField = firefoxDriver.FindElementById ("Passwd");
                var btnSignIn     = firefoxDriver.FindElementById ("signIn");

                // Sending Credentials to the browser
                loginField.SendKeys ("YOUREMAIL");
                passwordField.SendKeys ("YOURPASSWORD");
                btnSignIn.Click ();

                string lastPeople = "https://play.google.com/store/people/details?id=115037241907660526856";
                bool shouldcontinue = false;

                // Processing Reviewers Data
                foreach (string peopleUrl in mongoDB.FindPeopleUrls ())
                {
                    // Skipping until last link
                    if (peopleUrl == lastPeople)
                    {
                        shouldcontinue = true;
                    }

                    if (!shouldcontinue) continue;

                    // Navigating To the Reviewer Page
                    firefoxDriver.Navigate ().GoToUrl (peopleUrl);

                    // Executing Get Request for the Reviewer page on Google Play
                    string reviewerPage = firefoxDriver.PageSource;

                    // Extracting Reviewer Data from the Page
                    ReviewerPageData reviewerData = dataParser.ParsePeopleData (reviewerPage);

                    // Adding Url to the model
                    reviewerData.reviewerUrl = peopleUrl;

                    // Inserting it to the database - If no previous record of this Reviewer is found
                    if (!mongoDB.IsReviewerOnDatabase (peopleUrl))
                    {
                        mongoDB.Insert<ReviewerPageData> (reviewerData, "ReviewersData");
                    }
                }
            }

            // End of Processing + Console Feedback
            Console.WriteLine ("\n\n == Processing Summary ==");

            foreach (var status in _appStatus.Select (t => t.Value))
            {
                // Message
                string cMessage = "=> App : {0} - Status {1} - Reviews : {2}";

                Console.WriteLine (String.Format (cMessage, status.appName, status.status, status.reviews));
            }

            Console.ReadLine ();
        }
    }
}
