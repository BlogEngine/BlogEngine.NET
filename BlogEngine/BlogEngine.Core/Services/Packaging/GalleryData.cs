using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

/// <summary>
/// UNDONE: Doc: Description.
/// </summary>
public class PackageExtra
{
    /// <summary>
    /// Types of package.
    /// </summary>
    public enum PackageType
    {

        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        Extension,

        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        Theme,

        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        Widget
    }

    /// <summary>
    /// Type of package.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public PackageType PkgType { get; set; }

    /// <summary>
    /// Package ID.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Number of downloads.
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// Package rating.
    /// </summary>
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
    /// <summary>
    /// Reviews of Package.
    /// </summary>
    public List<Review> Reviews { get; set; }
}

/// <summary>
/// UNDONE: Doc: Description.
/// </summary>
public class Review
{
    /// <summary>
    /// Name of item being reviewed.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// UNDONE: Doc: Description of Ip.
    /// </summary>
    public string Ip { get; set; }
    /// <summary>
    /// Rating assigned to review.
    /// </summary>
    public int Rating { get; set; }
    /// <summary>
    /// UNDONE: Doc: Body of review.
    /// </summary>
    public string Body { get; set; }
    /// <summary>
    /// Date review was posted.
    /// </summary>
    public DateTime DatePosted { get; set; }
}
