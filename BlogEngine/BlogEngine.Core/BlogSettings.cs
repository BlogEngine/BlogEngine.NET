namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using System.Web;

    using BlogEngine.Core.Providers;
    using System.Web.Hosting;
    using System.IO;
    using System.Web.Caching;

    /// <summary>
    /// Represents the configured settings for the blog engine.
    /// </summary>
    public class BlogSettings
    {
        #region PRIVATE/PROTECTED/PUBLIC MEMBERS

        /// <summary>
        ///     Occurs when [changed].
        /// </summary>
        public static event EventHandler<EventArgs> Changed;

        /// <summary>
        ///     The blog settings singleton.
        /// </summary>
        /// <remarks>
        /// This should be created immediately instead of lazyloaded. It'll reduce the number of null checks that occur
        /// due to heavy reliance on calls to BlogSettings.Instance.
        /// </remarks>
        private static readonly Dictionary<Guid, BlogSettings> blogSettingsSingleton = new Dictionary<Guid, BlogSettings>();

        /// <summary>
        ///     The configured theme.
        /// </summary>
        private string configuredTheme = String.Empty;

        /// <summary>
        ///     The number of comments per page.
        /// </summary>
        private int commentsPerPage;

        /// <summary>
        ///     The enable http compression.
        /// </summary>
        private bool enableHttpCompression;

        /// <summary>
        ///     Whether passwords can be reset.
        /// </summary>
        private bool enablePasswordResets = true;

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The timeout in milliseconds for a remote download. Default is 30 seconds.
        /// </summary>
        private int remoteDownloadTimeout = defaultRemoteDownloadTimeout;
        private const int defaultRemoteDownloadTimeout = 30000;

        private int maxRemoteFileSize = defaultMaxRemoteFileSize;
        private const int defaultMaxRemoteFileSize = 524288;

        #endregion

        #region BlogSettings()

        /// <summary>
        ///     Prevents a default instance of the <see cref = "BlogSettings" /> class from being created. 
        ///     Initializes a new instance of the <see cref = "BlogSettings" /> class.
        /// </summary>
        private BlogSettings()
        {
            this.Load(Blog.CurrentInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blog"></param>
        private BlogSettings(Blog blog)
        {
            this.Load(blog);
        }

        #endregion

        /// <summary>
        ///     Gets the singleton instance of the <see cref = "BlogSettings" /> class.
        /// </summary>
        /// <value>A singleton instance of the <see cref = "BlogSettings" /> class.</value>
        /// <remarks>
        /// </remarks>
        public static BlogSettings Instance
        {
            get
            {
                return GetInstanceSettings(Blog.CurrentInstance);
            }
        }

        /// <summary>
        /// Returns the settings for the requested blog instance.
        /// </summary>
        public static BlogSettings GetInstanceSettings(Blog blog)
        {
            BlogSettings blogSettings;

            if (!blogSettingsSingleton.TryGetValue(blog.Id, out blogSettings))
            {
                lock (SyncRoot)
                {
                    if (!blogSettingsSingleton.TryGetValue(blog.Id, out blogSettings))
                    {
                        // settings will be loaded in constructor.
                        blogSettings = new BlogSettings(blog);

                        blogSettingsSingleton[blog.Id] = blogSettings;
                    }
                }
            }

            return blogSettings;
        }

        private bool? _isRazorTheme;
        /// <summary>
        /// Gets whether Theme is a razor theme.
        /// </summary>
        public bool IsRazorTheme
        {
            get
            {
                if (_isRazorTheme.HasValue) { return _isRazorTheme.Value; }

                _isRazorTheme = IsThemeRazor(this.Theme);
                return _isRazorTheme.Value;
            }
        }

        /// <summary>
        /// Determines if themeName is a razor theme.
        /// </summary>
        public static bool IsThemeRazor(string themeName)
        {
            string path = HostingEnvironment.MapPath(string.Format("~/Custom/Themes/{0}/site.cshtml", themeName));
            return File.Exists(path);
        }

        /// <summary>
        /// Takes into account factors such as if there is a theme override of if
        /// the theme is a Razor theme and returns the actual theme folder name
        /// for the current HTTP request.
        /// </summary>
        public string GetThemeWithAdjustments(string themeOverride)
        {
            string theme = this.Theme;
            bool isRazorTheme = configuredTheme == theme ? IsRazorTheme : IsThemeRazor(theme);
            if (!string.IsNullOrWhiteSpace(themeOverride))
            {
                theme = themeOverride;
                isRazorTheme = IsThemeRazor(theme);
            }
            return isRazorTheme ? "RazorHost" : theme;
        }

        #region Description

        /// <summary>
        ///     Gets or sets the description of the blog.
        /// </summary>
        /// <value>A brief synopsis of the blog content.</value>
        /// <remarks>
        ///     This value is also used for the description meta tag.
        /// </remarks>
        public string Description { get; set; }

        #endregion

        #region EnableHttpCompression

        /// <summary>
        ///     Gets or sets a value indicating if HTTP compression is enabled.
        /// </summary>
        /// <value><b>true</b> if compression is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableHttpCompression
        {
            get
            {
                return this.enableHttpCompression && !Utils.IsMono;
            }

            set
            {
                this.enableHttpCompression = value;
            }
        }

        #endregion

        #region EnableReferrerTracking

        /// <summary>
        ///     Gets or sets a value indicating if referral tracking is enabled.
        /// </summary>
        /// <value><b>true</b> if referral tracking is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableReferrerTracking { get; set; }

        #endregion

        #region NumberOfReferrerDays

        /// <summary>
        ///     Gets or sets a value indicating the number of days that referrer information should be stored.
        /// </summary>
        public int NumberOfReferrerDays { get; set; }

        #endregion

        #region EnableRelatedPosts

        /// <summary>
        ///     Gets or sets a value indicating if related posts are displayed.
        /// </summary>
        /// <value><b>true</b> if related posts are displayed, otherwise returns <b>false</b>.</value>
        public bool EnableRelatedPosts { get; set; }

        #endregion

        #region AlternateFeedUrl

        /// <summary>
        ///     Gets or sets the FeedBurner user name.
        /// </summary>
        public string AlternateFeedUrl { get; set; }

        #endregion

        #region FeedAuthor

        /// <summary>
        /// RSS feed author
        /// </summary>
        public string FeedAuthor { get; set; }

        #endregion

        #region TimeStampPostLinks

        /// <summary>
        ///     Gets or sets whether or not to time stamp post links.
        /// </summary>
        public bool TimeStampPostLinks { get; set; }

        #endregion

        #region Name

        /// <summary>
        ///     Gets or sets the name of the blog.
        /// </summary>
        /// <value>The title of the blog.</value>
        public string Name { get; set; }

        #endregion

        #region PostsPerPage

        /// <summary>
        ///     Gets or sets the number of posts to show an each page.
        /// </summary>
        /// <value>The number of posts to show an each page.</value>
        public int PostsPerPage { get; set; }

        #endregion

        #region ShowLivePreview

        /// <summary>
        ///     Gets or sets a value indicating if live preview of post is displayed.
        /// </summary>
        /// <value><b>true</b> if live previews are displayed, otherwise returns <b>false</b>.</value>
        public bool ShowLivePreview { get; set; }

        #endregion

        #region EnableRating

        /// <summary>
        ///     Gets or sets a value indicating if live preview of post is displayed.
        /// </summary>
        /// <value><b>true</b> if live previews are displayed, otherwise returns <b>false</b>.</value>
        public bool EnableRating { get; set; }

        #endregion

        #region ShowDescriptionInPostList

        /// <summary>
        ///     Gets or sets a value indicating if the full post is displayed in lists or only the description/excerpt.
        /// </summary>
        public bool ShowDescriptionInPostList { get; set; }

        #endregion

        #region DescriptionCharacters

        /// <summary>
        ///     Gets or sets a value indicating how many characters should be shown of the description
        /// </summary>
        public int DescriptionCharacters { get; set; }

        #endregion

        #region ShowDescriptionInPostListForPostsByTagOrCategory

        /// <summary>
        ///     Gets or sets a value indicating if the full post is displayed in lists by tag/category or only the description/excerpt.
        /// </summary>
        public bool ShowDescriptionInPostListForPostsByTagOrCategory { get; set; }

        #endregion

        #region DescriptionCharactersForPostsByTagOrCategory

        /// <summary>
        ///     Gets or sets a value indicating how many characters should be shown of the description when posts are shown by tag or category.
        /// </summary>
        public int DescriptionCharactersForPostsByTagOrCategory { get; set; }

        #endregion

        #region Enclosure support

        /// <summary>
        ///     Enable enclosures for RSS feeds
        /// </summary>
        public bool EnableEnclosures { get; set; }

        #endregion

        #region Tags Export

        /// <summary>
        ///     Enable exporting of tags in the RSS syndication feed.
        /// </summary>
        public bool EnableTagExport { get; set; }

        #endregion

        #region SyndicationFormat

        /// <summary>
        ///     Gets or sets the default syndication format used by the blog.
        /// </summary>
        /// <value>The default syndication format used by the blog.</value>
        /// <remarks>
        ///     If no value is specified, blog defaults to using RSS 2.0 format.
        /// </remarks>
        /// <seealso cref = "BlogEngine.Core.SyndicationFormat" />
        public string SyndicationFormat { get; set; }

        #endregion

        #region ThemeCookieName

        /// <summary>
        ///     The default theme cookie name.
        /// </summary>
        private const string DefaultThemeCookieName = "theme";

        /// <summary>
        ///     The theme cookie name.
        /// </summary>
        private string themeCookieName;

        /// <summary>
        ///     Gets or sets the name of the cookie that can override
        ///     the selected theme.
        /// </summary>
        /// <value>The name of the cookie that is checked while determining the theme.</value>
        /// <remarks>
        ///     The default value is "theme".
        /// </remarks>
        public string ThemeCookieName
        {
            get
            {
                return this.themeCookieName ?? DefaultThemeCookieName;
            }

            set
            {
                this.themeCookieName = value != DefaultThemeCookieName ? value : null;
            }
        }

        #endregion

        #region Theme

        /// <summary>
        ///     Gets or sets the current theme applied to the blog.
        ///     Default theme can be overridden in the query string
        ///     or cookie to let users select different theme
        /// </summary>
        /// <value>The configured theme for the blog.</value>
        public string Theme
        {
            get
            {
                var context = HttpContext.Current;
                if (context != null)
                {
                    var request = context.Request;
                    if (request.QueryString["theme"] != null)
                    {
                        return request.QueryString["theme"];
                    }

                    var cookie = request.Cookies[this.ThemeCookieName];
                    if (cookie != null)
                    {
                        return cookie.Value;
                    }

                    if (Utils.ShouldForceMainTheme(request))
                    {
                        return this.configuredTheme;
                    }
                }

                return this.configuredTheme;
            }

            set
            {
                this.configuredTheme = String.IsNullOrEmpty(value) ? String.Empty : value;
            }
        }

        #endregion

        #region CompressWebResource

        /// <summary>
        ///     Gets or sets a value indicating whether to compress WebResource.axd
        /// </summary>
        /// <value><c>true</c> if [compress web resource]; otherwise, <c>false</c>.</value>
        public bool CompressWebResource { get; set; }

        #endregion

        #region EnableOptimization

        /// <summary>
        ///     DO NOT USE: no longer needed and will be removed in later versions
        /// </summary>
        public bool EnableOptimization { get; set; }

        #endregion 

        #region UseBlogNameInPageTitles

        /// <summary>
        ///     Gets or sets a value indicating if whitespace in stylesheets should be removed
        /// </summary>
        /// <value><b>true</b> if whitespace is removed, otherwise returns <b>false</b>.</value>
        public bool UseBlogNameInPageTitles { get; set; }

        #endregion

        #region RequireSSLMetaWeblogAPI;

        /// <summary>
        ///     Gets or sets a value indicating whether [require SSL for MetaWeblogAPI connections].
        /// </summary>
        public bool RequireSslMetaWeblogApi { get; set; }

        #endregion

        #region EnableOpenSearch

        /// <summary>
        ///     Gets or sets a value indicating if whitespace in stylesheets should be removed
        /// </summary>
        /// <value><b>true</b> if whitespace is removed, otherwise returns <b>false</b>.</value>
        public bool EnableOpenSearch { get; set; }

        #endregion

        #region TrackingScript

        /// <summary>
        ///     Gets or sets the tracking script used to collect visitor data.
        /// </summary>
        public string TrackingScript { get; set; }

        #endregion

        #region ShowPostNavigation

        /// <summary>
        ///     Gets or sets a value indicating whether or not to show the post navigation.
        /// </summary>
        /// <value><c>true</c> if [show post navigation]; otherwise, <c>false</c>.</value>
        public bool ShowPostNavigation { get; set; }

        #endregion

        #region EnablePasswordReset

        /// <summary>
        ///     Gets or sets a value indicating whether or not to enable password resets.
        /// </summary>
        /// <value><c>true</c> if [enable self registration]; otherwise, <c>false</c>.</value>
        public bool EnablePasswordReset
        {
            get { return enablePasswordResets; }
            set { enablePasswordResets = value; }
        }

        #endregion

        #region SelfRegistration

        /// <summary>
        ///     Gets or sets a value indicating whether or not to enable self registration.
        /// </summary>
        /// <value><c>true</c> if [enable self registration]; otherwise, <c>false</c>.</value>
        public bool EnableSelfRegistration { get; set; }

        /// <summary>
        ///     Gets or sets the initial role assigned to users who self register.
        /// </summary>
        /// <value>The role name.</value>
        public string SelfRegistrationInitialRole { get; set; }

        /// <summary>
        /// If we need to create blog for self-registered user
        /// (instead of just add user to existing blog)
        /// </summary>
        public bool CreateBlogOnSelfRegistration { get; set; }

        #endregion

        #region HandleWwwSubdomain

        /// <summary>
        ///     Gets or sets how to handle the www subdomain of the url (for SEO purposes).
        /// </summary>
        public string HandleWwwSubdomain { get; set; }

        #endregion

        #region EnablePingBackSend

        /// <summary>
        ///     Gets or sets a value indicating whether [enable ping back send].
        /// </summary>
        /// <value><c>true</c> if [enable ping back send]; otherwise, <c>false</c>.</value>
        public bool EnablePingBackSend { get; set; }

        #endregion

        #region EnablePingBackReceive;

        /// <summary>
        ///     Gets or sets a value indicating whether [enable ping back receive].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable ping back receive]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePingBackReceive { get; set; }

        #endregion

        #region EnableTrackBackSend;

        /// <summary>
        ///     Gets or sets a value indicating whether [enable track back send].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable track back send]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTrackBackSend { get; set; }

        #endregion

        #region EnableTrackBackReceive;

        /// <summary>
        ///     Gets or sets a value indicating whether [enable track back receive].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable track back receive]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTrackBackReceive { get; set; }

        #endregion

        #region Email

        /// <summary>
        ///     Gets or sets the e-mail address notifications are sent to.
        /// </summary>
        /// <value>The e-mail address notifications are sent to.</value>
        public string Email { get; set; }

        #endregion

        #region SendMailOnComment

        /// <summary>
        ///     Gets or sets a value indicating if an enail is sent when a comment is added to a post.
        /// </summary>
        /// <value><b>true</b> if email notification of new comments is enabled, otherwise returns <b>false</b>.</value>
        public bool SendMailOnComment { get; set; }

        #endregion

        #region SmtpPassword

        /// <summary>
        ///     Gets or sets the password used to connect to the SMTP server.
        /// </summary>
        /// <value>The password used to connect to the SMTP server.</value>
        public string SmtpPassword { get; set; }

        #endregion

        #region SmtpServer

        /// <summary>
        ///     Gets or sets the DNS name or IP address of the SMTP server used to send notification emails.
        /// </summary>
        /// <value>The DNS name or IP address of the SMTP server used to send notification emails.</value>
        public string SmtpServer { get; set; }

        #endregion

        #region SmtpServerPort

        /// <summary>
        ///     Gets or sets the DNS name or IP address of the SMTP server used to send notification emails.
        /// </summary>
        /// <value>The DNS name or IP address of the SMTP server used to send notification emails.</value>
        public int SmtpServerPort { get; set; }

        #endregion

        #region SmtpUsername

        /// <summary>
        ///     Gets or sets the user name used to connect to the SMTP server.
        /// </summary>
        /// <value>The user name used to connect to the SMTP server.</value>
        public string SmtpUserName { get; set; }

        #endregion

        #region EnableSsl

        /// <summary>
        ///     Gets or sets a value indicating if SSL is enabled for sending e-mails
        /// </summary>
        public bool EnableSsl { get; set; }

        #endregion

        #region EmailSubjectPrefix

        /// <summary>
        ///     Gets or sets the email subject prefix.
        /// </summary>
        /// <value>The email subject prefix.</value>
        public string EmailSubjectPrefix { get; set; }

        #endregion

        #region DaysCommentsAreEnabled

        /// <summary>
        ///     Gets or sets the number of days that a post accepts comments.
        /// </summary>
        /// <value>The number of days that a post accepts comments.</value>
        /// <remarks>
        ///     After this time period has expired, comments on a post are disabled.
        /// </remarks>
        public int DaysCommentsAreEnabled { get; set; }

        #endregion

        #region EnableCountryInComments

        /// <summary>
        ///     Gets or sets a value indicating if dispay of the country of commenter is enabled.
        /// </summary>
        /// <value><b>true</b> if commenter country display is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableCountryInComments { get; set; }

        #endregion

        #region EnableWebsiteInComments
        /// <summary>
        ///     Gets or sets a value indicating if display of the website of commenter is enabled
        /// </summary>
        public bool EnableWebsiteInComments { get; set; }
        #endregion

        #region IsCommentsEnabled

        /// <summary>
        ///     Gets or sets a value indicating if comments are enabled for posts.
        /// </summary>
        /// <value><b>true</b> if comments can be made against a post, otherwise returns <b>false</b>.</value>
        public bool IsCommentsEnabled { get; set; }

        #endregion

        #region IsCoCommentEnabled

        /// <summary>
        ///     Only here so old themes won't break
        /// </summary>
        /// <value>false</value>
        public bool IsCoCommentEnabled { get; set; }

        #endregion

        #region Avatar

        /// <summary>
        ///     Gets or sets a value indicating if Gravatars are enabled or not.
        /// </summary>
        /// <value><b>true</b> if Gravatars are enabled, otherwise returns <b>false</b>.</value>
        public string Avatar { get; set; }

        #endregion

        #region IsCommentNestingEnabled

        /// <summary>
        ///     Gets or sets a value indicated if comments should be displayed as nested.
        /// </summary>
        /// <value><b>true</b> if comments should be displayed as nested, <b>false</b> for flat comments.</value>
        public bool IsCommentNestingEnabled { get; set; }

        #endregion

        #region Trust authenticated users

        ///<summary>
        ///    If true comments from authenticated users always approved
        ///</summary>
        public bool TrustAuthenticatedUsers { get; set; }

        #endregion

        #region White list count

        ///<summary>
        ///    Number of comments approved to add user to white list
        ///</summary>
        public int CommentWhiteListCount { get; set; }

        #endregion

        #region Black list count

        ///<summary>
        ///    Number of comments approved to add user to white list
        ///</summary>
        public int CommentBlackListCount { get; set; }

        #endregion

        #region AddIpToWhitelistFilterOnApproval

        ///<summary>
        ///    Automatically add IP address to white list filter when comment is approved.
        ///</summary>
        public bool AddIpToWhitelistFilterOnApproval { get; set; }

        #endregion

        #region AddIpToBlacklistFilterOnRejection

        ///<summary>
        ///    Automatically add IP address to black list filter when comment is rejected.
        ///</summary>
        public bool AddIpToBlacklistFilterOnRejection { get; set; }

        #endregion

        #region BlockAuthorOnCommentDelete

        ///<summary>
        ///    Automatically add author to Block list when comment is deleted.
        ///</summary>
        public bool BlockAuthorOnCommentDelete { get; set; }

        #endregion

        #region SecurityValidationKey

        /// <summary>
        ///     Gets or sets the security validation key.
        /// </summary>
        /// <value>The security validation key.</value>
        public string SecurityValidationKey { get; set; }

        #endregion

        #region Comments per page

        /// <summary>
        ///     Number of comments per page displayed in the comments admin section
        /// </summary>
        public int CommentsPerPage
        {
            get { return Math.Max(commentsPerPage, 5); }
            set { commentsPerPage = value; }
        }

        #endregion

        #region Comment providers and moderation

        /// <summary>
        /// Comments provider
        /// </summary>
        public enum CommentsBy
        {
            /// <summary>
            ///     Internal BlogEngine comments
            /// </summary>
            BlogEngine = 0,
            /// <summary>
            ///     Comments by Disqus
            /// </summary>
            Disqus = 1,
            /// <summary>
            ///     Comments by Facebook
            /// </summary>
            Facebook = 2
        }

        /// <summary>
        ///     Gets or sets a value indicating comment provider
        /// </summary>
        public CommentsBy CommentProvider { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating if comments moderation is used for posts.
        /// </summary>
        /// <value><b>true</b> if comments are moderated for posts, otherwise returns <b>false</b>.</value>
        public bool EnableCommentsModeration { get; set; }

        /// <summary>
        ///     Enables to report mistakes back to service
        /// </summary>
        public bool CommentReportMistakes { get; set; }

        /// <summary>
        ///     Short website name that used to identify Disqus account
        /// </summary>
        public string DisqusWebsiteName { get; set; }

        /// <summary>
        ///     Development mode to test disqus on local host
        /// </summary>
        public bool DisqusDevMode { get; set; }

        /// <summary>
        ///     Allow also to add comments to the pages
        /// </summary>
        public bool DisqusAddCommentsToPages { get; set; }

        #endregion

        #region BlogrollMaxLength

        /// <summary>
        ///     Gets or sets the maximum number of characters that are displayed from a blog-roll retrieved post.
        /// </summary>
        /// <value>The maximum number of characters to display.</value>
        public int BlogrollMaxLength { get; set; }

        #endregion

        #region BlogrollUpdateMinutes

        /// <summary>
        ///     Gets or sets the number of minutes to wait before polling blog-roll sources for changes.
        /// </summary>
        /// <value>The number of minutes to wait before polling blog-roll sources for changes.</value>
        public int BlogrollUpdateMinutes { get; set; }

        #endregion

        #region BlogrollVisiblePosts

        /// <summary>
        ///     Gets or sets the number of posts to display from a blog-roll source.
        /// </summary>
        /// <value>The number of posts to display from a blog-roll source.</value>
        public int BlogrollVisiblePosts { get; set; }

        #endregion

        #region EnableCommentSearch

        /// <summary>
        ///     Gets or sets a value indicating if search of post comments is enabled.
        /// </summary>
        /// <value><b>true</b> if post comments can be searched, otherwise returns <b>false</b>.</value>
        public bool EnableCommentSearch { get; set; }

        /// <summary>
        ///     If yes, checkbox for include comments in search added to UI
        /// </summary>
        public bool ShowIncludeCommentsOption { get; set; }

        #endregion

        #region SearchButtonText

        /// <summary>
        ///     Gets or sets the search button text to be displayed.
        /// </summary>
        /// <value>The search button text to be displayed.</value>
        public string SearchButtonText { get; set; }

        #endregion

        #region SearchCommentLabelText

        /// <summary>
        ///     Gets or sets the search comment label text to display.
        /// </summary>
        /// <value>The search comment label text to display.</value>
        public string SearchCommentLabelText { get; set; }

        #endregion

        #region SearchDefaultText

        /// <summary>
        ///     Gets or sets the default search text to display.
        /// </summary>
        /// <value>The default search text to display.</value>
        public string SearchDefaultText { get; set; }

        #endregion

        #region Endorsement

        /// <summary>
        ///     Gets or sets the URI of a web log that the author of this web log is promoting.
        /// </summary>
        /// <value>The <see cref = "Uri" /> of a web log that the author of this web log is promoting.</value>
        public string Endorsement { get; set; }

        #endregion

        #region PostsPerFeed

        /// <summary>
        ///     Gets or sets the maximum number of characters that are displayed from a blog-roll retrieved post.
        /// </summary>
        /// <value>The maximum number of characters to display.</value>
        public int PostsPerFeed { get; set; }

        #endregion

        #region AuthorName

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string AuthorName { get; set; }

        #endregion

        #region Language

        /// <summary>
        ///     Gets or sets the language this blog is written in.
        /// </summary>
        /// <value>The language this blog is written in.</value>
        /// <remarks>
        ///     Recommended best practice for the values of the Language element is defined by RFC 1766 [RFC1766] which includes a two-letter Language Code (taken from the ISO 639 standard [ISO639]), 
        ///     followed optionally, by a two-letter Country Code (taken from the ISO 3166 standard [ISO3166]).
        /// </remarks>
        /// <example>
        ///     en-US
        /// </example>
        public string Language { get; set; }

        #endregion

        #region GeocodingLatitude

        /// <summary>
        ///     Gets or sets the latitude component of the geocoding position for this blog.
        /// </summary>
        /// <value>The latitude value.</value>
        public float GeocodingLatitude { get; set; }

        #endregion

        #region GeocodingLongitude

        /// <summary>
        ///     Gets or sets the longitude component of the geocoding position for this blog.
        /// </summary>
        /// <value>The longitude value.</value>
        public float GeocodingLongitude { get; set; }

        #endregion

        #region ContactFormMessage;

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string ContactFormMessage { get; set; }

        #endregion

        #region ContactThankMessage

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string ContactThankMessage { get; set; }

        #endregion

        #region ContactErrorMessage;
        /// <summary>
        ///     Gets or sets a custom error message for this blog.
        /// </summary>
        /// <value>The error messagge for this blog.</value>
        public string ContactErrorMessage { get; set; }
        
        #endregion

        #region HtmlHeader

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string HtmlHeader { get; set; }

        #endregion

        #region Culture

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string Culture { get; set; }

        #endregion

        #region Timezone

        /// <summary>
        /// Time zone id
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Converts time passed from client into UTC server time
        /// </summary>
        /// <param name="localTime">ToUtc</param>
        /// <returns>Server time</returns>
        public DateTime ToUtc(DateTime ? localTime = null)
        {
            if(localTime == null || localTime == new DateTime()) // no time sent in, use "now"
                return DateTime.UtcNow;

            return localTime.Value.ToUniversalTime();
        }

        /// <summary>
        /// Converts time saved on the server from UTC to local user time offset by timezone
        /// </summary>
        /// <param name="serverTime">FromUtc</param>
        /// <returns>Client time</returns>
        public DateTime FromUtc(DateTime ? serverTime = null)
        {
            if (serverTime == null || serverTime == new DateTime())
                serverTime = DateTime.UtcNow;

            var zone = string.IsNullOrEmpty(Instance.TimeZoneId) ? "UTC" : Instance.TimeZoneId;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(zone);
            serverTime = DateTime.SpecifyKind(serverTime.Value, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeFromUtc(serverTime.Value, tz);
        }

        #endregion

        #region EnableContactAttachments

        /// <summary>
        ///     Gets or sets whether or not to allow visitors to send attachments via the contact form.
        /// </summary>
        public bool EnableContactAttachments { get; set; }

        #endregion

        #region EnableRecaptchaOnContactForm

        /// <summary>
        ///     Gets or sets whether or not to use Recaptcha on the contact form.
        /// </summary>
        public bool EnableRecaptchaOnContactForm { get; set; }

        #endregion

        #region EnableQuickNotes

        /// <summary>
        ///     Gets or sets a value indicating if QuickNotes module is enabled
        /// </summary>
        /// <value><b>true</b> if QuickNotes is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableQuickNotes { get; set; }

        #endregion

        #region RemoveExtensionsFromUrls

        /// <summary>
        ///     Gets or sets a value indicating if extensions (.aspx) should be removed from URLs
        /// </summary>
        /// <value><b>true</b> if should be removed, otherwise returns <b>false</b>.</value>
        public bool RemoveExtensionsFromUrls { get; set; }

        #endregion

        #region RedirectToRemoveFileExtension

        /// <summary>
        ///     Gets or sets a value indicating if incoming requests containing extensions (.aspx) should be redirected to a URL with the extension removed.
        /// </summary>
        /// <value><b>true</b> if should be redirected, otherwise returns <b>false</b>.</value>
        public bool RedirectToRemoveFileExtension { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets whether this application's handlers should be able to download and cache files hosted on other servers.
        /// </summary>
        /// <remarks>
        /// 
        /// Allowing the server's various handlers(Such as JavaScriptHandler and CssHandler) could potentially allow a an attacker
        /// to tie up the server by continuously requesting files of excess file size, or from servers that take forever to time out.
        /// 
        /// This is false by default.
        /// 
        /// </remarks>
        public bool AllowServerToDownloadRemoteFiles { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of time in milliseconds the server should spend downloading remote files. The default value is 30000.
        /// </summary>
        /// <remarks>
        /// 
        /// If the limit is set to something below 0, the defaultRemoteDownloadTimeout will be used instead.
        /// 0 is an acceptable value, users should use this value to indicate "unlimited time".
        /// </remarks>
        public int RemoteFileDownloadTimeout
        {
            get
            {
                if (this.remoteDownloadTimeout < 0)
                {
                    this.remoteDownloadTimeout = defaultRemoteDownloadTimeout;
                }
                return this.remoteDownloadTimeout;
            }
            set
            {
                if (value < 0) { value = defaultRemoteDownloadTimeout; }
                this.remoteDownloadTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed file size in bytes that BlogEngine can download from a remote server. Defaults to 512k.
        /// </summary>
        /// <remarks>
        /// 
        /// Set this value to 0 for unlimited file size.
        /// 
        /// </remarks>
        public int RemoteMaxFileSize
        {
            get
            {
                if (this.maxRemoteFileSize < 0)
                {
                    this.maxRemoteFileSize = defaultMaxRemoteFileSize;
                }
                return this.maxRemoteFileSize;
            }
            set
            {
                if (value < 0) { value = defaultMaxRemoteFileSize; }
                this.maxRemoteFileSize = value;
            }
        }

        #region Version()

        /// <summary>
        ///     The version.
        /// </summary>
        private static string version;

        /// <summary>
        /// Returns the BlogEngine.NET version information.
        /// </summary>
        /// <value>
        /// The BlogEngine.NET major, minor, revision, and build numbers.
        /// </value>
        /// <remarks>
        /// The current version is determined by extracting the build version of the BlogEngine.Core assembly.
        /// </remarks>
        /// <returns>
        /// The version.
        /// </returns>
        public string Version()
        {
            return version ?? (version = this.GetType().Assembly.GetName().Version.ToString());
        }

        #endregion

        #region Custom front page
        /// <summary>
        /// When file with name "FrontPage.aspx" or "FrontPage.cshtml"
        /// exists in the blog root, use it as custom front page
        /// </summary>
        public static string CustomFrontPage
        {
            get
            {
                // uncomment this line to disable caching for debugging
                // Blog.CurrentInstance.Cache.Remove(cacheKey);
                string cacheKey = "Blog-Custom-Front-Page";

                if (Blog.CurrentInstance.Cache[cacheKey] == null)
                {
                    Blog.CurrentInstance.Cache.Add(
                        cacheKey,
                        GetCustomFrontPage(),
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 15, 0),
                        CacheItemPriority.Low,
                        null);
                }
                return Blog.CurrentInstance.Cache[cacheKey].ToString();
            }
        }
        static string GetCustomFrontPage()
        {
            if (Blog.CurrentInstance.IsPrimary)
            {
                string razorPath = HostingEnvironment.MapPath("~/FrontPage.cshtml");
                string aspxPath = HostingEnvironment.MapPath("~/FrontPage.aspx");

                if (File.Exists(razorPath)) return "FrontPage.cshtml";
                if (File.Exists(aspxPath)) return "FrontPage.aspx";
            }
            return "";
        }
        #endregion

        #region "Methods"

        #region Load()

        private IDictionary<String, System.Reflection.PropertyInfo> GetSettingsTypePropertyDict()
        {
            var settingsType = this.GetType();

            var result = new System.Collections.Generic.Dictionary<String, System.Reflection.PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in settingsType.GetProperties())
            {
                result[prop.Name] = prop;
            }

            return result;

        }

        /// <summary>
        /// Initializes the singleton instance of the <see cref="BlogSettings"/> class.
        /// </summary>
        private void Load(Blog blog)
        {

            // ------------------------------------------------------------
            // 	Enumerate through individual settings nodes
            // ------------------------------------------------------------
            var dic = BlogService.LoadSettings(blog);
            var settingsProps = GetSettingsTypePropertyDict();

            foreach (System.Collections.DictionaryEntry entry in dic)
            {
                string name = (string)entry.Key;
                System.Reflection.PropertyInfo property = null;

                if (settingsProps.TryGetValue(name, out property))
                {
                    // ------------------------------------------------------------
                    // 	Attempt to apply configured setting
                    // ------------------------------------------------------------
                    try
                    {
                        if (property.CanWrite)
                        {
                            string value = (string)entry.Value;
                            var propType = property.PropertyType;
                            if (propType.IsEnum)
                            {
                                property.SetValue(this, Enum.Parse(propType, value), null);
                            }
                            else
                            {
                                property.SetValue(this, Convert.ChangeType(value, propType, CultureInfo.CurrentCulture), null);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.Log(string.Format("Error loading blog settings: {0}", e.Message));
                    }
                }

            }

        }

        #endregion

        #region OnChanged()

        /// <summary>
        /// Occurs when the settings have been changed.
        /// </summary>
        private static void OnChanged()
        {
            // Execute event handler
            if (Changed != null)
            {
                Changed(null, EventArgs.Empty);
            }
        }

        #endregion

        #region Save()

        /// <summary>
        /// Saves the settings to disk.
        /// </summary>
        public void Save()
        {
            var dic = new StringDictionary();
            var settingsType = this.GetType();

            // ------------------------------------------------------------
            // 	Enumerate through settings properties
            // ------------------------------------------------------------
            foreach (var propertyInformation in settingsType.GetProperties())
            {
                if (propertyInformation.Name != "Instance")
                {
                    // ------------------------------------------------------------
                    // 	Extract property value and its string representation
                    // ------------------------------------------------------------
                    var propertyValue = propertyInformation.GetValue(this, null);

                    string valueAsString;

                    // ------------------------------------------------------------
                    // 	Format null/default property values as empty strings
                    // ------------------------------------------------------------
                    if (propertyValue == null || propertyValue.Equals(Int32.MinValue) ||
                        propertyValue.Equals(Single.MinValue))
                    {
                        valueAsString = String.Empty;
                    }
                    else
                    {
                        valueAsString = propertyValue.ToString();
                    }

                    // ------------------------------------------------------------
                    // 	Write property name/value pair
                    // ------------------------------------------------------------
                    dic.Add(propertyInformation.Name, valueAsString);
                }
            }

            BlogService.SaveSettings(dic);
            _isRazorTheme = null;
            OnChanged();
        }

        #endregion

        #endregion


        #region "ErrorPage Title"
        /// <summary>
        ///     Gets or sets the Title Of Error Page.
        /// </summary>
        /// <value>The Title Error Page.</value>

        public string ErrorTitle { get; set; }

        #endregion

        #region "ErrorPage Body"
        /// <summary>
        ///     Gets or sets the Body Of Error Page.
        /// </summary>
        /// <value>The Body Error Page.</value>
        public string ErrorText { get; set; }

        #endregion

    }
}