using System;
using System.Linq;
using SharedLibrary;
using SharedLibrary.MongoDB;
using WebUtilsLib;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;
using SharedLibrary.Log;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharedLibrary.Proxies;

namespace PlayStoreCrawler
{
    class Crawler
    {
        private static Logger _logger;

        /// <summary>
        /// Entry point of the crawler
        /// </summary>
        /// <param name="args"></param>
        static void Main (string[] args)
        {
            // Setting Up Log
            LogSetup.InitializeLog ("PlayStoreCrawler.log", "info");
            _logger = LogManager.GetCurrentClassLogger ();

            // Control Variable (Bool - Should the process use proxies? )
            bool isUsingProxies = false;

            // Checking for the need to use HTTP proxies or not
            if (args != null && args.Length == 1)
            {
                _logger.Info ("Loading Proxies from File");

                // Setting flag to true
                isUsingProxies = true;

                // Loading proxies from .txt received as argument
                String fPath = args[0];

                // Sanity Check
                if (!File.Exists (fPath))
                {
                    _logger.Fatal ("Couldnt find proxies on path : " + fPath);
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
                    _logger.Fatal (ex);
                    System.Environment.Exit (-101);
                }
            }

            // Main Flow
            _logger.Info ("Started Bootstrapping Steps");

            // Scrapping "Play Store Categories"
            foreach (var categoriesKVP in BootstrapTerms.categoriesAndNames)
            {
                CrawlCategory (categoriesKVP.Key, categoriesKVP.Value, isUsingProxies);
            }

            // Queueing Apps that start with each of the characters from "A" to "Z"
            foreach (var character in BootstrapTerms.charactersSearchTerms)
            {
                CrawlStore (character, isUsingProxies);
            }

            /// ... Keep Adding characters / search terms in order to increase the crawler's reach
            // APP CATEGORIES
            foreach (var category in BootstrapTerms.categoriesSearchTerms)
            {
                CrawlStore (category, isUsingProxies);
            }

            // Extra "Random" Search terms to increase even more the crawler's reach
            foreach (var miscTerm in BootstrapTerms.miscSearchTerms)
            {
                CrawlStore (miscTerm, isUsingProxies);
            }

            // Country Names as Search terms to increase even more the crawler's reach
            foreach (var countryName in BootstrapTerms.countryNames)
            {
                CrawlStore (countryName, isUsingProxies);
            }
        }

        /// <summary>
        /// Executes a Search using the searchField as the search parameter, 
        /// paginates / scrolls the search results to the end adding all the url of apps
        /// it finds to a Mongo "QUEUE" collection
        /// </summary>
        /// <param name="searchField"></param>
        private static void CrawlStore (string searchField, bool shouldUseProxies)
        {
            // Console Feedback
            _logger.Warn ("Crawling Search Term : [ " + searchField + " ]");

            // Compiling Regular Expression used to parse the "pagToken" out of the Play Store
            Regex pagTokenRegex = new Regex (@"GAEi+.+\:S\:.{11}\\42", RegexOptions.Compiled);

            // HTML Response
            string response;

            // MongoDB Helper
            // Configuring MongoDB Wrapper
            MongoDBWrapper mongoDB   = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            // Ensuring the database has the proper indexe
            mongoDB.EnsureIndex ("Url");

            // Response Parser
            PlayStoreParser parser = new PlayStoreParser (); 

            // Executing Web Requests
            using (WebRequests server = new WebRequests ())
            {
                // Creating Request Object
                server.Headers.Add (Consts.ACCEPT_LANGUAGE);
                server.Host      = Consts.HOST;
                server.UserAgent = Consts.GITHUBURL;
                server.Encoding  = "utf-8";

                // Checking for the need to use "HTTP Proxies"
                if (shouldUseProxies)
                {
                    server.Proxy = ProxiesLoader.GetWebProxy ();
                }   

                // Executing Initial Request
                response    = server.Post (String.Format (Consts.CRAWL_URL, searchField), Consts.INITIAL_POST_DATA);

                // Parsing Links out of Html Page (Initial Request)                
                foreach (string url in parser.ParseAppUrls (response))
                {
                    // Checks whether the app have been already processed 
                    // or is queued to be processed
                    if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url)))
                    {
                        // Than, queue it :)
                        mongoDB.AddToQueue (url);
                        Thread.Sleep (250); // Hiccup
                    }
                }

