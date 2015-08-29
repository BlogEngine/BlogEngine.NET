using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Contracts;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Settings repository
    /// </summary>
    public class SettingsRepository : ISettingsRepository
    {
        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns>Settings object</returns>
        public Settings Get()
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminSettingsPages))
                throw new System.UnauthorizedAccessException();

            var ns = new Settings();
            var bs = BlogSettings.Instance;

            // basic
            ns.Name = bs.Name;
            ns.Description = bs.Description;
            ns.PostsPerPage = bs.PostsPerPage;
            ns.ThemeCookieName = bs.ThemeCookieName;
            ns.UseBlogNameInPageTitles = bs.UseBlogNameInPageTitles;
            ns.EnableRelatedPosts = bs.EnableRelatedPosts;
            ns.EnableRating = bs.EnableRating;
            ns.ShowDescriptionInPostList = bs.ShowDescriptionInPostList;
            ns.DescriptionCharacters = bs.DescriptionCharacters;
            ns.ShowDescriptionInPostListForPostsByTagOrCategory = bs.ShowDescriptionInPostListForPostsByTagOrCategory;
            ns.DescriptionCharactersForPostsByTagOrCategory = bs.DescriptionCharactersForPostsByTagOrCategory;
            ns.TimeStampPostLinks = bs.TimeStampPostLinks;
            ns.ShowPostNavigation = bs.ShowPostNavigation;
            ns.Culture = bs.Culture;
            ns.Timezone = bs.Timezone;
            ns.RemoveExtensionsFromUrls = bs.RemoveExtensionsFromUrls;
            ns.RedirectToRemoveFileExtension = bs.RedirectToRemoveFileExtension;
            ns.DesktopTheme = bs.Theme;
            
            // advanced
            ns.HandleWwwSubdomain = bs.HandleWwwSubdomain;
            ns.EnableHttpCompression = bs.EnableHttpCompression;
            ns.CompressWebResource = bs.CompressWebResource;
            ns.EnableOpenSearch = bs.EnableOpenSearch;
            ns.RequireSslForMetaWeblogApi = bs.RequireSslMetaWeblogApi;

            ns.EnablePasswordReset = bs.EnablePasswordReset;
            ns.EnableSelfRegistration = bs.EnableSelfRegistration;
            ns.CreateBlogOnSelfRegistration = bs.CreateBlogOnSelfRegistration;
            ns.AllowServerToDownloadRemoteFiles = bs.AllowServerToDownloadRemoteFiles;
            ns.RemoteFileDownloadTimeout = bs.RemoteFileDownloadTimeout;
            ns.RemoteMaxFileSize = bs.RemoteMaxFileSize;
            ns.SelfRegistrationInitialRole = bs.SelfRegistrationInitialRole;
            if (string.IsNullOrEmpty(ns.SelfRegistrationInitialRole))
            {
                // set to default editor role if not set in settings
                ns.SelfRegistrationInitialRole = BlogConfig.EditorsRole;
            }

            // feed
            ns.AuthorName = bs.AuthorName;
            ns.FeedAuthor = bs.FeedAuthor;
            ns.Endorsement = bs.Endorsement;
            ns.AlternateFeedUrl = bs.AlternateFeedUrl;
            ns.Language = bs.Language;
            ns.PostsPerFeed = bs.PostsPerFeed;
            ns.EnableEnclosures = bs.EnableEnclosures;
            ns.SyndicationFormat = bs.SyndicationFormat;
            ns.EnableTagExport = bs.EnableTagExport;

            // email
            ns.Email = bs.Email;
            ns.SmtpServer = bs.SmtpServer;
            ns.SmtpServerPort = bs.SmtpServerPort;
            ns.SmtpUserName = bs.SmtpUserName;
            ns.SmtpPassword = bs.SmtpPassword;
            ns.EmailSubjectPrefix = bs.EmailSubjectPrefix;
            ns.EnableSsl = bs.EnableSsl;
            ns.SendMailOnComment = bs.SendMailOnComment;

            // controls
            ns.SearchButtonText = bs.SearchButtonText;
            ns.SearchCommentLabelText = bs.SearchCommentLabelText;
            ns.SearchDefaultText = bs.SearchDefaultText;
            ns.EnableCommentSearch = bs.EnableCommentSearch;
            ns.ShowIncludeCommentsOption = bs.ShowIncludeCommentsOption;
            ns.ContactFormMessage = bs.ContactFormMessage;
            ns.ContactThankMessage = bs.ContactThankMessage;
            ns.ContactErrorMessage = bs.ContactErrorMessage;
            ns.EnableContactAttachments = bs.EnableContactAttachments;
            ns.EnableRecaptchaOnContactForm = bs.EnableRecaptchaOnContactForm;
            ns.ErrorTitle = bs.ErrorTitle;
            ns.ErrorText = bs.ErrorText;

            // custom code
            ns.HtmlHeader = bs.HtmlHeader;
            ns.TrackingScript = bs.TrackingScript;

            // comments
            ns.DaysCommentsAreEnabled = bs.DaysCommentsAreEnabled;
            ns.IsCommentsEnabled = bs.IsCommentsEnabled;
            ns.EnableCommentsModeration = bs.EnableCommentsModeration;
            ns.IsCommentNestingEnabled = bs.IsCommentNestingEnabled;
            ns.Avatar = bs.Avatar;
            ns.EnablePingBackSend = bs.EnablePingBackSend;
            ns.EnablePingBackReceive = bs.EnablePingBackReceive;
            ns.EnableTrackBackSend = bs.EnableTrackBackSend;
            ns.EnableTrackBackReceive = bs.EnableTrackBackReceive;
            ns.CommentsPerPage = bs.CommentsPerPage;
            ns.EnableCountryInComments = bs.EnableCountryInComments;
            ns.EnableWebsiteInComments = bs.EnableWebsiteInComments;
            ns.ShowLivePreview = bs.ShowLivePreview;

            ns.ModerationType = bs.ModerationType;
            ns.UseDisqus = bs.ModerationType == BlogSettings.Moderation.Disqus;
            ns.DisqusDevMode = bs.DisqusDevMode;
            ns.DisqusAddCommentsToPages = bs.DisqusAddCommentsToPages;
            ns.DisqusWebsiteName = bs.DisqusWebsiteName;

            // custom filters
            ns.CommentWhiteListCount = bs.CommentWhiteListCount;
            ns.CommentBlackListCount = bs.CommentBlackListCount;
            ns.AddIpToWhitelistFilterOnApproval = bs.AddIpToWhitelistFilterOnApproval;
            ns.TrustAuthenticatedUsers = bs.TrustAuthenticatedUsers;
            ns.BlockAuthorOnCommentDelete = bs.BlockAuthorOnCommentDelete;
            ns.AddIpToBlacklistFilterOnRejection = bs.AddIpToBlacklistFilterOnRejection;

            return ns;
        }

        /// <summary>
        /// Update settings
        /// </summary>
        /// <param name="ns">New settings item</param>
        /// <returns>True on success</returns>
        public bool Update(Settings ns)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminSettingsPages))
                throw new System.UnauthorizedAccessException();

            var bs = BlogSettings.Instance;

            bs.Name = ns.Name;
            bs.Description = ns.Description;
            bs.PostsPerPage = ns.PostsPerPage;
            bs.ThemeCookieName = ns.ThemeCookieName;
            bs.UseBlogNameInPageTitles = ns.UseBlogNameInPageTitles;
            bs.EnableRelatedPosts = ns.EnableRelatedPosts;
            bs.EnableRating = ns.EnableRating;
            bs.ShowDescriptionInPostList = ns.ShowDescriptionInPostList;
            bs.DescriptionCharacters = ns.DescriptionCharacters;
            bs.ShowDescriptionInPostListForPostsByTagOrCategory = ns.ShowDescriptionInPostListForPostsByTagOrCategory;
            bs.DescriptionCharactersForPostsByTagOrCategory = ns.DescriptionCharactersForPostsByTagOrCategory;
            bs.TimeStampPostLinks = ns.TimeStampPostLinks;
            bs.ShowPostNavigation = ns.ShowPostNavigation;
            bs.Culture = ns.Culture;
            bs.Timezone = ns.Timezone;
            bs.RemoveExtensionsFromUrls = ns.RemoveExtensionsFromUrls;
            bs.RedirectToRemoveFileExtension = ns.RedirectToRemoveFileExtension;
            bs.Theme = ns.DesktopTheme;

            // advanced
            bs.HandleWwwSubdomain = ns.HandleWwwSubdomain;
            bs.EnableHttpCompression = ns.EnableHttpCompression;
            bs.CompressWebResource = ns.CompressWebResource;
            bs.EnableOpenSearch = ns.EnableOpenSearch;
            bs.RequireSslMetaWeblogApi = ns.RequireSslForMetaWeblogApi;

            bs.EnablePasswordReset = ns.EnablePasswordReset;
            bs.EnableSelfRegistration = ns.EnableSelfRegistration;
            bs.CreateBlogOnSelfRegistration = ns.CreateBlogOnSelfRegistration;
            bs.AllowServerToDownloadRemoteFiles = ns.AllowServerToDownloadRemoteFiles;
            bs.RemoteFileDownloadTimeout = ns.RemoteFileDownloadTimeout;
            bs.RemoteMaxFileSize = ns.RemoteMaxFileSize;
            bs.SelfRegistrationInitialRole = ns.SelfRegistrationInitialRole;

            // feed
            bs.AuthorName = ns.AuthorName;
            bs.FeedAuthor = ns.FeedAuthor;
            bs.Endorsement = ns.Endorsement;
            bs.AlternateFeedUrl = ns.AlternateFeedUrl;
            bs.Language = ns.Language;
            bs.PostsPerFeed = ns.PostsPerFeed;
            bs.EnableEnclosures = ns.EnableEnclosures;
            bs.SyndicationFormat = ns.SyndicationFormat;
            bs.EnableTagExport = ns.EnableTagExport;

            // email
            bs.Email = ns.Email;
            bs.SmtpServer = ns.SmtpServer;
            bs.SmtpServerPort = ns.SmtpServerPort;
            bs.SmtpUserName = ns.SmtpUserName;
            bs.SmtpPassword = ns.SmtpPassword;
            bs.EmailSubjectPrefix = ns.EmailSubjectPrefix;
            bs.EnableSsl = ns.EnableSsl;
            bs.SendMailOnComment = ns.SendMailOnComment;

            // controls
            bs.SearchButtonText = ns.SearchButtonText;
            bs.SearchCommentLabelText = ns.SearchCommentLabelText;
            bs.SearchDefaultText = ns.SearchDefaultText;
            bs.EnableCommentSearch = ns.EnableCommentSearch;
            bs.ShowIncludeCommentsOption = ns.ShowIncludeCommentsOption;
            bs.ContactFormMessage = ns.ContactFormMessage;
            bs.ContactThankMessage = ns.ContactThankMessage;
            bs.ContactErrorMessage = ns.ContactErrorMessage;
            bs.EnableContactAttachments = ns.EnableContactAttachments;
            bs.EnableRecaptchaOnContactForm = ns.EnableRecaptchaOnContactForm;
            bs.ErrorTitle = ns.ErrorTitle;
            bs.ErrorText = ns.ErrorText;

            // custom code
            bs.HtmlHeader = ns.HtmlHeader;
            bs.TrackingScript = ns.TrackingScript;

            // settings
            bs.DaysCommentsAreEnabled = ns.DaysCommentsAreEnabled;
            bs.IsCommentsEnabled = ns.IsCommentsEnabled;
            bs.EnableCommentsModeration = ns.EnableCommentsModeration;
            bs.IsCommentNestingEnabled = ns.IsCommentNestingEnabled;
            bs.Avatar = ns.Avatar;
            bs.EnablePingBackSend = ns.EnablePingBackSend;
            bs.EnablePingBackReceive = ns.EnablePingBackReceive;
            bs.EnableTrackBackSend = ns.EnableTrackBackSend;
            bs.EnableTrackBackReceive = ns.EnableTrackBackReceive;
            bs.CommentsPerPage = ns.CommentsPerPage;
            bs.EnableCountryInComments = ns.EnableCountryInComments;
            bs.EnableWebsiteInComments = ns.EnableWebsiteInComments;
            bs.ShowLivePreview = ns.ShowLivePreview;

            bs.ModerationType = ns.ModerationType;
            //bs.ModerationType = ns.UseDisqus ? BlogSettings.Moderation.Disqus : BlogSettings.Moderation.Auto;
            bs.DisqusDevMode = ns.DisqusDevMode;
            bs.DisqusAddCommentsToPages = ns.DisqusAddCommentsToPages;
            bs.DisqusWebsiteName = ns.DisqusWebsiteName;

            // custom filters
            bs.CommentWhiteListCount = ns.CommentWhiteListCount;
            bs.CommentBlackListCount = ns.CommentBlackListCount;
            bs.AddIpToWhitelistFilterOnApproval = ns.AddIpToWhitelistFilterOnApproval;
            bs.TrustAuthenticatedUsers = ns.TrustAuthenticatedUsers;
            bs.BlockAuthorOnCommentDelete = ns.BlockAuthorOnCommentDelete;
            bs.AddIpToBlacklistFilterOnRejection = ns.AddIpToBlacklistFilterOnRejection;

            bs.Save();
            return true;
        }
    }
}
