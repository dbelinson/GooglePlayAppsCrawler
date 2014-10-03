using BDC.BDCCommons;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebUtilsLib;

namespace ReviewsParser
{
    /* THIS CONSOLE APPLICATION IS JUST A BASIC SCRAPER FOR GOOGLE PLAY STORE REVIEWS OF APPS.
     * THIS LAYER IS NOT INCLUDED ON THE AUTOMATED FLOW OF THE CRAWLER, AND SHOULD BE USED FOR SMALL
     * SCALE PARSING OF REVIEWS. 
     * 
     * ReviewsWorker used the existing database of apps to get the "Id's" of the apps of interest, and parse out their reviews.
     * This project is only a Proof of Concept (POC) and it is not ready for "Prime-Time" / "Full Scale" usage.
     */

    public class Program
    {
        // Arguments Quick Documentation:
        // args[0] = number of apps to be processed (int)
        // args[1] = number of pages of reviews to be fetched per app (each page contains 20 reviews)
        private static Dictionary<String, int> _arguments = new Dictionary<String, int> ();

        static void Main (string[] args)
        {
            // Configuring Log Object Threshold
            LogWriter.Threshold = TLogEventLevel.Information;
            LogWriter.LogEvent  += LogWriter_LogEvent;

            // Parsing Arguments
            LogWriter.Info ("Checking for Arguments");

            if (args == null || args.Length != 3)
            {
                LogWriter.Fatal ("Arguments Fatal", "Incorrect number of arguments received. Try passing two.");
                return; // Halts.
            }

            LogWriter.Info ("Reading Arguments");

            // Reading actual arguments received
            _arguments.Add ("AppsToProcess", Int32.Parse (args[0]));
            _arguments.Add ("ReviewsPagePerApp", Int32.Parse (args[1]));
            _arguments.Add ("AppsToSkip", Int32.Parse (args[2]));

            // Building MongoDB Query - This query specifies which applications you want to parse out the reviews
            // For more regarding MongoDB Queries, check the documentation on the project wiki page
            //var mongoQuery = Query.EQ ("Instalations", "1,000,000 - 5,000,000");
            var mongoQuery = Query.EQ ("Category", "/store/apps/category/EDUCATION");

            LogWriter.Info ("Configuring MonboDB Client");

            // Creating instance of Mongo Handler for the main collection
            MongoDBWrapper mongoClient = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoClient.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            LogWriter.Info ("Iterating over Apps");

            // App URL Prefix (must be removed in order to obtain the app ID)
            string playStorePrefix = "https://play.google.com/store/apps/details?id=";

            // Creating Play Store Parser
            PlayStoreParser parser = new PlayStoreParser ();

            // Iterating over Query Results for the App Ids
            foreach (var appRecord in mongoClient.FindMatch<AppModel>(mongoQuery, _arguments["AppsToProcess"], _arguments["AppsToSkip"]))
            {
                // Extracting app ID from URL
                string appId = appRecord.Url.Replace(playStorePrefix, String.Empty);

                // Console Feedback
                LogWriter.Info("Processing App [ " + appRecord.Name + " ] ");

                bool shouldSkipApp = false;

                // Iterating over Review Pages up to the max received as argument
                for (int currentPage = 1; currentPage <= _arguments["ReviewsPagePerApp"]; currentPage++)
                {
                    // Checking for the need to skip this app in case of duplicated review
                    if (shouldSkipApp)
                        break;

                    try
                    {
                        // Page Feedback
                        LogWriter.Info("\tCurrent Page: " + currentPage);

                        // Issuing Request for Reviews
                        string response = GetAppReviews(appId, currentPage);

                        // Checking for Blocking Situation
                        if (String.IsNullOrEmpty(response))
                        {
                            LogWriter.Info("Blocked by Play Store. Sleeping process for 10 minutes before retrying.");

                            // Thread Wait for 10 Minutes
                            Thread.Sleep(10 * 60 * 1000);
                        }

                        // Checking for "No Reviews" app
                        if (response.Length < 50)
                        {
                            LogWriter.Info("No Reviews for this app. Skipping");
                            break;
                        }

                        // Normalizing Response to Proper HTML
                        response = NormalizeResponse(response);

                        // Iterating over Parsed Reviews
                        foreach (var review in parser.ParseReviews(response))
                        {
                            // Adding App Data to the review
                            review.appID = appId;
                            review.appName = appRecord.Name;
                            review.appURL = appRecord.Url;

                            // Adding processing timestamp to the model
                            review.timestamp = DateTime.Now;

                            // Building Query to check for duplicated review
                            var duplicatedReviewQuery = Query.EQ("permalink", review.permalink);

                            // Checking for duplicated review before inserting it
                            if (mongoClient.FindMatch<AppReview>(duplicatedReviewQuery, 1, 0, Consts.REVIEWS_COLLECTION).Count() == 0)
                            {
                                // Inserting Review into MongoDB
                                mongoClient.Insert<AppReview>(review, Consts.REVIEWS_COLLECTION);
                            }
                            else
                            {
                                LogWriter.Info("Duplicated Review", "Review already parsed. Skipping App");
                                //shouldSkipApp = true;
                                //break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Error(ex);
                    }
                }
            }
        }

        private static string GetAppReviews (string appID, int reviewsPage)
        {
            // Creating Instance of HTTP Requests Handler
            using (WebRequests httpClient = new WebRequests ())
            {
                // Configuring Request Object
                httpClient.Host              = Consts.HOST;
                httpClient.Origin            = Consts.ORIGIN;
                httpClient.Encoding          = "utf-8";
                httpClient.AllowAutoRedirect = true;
                httpClient.Accept            = "*/*";
                httpClient.UserAgent         = Consts.USER_AGENT;
                httpClient.ContentType       = "application/x-www-form-urlencoded;charset=UTF-8";
                httpClient.EncodingDetection = WebRequests.CharsetDetection.DefaultCharset;
                httpClient.Headers.Add (Consts.ACCEPT_LANGUAGE);

                // Assembling Post Data
                string postData = String.Format (Consts.REVIEWS_POST_DATA, reviewsPage, appID);

                // Issuing Request
                return httpClient.Post (Consts.REVIEWS_URL, postData);
            }
        }

        private static string NormalizeResponse (string jsonResponse)
        {
            // Replacing invalid characters with valid ones to ensure HTML correct formation
            string validHTML = jsonResponse.Replace ("\\u003c", "<").Replace ("\\u003d", "=").Replace ("\\u003e", ">")
                                           .Replace ("\\u0026amp;", "&").Replace (@"\""", @"""");

            // Removing HTML Garbage
            validHTML = validHTML.Substring  (validHTML.IndexOf ("<div class="));

            return validHTML;
        }

        // Redirects the Logging Events to the console instead of an log file
        static void LogWriter_LogEvent (TLogMessage msg)
        {
            Console.WriteLine(msg.EVT_MESSAGE);
        }
    }
}
