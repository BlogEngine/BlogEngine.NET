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
        /// Time zone
        /// </summary>
        public double Timezone { get; set; }
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
        /// <summary>
        /// Default mobile theme
        /// </summary>
        public string MobileTheme { get; set; }

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
        /// Enable error logging
        /// </summary>
        public bool EnableErrorLogging { get; set; }
        /// <summary>
        /// Gallery feed url
        /// </summary>
        public string GalleryFeedUrl { get; set; }

        public bool EnablePasswordReset { get; set; }
        public bool EnableSelfRegistration { get; set; }
        public bool CreateBlogOnSelfRegistration { get; set; }
        public bool AllowServerToDownloadRemoteFiles { get; set; }
        public int RemoteFileDownloadTimeout { get; set; }
        public int RemoteMaxFileSize { get; set; }
        public string SelfRegistrationInitialRole { get; set; }

        // feed
        public string AuthorName { get; set; }
        public string FeedAuthor { get; set; }
        public string Endorsement { get; set; }
        public string AlternateFeedUrl { get; set; }
        public string Language { get; set; }
        public int PostsPerFeed { get; set; }
        public bool EnableEnclosures { get; set; }
        public bool EnableTagExport { get; set; }
        public string SyndicationFormat { get; set; }

        // email
        public string Email { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public string EmailSubjectPrefix { get; set; }
        public bool EnableSsl { get; set; }
        public bool SendMailOnComment { get; set; }

        // controls
        public string SearchButtonText { get; set; }
        public string SearchCommentLabelText { get; set; }
        public string SearchDefaultText { get; set; }
        public bool EnableCommentSearch { get; set; }
        public bool ShowIncludeCommentsOption { get; set; }
        public string ContactFormMessage { get; set; }
        public string ContactThankMessage { get; set; }
        public string ContactErrorMessage { get; set; }
        public bool EnableContactAttachments { get; set; }
        public bool EnableRecaptchaOnContactForm { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorText { get; set; }

        // custom code
        public string HtmlHeader { get; set; }
        public string TrackingScript { get; set; }

        // comments
        public int DaysCommentsAreEnabled { get; set; }
        public bool IsCommentsEnabled { get; set; }
        public bool EnableCommentsModeration { get; set; }
        public bool IsCommentNestingEnabled { get; set; }
        public bool IsCoCommentEnabled { get; set; }
        public string Avatar { get; set; }
        public bool EnablePingBackSend { get; set; }
        public bool EnablePingBackReceive { get; set; }
        public bool EnableTrackBackSend { get; set; }
        public bool EnableTrackBackReceive { get; set; }
        public string ThumbnailServiceApi { get; set; }
        public int CommentsPerPage { get; set; }
        public bool EnableCountryInComments { get; set; }
        public bool EnableWebsiteInComments { get; set; }
        public bool ShowLivePreview { get; set; }

        public bool UseDisqus { get; set; }
        public bool DisqusDevMode { get; set; }
        public bool DisqusAddCommentsToPages { get; set; }
        public string DisqusWebsiteName { get; set; }

        // custom filters
        public int CommentWhiteListCount { get; set; }
        public int CommentBlackListCount { get; set; }
        public bool AddIpToWhitelistFilterOnApproval { get; set; }
        public bool TrustAuthenticatedUsers { get; set; }
        public bool BlockAuthorOnCommentDelete { get; set; }
        public bool AddIpToBlacklistFilterOnRejection { get; set; }
    }
}
