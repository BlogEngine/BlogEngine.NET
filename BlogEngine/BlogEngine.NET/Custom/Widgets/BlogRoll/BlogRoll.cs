using BlogEngine.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace BlogEngine.NET.Custom.Widgets
{
    public class BlogRoll
    {
        public BlogRoll()
        {
            Xfn = new XfnList();
        }

        public BlogRoll(string id)
        {
            var gId = Guid.Parse(id);
            var rolls = BlogRollItem.BlogRolls;
            var broll = rolls.Where(r => r.Id == gId).FirstOrDefault();

            Title = broll.Title;
            Description = broll.Description;
            WebSite = broll.BlogUrl.ToString();
            FeedUrl = broll.FeedUrl.ToString();

            var x = broll.Xfn.Split(' ').ToList();
            Xfn = new XfnList();

            if (x.Contains("contact")) { Xfn.contact = true; }
            if (x.Contains("acquaintance")) { Xfn.acquaintance = true; }
            if (x.Contains("friend")) { Xfn.friend = true; }
            if (x.Contains("met")) { Xfn.met = true; }
            if (x.Contains("coworker")) { Xfn.coworker = true; }
            if (x.Contains("colleague")) { Xfn.colleague = true; }

            if (x.Contains("coresident")) { Xfn.coresident = true; }
            if (x.Contains("neighbor")) { Xfn.neighbor = true; }
            if (x.Contains("child")) { Xfn.child = true; }
            if (x.Contains("parent")) { Xfn.parent = true; }
            if (x.Contains("sibling")) { Xfn.sibling = true; }
            if (x.Contains("spouse")) { Xfn.spouse = true; }

            if (x.Contains("kin")) { Xfn.kin = true; }
            if (x.Contains("muse")) { Xfn.muse = true; }
            if (x.Contains("crush")) { Xfn.crush = true; }
            if (x.Contains("date")) { Xfn.date = true; }
            if (x.Contains("sweetheart")) { Xfn.sweetheart = true; }
            if (x.Contains("me")) { Xfn.me = true; }
        }

        #region Puplic properties

        public string Title { get; set; }
        public string Description { get; set; }
        public string WebSite { get; set; }
        public string FeedUrl { get; set; }
        public XfnList Xfn { get; set; }

        #endregion

        public void SaveForm(NameValueCollection form)
        {
            Title = form["txtTitle"];
            Description = form["txtDesc"];
            WebSite = form["txtWebsite"];
            FeedUrl = form["txtUrl"];

            Xfn.contact = form["cblXfn_0"] == "on" ? true : false;
            Xfn.acquaintance = form["cblXfn_1"] == "on" ? true : false;
            Xfn.friend = form["cblXfn_2"] == "on" ? true : false;
            Xfn.met = form["cblXfn_3"] == "on" ? true : false;
            Xfn.coworker = form["cblXfn_4"] == "on" ? true : false;
            Xfn.colleague = form["cblXfn_5"] == "on" ? true : false;
            Xfn.coresident = form["cblXfn_6"] == "on" ? true : false;
            Xfn.neighbor = form["cblXfn_7"] == "on" ? true : false;
            Xfn.child = form["cblXfn_8"] == "on" ? true : false;
            Xfn.parent = form["cblXfn_9"] == "on" ? true : false;
            Xfn.sibling = form["cblXfn_10"] == "on" ? true : false;
            Xfn.spouse = form["cblXfn_11"] == "on" ? true : false;
            Xfn.kin = form["cblXfn_12"] == "on" ? true : false;
            Xfn.muse = form["cblXfn_13"] == "on" ? true : false;
            Xfn.crush = form["cblXfn_14"] == "on" ? true : false;
            Xfn.date = form["cblXfn_15"] == "on" ? true : false;
            Xfn.sweetheart = form["cblXfn_16"] == "on" ? true : false;
            Xfn.me = form["cblXfn_17"] == "on" ? true : false;
        }

        public string GetXfnString(NameValueCollection form)
        {
            var s = "";

            if (form["cblXfn_0"] == "on") { s += "contact "; }
            if (form["cblXfn_1"] == "on") { s += "acquaintance "; }
            if (form["cblXfn_2"] == "on") { s += "friend "; }
            if (form["cblXfn_3"] == "on") { s += "met "; }
            if (form["cblXfn_4"] == "on") { s += "coworker "; }
            if (form["cblXfn_5"] == "on") { s += "colleague "; }

            if (form["cblXfn_6"] == "on") { s += "coresident "; }
            if (form["cblXfn_7"] == "on") { s += "neighbor "; }
            if (form["cblXfn_8"] == "on") { s += "child "; }
            if (form["cblXfn_9"] == "on") { s += "parent "; }
            if (form["cblXfn_10"] == "on") { s += "sibling "; }
            if (form["cblXfn_11"] == "on") { s += "spouse "; }

            if (form["cblXfn_12"] == "on") { s += "kin "; }
            if (form["cblXfn_13"] == "on") { s += "muse "; }
            if (form["cblXfn_14"] == "on") { s += "crush "; }
            if (form["cblXfn_15"] == "on") { s += "date "; }
            if (form["cblXfn_16"] == "on") { s += "sweetheart "; }
            if (form["cblXfn_17"] == "on") { s += "me "; }

            return s.Trim();
        }

        public void Add(NameValueCollection form)
        {
            BlogRollItem br = new BlogRollItem();
            br.Title = form["txtTitle"].Replace(@"\", "'");
            br.Description = form["txtDesc"];
            br.BlogUrl = new Uri(form["txtWebsite"]);
            br.FeedUrl = new Uri(form["txtUrl"]);

            br.Xfn = GetXfnString(form);

            int largestSortIndex = -1;
            foreach (BlogRollItem brExisting in BlogRollItem.BlogRolls)
            {
                if (brExisting.SortIndex > largestSortIndex)
                {
                    largestSortIndex = brExisting.SortIndex;
                }
            }
            br.SortIndex = largestSortIndex + 1;
            br.Save();
        }

        public void Update(NameValueCollection form, string id)
        {
            try
            {
                var br = BlogRollItem.GetBlogRollItem(Guid.Parse(id));
                if (br != null)
                {
                    br.Title = form["txtTitle"].Replace(@"\", "'");
                    br.Description = form["txtDesc"];
                    br.BlogUrl = new Uri(form["txtWebsite"]);
                    br.FeedUrl = new Uri(form["txtUrl"]);
                    br.Xfn = GetXfnString(form);
                    br.Save();
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Custom.Widgets.BlogRoll.Update", ex);
            }
        }

        public void Delete(string id)
        {
            try
            {
                var br = BlogRollItem.GetBlogRollItem(Guid.Parse(id));
                br.Delete();
                br.Save();
            }
            catch (Exception ex)
            {
                Utils.Log("Custom.Widgets.BlogRoll.Delete", ex);
            }
            
        }


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
                if(cnt > 2)
                {
                    break;
                }
            }
            return lst;
        }
        public string Shorten(string textToShorten)
        {
            return HttpUtility.HtmlEncode(textToShorten.Length > BlogSettings.Instance.BlogrollMaxLength
                       ? string.Format("{0}...", textToShorten.Substring(0, BlogSettings.Instance.BlogrollMaxLength).Trim())
                       : textToShorten);
        }

    }
    public class XfnList
    {
        public bool contact { get; set; }
        public bool acquaintance { get; set; }
        public bool friend { get; set; }
        public bool met { get; set; }
        public bool coworker { get; set; }
        public bool colleague { get; set; }
        public bool coresident { get; set; }
        public bool neighbor { get; set; }
        public bool child { get; set; }
        public bool parent { get; set; }
        public bool sibling { get; set; }
        public bool spouse { get; set; }
        public bool kin { get; set; }
        public bool muse { get; set; }
        public bool crush { get; set; }
        public bool date { get; set; }
        public bool sweetheart { get; set; }
        public bool me { get; set; }
    }
}