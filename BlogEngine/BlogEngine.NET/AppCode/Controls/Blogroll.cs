// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Creates and displays a dynamic blogroll.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Xml;

    using BlogEngine.Core;

    /// <summary>
    /// Creates and displays a dynamic blogroll.
    /// </summary>
    public class Blogroll : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Blogroll"/> class.
        /// </summary>
        public Blogroll()
        {
            this.ShowRssIcon = true;
            BlogRollItem.Saved += BlogRollItemSaved;
        }

        /// <summary>
        /// Handles the Saved event of the BlogRollItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        private static void BlogRollItemSaved(object sender, SavedEventArgs e)
        {
            switch (e.Action)
            {
                case SaveAction.Insert:
                    AddBlog((BlogRollItem)sender);
                    break;
                case SaveAction.Delete:
                    var affected = Items.FirstOrDefault(req => req.RollItem.Equals(sender));
                    Items.Remove(affected);
                    break;
            }

            if (((e.Action == SaveAction.Insert || e.Action == SaveAction.Delete) || e.Action == SaveAction.Update) &&
                Items.Count > 0)
            {
                // Re-sort _Items collection in case sorting of blogroll items was changed.
                Items.Sort((br1, br2) => br1.RollItem.CompareTo(br2.RollItem));
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Page.IsPostBack || this.Page.IsCallback)
            {
                return;
            }

            var ul = this.DisplayBlogroll();
            using (var sw = new StringWriter())
            {
                ul.RenderControl(new HtmlTextWriter(sw));
                var html = sw.ToString();

                writer.WriteLine("<div id=\"blogroll\">");
                writer.WriteLine(html);
                writer.WriteLine("</div>");
            }
        }

        #region Private fields

        /// <summary>
        /// The last updated.
        /// </summary>
        private static Dictionary<Guid, DateTime> blogsLastUpdated = new Dictionary<Guid, DateTime>();

        /// <summary>
        /// The items.
        /// </summary>
        private static Dictionary<Guid, List<BlogRequest>> blogsItems = new Dictionary<Guid, List<BlogRequest>>();

        #endregion

        #region Properties

        private static DateTime LastUpdated
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;

                if (!blogsLastUpdated.ContainsKey(blogId))
                {
                    lock (SyncRoot)
                    {
                        if (!blogsLastUpdated.ContainsKey(blogId))
                        {
                            blogsLastUpdated[blogId] = DateTime.Now;
                        }
                    }
                }

                return blogsLastUpdated[blogId];
            }
            set
            {
                blogsLastUpdated[Blog.CurrentInstance.Id] = value;
            }
        }

        private static List<BlogRequest> Items
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;

                if (!blogsItems.ContainsKey(blogId))
                {
                    lock (SyncRoot)
                    {
                        if (!blogsItems.ContainsKey(blogId))
                        {
                            blogsItems[blogId] = new List<BlogRequest>();
                        }
                    }
                }

                return blogsItems[blogId];
            }
        }

        /// <summary>
        ///     Determines whether the RSS icon is displayed (default true)
        /// </summary>
        public bool ShowRssIcon { get; set; }

        /// <summary>
        ///     Display the description of the blog as a tool tip (default false)
        /// </summary>
        public bool DescriptionToolTip { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Displays the RSS item collection.
        /// </summary>
        /// <returns>
        /// The output.
        /// </returns>
        private HtmlGenericControl DisplayBlogroll()
        {
            if (DateTime.Now > LastUpdated.AddMinutes(BlogSettings.Instance.BlogrollUpdateMinutes) &&
                BlogSettings.Instance.BlogrollVisiblePosts > 0)
            {
                Items.Clear();
                LastUpdated = DateTime.Now;
            }

            if (Items.Count == 0)
            {
                lock (SyncRoot)
                {
                    if (Items.Count == 0)
                    {
                        if (BlogRollItem.BlogRolls != null)
                        {
                            foreach (var roll in BlogRollItem.BlogRolls)
                            {
                                AddBlog(roll);
                            }
                        }
                    }
                }
            }

            return this.BindControls();
        }

        /// <summary>
        /// Parses the processed RSS items and returns the HTML
        /// </summary>
        /// <returns>
        /// The output.
        /// </returns>
        private HtmlGenericControl BindControls()
        {
            var ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "xoxo");
            foreach (var item in Items)
            {
                HtmlAnchor feedAnchor = null;

                if (this.ShowRssIcon)
                {
                    feedAnchor = new HtmlAnchor { HRef = item.RollItem.FeedUrl.AbsoluteUri };

                    using (var image = new HtmlImage
                        {
                            Src = string.Format("{0}Content/images/blog/rssButton.png", Utils.ApplicationRelativeWebRoot),
                            Alt = string.Format("RSS feed for {0}", item.RollItem.Title)
                        })
                    {
                        image.Width = 12;
                        image.Height = 12;
                        feedAnchor.Controls.Add(image);
                    }
                }

                var webAnchor = new HtmlAnchor
                {
                    HRef = item.RollItem.BlogUrl.AbsoluteUri,
                    InnerHtml = EnsureLength(item.RollItem.Title)
                };

                if (this.DescriptionToolTip)
                {
                    webAnchor.Attributes["title"] = item.RollItem.Description;
                }

                if (!String.IsNullOrEmpty(item.RollItem.Xfn))
                {
                    webAnchor.Attributes["rel"] = item.RollItem.Xfn;
                }

                using (var li = new HtmlGenericControl("li"))
                {
                    if (null != feedAnchor)
                    {
                        li.Controls.Add(feedAnchor);
                    }

                    li.Controls.Add(webAnchor);

                    AddRssChildItems(item, li);
                    ul.Controls.Add(li);
                }
            }

            return ul;
        }

        /// <summary>
        /// Adds the RSS child items.
        /// </summary>
        /// <param name="item">The blog request item.</param>
        /// <param name="li">The list item.</param>
        private static void AddRssChildItems(BlogRequest item, HtmlGenericControl li)
        {
            if (item.ItemTitles.Count <= 0 || BlogSettings.Instance.BlogrollVisiblePosts <= 0)
            {
                return;
            }

            using (var div = new HtmlGenericControl("ul"))
            {
                for (var i = 0; i < item.ItemTitles.Count; i++)
                {
                    if (i >= BlogSettings.Instance.BlogrollVisiblePosts)
                    {
                        break;
                    }

                    var subLi = new HtmlGenericControl("li");
                    using (var a = new HtmlAnchor
                        {
                            HRef = item.ItemLinks[i], Title = HttpUtility.HtmlEncode(item.ItemTitles[i]), InnerHtml = EnsureLength(item.ItemTitles[i])
                        })
                    {
                        subLi.Controls.Add(a);
                    }

                    div.Controls.Add(subLi);
                }

                li.Controls.Add(div);
            }
        }

        /// <summary>
        /// Ensures that the name is no longer than the MaxLength.
        /// </summary>
        /// <param name="textToShorten">
        /// The text To Shorten.
        /// </param>
        /// <returns>
        /// The ensure length.
        /// </returns>
        private static string EnsureLength(string textToShorten)
        {
			return HttpUtility.HtmlEncode(textToShorten.Length > BlogSettings.Instance.BlogrollMaxLength
                       ? string.Format("{0}...", textToShorten.Substring(0, BlogSettings.Instance.BlogrollMaxLength).Trim())
                       : textToShorten);
        }

        /// <summary>
        /// Adds a blog to the item collection and start retrieving the blogs.
        /// </summary>
        /// <param name="br">
        /// The blogroll item.
        /// </param>
        private static void AddBlog(BlogRollItem br)
        {
            var affected = Items.FirstOrDefault(r => r.RollItem.Equals(br));
            if (affected != null)
            {
                return;
            }

            var req = (HttpWebRequest)WebRequest.Create(br.FeedUrl);
            req.Credentials = CredentialCache.DefaultNetworkCredentials;

            var blogRequest = new BlogRequest(br, req);
            Items.Add(blogRequest);

            GetRequestData data = new GetRequestData()
            {
                BlogInstanceId = Blog.CurrentInstance.Id,
                BlogRequest = blogRequest
            };

            req.BeginGetResponse(ProcessResponse, data);
        }

        /// <summary>
        /// Gets the request and processes the response.
        /// </summary>
        /// <param name="async">
        /// The async result.
        /// </param>
        private static void ProcessResponse(IAsyncResult asyncResult)
        {
            GetRequestData data = (GetRequestData)asyncResult.AsyncState;
            Blog.InstanceIdOverride = data.BlogInstanceId;
            var blogReq = data.BlogRequest;

            try
            {
                using (var response = (HttpWebResponse)blogReq.Request.EndGetResponse(asyncResult))
                {
                    var doc = new XmlDocument();
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        doc.Load(responseStream);
                    }

                    var nodes = doc.SelectNodes("rss/channel/item");
                    if (nodes != null)
                    {
                        foreach (XmlNode node in nodes)
                        {
                            var titleNode = node.SelectSingleNode("title");
                            var linkNode = node.SelectSingleNode("link");
                            if (titleNode == null || linkNode == null)
                            {
                                continue;
                            }

                            var title = titleNode.InnerText;
                            var link = linkNode.InnerText;

                            // var date = DateTime.Now;
                            // if (node.SelectSingleNode("pubDate") != null)
                            // {
                            //     date = DateTime.Parse(node.SelectSingleNode("pubDate").InnerText);
                            // }
                            blogReq.ItemTitles.Add(title);
                            blogReq.ItemLinks.Add(link);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("App_Code.Controls.Blogroll.ProcessRequest", ex);
            }
        }

        #endregion

        /// <summary>
        /// Data used during the async HTTP request for blogrolls.
        /// </summary>
        private class GetRequestData
        {
            public Guid BlogInstanceId { get; set; }
            public BlogRequest BlogRequest { get; set; }
        }
    }

    /// <summary>
    /// The blog request.
    /// </summary>
    internal class BlogRequest
    {
        /// <summary>
        /// Gets or sets the roll item.
        /// </summary>
        /// <value>The roll item.</value>
        internal BlogRollItem RollItem { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        internal HttpWebRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the item titles.
        /// </summary>
        /// <value>The item titles.</value>
        internal List<string> ItemTitles { get; set; }

        /// <summary>
        /// Gets or sets the item links.
        /// </summary>
        /// <value>The item links.</value>
        internal List<string> ItemLinks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogRequest"/> class.
        /// </summary>
        /// <param name="rollItem">
        /// The roll item.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        internal BlogRequest(BlogRollItem rollItem, HttpWebRequest request)
        {
            this.ItemTitles = new List<string>();
            this.ItemLinks = new List<string>();
            this.RollItem = rollItem;
            this.Request = request;
        }
    }
}