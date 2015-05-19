/* Author:      Joel Thoms (http://joel.net)
 * Copyright:   2006 Joel Thoms (http://joel.net)
 * About:       Akismet (http://akismet.com) .Net 2.0 API allow you to check
 *              Akismet's spam database to verify your comments and prevent spam.
 * 
 * Note:        Do not launch 'DEBUG' code on your site.  Only build 'RELEASE' for your site.  Debug code contains
 *              Console.WriteLine's, which are not desireable on a website.
*/

namespace Joel.Net
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web;
    
    #region - public class AkismetComment -

    /// <summary>
    /// The akismet comment.
    /// </summary>
    public class AkismetComment
    {
        #region Constants and Fields

        /// <summary>
        ///     The blog.
        /// </summary>
        public string Blog { get; set; }

        /// <summary>
        ///     The comment author.
        /// </summary>
        public string CommentAuthor { get; set; }

        /// <summary>
        ///     The comment author email.
        /// </summary>
        public string CommentAuthorEmail { get; set; }

        /// <summary>
        ///     The comment author url.
        /// </summary>
        public string CommentAuthorUrl { get; set; }

        /// <summary>
        ///     The comment content.
        /// </summary>
        public string CommentContent { get; set; }

        /// <summary>
        ///     The comment type.
        /// </summary>
        public string CommentType { get; set; }

        /// <summary>
        ///     The permalink.
        /// </summary>
        public string Permalink { get; set; }

        /// <summary>
        ///     The referrer.
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        ///     The user agent.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     The user ip.
        /// </summary>
        public string UserIp { get; set; }

        #endregion
    }

    #endregion

    /// <summary>
    /// The akismet.
    /// </summary>
    public class Akismet
    {
        #region Constants and Fields

        /// <summary>
        ///     The api key.
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        ///     The blog string.
        /// </summary>
        private readonly string blog;

        /// <summary>
        ///     The comment check url.
        /// </summary>
        private readonly string commentCheckUrl = "http://{0}.rest.akismet.com/1.1/comment-check";

        /// <summary>
        ///     The submit ham url.
        /// </summary>
        private readonly string submitHamUrl = "http://{0}.rest.akismet.com/1.1/submit-ham";

        /// <summary>
        ///     The submit spam url.
        /// </summary>
        private readonly string submitSpamUrl = "http://{0}.rest.akismet.com/1.1/submit-spam";

        /// <summary>
        ///     The user agent.
        /// </summary>
        private readonly string userAgent = "Joel.Net's Akismet API/1.0";

        /// <summary>
        ///     The verify url.
        /// </summary>
        private readonly string verifyUrl = "http://rest.akismet.com/1.1/verify-key";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Akismet"/> class. 
        ///     Creates an Akismet API object.
        /// </summary>
        /// <param name="apiKey">
        /// Your wordpress.com API key.
        /// </param>
        /// <param name="blog">
        /// URL to your blog
        /// </param>
        /// <param name="userAgent">
        /// Name of application using API.  example: "Joel.Net's Akismet API/1.0"
        /// </param>
        /// <remarks>
        /// Accepts required fields 'apiKey', 'blog', 'userAgent'.
        /// </remarks>
        public Akismet(string apiKey, string blog, string userAgent)
        {
            this.CharSet = "UTF-8";
            this.apiKey = apiKey;
            if (userAgent != null)
            {
                this.userAgent = string.Format("{0} | Akismet/1.11", userAgent);
            }

            this.blog = blog;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Akismet"/> class. 
        ///     Create an Akismet API object
        /// </summary>
        /// <param name="apiKey">
        /// Your wordpress.com API key.
        /// </param>
        /// <param name="blog">
        /// URL to your blog
        /// </param>
        /// <param name="userAgent">
        /// Name of application using API.  example: "Joel.Net's Akismet API/1.0"
        /// </param>
        /// <param name="serviceUrl">
        /// The url for the service (uses Akismet service by default)
        /// </param>
        public Akismet(string apiKey, string blog, string userAgent, string serviceUrl)
            : this(apiKey, blog, userAgent)
        {
            this.verifyUrl = string.Format("http://{0}/1.1/verify-key", serviceUrl);
            this.commentCheckUrl = string.Format("http://{{0}}.{0}/1.1/comment-check", serviceUrl);
            this.submitSpamUrl = string.Format("http://{{0}}.{0}/1.1/submit-spam", serviceUrl);
            this.submitHamUrl = string.Format("http://{{0}}.{0}/1.1/submit-ham", serviceUrl);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the character set.
        /// </summary>
        public string CharSet { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks TypePadComment object against Akismet database.
        /// </summary>
        /// <param name="comment">
        /// TypePadComment object to check.
        /// </param>
        /// <returns>
        /// 'True' if spam, 'False' if not spam.
        /// </returns>
        public bool CommentCheck(AkismetComment comment)
        {
            var value =
                Convert.ToBoolean(
                    this.HttpPost(String.Format(this.commentCheckUrl, this.apiKey), CreateData(comment), this.CharSet));
            return value;
        }

        /// <summary>
        /// Retracts false positive from Akismet database.
        /// </summary>
        /// <param name="comment">
        /// TypePadComment object to retract.
        /// </param>
        public void SubmitHam(AkismetComment comment)
        {
            this.HttpPost(String.Format(this.submitHamUrl, this.apiKey), CreateData(comment), this.CharSet);
        }

        /// <summary>
        /// Submits TypePadComment object into Akismet database.
        /// </summary>
        /// <param name="comment">
        /// TypePadComment object to submit.
        /// </param>
        public void SubmitSpam(AkismetComment comment)
        {
            this.HttpPost(String.Format(this.submitSpamUrl, this.apiKey), CreateData(comment), this.CharSet);
        }

        /// <summary>
        /// Verifies your wordpress.com key.
        /// </summary>
        /// <returns>
        /// 'True' is key is valid.
        /// </returns>
        public bool VerifyKey()
        {
            var response = this.HttpPost(
                this.verifyUrl, 
                String.Format("key={0}&blog={1}", this.apiKey, HttpUtility.UrlEncode(this.blog)), 
                this.CharSet);
            var value = (response == "valid") ? true : false;
            return value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Takes an TypePadComment object and returns an (escaped) string of data to POST.
        /// </summary>
        /// <param name="comment">
        /// TypePadComment object to translate.
        /// </param>
        /// <returns>
        /// A System.String containing the data to POST to Akismet API.
        /// </returns>
        private static string CreateData(AkismetComment comment)
        {
            var value =
                String.Format(
                    "blog={0}&user_ip={1}&user_agent={2}&referrer={3}&permalink={4}&comment_type={5}" +
                    "&comment_author={6}&comment_author_email={7}&comment_author_url={8}&comment_content={9}", 
                    HttpUtility.UrlEncode(comment.Blog), 
                    HttpUtility.UrlEncode(comment.UserIp), 
                    HttpUtility.UrlEncode(comment.UserAgent), 
                    HttpUtility.UrlEncode(comment.Referrer), 
                    HttpUtility.UrlEncode(comment.Permalink), 
                    HttpUtility.UrlEncode(comment.CommentType), 
                    HttpUtility.UrlEncode(comment.CommentAuthor), 
                    HttpUtility.UrlEncode(comment.CommentAuthorEmail), 
                    HttpUtility.UrlEncode(comment.CommentAuthorUrl), 
                    HttpUtility.UrlEncode(comment.CommentContent));

            return value;
        }

        /// <summary>
        /// Performs a web request.
        /// </summary>
        /// <param name="url">
        /// The URL of the request.
        /// </param>
        /// <param name="data">
        /// The data to post.
        /// </param>
        /// <param name="charset">
        /// The char set.
        /// </param>
        /// <returns>
        /// The response string.
        /// </returns>
        private string HttpPost(string url, string data, string charset)
        {
            var value = string.Empty;

            // Initialize Connection
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = string.Format("application/x-www-form-urlencoded; charset={0}", charset);
            request.UserAgent = this.userAgent;
            request.ContentLength = data.Length;

            // Write Data
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(data);
            }

            // Read Response
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    value = reader.ReadToEnd();
                }
            }

            return value;
        }

        #endregion
    }
}

