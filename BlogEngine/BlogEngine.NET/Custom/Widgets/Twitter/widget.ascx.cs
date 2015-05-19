// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.Twitter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.UI.WebControls;
    using System.Xml;

    using App_Code.Controls;

    using BlogEngine.Core;

    /// <summary>
    /// The widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Constants and Fields

        /// <summary>
        /// The link format.
        /// </summary>
        private const string LinkFormat = "<a href=\"{0}{1}\" rel=\"nofollow\">{1}</a>";

        /// <summary>
        /// The twitter feeds cache key.
        /// </summary>
        private const string TwitterFeedsCacheKey = "twits";

        /// <summary>
        /// The twitter settings cache key.
        /// </summary>
        private const string TwitterSettingsCacheKey = "twitter-settings"; // same key used in edit.ascx.cs.

        /// <summary>
        /// The link regex.
        /// </summary>
        private static readonly Regex LinkRegex =
            new Regex(
                "((http://|https://|www\\.)([A-Z0-9.\\-]{1,})\\.[0-9A-Z?;~&\\(\\)#,=\\-_\\./\\+]{2,})", 
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The last feed data file name.
        /// </summary>
        private static Dictionary<Guid, string> lastFeedDataFileName = new Dictionary<Guid, string>();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether or not the widget can be edited.
        ///     <remarks>
        ///         The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        ///     </remarks>
        /// </summary>
        /// <value></value>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets the name. It must be exactly the same as the folder that contains the widget.
        /// </summary>
        /// <value></value>
        public override string Name
        {
            get
            {
                return "Twitter";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluates the replacement for each link match.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <returns>
        /// The evaluator.
        /// </returns>
        public string Evaluator(Match match)
        {
            var info = CultureInfo.InvariantCulture;
            return string.Format(info, LinkFormat, !match.Value.Contains("://") ? "http://" : string.Empty, match.Value);
        }

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            var settings = this.GetTwitterSettings();

            if (settings.AccountUrl != null && !string.IsNullOrEmpty(settings.FollowMeText))
            {
                this.hlTwitterAccount.NavigateUrl = settings.AccountUrl.ToString();
                this.hlTwitterAccount.Text = settings.FollowMeText;
            }

            if (settings.FeedUrl == null)
            {
                return;
            }
            
            if (Blog.CurrentInstance.Cache[TwitterFeedsCacheKey] == null)
            {
                var doc = GetLastFeed();
                if (doc != null)
                {
                    Blog.CurrentInstance.Cache[TwitterFeedsCacheKey] = doc.OuterXml;
                }
            }

            if (Blog.CurrentInstance.Cache[TwitterFeedsCacheKey] != null)
            {
                var xml = (string)Blog.CurrentInstance.Cache[TwitterFeedsCacheKey];
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                this.BindFeed(doc, settings.MaxItems);
            }

            if (DateTime.Now <= settings.LastModified.AddMinutes(settings.PollingInterval))
            {
                return;
            }
            
            settings.LastModified = DateTime.Now;
            BeginGetFeed(settings.FeedUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemDataBound event of the repItems control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.
        /// </param>
        protected void RepItemsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var text = (Label)e.Item.FindControl("lblItem");
            var date = (Label)e.Item.FindControl("lblDate");
            var twit = (Twit)e.Item.DataItem;
            text.Text = twit.Title;
            date.Text = twit.PubDate.ToString("MMMM d. HH:mm");
        }

        /// <summary>
        /// Gets the last name of the feed data file.
        /// </summary>
        /// <returns>
        /// The get last feed data file name.
        /// </returns>
        private static string GetLastFeedDataFileName()
        {
            if (!lastFeedDataFileName.ContainsKey(Blog.CurrentInstance.Id))
            {
                lastFeedDataFileName[Blog.CurrentInstance.Id] = HostingEnvironment.MapPath(Path.Combine(Blog.CurrentInstance.StorageLocation, "twitter_feeds.xml"));
            }

            return lastFeedDataFileName[Blog.CurrentInstance.Id];
        }

        /// <summary>
        /// Saves the last feed.
        /// </summary>
        /// <param name="doc">
        /// The xml doc.
        /// </param>
        private static void SaveLastFeed(XmlDocument doc)
        {
            try
            {
                var file = GetLastFeedDataFileName();
                doc.Save(file);
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Error saving last twitter feed load to data store.  Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Begins the get feed.
        /// </summary>
        /// <param name="url">The URL to get.</param>
        private static void BeginGetFeed(Uri url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;

                GetRequestData data = new GetRequestData()
                {
                    BlogInstanceId = Blog.CurrentInstance.Id,
                    HttpWebRequest = request
                };

                request.BeginGetResponse(EndGetResponse, data);
            }
            catch (Exception ex)
            {
                var msg = "Error requesting Twitter feed.";
                msg += string.Format(" {0}", ex.Message);

                Utils.Log(msg);
            }
        }

        /// <summary>
        /// Binds the feed.
        /// </summary>
        /// <param name="doc">
        /// The xml doc.
        /// </param>
        /// <param name="maxItems">
        /// The max items.
        /// </param>
        private void BindFeed(XmlDocument doc, int maxItems)
        {
            var items = doc.SelectNodes("//channel/item");
            var twits = new List<Twit>();
            var count = 0;
            if (items != null)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    if (count == maxItems)
                    {
                        break;
                    }

                    var node = items[i];
                    var twit = new Twit();
                    var descriptionNode = node.SelectSingleNode("description");
                    if (descriptionNode != null)
                    {
                        var title = descriptionNode.InnerText;

                        if (title.Contains("@"))
                        {
                            continue;
                        }

                        if (title.Contains(":"))
                        {
                            var start = title.IndexOf(":") + 1;
                            title = title.Substring(start);
                        }

                        twit.Title = this.ResolveLinks(title);
                    }

                    var pubdateNode = node.SelectSingleNode("pubDate");
                    if (pubdateNode != null)
                    {
                        twit.PubDate = DateTime.Parse(pubdateNode.InnerText, CultureInfo.InvariantCulture);
                    }

                    var linkNode = node.SelectSingleNode("link");
                    if (linkNode != null)
                    {
                        twit.Url = new Uri(linkNode.InnerText, UriKind.Absolute);
                    }
                    
                    twits.Add(twit);

                    count++;
                }
            }

            twits.Sort();
            this.repItems.DataSource = twits;
            this.repItems.DataBind();
        }

        /// <summary>
        /// Ends the get response.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private static void EndGetResponse(IAsyncResult result)
        {
            try
            {
                GetRequestData data = (GetRequestData)result.AsyncState;
                Blog.InstanceIdOverride = data.BlogInstanceId;

                using (var response = (HttpWebResponse)data.HttpWebRequest.GetResponse())
                {
                    var doc = new XmlDocument();
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        doc.Load(responseStream);
                    }

                    Blog.CurrentInstance.Cache[TwitterFeedsCacheKey] = doc.OuterXml;
                    SaveLastFeed(doc);
                }
            }
            catch (Exception ex)
            {
                var msg = "Error retrieving Twitter feed.";
                msg += string.Format(" {0}", ex.Message);
                Utils.Log(msg);
            }
        }

        /// <summary>
        /// Gets the last feed.
        /// </summary>
        /// <returns>
        /// The xml document.
        /// </returns>
        private static XmlDocument GetLastFeed()
        {
            var file = GetLastFeedDataFileName();
            XmlDocument doc = null;

            try
            {
                if (File.Exists(file))
                {
                    doc = new XmlDocument();
                    doc.Load(file);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Error retrieving last twitter feed load from data store.  Error: " + ex.Message);
            }

            return doc;
        }

        /// <summary>
        /// Gets the twitter settings.
        /// </summary>
        /// <returns>
        /// The twitter settings.
        /// </returns>
        private TwitterSettings GetTwitterSettings()
        {
            var twitterSettings = Blog.CurrentInstance.Cache[TwitterSettingsCacheKey] as TwitterSettings;

            if (twitterSettings != null)
            {
                return twitterSettings;
            }

            twitterSettings = new TwitterSettings();

            // Defaults
            var maxItems = 3;
            var pollingInterval = 15;
            const string FollowMeText = "Follow me";

            var settings = this.GetSettings();

            if (settings.ContainsKey("accounturl") && !string.IsNullOrEmpty(settings["accounturl"]))
            {
                Uri accountUrl;
                Uri.TryCreate(settings["accounturl"], UriKind.Absolute, out accountUrl);
                twitterSettings.AccountUrl = accountUrl;
            }

            if (settings.ContainsKey("feedurl") && !string.IsNullOrEmpty(settings["feedurl"]))
            {
                Uri feedUrl;
                Uri.TryCreate(settings["feedurl"], UriKind.Absolute, out feedUrl);
                twitterSettings.FeedUrl = feedUrl;
            }

            if (settings.ContainsKey("pollinginterval") && !string.IsNullOrEmpty(settings["pollinginterval"]))
            {
                int tempPollingInterval;
                if (int.TryParse(settings["pollinginterval"], out tempPollingInterval))
                {
                    if (tempPollingInterval > 0)
                    {
                        pollingInterval = tempPollingInterval;
                    }
                }
            }

            twitterSettings.PollingInterval = pollingInterval;

            if (settings.ContainsKey("maxitems") && !string.IsNullOrEmpty(settings["maxitems"]))
            {
                int tempMaxItems;
                if (int.TryParse(settings["maxitems"], out tempMaxItems))
                {
                    if (tempMaxItems > 0)
                    {
                        maxItems = tempMaxItems;
                    }
                }
            }

            twitterSettings.MaxItems = maxItems;

            twitterSettings.FollowMeText = settings.ContainsKey("followmetext") &&
                                           !string.IsNullOrEmpty(settings["followmetext"])
                                               ? settings["followmetext"]
                                               : FollowMeText;

            Blog.CurrentInstance.Cache[TwitterSettingsCacheKey] = twitterSettings;

            return twitterSettings;
        }

        /// <summary>
        /// The event handler that is triggered every time a comment is served to a client.
        /// </summary>
        /// <param name="body">
        /// The body string.
        /// </param>
        /// <returns>
        /// The resolve links.
        /// </returns>
        private string ResolveLinks(string body)
        {
            return LinkRegex.Replace(body, new MatchEvaluator(this.Evaluator));
        }

        #endregion

        /// <summary>
        /// Data used during the async HTTP request for tweets.
        /// </summary>
        private class GetRequestData
        {
            public Guid BlogInstanceId { get; set; }
            public HttpWebRequest HttpWebRequest { get; set; }
        }

        /// <summary>
        /// The tweet.
        /// </summary>
        private struct Twit : IComparable<Twit>
        {
            #region Constants and Fields

            /// <summary>
            /// The pub date.
            /// </summary>
            public DateTime PubDate;

            /// <summary>
            /// The title.
            /// </summary>
            public string Title;

            /// <summary>
            /// The url.
            /// </summary>
            public Uri Url;

            #endregion

            #region Implemented Interfaces

            #region IComparable<Twit>

            /// <summary>
            /// The compare to.
            /// </summary>
            /// <param name="other">
            /// The other.
            /// </param>
            /// <returns>
            /// The compare to.
            /// </returns>
            public int CompareTo(Twit other)
            {
                return other.PubDate.CompareTo(this.PubDate);
            }

            #endregion

            #endregion
        }

        /// <summary>
        /// The twitter settings.
        /// </summary>
        internal class TwitterSettings
        {
            #region Constants and Fields

            /// <summary>
            /// The account url.
            /// </summary>
            public Uri AccountUrl;

            /// <summary>
            /// The feed url.
            /// </summary>
            public Uri FeedUrl;

            /// <summary>
            /// The follow me text.
            /// </summary>
            public string FollowMeText;

            /// <summary>
            /// The last modified.
            /// </summary>
            public DateTime LastModified;

            /// <summary>
            /// The max items.
            /// </summary>
            public int MaxItems;

            /// <summary>
            /// The polling interval.
            /// </summary>
            public int PollingInterval;

            #endregion
        }
    }
}