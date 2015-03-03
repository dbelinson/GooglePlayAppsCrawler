using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class ReviewerPageData
    {
        public ObjectId                _id          { get; set; }
        public string                  reviewerName { get; set; }
        public string                  reviewerUrl  { get; set; }
        public List<UserReviewSummary> reviews      { get; set; }
    }

    public class UserReviewSummary
    {
        public string appName { get; set; }
        public string appId   { get; set; }
        public int    stars   { get; set; }
    }
}