namespace App_Code.Extensions
{
    using System;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;
    using System.Collections.Generic;
    using Joel.Net;

    /// <summary>
    /// Akismet Filter
    /// </summary>
    [Extension("Akismet anti-spam comment filter", "1.0", "<a href=\"http://dotnetblogengine.net\">BlogEngine.NET</a>", 0, false)]
    public class AkismetFilter : ICustomFilter
    {
        #region Constants and Fields

        /// <summary>
        ///     The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        ///     The api.
        /// </summary>
        private static Dictionary<Guid, Akismet> blogsApi = new Dictionary<Guid, Akismet>();

        /// <summary>
        ///     The key.
        /// </summary>
        private static Dictionary<Guid, string> blogsKey = new Dictionary<Guid, string>();

        /// <summary>
        ///     The settings.
        /// </summary>
        private static Dictionary<Guid, ExtensionSettings> blogsSettings = new Dictionary<Guid, ExtensionSettings>();

        /// <summary>
        ///     The site.
        /// </summary>
        private static Dictionary<Guid, string> blogsSite = new Dictionary<Guid, string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "AkismetFilter" /> class.
        /// </summary>
        public AkismetFilter()
        {
            this.InitSettings();
        }

        #endregion

        #region Properties

        private static Akismet Api
        {
            get
            {
                Akismet akismet = null;
                blogsApi.TryGetValue(Blog.CurrentInstance.Id, out akismet);

                return akismet;
            }
            set
            {
                blogsApi[Blog.CurrentInstance.Id] = value;
            }
        }

