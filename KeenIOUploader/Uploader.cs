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

            // From This point on, you can change your code to reflect your own "Reading" logic
            // What I've done is simply read the records from the MongoDB database and Upload them to Keen.IO
            foreach (var currentApp in mongoDB.FindMatch<AppModel> (Query.NE ("Uploaded", true)))
            {
                try
                {
                    // Adding Event to Keen.IO
                    keenClient.AddEvent ("PlayStore2014", currentApp);

                    // Incrementing Counter
                    _appsCounter++;

                    // Console feedback Every 100 Processed Apps
                    if (_appsCounter % 100 == 0)
                    {
                        Console.WriteLine ("Uploaded : " + _appsCounter);
                    }

                    mongoDB.SetUpdated (currentApp.Url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine ("\n\t" + ex.Message);
                }
            }
        }
    }
}
