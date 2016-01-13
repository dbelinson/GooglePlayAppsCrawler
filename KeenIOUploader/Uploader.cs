using Keen.Core;
using MongoDB.Driver.Builders;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.MongoDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeenIOUploader
{
    /*
     * ALL THE ANALYTICS OF THIS CRAWLER PROJECT ARE POWERED BY KEEN.IO 
     * This "Uploader" Project is a simple guide, wrote to teach you how to
     * upload your own Data to a Keen.IO project.
     * 
     * This code can be used by you, to upload your own dataset to your own Keen.IO
     * project, but you will have to provide your own set of keys. 
     * 
     * For more information about Keen.IO and their participation on this project,
     * please refer to : https://github.com/MarcelloLins/GooglePlayAppsCrawler/wiki/Analytics-Playground
     * 
     */
    class Uploader
    {
        private static string _keenIOProjectID;
        private static string _keenIOMasterKey;
        private static string _keenIOWriteKey;
        private static string _keenIOReadKey;
        private static string _bucketName;

        private static int    _appsCounter = 0;


        private static int SendEventsToKeep(Keen.Core.KeenClient keenClient, List<AppModel> eventsToSend, MongoDBWrapper mongoDB)
        {
            try
            {
                // Adding Event to Keen.IO
                keenClient.AddEvents("PlayStore2014", eventsToSend);

                // Incrementing Counter
                _appsCounter += eventsToSend.Count;

                // Console feedback Every 100 Processed Apps
                if (_appsCounter % 100 == 0)
                {
                    Console.WriteLine("Uploaded : " + _appsCounter);
                }

                foreach (var e in eventsToSend)
                {
                    mongoDB.SetUpdated(e.Url);
                }

                return eventsToSend.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\t" + ex.Message);
            }

            return 0;
        }

        static void Main (string[] args)
        {
            // Loading Keen.IO Keys and Misc. from Config File
            _keenIOProjectID = ConfigurationManager.AppSettings["keenIOProjectID"];
            _keenIOMasterKey = ConfigurationManager.AppSettings["keenIOMasterKey"];
            _keenIOWriteKey  = ConfigurationManager.AppSettings["keenIOWriteKey"];
            _keenIOReadKey   = ConfigurationManager.AppSettings["keenIOReadKey"];
            _bucketName      = ConfigurationManager.AppSettings["keenIOBucketName"];

            // Configuring MongoDB Wrapper for connection and queries
            MongoDBWrapper mongoDB   = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            // Creating Keen.IO Variables
            var projectSettings = new ProjectSettingsProvider (_keenIOProjectID, _keenIOMasterKey, _keenIOWriteKey, _keenIOReadKey);
            var keenClient      = new KeenClient (projectSettings);

            var eventsToSend = new List<AppModel>();
            long totalProcessed = 0;
            long totalSent = 0;

            DateTime start = DateTime.Now;

            // From This point on, you can change your code to reflect your own "Reading" logic
            // What I've done is simply read the records from the MongoDB database and Upload them to Keen.IO

            // if(args.Length != 0 && args[0] == "reset")
            {
                int count = 0;

                foreach (var currentApp in mongoDB.FindMatch<AppModel>(Query.NE("Uploaded", true)))
                {
                    mongoDB.SetUpdated(currentApp.Url, false);
                    ++count;

                    if((count % 100) == 0)
                    {
                        Console.WriteLine("Reset update for {0}", count);
                    }
                }
            }

            foreach (var currentApp in mongoDB.FindMatch<AppModel> (Query.NE ("Uploaded", true)))
            {
                if (eventsToSend.Count < 1000)
                {
                    eventsToSend.Add(currentApp);
                    continue;
                }

                var sent = SendEventsToKeep(keenClient, eventsToSend, mongoDB);

                totalProcessed += eventsToSend.Count;
                totalSent += sent;

                Console.WriteLine("processed {0} events took {1}: ({2} events per sec)", totalProcessed, DateTime.Now - start, ((double)totalProcessed) / (DateTime.Now - start).TotalSeconds);

                eventsToSend.Clear();
            }

            {
                var sent = SendEventsToKeep(keenClient, eventsToSend, mongoDB);
                totalProcessed += eventsToSend.Count;
                Console.WriteLine("processed {0} events took {1}: ({2} events per sec)", totalProcessed, DateTime.Now - start, ((double)totalProcessed) / (DateTime.Now - start).TotalSeconds); 
            }

            if(totalProcessed != totalSent)
            {
                totalProcessed = 0;
                totalSent = 0;

                foreach (var currentApp in mongoDB.FindMatch<AppModel>(Query.NE("Uploaded", true)))
                {
                    if (eventsToSend.Count < 1)
                    {
                        eventsToSend.Add(currentApp);
                        continue;
                    }

                    var sent = SendEventsToKeep(keenClient, eventsToSend, mongoDB);

                    totalProcessed += eventsToSend.Count;
                    totalSent += sent;

                    Console.WriteLine("processed {0} events took {1}: ({2} events per sec)", totalProcessed, DateTime.Now - start, ((double)totalProcessed) / (DateTime.Now - start).TotalSeconds);

                    eventsToSend.Clear();
                }

                {
                    var sent = SendEventsToKeep(keenClient, eventsToSend, mongoDB);
                    totalProcessed += eventsToSend.Count;
                    Console.WriteLine("processed {0} events took {1}: ({2} events per sec)", totalProcessed, DateTime.Now - start, ((double)totalProcessed) / (DateTime.Now - start).TotalSeconds);
                }
            }
        }
    }
}
