using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using NLog;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.MongoDB;
using SharedLibrary.Proxies;
using SharedLibrary.Reviews;
using System;
using System.Collections.Generic;
using System.IO;
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

    public class ReviewWorker
    {
        static void Main (string[] args)
        {
            // Configuring Log Object
            Logger logger = LogManager.GetCurrentClassLogger ();

            // Control Variable (Bool - Should the process use proxies? )
            bool isUsingProxies = false;

            // Checking for the need to use proxies
            if (args != null && args.Length == 1)
            {
                // Setting flag to true
                isUsingProxies = true;

                // Loading proxies from .txt received as argument
                String fPath = args[0];

                // Sanity Check
                if (!File.Exists (fPath))
                {
                    logger.Fatal ("Couldnt find proxies on path : " + fPath);
                    System.Environment.Exit (-100);
                }

                // Reading Proxies from File
                string[] fLines = File.ReadAllLines (fPath, Encoding.GetEncoding ("UTF-8"));

                try
                {
                    // Actual Load of Proxies
                    ProxiesLoader.Load (fLines.ToList ());
                }
                catch (Exception ex)
                {
                    logger.Fatal (ex);
                    System.Environment.Exit (-101);
                }
            }
            
            // MongoDB instance Creation
            logger.Info ("Configuring MonboDB Client");

            // Creating instance of Mongo Handler for the main collection
            MongoDBWrapper mongoClient = new MongoDBWrapper ();
            string fullServerAddress   = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoClient.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            logger.Info ("Iterating over Apps");

            // Creating Play Store Parser
            PlayStoreParser parser = new PlayStoreParser ();
            
            // App Model
            AppModel appRecord;

            // Control Variable
            bool noError = true;

            // Finding all the "Apps" that didn't have the reviews visited yet
            while ((appRecord = mongoClient.FindAndModifyReviews ()) != null)
            {
                // Extracting app ID from URL
                string appId = appRecord.Url.Replace (Consts.PLAY_STORE_PREFIX, String.Empty);

                // Console Feedback
                logger.Info ("Processing App [ " + appRecord.Name + " ] ");
                
                try
                {
                    // Issuing Request for Reviews
                    string response = ReviewsWrapper.GetAppReviews (appId, 1, isUsingProxies);

                    // Checking for Blocking Situation
                    if (String.IsNullOrEmpty(response))
                    {
                        logger.Info ("Blocked by Play Store. Sleeping process for 10 minutes before retrying.");

                        // Thread Wait for 10 seconds
                        Thread.Sleep (TimeSpan.FromSeconds (10));
                    }

                    // Checking for "No Reviews" app
                    if (response.Length < 50)
                    {
                        logger.Info ("No Reviews for this app. Skipping");
                        continue;
                    }

                    // Normalizing Response to Proper HTML
                    response = ReviewsWrapper.NormalizeResponse (response);

                    // List of Reviews
                    List<AppReview> reviews = new List<AppReview> ();

                    // Iterating over Parsed Reviews
                    foreach (var review in parser.ParseReviews (response))
                    {
                        // Adding App Data to the review
                        review.appID     = appId;
                        review.appName   = appRecord.Name;
                        review.appURL    = appRecord.Url;

                        // Capture Timestamp to the model
                        review.timestamp = DateTime.Now;

                        // Adding reviews to the current list
                        reviews.Add (review);
                    }

                    // Any Review Found ?
                    if (reviews.Count > 0)
                    {
                        // Checking if there was any previous list of reviews
                        if (appRecord.Reviews == null)
                        {
                            appRecord.Reviews = reviews;
                        }
                        else // Previous List found - Appending only the new ones
                        {
                            foreach (var review in reviews)
                            {
                                if (!appRecord.Reviews.Any (t => t.permalink.Equals (review.permalink)))
                                {
                                    appRecord.Reviews.Add (review);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error (ex);
                    noError = false;
                }
                finally
                {
                    // Toggling back the "ReviewsStatus" attribute from the model
                    if (noError)
                    {
                        appRecord.ReviewsStatus = "Visited";
                        mongoClient.SaveRecord<AppModel> (appRecord);
                    }
                    else // "Error" status
                    {
                        appRecord.ReviewsStatus = "Error";
                        mongoClient.SaveRecord<AppModel> (appRecord);
                    }
                }
            }
        }
    }
}