        private static string Key
        {
            get
            {
                string key = null;
                blogsKey.TryGetValue(Blog.CurrentInstance.Id, out key);

                return key;
            }
            set
            {
                blogsKey[Blog.CurrentInstance.Id] = value;
            }
        }

        private static string Site
        {
            get
            {
                string key = null;
                blogsSite.TryGetValue(Blog.CurrentInstance.Id, out key);

                return key;
            }
            set
            {
                blogsSite[Blog.CurrentInstance.Id] = value;
            }
        }

        private static ExtensionSettings Settings
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;
                ExtensionSettings settings = null;
                blogsSettings.TryGetValue(blogId, out settings);

                if (settings == null)
                {
                    lock (syncRoot)
                    {
                        blogsSettings.TryGetValue(blogId, out settings);

                        if (settings == null)
                        {
                            var extensionSettings = new ExtensionSettings("AkismetFilter") { IsScalar = true };

                            extensionSettings.AddParameter("SiteURL", "Site URL");
                            extensionSettings.AddParameter("ApiKey", "API Key");

                            extensionSettings.AddValue("SiteURL", "http://example.com/blog");
                            extensionSettings.AddValue("ApiKey", "123456789");

                            blogsSettings[blogId] = ExtensionManager.InitSettings("AkismetFilter", extensionSettings);
                            ExtensionManager.SetStatus("AkismetFilter", false);
                        }
                    }
                }

                return settings;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether FallThrough.
        /// </summary>
        public bool FallThrough { get; private set; }

        #endregion

        #region Implemented Interfaces

        #region ICustomFilter

        /// <summary>
        /// Check if comment is spam
        /// </summary>
        /// <param name="comment">
        /// BlogEngine comment
        /// </param>
        /// <returns>
        /// True if comment is spam
        /// </returns>
        public bool Check(Comment comment)
        {
            if (Api == null)
            {
                this.Initialize();
            }

            if (Settings == null)
            {
                this.InitSettings();
            }

            var akismetComment = GetAkismetComment(comment);
            var spam = Api.CommentCheck(akismetComment);
            this.FallThrough = !spam;
            return spam;
        }

        /// <summary>
        /// Initializes anti-spam service
        /// </summary>
        /// <returns>
        /// True if service online and credentials validated
        /// </returns>
        public bool Initialize()
        {
            if (!ExtensionManager.ExtensionEnabled("AkismetFilter"))
            {
                return false;
            }

            if (Settings == null)
            {
                this.InitSettings();
            }

            Site = Settings.GetSingleValue("SiteURL");
            Key = Settings.GetSingleValue("ApiKey");
            Api = new Akismet(Key, Site, string.Format("BlogEngine.NET {0}", BlogSettings.Instance.Version()));

            return Api.VerifyKey();
        }

        /// <summary>
        /// The report.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        public void Report(Comment comment)
        {
            if (Api == null && !this.Initialize())
            {
                return;
            }

            if (Settings == null)
            {
                this.InitSettings();
            }

            var akismetComment = GetAkismetComment(comment);

            if (comment.IsApproved)
            {
                Utils.Log(
                    string.Format("Akismet: Reporting NOT spam from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
                Api.SubmitHam(akismetComment);
            }
            else
            {
                Utils.Log(string.Format("Akismet: Reporting SPAM from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
                Api.SubmitSpam(akismetComment);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets an akismet comment.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// An Akismet Comment.
        /// </returns>
        private static AkismetComment GetAkismetComment(Comment comment)
        {
            var akismetComment = new AkismetComment
                {
                    Blog = Settings.GetSingleValue("SiteURL"), 
                    UserIp = comment.IP, 
                    CommentContent = comment.Content, 
                    CommentType = "comment", 
                    CommentAuthor = comment.Author, 
                    CommentAuthorEmail = comment.Email
                };
            if (comment.Website != null)
            {
                akismetComment.CommentAuthorUrl = comment.Website.OriginalString;
            }

            return akismetComment;
        }

        private void InitSettings()
        {
            // call Settings getter so default settings are loaded on application start.
            var s = Settings;
        }

        #endregion
    }
}