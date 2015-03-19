using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using NLog;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.MongoDB;
using SharedLibrary.Reviews;
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
            // Configuring Log Object
            Logger logger = LogManager.GetCurrentClassLogger ();

            // Parsing Arguments
            logger.Info ("Checking for Arguments");

            if (args == null || args.Length != 3)
            {
                logger.Fatal ("Arguments Fatal", "Incorrect number of arguments received. Try passing two.");
                return; // Halts.
            }

            logger.Info ("Reading Arguments");

            // Reading actual arguments received
            _arguments.Add ("AppsToProcess", Int32.Parse (args[0]));
            _arguments.Add ("ReviewsPagePerApp", Int32.Parse (args[1]));
            _arguments.Add ("AppsToSkip", Int32.Parse (args[2]));

            // Building MongoDB Query - This query specifies which applications you want to parse out the reviews
            // For more regarding MongoDB Queries, check the documentation on the project wiki page
            //var mongoQuery = Query.EQ ("Instalations", "1,000,000 - 5,000,000");
            var mongoQuery = Query.EQ ("Category", "/store/apps/category/EDUCATION");

            logger.Info ("Configuring MonboDB Client");

            // Creating instance of Mongo Handler for the main collection
            MongoDBWrapper mongoClient = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoClient.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            logger.Info ("Iterating over Apps");

            // Creating Play Store Parser
            PlayStoreParser parser = new PlayStoreParser ();

            // Iterating over Query Results for the App Ids
            foreach (var appRecord in mongoClient.FindMatch<AppModel>(mongoQuery, _arguments["AppsToProcess"], _arguments["AppsToSkip"]))
            {
                // Extracting app ID from URL
                string appId = appRecord.Url.Replace(Consts.PLAY_STORE_PREFIX, String.Empty);

                // Console Feedback
                logger.Info ("Processing App [ " + appRecord.Name + " ] ");

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
                        logger.Info ("\tCurrent Page: " + currentPage);

                        // Issuing Request for Reviews
                        string response = ReviewsWrapper.GetAppReviews (appId, currentPage);

                        // Checking for Blocking Situation
                        if (String.IsNullOrEmpty(response))
                        {
                            logger.Info ("Blocked by Play Store. Sleeping process for 10 minutes before retrying.");

                            // Thread Wait for 10 Minutes
                            Thread.Sleep(10 * 60 * 1000);
                        }

                        // Checking for "No Reviews" app
                        if (response.Length < 50)
                        {
                            logger.Info ("No Reviews for this app. Skipping");
                            break;
                        }

                        // Normalizing Response to Proper HTML
                        response = ReviewsWrapper.NormalizeResponse (response);

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
                                logger.Info ("Duplicated Review", "Review already parsed. Skipping App");
                                //shouldSkipApp = true;
                                //break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error (ex);
                    }
                }
            }
        }
    }
}
