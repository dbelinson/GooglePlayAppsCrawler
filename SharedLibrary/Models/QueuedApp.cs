using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models
{
    public class QueuedApp
    {
		[BsonIgnoreIfDefault]
        public ObjectId _id { get; set; }
        public String Url   { get; set; }
        public bool IsBusy  { get; set; }
    }
}
