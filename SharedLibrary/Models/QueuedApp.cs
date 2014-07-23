using System;
using System.Linq;
using MongoDB.Bson;

namespace SharedLibrary.Models
{
    public class QueuedApp
    {
        public ObjectId _id { get; set; }
        public String Url   { get; set; }
        public bool IsBusy  { get; set; }
    }
}