                // Executing Requests for more Play Store Links
                int initialSkip       = 48;
                int currentMultiplier = 1;
                int errorsCount       = 0;
                do
                {
                    // Finding pagToken from HTML
                    var rgxMatch = pagTokenRegex.Match (response);

                    // If there's no match, skips it
                    if (!rgxMatch.Success)
                    {
                        break;
                    }

                    // Reading Match from Regex, and applying needed replacements
                    string pagToken = rgxMatch.Value.Replace (":S:", "%3AS%3A").Replace("\\42", String.Empty).Replace(@"\\u003d", String.Empty);

                    // Assembling new PostData with paging values
                    string postData = String.Format (Consts.POST_DATA, pagToken);

                    // Executing request for values
                    response = server.Post (String.Format (Consts.CRAWL_URL, searchField), postData);

                    // Checking Server Status
                    if (server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.Error ("Http Error" + " - Status Code [ " + server.StatusCode + " ]");
                        errorsCount++;
                        continue;
                    }

                    // Parsing Links
                    foreach (string url in parser.ParseAppUrls (response))
                    {
                        // Checks whether the app have been already processed 
                        // or is queued to be processed
                        if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url)))
                        {
                            // Than, queue it :)
                            mongoDB.AddToQueue (url);
                            Thread.Sleep (250); // Hiccup
                        }
                    }

                    // Incrementing Paging Multiplier
                    currentMultiplier++;

                }  while (parser.AnyResultFound (response) && errorsCount <= Consts.MAX_REQUEST_ERRORS);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryUrl"></param>
        private static void CrawlCategory (string categoryUrl, string categoryName, bool shouldUseProxies)
        {
            // Console Feedback
            _logger.Warn ("Crawling Category : [ " + categoryName + " ]");

            // Hashset of urls used to keep track of what's been parsed already
            HashSet<String> foundUrls = new HashSet<String> ();

            // Control variable to avoid "Loop" on pagging
            bool isDonePagging = false;

            // HTML Response
            string response;

            // MongoDB Helper
            // Configuring MongoDB Wrapper
            MongoDBWrapper mongoDB   = new MongoDBWrapper ();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            // Ensuring the database has the proper indexe
            mongoDB.EnsureIndex ("Url");

            // Response Parser
            PlayStoreParser parser = new PlayStoreParser (); 

            // Executing Web Requests
            using (WebRequests server = new WebRequests ())
            {
                // Creating Request Object
                server.Headers.Add (Consts.ACCEPT_LANGUAGE);
                server.Host      = Consts.HOST;
                server.UserAgent = Consts.GITHUBURL;
                server.Encoding  = "utf-8";

                // Executing Initial Request
                response = server.Get (categoryUrl);

                // Parsing Links out of Html Page (Initial Request)                
                foreach (string url in parser.ParseAppUrls (response))
                {
                    // Saving found url on local hashset
                    foundUrls.Add (url);

                    // Checks whether the app have been already processed 
                    // or is queued to be processed
                    if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url)))
                    {
                        // Than, queue it :)
                        mongoDB.AddToQueue (url);
                    }
                }

                // Executing Requests for more Play Store Links
                int baseSkip       = 60;
                int currentMultiplier = 1;
                int errorsCount       = 0;
                do
                {
                    // Assembling new PostData with paging values
                    string postData = String.Format (Consts.CATEGORIES_POST_DATA, (currentMultiplier * baseSkip), baseSkip);

                    // Executing request for values
                    response = server.Post (String.Format (categoryUrl + "?authuser=0"), postData);

                    // Checking Server Status
                    if (server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.Error ("Http Error" + " - Status Code [ " + server.StatusCode + " ]");
                        errorsCount++;
                        continue;
                    }

                    // Parsing Links
                    foreach (string url in parser.ParseAppUrls (response))
                    {
                        // If a certain app is found twice, it means that the "pagging" logic got stuck into a 
                        // Loop, so the all the apps for this category were parsed already
                        if (foundUrls.Contains (url))
                        {
                            isDonePagging = true;
                            break;
                        }

                        // Saving found url on local hashset
                        foundUrls.Add (url);

                        // Checks whether the app have been already processed 
                        // or is queued to be processed
                        if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url)))
                        {
                            // Than, queue it :)
                            mongoDB.AddToQueue (url);
                        }
                    }

                    // Incrementing Paging Multiplier
                    currentMultiplier++;

                }  while (!isDonePagging && errorsCount <= Consts.MAX_REQUEST_ERRORS);
            }
        }
    }
}
