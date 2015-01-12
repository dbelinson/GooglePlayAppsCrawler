using System;
using System.Linq;
using BDC.BDCCommons;
using SharedLibrary;
using SharedLibrary.MongoDB;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using SharedLibrary.Models;
using MongoDB.Driver.Builders;

namespace AppsExporter
{
    class Program
    {
        ///  *** READ THIS BEFORE YOU START. ***
        ///  *** I MEAN IT, PLEASE, READ IT  ***
        /// 
        ///  This exporting helper will download ALL THE APPS found on the database, and
        ///  dump it to a CSV file (with headers).
        ///  
        ///  Note that, since the database is Hosted on AWS, i will PAY (for the internet outbound traffic) if you execute a full database export,
        ///  so, if you are going to execute a full export, please, get in touch with me before running this project, or send me a donation
        ///  via paypal on marcello.grechi@gmail.com
        ///  
        ///  Also, be nice with the database.
        ///  
        ///  ** END OF WARNING ***

        static void Main (string[] args)
        {
            // Logs Counter
            int processedApps = 0;

            // Configuring Log Object Threshold
            LogWriter.Threshold = TLogEventLevel.Information;
            
            // Overriding LogWriter Event
            LogWriter.LogEvent += LogWriter_LogEvent;
            
            LogWriter.Info("Checking Arguments");
            
            // Periodic Log Timer
            Timer loggingThread = new Timer((TimerCallback) =>
            {
                LogWriter.Info ("Processed Apps: " + processedApps);

            }, null, 10000, 10000);
            
            // Validating Arguments
            if (!ValidateArgs (args))
            {
                LogWriter.Fatal ("Invalid Args", "Args must have 1 element");
                return;
            }

            LogWriter.Info("Checking Write Permissions on output Path");
            // Validating Write Permissions on output path
            if (!ValidateFilePermissions (args[0]))
            {
                LogWriter.Fatal("Insuficient Permissions", "Cannot write on path : " + args[0]);
                return;
            }

            // Configuring MongoDB Wrapper
            MongoDBWrapper mongoDB = new MongoDBWrapper();
            string fullServerAddress = String.Join(":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);
            
            // Opening Output Stream
            using (StreamWriter sWriter = new StreamWriter (args[0], true, Encoding.GetEncoding("ISO-8859-1")))
            {
                // Auto Flush Content
                sWriter.AutoFlush = true;

                // Writing Headers
                String headersLine = "_id,Url,ReferenceDate,Name,Developer,IsTopDeveloper,DeveloperURL,PublicationDate,"
                                   + "Category,IsFree,Price,Reviewers,CoverImgUrl,Description,Score.Total,Score.Count,Score.FiveStars,"
                                   + "Score.FourStars,Score.ThreeStars,Score.TwoStars,Score.OneStars,LastUpdateDate"
                                   + "AppSize,Instalations,CurrentVersion,MinimumOSVersion,ContentRating,HaveInAppPurchases,DeveloperEmail,DeveloperWebsite,DeveloperPrivacyPolicy";

                sWriter.WriteLine (headersLine);

                // Example of MongoDB Query Construction
                // Queries for records which have the attribute "IsTopDeveloper" equal to "false"
                var mongoQuery = Query.EQ ("IsTopDeveloper", false);

                // Reading all apps from the database
                // USAGE: CHANGE FindMatches to FindAll if you want to export all the records from the database
                foreach (AppModel app in mongoDB.FindMatch<AppModel>(mongoQuery, 10, 0))
                {
                    try
                    {
                        // Writing line to File
                        sWriter.WriteLine (app.ToString ());
                        processedApps++;
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Error (ex);
                    }
                }
            }

            // Logging end of the Process
            LogWriter.Info ("Finished Exporting Database");

            // Removing Event
            LogWriter.LogEvent -= LogWriter_LogEvent;
        }

        static void LogWriter_LogEvent (TLogMessage msg)
        {
            Console.WriteLine (msg.EVT_MESSAGE);
        }

        private static bool ValidateArgs (string[] args)
        {
            return args.Length == 1;
        }

        private static bool ValidateFilePermissions (string filePath)
        {
            string directoryName = Directory.GetParent (filePath).FullName;

            PermissionSet permissionSet = new PermissionSet (PermissionState.None);

            FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.Write, directoryName);

            permissionSet.AddPermission(writePermission);

            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
