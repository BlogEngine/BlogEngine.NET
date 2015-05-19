namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Trackback Handler
    /// </summary>
    public class TrackbackHandler : IHttpHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The HTML Regex.
        /// </summary>
        private static readonly Regex RegexHtml =
            new Regex(
                @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>",
                RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Whether the source has link.
        /// </summary>
        private bool sourceHasLink;

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when a trackback is accepted as valid and added as a comment.
        /// </summary>
        public static event EventHandler<EventArgs> Accepted;

        /// <summary>
        ///     Occurs when a hit is made to the trackback.axd handler.
        /// </summary>
        public static event EventHandler<CancelEventArgs> Received;

        /// <summary>
        ///     Occurs when a trackback request is rejected because the sending
        ///     website already made a trackback or pingback to the specific page.
        /// </summary>
        public static event EventHandler<EventArgs> Rejected;

        /// <summary>
        ///     Occurs when the request comes from a spammer.
        /// </summary>
        public static event EventHandler<EventArgs> Spammed;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when [spammed].
        /// </summary>
        /// <param name="url">The URL string.</param>
        public static void OnSpammed(string url)
        {
            if (Spammed != null)
            {
                Spammed(url, EventArgs.Empty);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that 
        ///     implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> 
        ///     object that provides references to the intrinsic server objects 
        ///     (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            if (!BlogSettings.Instance.IsCommentsEnabled || !BlogSettings.Instance.EnableTrackBackReceive)
            {
                context.Response.StatusCode = 404;
                context.Response.End();
            }

            var e = new CancelEventArgs();
            this.OnReceived(e);
            if (e.Cancel)
            {
                return;
            }

            var postId = context.Request.Params["id"];

            var title = context.Request.Params["title"];
            var excerpt = context.Request.Params["excerpt"];
            var blogName = context.Request.Params["blog_name"];
            var url = string.Empty;

            if (context.Request.Params["url"] != null)
            {
                url = context.Request.Params["url"].Split(',')[0];
            }

            Post post;

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(postId) && !string.IsNullOrEmpty(blogName) &&
                postId.Length == 36)
            {
                post = Post.GetPost(new Guid(postId));
                this.ExamineSourcePage(url, post.AbsoluteLink.ToString());
                var containsHtml = !string.IsNullOrEmpty(excerpt) &&
                                   (RegexHtml.IsMatch(excerpt) || RegexHtml.IsMatch(title) ||
                                    RegexHtml.IsMatch(blogName));

                if (IsFirstPingBack(post, url) && this.sourceHasLink && !containsHtml)
                {
                    AddComment(url, post, blogName, title, excerpt);
                    this.OnAccepted(url);
                    context.Response.Write(
                        "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>0</error></response>");
                    context.Response.End();
                }
                else if (!IsFirstPingBack(post, url))
                {
                    this.OnRejected(url);
                    context.Response.Write(
                        "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>Trackback already registered</error></response>");
                    context.Response.End();
                }
                else if (!this.sourceHasLink || containsHtml)
                {
                    OnSpammed(url);
                    context.Response.Write(
                        "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>The source page does not link</error></response>");
                    context.Response.End();
                }
            }
            else
            {
                context.Response.Redirect(Utils.RelativeWebRoot);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Called when [accepted].
        /// </summary>
        /// <param name="url">The URL string.</param>
        protected virtual void OnAccepted(string url)
        {
            if (Accepted != null)
            {
                Accepted(url, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="Received"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnReceived(CancelEventArgs e)
        {
            if (Received != null)
            {
                Received(null, e);
            }
        }

        /// <summary>
        /// Called when [rejected].
        /// </summary>
        /// <param name="url">The URL string.</param>
        protected virtual void OnRejected(string url)
        {
            if (Rejected != null)
            {
                Rejected(url, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Insert the pingback as a comment on the post.
        /// </summary>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <param name="post">
        /// The post to comment on.
        /// </param>
        /// <param name="blogName">
        /// The blog Name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="excerpt">
        /// The excerpt.
        /// </param>
        private static void AddComment(string sourceUrl, Post post, string blogName, string title, string excerpt)
        {
            var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Author = blogName,
                    Website = new Uri(sourceUrl),
                    Content = title + Environment.NewLine + Environment.NewLine + excerpt,
                    Email = "trackback",
                    Parent = post,
                    DateCreated = DateTime.Now,
                    IP = Utils.GetClientIP(),
                    IsApproved = true
                };
            post.AddComment(comment);
        }

        /// <summary>
        /// Checks to see if the source has already pinged the target.
        ///     If it has, there is no reason to add it again.
        /// </summary>
        /// <param name="post">
        /// The post to check.
        /// </param>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <returns>
        /// Whether is first ping back.
        /// </returns>
        private static bool IsFirstPingBack(Post post, string sourceUrl)
        {
            return
                !post.Comments.Any(
                    comment =>
                        comment.Website != null &&
                        comment.Website.ToString().Equals(sourceUrl, StringComparison.OrdinalIgnoreCase)) &&
                !post.Comments.Any(
                    comment =>
                        comment.IP != null &&
                        comment.IP == Utils.GetClientIP());
        }

        /// <summary>
        /// Parse the HTML of the source page.
        /// </summary>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <param name="targetUrl">
        /// The target Url.
        /// </param>
        private void ExamineSourcePage(string sourceUrl, string targetUrl)
        {
            try
            {
                var remoteFile = new RemoteFile(new Uri(sourceUrl), true);
                var html = remoteFile.GetFileAsString();
                this.sourceHasLink = html.ToUpperInvariant().Contains(targetUrl.ToUpperInvariant());
                
            }
            catch (WebException)
            {
                this.sourceHasLink = false;

                // throw new ArgumentException("Trackback sender does not exists: " + sourceUrl, ex);
            }
        }

        #endregion
    }
}