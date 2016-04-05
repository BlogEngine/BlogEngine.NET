using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Web;
using System.Linq;
using System.Collections.Specialized;

namespace BlogEngine.Core.Data.ViewModels
{
    /// <summary>
    /// Blogroll VM
    /// </summary>
    public class BlogRollVM
    {
        /// <summary>
        /// Blogroll items
        /// </summary>
        public List<BlogRollItem> BlogRolls
        {
            get
            {
                return Providers.BlogService.FillBlogRolls();
            }
        }
        /// <summary>
        /// Add blogroll
        /// </summary>
        /// <param name="form">submitted form</param>
        public void Add(NameValueCollection form)
        {
            var br = new BlogRollItem();
            br.Title = form["txtTitle"];
            br.Description = form["txtDesc"];
            br.BlogUrl = new Uri(form["txtWebsite"]);
            br.FeedUrl = new Uri(form["txtUrl"]);
            br.Xfn = GetXfn(form);
            Providers.BlogService.InsertBlogRoll(br);
        }
        /// <summary>
        /// Update blogroll
        /// </summary>
        /// <param name="form">Submitter form</param>
        /// <param name="id">Blogroll id</param>
        public void Update(NameValueCollection form, string id)
        {
            Guid gId;
            if (Guid.TryParse(id, out gId))
            {
                var br = Providers.BlogService.SelectBlogRoll(gId);
                br.Title = form["txtTitle"];
                br.Description = form["txtDesc"];
                br.BlogUrl = new Uri(form["txtWebsite"]);
                br.FeedUrl = new Uri(form["txtUrl"]);
                br.Xfn = GetXfn(form);
                Providers.BlogService.UpdateBlogRoll(br);
            }
            else
            {
                Utils.Log("Can not update BlogRoll " + id);
            }
        }
        /// <summary>
        /// Delete blogroll
        /// </summary>
        /// <param name="id">Blogroll ID</param>
        public void Delete(string id)
        {
            Guid gId;
            if (Guid.TryParse(id, out gId))
            {
                var roll = Providers.BlogService.SelectBlogRoll(gId);
                Providers.BlogService.DeleteBlogRoll(roll);
            }
            else
            {
                Utils.Log("Can not delete BlogRoll " + id);
            }
        }
        /// <summary>
        /// Get blogroll feeds
        /// </summary>
        /// <param name="feedUrl">URL</param>
        /// <returns>List of posts</returns>
        public List<SyndicationItem> GetFeeds(string feedUrl)
        {
            var lastNews = new List<SyndicationItem>();

            try
            {
                lastNews.AddRange(GetItemsFromUrl(feedUrl));
                lastNews.Sort(delegate (SyndicationItem x, SyndicationItem y) { return y.PublishDate.CompareTo(x.PublishDate); });
            }
            catch (Exception) { }

            return lastNews;
        }
        /// <summary>
        /// Shorten post title
        /// </summary>
        /// <param name="textToShorten">Title</param>
        /// <returns>Shorten title</returns>
        public string Shorten(string textToShorten)
        {
            return textToShorten.Length > BlogSettings.Instance.BlogrollMaxLength
                       ? string.Format("{0}...", textToShorten.Substring(0, BlogSettings.Instance.BlogrollMaxLength).Trim())
                       : textToShorten;
        }

        #region Methods
        private List<SyndicationItem> GetItemsFromUrl(string url)
        {
            var lst = new List<SyndicationItem>();
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            var cnt = 0;
            foreach (SyndicationItem item in feed.Items)
            {
                lst.Add(item);
                cnt++;
                if (cnt > 2)
                {
                    break;
                }
            }
            return lst;
        }

        private string GetXfn(NameValueCollection form)
        {
            var xfn = "";
            if (form["cblXfn_0"] == "on") xfn += "contact,";
            if (form["cblXfn_1"] == "on") xfn += "acquaintance,";
            if (form["cblXfn_2"] == "on") xfn += "friend,";
            if (form["cblXfn_3"] == "on") xfn += "met,";
            if (form["cblXfn_4"] == "on") xfn += "coworker,";
            if (form["cblXfn_5"] == "on") xfn += "colleague,";
            if (form["cblXfn_6"] == "on") xfn += "coresident,";
            if (form["cblXfn_7"] == "on") xfn += "neighbor,";
            if (form["cblXfn_8"] == "on") xfn += "child,";
            if (form["cblXfn_9"] == "on") xfn += "parent,";
            if (form["cblXfn_10"] == "on") xfn += "sibling,";
            if (form["cblXfn_11"] == "on") xfn += "spouse,";
            if (form["cblXfn_12"] == "on") xfn += "kin,";
            if (form["cblXfn_13"] == "on") xfn += "muse,";
            if (form["cblXfn_14"] == "on") xfn += "crush,";
            if (form["cblXfn_15"] == "on") xfn += "date,";
            if (form["cblXfn_16"] == "on") xfn += "sweetheart,";
            if (form["cblXfn_17"] == "on") xfn += "me,";
            if (xfn.Length > 0) xfn = xfn.Substring(0, xfn.Length - 1);
            return xfn;
        }
        #endregion

    }
}
