namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Blog settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Settings() { }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Posts per page
        /// </summary>
        public int PostsPerPage { get; set; }
        /// <summary>
        /// Theme cookie name used to override default theme for a session
        /// </summary>
        public string ThemeCookieName { get; set; }
        /// <summary>
        /// Blog name in page titles
        /// </summary>
        public bool UseBlogNameInPageTitles { get; set; }
        /// <summary>
        /// Enables related posts
        /// </summary>
        public bool EnableRelatedPosts { get; set; }
        /// <summary>
        /// Enables post ratings
        /// </summary>
        public bool EnableRating { get; set; }
        /// <summary>
        /// Shows post description instead of content
        /// </summary>
        public bool ShowDescriptionInPostList { get; set; }
        /// <summary>
        /// Number of characters in post description
        /// </summary>
        public int DescriptionCharacters { get; set; }
        /// <summary>
        /// Only shows post description for tags and categories
        /// </summary>
        public bool ShowDescriptionInPostListForPostsByTagOrCategory { get; set; }
        /// <summary>
        /// Number of characters for description in tags/category lists
        /// </summary>
        public int DescriptionCharactersForPostsByTagOrCategory { get; set; }
        /// <summary>
        /// Show time stamp
        /// </summary>
        public bool TimeStampPostLinks { get; set; }
        /// <summary>
        /// Show post navigation
        /// </summary>
        public bool ShowPostNavigation { get; set; }
        /// <summary>
        /// Culture
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// Time zone id
        /// </summary>
        public string TimeZoneId { get; set; }
        /// <summary>
        /// Removes extensions from urls
        /// </summary>
        public bool RemoveExtensionsFromUrls { get; set; }
        /// <summary>
        /// Sets redirect if file extension not used (for updated blogs)
        /// </summary>
        public bool RedirectToRemoveFileExtension { get; set; }
        /// <summary>
        /// How to handle www sub-domain
        /// </summary>
        public string HandleWwwSubdomain { get; set; }
        /// <summary>
        /// Default desktop theme
        /// </summary>
        public string DesktopTheme { get; set; }

        // advanced settings
        /// <summary>
        /// Enable HTTP compression
        /// </summary>
        public bool EnableHttpCompression { get; set; }
        /// <summary>
        /// Compress web resources
        /// </summary>
        public bool CompressWebResource { get; set; }
        /// <summary>
        /// Enable open search
        /// </summary>
        public bool EnableOpenSearch { get; set; }
        /// <summary>
        /// Require SSL for meta weblog api
        /// </summary>
        public bool RequireSslForMetaWeblogApi { get; set; }

        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnablePasswordReset { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableSelfRegistration { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool CreateBlogOnSelfRegistration { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool AllowServerToDownloadRemoteFiles { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public int RemoteFileDownloadTimeout { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public int RemoteMaxFileSize { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string SelfRegistrationInitialRole { get; set; }

        // feed
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string AuthorName { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string FeedAuthor { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Endorsement { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string AlternateFeedUrl { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public int PostsPerFeed { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableEnclosures { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableTagExport { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string SyndicationFormat { get; set; }

        // email
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public int SmtpServerPort { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string SmtpUserName { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string SmtpPassword { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string EmailSubjectPrefix { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableSsl { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool SendMailOnComment { get; set; }

        // controls
        public string SearchButtonText { get; set; }
        public string SearchCommentLabelText { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string SearchDefaultText { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableCommentSearch { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool ShowIncludeCommentsOption { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string ContactFormMessage { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string ContactThankMessage { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string ContactErrorMessage { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableContactAttachments { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableRecaptchaOnContactForm { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string ErrorTitle { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string ErrorText { get; set; }

        // custom code
        public string HtmlHeader { get; set; }
        public string TrackingScript { get; set; }

        // comments
        public int DaysCommentsAreEnabled { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool IsCommentsEnabled { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableCommentsModeration { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool IsCommentNestingEnabled { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnablePingBackSend { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnablePingBackReceive { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableTrackBackSend { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableTrackBackReceive { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public int CommentsPerPage { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableCountryInComments { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool EnableWebsiteInComments { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool ShowLivePreview { get; set; }

        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public BlogSettings.CommentsBy CommentProvider { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool DisqusDevMode { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool DisqusAddCommentsToPages { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string DisqusWebsiteName { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string FacebookAppId { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string FacebookLanguage { get; set; }

        // custom filters
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool TrustAuthenticatedUsers { get; set; }
    }
}
