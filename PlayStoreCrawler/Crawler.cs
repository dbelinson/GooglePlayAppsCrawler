using System;
using System.Linq;
using SharedLibrary;
using SharedLibrary.MongoDB;
using WebUtilsLib;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;
using SharedLibrary.Log;

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

            // Crawling App Store using all characters as the Search Input
            CrawlStore ("A");
            CrawlStore ("B");
            CrawlStore ("C");
            CrawlStore ("D");
            CrawlStore ("E");
            CrawlStore ("F");
            CrawlStore ("G");
            CrawlStore ("H");
            CrawlStore ("I");
            CrawlStore ("J");
            CrawlStore ("K");
            CrawlStore ("L");
            CrawlStore ("M");
            CrawlStore ("N");
            CrawlStore ("O");
            CrawlStore ("P");
            CrawlStore ("Q");
            CrawlStore ("R");
            CrawlStore ("S");
            CrawlStore ("T");
            CrawlStore ("U");
            CrawlStore ("V");
            CrawlStore ("X");
            CrawlStore ("Y");
            CrawlStore ("Z");
            CrawlStore ("W");
            /// ... Keep Adding characters / search terms in order to increase the crawler's reach
            // APP CATEGORIES
            CrawlStore ("BOOKS");
            CrawlStore ("BUSINESS");
            CrawlStore ("COMICS");
            CrawlStore ("COMMUNICATION");
            CrawlStore ("EDUCATION");
            CrawlStore ("ENTERTAINMENT");
            CrawlStore ("FINANCE");
            CrawlStore ("HEALTH");
            CrawlStore ("LIFESTYLE");
            CrawlStore ("LIVE WALLPAPER");
            CrawlStore ("MEDIA");
            CrawlStore ("MEDICAL");
            CrawlStore ("MUSIC");
            CrawlStore ("NEWS");
            CrawlStore ("PERSONALIZATION");
            CrawlStore ("PHOTOGRAPHY");
            CrawlStore ("PRODUCTIVITY");
            CrawlStore ("SHOPPING");
            CrawlStore ("SOCIAL");
            CrawlStore ("SPORTS");
            CrawlStore ("TOOLS");
            CrawlStore ("TRANSPORTATION");
            CrawlStore ("TRAVEL");
            CrawlStore ("WEATHER");
            CrawlStore ("WIDGETS");
            CrawlStore ("ARCADE");
            CrawlStore ("BRAIN");
            CrawlStore ("CASUAL");
            CrawlStore ("CARDS");
            CrawlStore ("RACING");

            // Extra "Random" Search terms to increase even more the crawler's reach
            CrawlStore ("INDIE");
            CrawlStore ("ZOMBIE");
            CrawlStore ("CATS");
            CrawlStore ("ROOT");
            CrawlStore ("GPS");
            CrawlStore ("BLUETOOTH");
            CrawlStore ("COMPASS");
            CrawlStore ("WALLPAPER");
            CrawlStore ("TORRENT");
            CrawlStore ("PORN");
            CrawlStore ("PLAYER");
            CrawlStore ("WINE");
            CrawlStore ("ANTIVIRUS");
            CrawlStore ("PORN");

            // Country Names as Search terms to increase even more the crawler's reach
            CrawlStore ("Afghanistan");
            CrawlStore ("Albania");
            CrawlStore ("Algeria");
            CrawlStore ("American");
            CrawlStore ("Andorra");
            CrawlStore ("Angola");
            CrawlStore ("Anguilla");
            CrawlStore ("Antigua");
            CrawlStore ("Argentina");
            CrawlStore ("Armenia");
            CrawlStore ("Aruba");
            CrawlStore ("Australia");
            CrawlStore ("Austria");
            CrawlStore ("Azerbaijan");
            CrawlStore ("Bahamas");
            CrawlStore ("Bahrain");
            CrawlStore ("Bangladesh");
            CrawlStore ("Barbados");
            CrawlStore ("Belarus");
            CrawlStore ("Belgium");
            CrawlStore ("Belize");
            CrawlStore ("Benin");
            CrawlStore ("Bermuda");
            CrawlStore ("Bhutan");
            CrawlStore ("Bolivia");
            CrawlStore ("Bosnia");
            CrawlStore ("Botswana");
            CrawlStore ("Bouvet");
            CrawlStore ("Brazil");
            CrawlStore ("Brunei");
            CrawlStore ("Bulgaria");
            CrawlStore ("Burkina");
            CrawlStore ("Burundi");
            CrawlStore ("Cambodia");
            CrawlStore ("Cameroon");
            CrawlStore ("Canada");
            CrawlStore ("Cape");
            CrawlStore ("Cayman");
            CrawlStore ("Central");
            CrawlStore ("Chad");
            CrawlStore ("Chile");
            CrawlStore ("China");
            CrawlStore ("Christmas");
            CrawlStore ("Cocos");
            CrawlStore ("Colombia");
            CrawlStore ("Comoros");
            CrawlStore ("Congo");
            CrawlStore ("Congo");
            CrawlStore ("Cook");
            CrawlStore ("Costa");
            CrawlStore ("Croatia");
            CrawlStore ("Cuba");
            CrawlStore ("Cyprus");
            CrawlStore ("Czech");
            CrawlStore ("Denmark");
            CrawlStore ("Djibouti");
            CrawlStore ("Dominica");
            CrawlStore ("Dominican");
            CrawlStore ("Ecuador");
            CrawlStore ("Egypt");
            CrawlStore ("El");
            CrawlStore ("Equatorial");
            CrawlStore ("Eritrea");
            CrawlStore ("Estonia");
            CrawlStore ("Ethiopia");
            CrawlStore ("Falkland");
            CrawlStore ("Faroe");
            CrawlStore ("Fiji");
            CrawlStore ("Finland");
            CrawlStore ("France");
            CrawlStore ("French");
            CrawlStore ("Gabon");
            CrawlStore ("Gambia");
            CrawlStore ("Georgia");
            CrawlStore ("Germany");
            CrawlStore ("Ghana");
            CrawlStore ("Gibraltar");
            CrawlStore ("Greece");
            CrawlStore ("Greenland");
            CrawlStore ("Grenada");
            CrawlStore ("Guadeloupe");
            CrawlStore ("Guam");
            CrawlStore ("Guatemala");
            CrawlStore ("Guinea");
            CrawlStore ("Guinea");
            CrawlStore ("Guyana");
            CrawlStore ("Haiti");
            CrawlStore ("Holy");
            CrawlStore ("Honduras");
            CrawlStore ("Hong");
            CrawlStore ("Hungary");
            CrawlStore ("Iceland");
            CrawlStore ("India");
            CrawlStore ("Indonesia");
            CrawlStore ("Iran");
            CrawlStore ("Iraq");
            CrawlStore ("Ireland");
            CrawlStore ("Israel");
            CrawlStore ("Italy");
            CrawlStore ("Ivory");
            CrawlStore ("Jamaica");
            CrawlStore ("Japan");
            CrawlStore ("Jordan");
            CrawlStore ("Kazakhstan");
            CrawlStore ("Kenya");
            CrawlStore ("Kiribati");
            CrawlStore ("Kuwait");
            CrawlStore ("Kyrgyzstan");
            CrawlStore ("Laos");
            CrawlStore ("Latvia");
            CrawlStore ("Lebanon");
            CrawlStore ("Lesotho");
            CrawlStore ("Liberia");
            CrawlStore ("Libya");
            CrawlStore ("Liechtenstein");
            CrawlStore ("Lithuania");
            CrawlStore ("Luxembourg");
            CrawlStore ("Macau");
            CrawlStore ("Macedonia");
            CrawlStore ("Madagascar");
            CrawlStore ("Malawi");
            CrawlStore ("Malaysia");
            CrawlStore ("Maldives");
            CrawlStore ("Mali");
            CrawlStore ("Malta");
            CrawlStore ("Marshall");
            CrawlStore ("Martinique");
            CrawlStore ("Mauritania");
            CrawlStore ("Mauritius");
            CrawlStore ("Mayotte");
            CrawlStore ("Mexico");
            CrawlStore ("Micronesia");
            CrawlStore ("Moldova");
            CrawlStore ("Monaco");
            CrawlStore ("Mongolia");
            CrawlStore ("Montenegro");
            CrawlStore ("Montserrat");
            CrawlStore ("Morocco");
            CrawlStore ("Mozambique");
            CrawlStore ("Myanmar");
            CrawlStore ("Namibia");
            CrawlStore ("Nauru");
            CrawlStore ("Nepal");
            CrawlStore ("Netherlands");
            CrawlStore ("Netherlands");
            CrawlStore ("New");
            CrawlStore ("New");
            CrawlStore ("Nicaragua");
            CrawlStore ("Niger");
            CrawlStore ("Nigeria");
            CrawlStore ("Niue");
            CrawlStore ("Norfolk");
            CrawlStore ("North");
            CrawlStore ("Northern");
            CrawlStore ("Norway");
            CrawlStore ("Oman");
            CrawlStore ("Pakistan");
            CrawlStore ("Palau");
            CrawlStore ("Panama");
            CrawlStore ("Papua");
            CrawlStore ("Paraguay");
            CrawlStore ("Peru");
            CrawlStore ("Philippines");
            CrawlStore ("Pitcairn");
            CrawlStore ("Poland");
            CrawlStore ("Polynesia");
            CrawlStore ("Portugal");
            CrawlStore ("Puerto");
            CrawlStore ("Qatar");
            CrawlStore ("Reunion");
            CrawlStore ("Romania");
            CrawlStore ("Russia");
            CrawlStore ("Rwanda");
            CrawlStore ("Saint");
            CrawlStore ("Saint");
            CrawlStore ("Saint");
            CrawlStore ("Saint");
            CrawlStore ("Saint");
            CrawlStore ("Samoa");
            CrawlStore ("San");
            CrawlStore ("Sao");
            CrawlStore ("Saudi");
            CrawlStore ("Senegal");
            CrawlStore ("Serbia");
            CrawlStore ("Seychelles");
            CrawlStore ("Sierra");
            CrawlStore ("Singapore");
            CrawlStore ("Slovakia");
            CrawlStore ("Slovenia");
            CrawlStore ("Solomon");
            CrawlStore ("Somalia");
            CrawlStore ("South");
            CrawlStore ("South");
            CrawlStore ("South");
            CrawlStore ("South");
            CrawlStore ("Spain");
            CrawlStore ("Sri");
            CrawlStore ("Sudan");
            CrawlStore ("Suriname");
            CrawlStore ("Svalbard");
            CrawlStore ("Swaziland");
            CrawlStore ("Sweden");
            CrawlStore ("Switzerland");
            CrawlStore ("Syria");
            CrawlStore ("Taiwan");
            CrawlStore ("Tajikistan");
            CrawlStore ("Tanzania");
            CrawlStore ("Thailand");
            CrawlStore ("Timor");
            CrawlStore ("Togo");
            CrawlStore ("Tokelau");
            CrawlStore ("Tonga");
            CrawlStore ("Trinidad");
            CrawlStore ("Tunisia");
            CrawlStore ("Turkey");
            CrawlStore ("Turkmenistan");
            CrawlStore ("Turks");
            CrawlStore ("Tuvalu");
            CrawlStore ("Uganda");
            CrawlStore ("Ukraine");
            CrawlStore ("United");
            CrawlStore ("United");
            CrawlStore ("United");
            CrawlStore ("Uruguay");
            CrawlStore ("Uzbekistan");
            CrawlStore ("Vanuatu");
            CrawlStore ("Venezuela");
            CrawlStore ("Vietnam");
            CrawlStore ("Virgin");
            CrawlStore ("Wallis");
            CrawlStore ("Yemen");
            CrawlStore ("Zambia");
            CrawlStore ("Zimbabwe");
        }

        /// <summary>
        /// Executes a Search using the searchField as the search parameter, 
        /// paginates / scrolls the search results to the end adding all the url of apps
        /// it finds to a AWS SQS queue
        /// </summary>
        /// <param name="searchField"></param>
        private static void CrawlStore (string searchField)
        {
            // Console Feedback
            _logger.Info ("Crawling Search Term : [ " + searchField + " ]");

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
                server.Host = Consts.HOST;

                // Executing Initial Request
                response    = server.Post (String.Format (Consts.CRAWL_URL, searchField), Consts.INITIAL_POST_DATA);

                // Parsing Links out of Html Page (Initial Request)                
                foreach (string url in parser.ParseAppUrls (response))
                {
                    // Checks whether the app have been already processed 
                    // or is queued to be processed
                    if ((!mongoDB.AppProcessed (Consts.APP_URL_PREFIX + url)) && (!mongoDB.AppQueued (url)))
                    {
                        // Console Feedback
                        _logger.Info ("Queued App");

                        // Than, queue it :)
                        mongoDB.AddToQueue (url);
                        Thread.Sleep (250); // Hiccup
                    }
                    else
                    {
                        // Console Feedback
                        _logger.Info ("Duplicated App. Skipped");
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
                            // Console Feedback
                            Console.WriteLine (" . Queued App");

                            // Than, queue it :)
                            mongoDB.AddToQueue (url);
                            Thread.Sleep (250); // Hiccup
                        }
                        else
                        {
                            // Console Feedback
                            Console.WriteLine (" . Duplicated App. Skipped");
                        }
                    }

                    // Incrementing Paging Multiplier
                    currentMultiplier++;

                }  while (parser.AnyResultFound (response) && errorsCount <= Consts.MAX_REQUEST_ERRORS);
            }
        }
    }
}
