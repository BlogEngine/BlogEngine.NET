using BlogEngine.Core.Data.Models;
using Newtonsoft.Json;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Online gallery
    /// </summary>
    public static class Gallery
    {
        /// <summary>
        /// Load gallery packages
        /// </summary>
        /// <param name="packages">Packages to load</param>
        public static void Load(List<Package> packages)
        {
            try
            {
                var packs = GetNugetPackages().ToList();
                var extras = GetPackageExtras();

                foreach (var pkg in packs)
                {
                    if (pkg.IsLatestVersion)
                    {
                        var jp = new Package
                        {
                            Id = pkg.Id,
                            Authors = pkg.Authors == null ? "unknown" : string.Join(", ", pkg.Authors),
                            Description = pkg.Description.Length > 140 ? string.Format("{0}...", pkg.Description.Substring(0, 140)) : pkg.Description,
                            DownloadCount = pkg.DownloadCount,
                            LastUpdated = pkg.Published != null ? pkg.Published.Value.ToString("yyyy-MM-dd HH:mm") : "", // format for sort order to work with strings
                            Title = string.IsNullOrEmpty(pkg.Title) ? pkg.Id : pkg.Title,
                            OnlineVersion = pkg.Version.ToString(),
                            Website = pkg.ProjectUrl == null ? null : pkg.ProjectUrl.ToString(),
                            Tags = pkg.Tags,
                            IconUrl = pkg.IconUrl == null ? null : pkg.IconUrl.ToString()
                        };

                        if (!string.IsNullOrEmpty(jp.IconUrl) && !jp.IconUrl.StartsWith("http:"))
                            jp.IconUrl = Constants.GalleryUrl + jp.IconUrl;

                        if (string.IsNullOrEmpty(jp.IconUrl))
                            jp.IconUrl = DefaultThumbnail("");

                        if (extras != null && extras.Count() > 0)
                        {
                            var extra = extras.Where(e => e.Id.ToLower() == pkg.Id.ToLower() + "." + pkg.Version).FirstOrDefault();

                            if (extra != null)
                            {
                                jp.Extra = extra;
                                jp.DownloadCount = extra.DownloadCount;
                                jp.Rating = extra.Rating;
                                jp.PackageType = extra.PkgType.ToString();
                            }
                        }
                        packages.Add(jp);
                    }
                }

            }
            catch (Exception ex)
            {
                Utils.Log("BlogEngine.Core.Packaging.Load", ex);
            }   
        }

        /// <summary>
        /// Gets extra filds from remote gallery if gallery supports it
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <returns>Object with extra package fields</returns>
        public static PackageExtra GetPackageExtra(string id)
        {
            try
            {
                var url = BlogConfig.GalleryFeedUrl.Replace("/nuget", "/api/extras/" + id);
                WebClient wc = new WebClient();
                string json = wc.DownloadString(url);
                return JsonConvert.DeserializeObject<PackageExtra>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// BlogEngine.Gallery implements Nuget.Server
        /// and adds fields like download counts, reviews, ratings etc.
        /// </summary>
        /// <returns>List of extra fields if exist</returns>
        public static IEnumerable<PackageExtra> GetPackageExtras()
        {
            try
            {
                var url = BlogConfig.GalleryFeedUrl.Replace("/nuget", "/api/extras");
                WebClient wc = new WebClient();
                string json = wc.DownloadString(url);
                return JsonConvert.DeserializeObject<List<PackageExtra>>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Rate package
        /// </summary>
        /// <param name="id">Pacakge ID</param>
        /// <param name="review">Review</param>
        /// <returns>Empty if success, message otherwise</returns>
        public static string RatePackage(string id, Review review)
        {
            try
            {
                var url = BlogConfig.GalleryFeedUrl.Replace("/nuget", "/api/review?pkgId=" + id);
                WebClient wc = new WebClient();
                var data = JsonConvert.SerializeObject(review);
                wc.Headers.Add("content-type", "application/json");
                var result = wc.UploadString(url, "PUT", data);

                // sanitize result replacing garbage returned by service
                if (result == "\"\"") result = string.Empty;
                result = result.Replace("\"", "").Replace("\\", "");

                Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                return result;
            }
            catch (Exception ex)
            {
                Utils.Log("Error rating package", ex);
                return ex.Message;
            }
        }

        #region Private methods

        static IEnumerable<IPackage> GetNugetPackages()
        {
            var rep = PackageRepositoryFactory.Default.CreateRepository(BlogConfig.GalleryFeedUrl);
            return rep.GetPackages();
        }

        static string DefaultThumbnail(string packageType)
        {
            switch (packageType)
            {
                case "Theme":
                    return $"{Utils.ApplicationRelativeWebRoot}Content/images/blog/Theme.png";
                case "Extension":
                    return $"{Utils.ApplicationRelativeWebRoot}Content/images/blog/ext.png";
                case "Widget":
                    return $"{Utils.ApplicationRelativeWebRoot}Content/images/blog/Widget.png";
            }
            return $"{Utils.ApplicationRelativeWebRoot}Content/images/blog/pkg.png";
        }

        #endregion
    }
}