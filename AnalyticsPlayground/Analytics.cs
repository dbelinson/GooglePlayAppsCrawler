using Keen.Core;
using Keen.Core.Query;
using Newtonsoft.Json.Linq;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * All the Analytics provided by this project are powered by Keen.IO (Thanks Josh for the awesome support)
 * 
 * Some documentation:
 *      - Root Documentation Page : https://keen.io/docs/
 *      - Group By                : https://keen.io/docs/data-analysis/group-by/
 *      - API Reference           : https://keen.io/docs/api/reference/
 *      - SDKs                    : https://keen.io/docs/SDKs/
 */

namespace AnalyticsPlayground
{
    class Analytics
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
            
            // Creating Keen.IO Variables - Yes, i am setting my read key as the master key, so that you can read the bucket I have created with data
            var projectSettings = new ProjectSettingsProvider (_keenIOProjectID,masterKey:_keenIOReadKey);
            var keenClient      = new KeenClient (projectSettings);


            /*********************************************************************
             *          EXECUTING SIMPLE ANALYTICS QUERIES ON KEEN.IO 
             **********************************************************************/

            // Query 1 - Average App Price grouped by Category
            Dictionary<String,String> parameters = new Dictionary<String,String>();
            parameters.Add ("event_collection", "PlayStore2014");
            parameters.Add ("target_property", "Price");
            parameters.Add ("group_by", "Category");

            JObject keenResponse = keenClient.Query (KeenConstants.QueryAverage, parameters);

            PrintQueryTitle ("Query 1 - Average App Price grouped by Category");
            Console.WriteLine (keenResponse.ToSafeString ());
            PrintSeparator ();

            // Query 2 - Most Expensive app for sale of each category
            keenResponse = keenClient.Query (KeenConstants.QueryMaximum, parameters);

            PrintQueryTitle ("Query 2 - Most Expensive app for sale of each category");
            Console.WriteLine (keenResponse.ToSafeString ());
            PrintSeparator ();
            
            // Query 3 - Most Expensive App for sale of all (without group by)
            parameters.Remove ("group_by");
            keenResponse = keenClient.Query (KeenConstants.QueryMaximum, parameters);

            PrintQueryTitle ("Query 3 - Most Expensive App for sale of all (without group by)");
            Console.WriteLine (keenResponse.ToSafeString ());
            PrintSeparator ();

            Console.ReadKey ();

        }

        private static void PrintSeparator ()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ("\n ***************************************************************** \n");
            Console.ResetColor ();
        }

        private static void PrintQueryTitle (string queryTitle)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ("\t " + queryTitle);
            Console.ResetColor ();
        }

    }
}
