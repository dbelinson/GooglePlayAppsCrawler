using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilsLib;

namespace SharedLibrary.Reviews
{
    public class ReviewsWrapper
    {
        public static string GetAppReviews (string appID, int reviewsPage)
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

        public static string NormalizeResponse (string jsonResponse)
        {
            // Replacing invalid characters with valid ones to ensure HTML correct formation
            string validHTML = jsonResponse.Replace ("\\u003c", "<").Replace ("\\u003d", "=").Replace ("\\u003e", ">")
                                           .Replace ("\\u0026amp;", "&").Replace (@"\""", @"""");

            // Removing HTML Garbage
            validHTML = validHTML.Substring (validHTML.IndexOf ("<div class="));

            return validHTML;
        }
    }
}
