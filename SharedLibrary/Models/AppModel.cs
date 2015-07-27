using System;
using System.Linq;
using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models
{
    // Serialization Models for App Data to be stored on MongoDB
    public class AppModel
    {
        public ObjectId _id                    {get;set;}
        public DateTime ReferenceDate          {get;set;}
        public string   Url                    {get;set;}
        public string[] RelatedUrls            {get;set;}
        public string   Name                   {get;set;}
        public string   Developer              {get;set;}
        public bool     IsTopDeveloper         {get;set;}
        public string   DeveloperURL           {get;set;}
        public string   DeveloperNormalizedDomain {get;set;}
        public DateTime PublicationDate        {get;set;}
        public string   Category               {get;set;}
        public bool     IsFree                 {get;set;}
        public double   Price                  {get;set;}
        [BsonIgnoreIfNull]
        public string   Currency               {get;set;}
        public double   Reviewers              {get;set;}
        public string   CoverImgUrl            {get;set;}
        public string[] Screenshots            {get;set;}
        public string   Description            {get;set;}
        public string   WhatsNew               {get;set;}
        public Score    Score                  {get;set;}
        public DateTime LastUpdateDate         {get;set;}
        public Double   AppSize                {get;set;}
        public string   Instalations           {get;set;}
        public string   CurrentVersion         {get;set;}
        public string   MinimumOSVersion       {get;set;}
        public string   ContentRating          {get;set;}
        public bool     HaveInAppPurchases     {get;set;}
        public string   DeveloperEmail         {get;set;}
        public string   DeveloperWebsite       {get;set;}
        public string   DeveloperPrivacyPolicy {get;set;}
        public string   PhysicalAddress        {get;set;} 

        // Reviews Related Attributes
        public string   ReviewsStatus           {get;set;} 
        public List<AppReview> Reviews          {get;set;} 

        // Class Constructor
        public AppModel ()
        {
            // Default value for Reviews Status
            ReviewsStatus = "Unvisited";
        }

        // Override of the ToString Method
        public string ToString()
        {
            // Serializing Object as CSV
            return String.Join (",", 
                                ("\"" + Url                    + "\""),
                                ("\"" + ReferenceDate.ToString ("yyyy-MM-dd").Replace (",","") + "\""),
                                ("\"" + Name.Replace (",", "") + "\" "),
                                ("\"" + Developer.Replace (",", "") + "\""),
                                ("\"" + IsTopDeveloper         + "\""),
                                ("\"" + DeveloperURL.Replace (",", "") + "\""),
                                ("\"" + PublicationDate.ToString ("yyyy-MM-dd").Replace (",", "") + "\""),
                                ("\"" + Category.Replace (",", "") + "\""),
                                ("\"" + IsFree                 + "\""),
                                ("\"" + Price                  + "\""), 
                                ("\"" + Reviewers              + "\""),
                                ("\"" + Score.Total            + "\""), 
                                ("\"" + Score.Count            + "\""), 
                                ("\"" + Score.FiveStars        + "\""),
                                ("\"" + Score.FourStars        + "\""),
                                ("\"" + Score.ThreeStars       + "\""),
                                ("\"" + Score.TwoStars         + "\""),
                                ("\"" + Score.OneStars         + "\""),
                                ("\"" + LastUpdateDate.ToString ("yyyy-MM-dd") + "\""),
                                //("\"" + AppSize                + "\""),
                                ("\"" + Instalations.Replace (",", ".") + "\""),
                                ("\"" + CurrentVersion.Replace (",", "") + "\""),
                                ("\"" + MinimumOSVersion.Replace (",", "") + "\""),
                                ("\"" + ContentRating.Replace (",", "") + "\""),
                                ("\"" + HaveInAppPurchases     +"\""),
                                ("\"" + DeveloperEmail.Replace (",", "") + "\""),
                                ("\"" + DeveloperWebsite.Replace (",", "") + "\""),
                                ("\"" + DeveloperPrivacyPolicy.Replace (",", "") + "\""),
                                ("\"" + Description.Replace (",", "") + "\""));
        }
    }

    public class Score
    {
        public double Total      { get; set; }
        public double Count      { get; set; }
        public double FiveStars  { get; set; }
        public double FourStars  { get; set; }
        public double ThreeStars { get; set; }
        public double TwoStars   { get; set; }
        public double OneStars   { get; set; }
    }
}
