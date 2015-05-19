namespace App_Code.Extensions
{
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;

    using Joel.Net;
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// The type pad filter.
    /// </summary>
    [Extension("TypePad anti-spam comment filter (based on AkismetFilter)", "1.0", 
        "<a href=\"http://lucsiferre.net\">By Chris Nicola</a>", 0, false)]
    public class TypePadFilter : ICustomFilter
    {
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// The Akismet api.
        /// </summary>
        private static Dictionary<Guid, Akismet> blogsApi = new Dictionary<Guid, Akismet>();

        /// <summary>
        /// The fall through.
        /// </summary>
        private bool fallThrough = true;

        /// <summary>
        /// The TypePad key.
        /// </summary>
        private static Dictionary<Guid, string> blogsKey = new Dictionary<Guid, string>();

        /// <summary>
        /// The settings.
        /// </summary>
        private static Dictionary<Guid, ExtensionSettings> blogsSettings = new Dictionary<Guid, ExtensionSettings>();

        /// <summary>
        /// The TypePad site.
        /// </summary>
        private static Dictionary<Guid, string> blogsSite = new Dictionary<Guid, string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePadFilter"/> class.
        /// </summary>
        public TypePadFilter()
        {
            InitSettings();
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
                            var extensionSettings = new ExtensionSettings("TypePadFilter") { IsScalar = true };

                            extensionSettings.AddParameter("SiteURL", "Site URL");
                            extensionSettings.AddParameter("ApiKey", "API Key");

                            extensionSettings.AddValue("SiteURL", "http://example.com/blog");
                            extensionSettings.AddValue("ApiKey", "123456789");

                            blogsSettings[blogId] = ExtensionManager.InitSettings("TypePadFilter", extensionSettings);
                            ExtensionManager.SetStatus("TypePadFilter", false);
                        }
                    }
                }

                return settings;
            }
        }

        /// <summary>
        /// Gets a value indicating whether FallThrough.
        /// </summary>
        public bool FallThrough
        {
            get
            {
                return fallThrough;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICustomFilter

        /// <summary>
        /// Check if comment is spam
        /// </summary>
        /// <param name="comment">BlogEngine comment</param>
        /// <returns>True if comment is spam</returns>
        public bool Check(Comment comment)
        {
            if (Api == null)
            {
                this.Initialize();
            }

            var typePadComment = GetAkismetComment(comment);
            var isspam = Api.CommentCheck(typePadComment);
            fallThrough = !isspam;
            return isspam;
        }

        /// <summary>
        /// Initializes anti-spam service
        /// </summary>
        /// <returns>
        /// True if service online and credentials validated
        /// </returns>
        public bool Initialize()
        {
            if (!ExtensionManager.ExtensionEnabled("TypePadFilter"))
            {
                return false;
            }

            Site = Settings.GetSingleValue("SiteURL");
            Key = Settings.GetSingleValue("ApiKey");
            Api = new Akismet(Key, Site, "BlogEngine.NET 1.5", "api.antispam.typepad.com");

            return Api.VerifyKey();
        }

        /// <summary>
        /// Report mistakes back to service
        /// </summary>
        /// <param name="comment">BlogEngine comment</param>
        public void Report(Comment comment)
        {
            if (Api == null)
            {
                this.Initialize();
            }

            var akismetComment = GetAkismetComment(comment);

            if (comment.IsApproved)
            {
                Utils.Log(string.Format("TypePad: Reporting NOT spam from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
                Api.SubmitHam(akismetComment);
            }
            else
            {
                Utils.Log(string.Format("TypePad: Reporting SPAM from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
                Api.SubmitSpam(akismetComment);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the akismet comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>An Akismet Comment.</returns>
        private static AkismetComment GetAkismetComment(Comment comment)
        {
            var akismetComment = new AkismetComment
                {
                    Blog = Settings.GetSingleValue("SiteURL"),
                    UserIp = comment.IP,
                    CommentContent = comment.Content,
                    CommentAuthor = comment.Author,
                    CommentAuthorEmail = comment.Email
                };
            if (comment.Website != null)
            {
                akismetComment.CommentAuthorUrl = comment.Website.OriginalString;
            }

            return akismetComment;
        }

        public void InitSettings()
        {
            // call Settings getter so default settings are loaded on application start.
            var s = Settings;
        }

        #endregion
    }
}