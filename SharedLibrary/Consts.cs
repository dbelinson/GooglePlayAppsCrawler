using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;

namespace SharedLibrary
{
    public class Consts
    {
        // Web Request Parameters and URLs
        public static readonly string CRAWL_URL          = "https://play.google.com/store/search?q={0}&c=apps";
        public static readonly string REVIEWS_URL        = "https://play.google.com/store/getreviews";
        public static readonly string HOST               = "play.google.com";
        public static readonly string ORIGIN             = "https://play.google.com";
        public static readonly string REFERER            = "https://play.google.com/store/apps";
        public static readonly string INITIAL_POST_DATA  = "ipf=1&xhr=1";
        public static readonly string PLAY_STORE_PREFIX  = "https://play.google.com/store/apps/details?id=";
        public static readonly string GITHUBURL          = "Google Play Crawler - [https://github.com/MarcelloLins/GooglePlayAppsCrawler]";
        
        // Old Post Data - To Allow Rolling Back if Needed
        // public static readonly string POST_DATA          = "start={0}&num=48&numChildren=0&ipf=1&xhr=1";
        
        public static readonly string POST_DATA            = "start=0&num=0&numChildren=0&pagTok={0}&ipf=1&xhr=1";
        public static readonly string CATEGORIES_POST_DATA = "start={0}&num={1}&numChildren=0&ipf=1&xhr=1";
        public static readonly string APP_URL_PREFIX       = "https://play.google.com";
        public static readonly string ACCEPT_LANGUAGE      = "Accept-Language: en-US,en;q=0.6,en;q=0.4,es;q=0.2";
        public static readonly string REVIEWS_POST_DATA    = "reviewType=0&pageNum={0}&id={1}&reviewSortOrder=2&xhr=1";
        public static readonly string USER_AGENT           = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 Safari/537.36";
                                                           
        // XPaths                                          
        public static readonly string APP_LINKS            = "//div[@class='details']/a[@class='card-click-target' and @tabindex='-1' and @aria-hidden='true']";
        public static readonly string APP_NAME             = "//div[@class='info-container']/div[@class='document-title' and @itemprop='name']/div";
        public static readonly string APP_CATEGORY         = "//div/a[@class='document-subtitle category']";
        public static readonly string APP_DEV              = "//div[@class='info-container']/div[@itemprop='author']/a/span[@itemprop='name']";
        public static readonly string APP_TOP_DEV          = "//meta[@itemprop='topDeveloperBadgeUrl']";
        public static readonly string DEV_URL              = "//div[@class='info-container']/div[@itemprop='author']/meta[@itemprop='url']";
        public static readonly string APP_PUBLISH_DATE     = "//div[@class='info-container']/div[@itemprop='author']/div[@class='document-subtitle']";
        public static readonly string APP_FREE_PAID        = "//span[@itemprop='offers' and @itemtype='http://schema.org/Offer']/meta[@itemprop='price']";
        public static readonly string APP_REVIEWERS        = "//div[@class='header-star-badge']/div[@class='stars-count']";
        public static readonly string APP_DESCRIPTION      = "//div[@class='show-more-content text-body' and @itemprop='description']";
        public static readonly string APP_SCORE_VALUE      = "//div[@class='rating-box']/div[@class='score-container']/meta[@itemprop='ratingValue']";
        public static readonly string APP_SCORE_COUNT      = "//div[@class='rating-box']/div[@class='score-container']/meta[@itemprop='ratingCount']";
        public static readonly string APP_FIVE_STARS       = "//div[@class='rating-histogram']/div[@class='rating-bar-container five']/span[@class='bar-number']";
        public static readonly string APP_FOUR_STARS       = "//div[@class='rating-histogram']/div[@class='rating-bar-container four']/span[@class='bar-number']";
        public static readonly string APP_THREE_STARS      = "//div[@class='rating-histogram']/div[@class='rating-bar-container three']/span[@class='bar-number']";
        public static readonly string APP_TWO_STARS        = "//div[@class='rating-histogram']/div[@class='rating-bar-container two']/span[@class='bar-number']";
        public static readonly string APP_ONE_STARS        = "//div[@class='rating-histogram']/div[@class='rating-bar-container one']/span[@class='bar-number']";
        public static readonly string APP_COVER_IMG        = "//div[@class='details-info']/div[@class='cover-container']/img[@class='cover-image']";
        public static readonly string APP_UPDATE_DATE      = "//div[@class='meta-info']/div[@itemprop='datePublished']";
        public static readonly string APP_SIZE             = "//div[@class='meta-info']/div[@itemprop='fileSize']";
        public static readonly string APP_VERSION          = "//div[@class='content' and @itemprop='softwareVersion']";
        public static readonly string APP_INSTALLS         = "//div[@class='content' and @itemprop='numDownloads']";
        public static readonly string APP_CONTENT_RATING   = "//div[@class='content' and @itemprop='contentRating']";
        public static readonly string APP_OS_REQUIRED      = "//div[@class='content' and @itemprop='operatingSystems']";
        public static readonly string EXTRA_APPS           = "//div[@class='card-content id-track-click id-track-impression']/a[@class='card-click-target']";
        public static readonly string IN_APP_PURCHASE      = "//div[@class='info-container']/div[@class='inapp-msg']";
        public static readonly string DEVELOPER_URLS       = "//div[@class='content contains-text-link']/a[@class='dev-link']";
        public static readonly string PHYSICAL_ADDRESS     = "//div[@class='content physical-address']";
        public static readonly string APP_SCREENSHOTS      = "//div[@class='thumbnails' ]//img[contains(@class,'screenshot')]";
        public static readonly string WHATS_NEW            = "//div[@class='recent-change']";

        // HTML Values
		public static string NO_RESULT_MESSAGE 		{ get { var lang = ConfigurationManager.GetSection("languageStrings") as NameValueCollection; return lang[lang["chosen-language"]]; } }
        
        // Retry Values
        public static readonly int MAX_REQUEST_ERRORS   = 100;
        public static readonly int MAX_QUEUE_TRIES      = 5;
        
        // MongoDB - Remote Server
		public static string MONGO_SERVER           { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["server"]; } }
		public static string MONGO_PORT             { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["port"]; } }
		public static string MONGO_USER             { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["user"]; } }
		public static string MONGO_PASS             { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["password"]; } }
		public static string MONGO_DATABASE         { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["database"]; } }
		public static string MONGO_COLLECTION       { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["apps-collection"]; } }
		public static string REVIEWS_COLLECTION     { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["reviews-collection"]; } }
		public static string QUEUED_APPS_COLLECTION { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["queued-apps-collection"]; } }
		public static string REVIEWERS_COLLECTION   { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["reviewers-collection"]; } }
		public static string MONGO_AUTH_DB          { get { var mongo = ConfigurationManager.GetSection("mongoSettings") as NameValueCollection; return mongo["auth-db"]; } }
        public static readonly int    MONGO_TIMEOUT          = 120000;

        // Date Time Format
        public static readonly string DATE_FORMAT       = "yyyy MMMM dd";
    }
}
