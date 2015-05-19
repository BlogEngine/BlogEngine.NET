namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Recieves pingbacks from other blogs and websites, and 
    ///     registers them as a comment.
    /// </summary>
    public class PingbackHandler : IHttpHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The success.
        /// </summary>
        private const string Success =
            "<methodResponse><params><param><value><string>Thanks!</string></value></param></params></methodResponse>";

        /// <summary>
        /// The regex html.
        /// </summary>
        private static readonly Regex RegexHtml =
            new Regex(
                @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>",
                RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The regex title.
        /// </summary>
        private static readonly Regex RegexTitle = new Regex(
            @"(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Whether contains html.
        /// </summary>
        private bool containsHtml;

        /// <summary>
        /// Whether source has link.
        /// </summary>
        private bool sourceHasLink;

        /// <summary>
        /// The title.
        /// </summary>
        private string title;

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when a pingback is accepted as valid and added as a comment.
        /// </summary>
        public static event EventHandler<EventArgs> Accepted;

        /// <summary>
        ///     Occurs when a hit is made to the trackback.axd handler.
        /// </summary>
        public static event EventHandler<CancelEventArgs> Received;

        /// <summary>
        ///     Occurs when a pingback request is rejected because the sending
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
            if (!BlogSettings.Instance.IsCommentsEnabled || !BlogSettings.Instance.EnablePingBackReceive)
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

            var doc = RetrieveXmlDocument(context);
            var list = doc.SelectNodes("methodCall/params/param/value/string") ??
                       doc.SelectNodes("methodCall/params/param/value");

            if (list == null)
            {
                return;
            }

            var sourceUrl = list[0].InnerText.Trim();
            var targetUrl = list[1].InnerText.Trim();

            this.ExamineSourcePage(sourceUrl, targetUrl);
            context.Response.ContentType = "text/xml";

            var post = GetPostByUrl(targetUrl);
            if (post != null)
            {
                if (IsFirstPingBack(post, sourceUrl))
                {
                    if (this.sourceHasLink && !this.containsHtml)
                    {
                        this.AddComment(sourceUrl, post);
                        this.OnAccepted(sourceUrl);
                        context.Response.Write(Success);
                    }
                    else if (!this.sourceHasLink)
                    {
                        SendError(
                            context,
                            17,
                            "The source URI does not contain a link to the target URI, and so cannot be used as a source.");
                    }
                    else
                    {
                        OnSpammed(sourceUrl);

                        // Don't let spammers know we exist.
                        context.Response.StatusCode = 404;
                    }
                }
                else
                {
                    this.OnRejected(sourceUrl);
                    SendError(context, 48, "The pingback has already been registered.");
                }
            }
            else
            {
                SendError(context, 32, "The specified target URI does not exist.");
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
        /// Parse the source URL to get the domain.
        ///     It is used to fill the Author property of the comment.
        /// </summary>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <returns>
        /// The get domain.
        /// </returns>
        private static string GetDomain(string sourceUrl)
        {
            var start = sourceUrl.IndexOf("://") + 3;
            var stop = sourceUrl.IndexOf("/", start);
            return sourceUrl.Substring(start, stop - start).Replace("www.", string.Empty);
        }

        /// <summary>
        /// Retrieve the post that belongs to the target URL.
        /// </summary>
        /// <param name="url">The url string.</param>
        /// <returns>The post from the url.</returns>
        private static Post GetPostByUrl(string url)
        {
            var start = url.LastIndexOf("/") + 1;
            var stop = url.ToUpperInvariant().IndexOf(".ASPX");
            var name = url.Substring(start, stop - start).ToLowerInvariant();

            return (from post in Post.Posts
                    let legalTitle = Utils.RemoveIllegalCharacters(post.Title).ToLowerInvariant()
                    where name == legalTitle
                    select post).FirstOrDefault();
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
        /// The is first ping back.
        /// </returns>
        private static bool IsFirstPingBack(Post post, string sourceUrl)
        {
            foreach (var comment in post.Comments)
            {
                if (comment.Website != null &&
                    comment.Website.ToString().Equals(sourceUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (comment.IP != null && comment.IP == Utils.GetClientIP())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Retrieves the content of the input stream
        ///     and return it as plain text.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The parse request.
        /// </returns>
        private static string ParseRequest(HttpContext context)
        {
            var buffer = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(buffer, 0, buffer.Length);

            return Encoding.Default.GetString(buffer);
        }

        /// <summary>
        /// The retrieve xml document.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// An Xml Document.
        /// </returns>
        private static XmlDocument RetrieveXmlDocument(HttpContext context)
        {
            var xml = ParseRequest(context);
            if (!xml.Contains("<methodName>pingback.ping</methodName>"))
            {
                context.Response.StatusCode = 404;
                context.Response.End();
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        /// <summary>
        /// The send error.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="code">
        /// The code number.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void SendError(HttpContext context, int code, string message)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\"?>");
            sb.Append("<methodResponse>");
            sb.Append("<fault>");
            sb.Append("<value>");
            sb.Append("<struct>");
            sb.Append("<member>");
            sb.Append("<name>faultCode</name>");
            sb.AppendFormat("<value><int>{0}</int></value>", code);
            sb.Append("</member>");
            sb.Append("<member>");
            sb.Append("<name>faultString</name>");
            sb.AppendFormat("<value><string>{0}</string></value>", message);
            sb.Append("</member>");
            sb.Append("</struct>");
            sb.Append("</value>");
            sb.Append("</fault>");
            sb.Append("</methodResponse>");

            context.Response.Write(sb.ToString());
        }

        /// <summary>
        /// Insert the pingback as a comment on the post.
        /// </summary>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <param name="post">
        /// The post to add the comment to.
        /// </param>
        private void AddComment(string sourceUrl, Post post)
        {
            var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Author = GetDomain(sourceUrl),
                    Website = new Uri(sourceUrl)
                };
            comment.Content = string.Format("Pingback from {0}{1}{2}{3}", comment.Author, Environment.NewLine, Environment.NewLine, this.title);
            comment.DateCreated = DateTime.Now;
            comment.Email = "pingback";
            comment.IP = Utils.GetClientIP();
            comment.Parent = post;
            comment.IsApproved = true; // NOTE: Pingback comments are approved by default.
            post.AddComment(comment);
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
                this.title = RegexTitle.Match(html).Value.Trim();
                this.containsHtml = RegexHtml.IsMatch(this.title);
                this.sourceHasLink = html.ToUpperInvariant().Contains(targetUrl.ToUpperInvariant());
            }
            catch (WebException)
            {
                this.sourceHasLink = false;
            }
        }

        #endregion
    }
}