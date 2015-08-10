using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

public class PackageExtra
{
    public enum PackageType
    {
        Extension, Theme, Widget
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public PackageType PkgType { get; set; }

    public string Id { get; set; }
    public int DownloadCount { get; set; }

    public float Rating
    {
        get
        {
            if (Reviews == null || Reviews.Count == 0) return 0;

            float totalVoters = 0;
            float totalPoints = 0;

            for (int i = 0; i < Reviews.Count; i++)
            {
                totalVoters++;
                totalPoints += Reviews[i].Rating;
            }
            return Convert.ToInt32(totalPoints / totalVoters);
        }
    }
    public List<Review> Reviews { get; set; }
}

public class Review
{
    public string Name { get; set; }
    public string Ip { get; set; }
    public int Rating { get; set; }
    public string Body { get; set; }
    public DateTime DatePosted { get; set; }
}
