using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class AppReview
    {
        public ObjectId _id         { get; set; }
        public string   appName     { get; set; }
        public string   appID       { get; set; }
        public string   appURL      { get; set; }
        public string   authorName  { get; set; }
        public DateTime reviewDate  { get; set; }
        public string   permalink   { get; set; }
        public int      starRatings { get; set; }
        public string   reviewTitle { get; set; }
        public string   reviewBody  { get; set; }
        public DateTime timestamp   { get; set; }
    }
}
