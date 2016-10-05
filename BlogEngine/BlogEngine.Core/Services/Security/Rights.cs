using System;

namespace BlogEngine.Core
{

    /// <summary>
    /// Enum that represents rights or permissions that are used through out BlogEngine.
    /// </summary>
    /// <remarks>
    /// 
    /// Each Rights enum value is wrapped by an associated Right instance that contains information about roles/descriptions/etc.
    /// 
    /// When a Rights value is serialized to persistant storage, the enum's string name should be used in order to prevent
    /// conflicts with value changes due to new values being added(either through updates or customization).
    /// 
    /// Also, at the moment this doesn't nearly represent all the current possible actions. This is just a few
    /// test values to play with.
    /// 
    /// I'd recommend using a common word pattern when used. Ie: Create/Edit/Delete/Publish as prefixes. The names
    /// should be very specific to what they allow in order to avoid confusion. For example, don't use a name like
    /// "ViewPosts". Use something that also specifies the kinds of posts, like ViewPublicPosts, ViewPrivatePosts, or
    /// ViewUnpublishedPosts.
    /// 
    /// </remarks>
    public enum Rights
    {

        /// <summary>
        /// Represents a user that has no rights or permissions. This flag should not be used in combination with any other flag.
        /// </summary>
        /// <remarks>
        /// 
        /// This value isn't meant for public consumption.
        /// 
        /// </remarks>
        None = 0,

        #region Misc

        /// <summary>
        /// A user is allowed to view exception messages.
        /// </summary>
        [RightDetails(Category = RightCategory.General)]
        ViewDetailedErrorMessages,

        /// <summary>
        /// A user is allowed to access administration dashboard.
        /// </summary>
        [RightDetails(Category = RightCategory.General)]
        ViewDashboard,

        /// <summary>
        /// A user is allowed to access administration pages.
        /// Typically, a blog where self-registration is allowed
        /// would restrict this right from guest users.
        /// </summary>
        [RightDetails(Category = RightCategory.General)]
        AccessAdminPages,

        /// <summary>
        /// A user is allowed to access admin settings pages.
        /// </summary>
        [RightDetails(Category = RightCategory.General)]
        AccessAdminSettingsPages,

        #endregion

        #region "Comments"

        /// <summary>
        /// A user is allowed to view comments on a post.
        /// </summary>
        [RightDetails(Category = RightCategory.Comments)]
        ViewPublicComments,

        /// <summary>
        /// A user is allowed to view comments that have not been moderation yet.
        /// </summary>
        [RightDetails(Category = RightCategory.Comments)]
        ViewUnmoderatedComments,

        /// <summary>
        /// A user is allowed to create and submit comments for posts or pages.
        /// </summary>
        [RightDetails(Category = RightCategory.Comments)]
        CreateComments,

        /// <summary>
        /// User can approve, delete, or mark comments as spam.
        /// </summary>
        [RightDetails(Category = RightCategory.Comments)]
        ModerateComments,

        #endregion
 
        #region Posts

        /// <summary>
        /// A user is allowed to view posts that are both published and public.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        ViewPublicPosts,

        /// <summary>
        /// A user is allowed to view unpublished posts.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        ViewUnpublishedPosts,

        ///// <summary>
        ///// A user is allowed to view non-public posts.
        ///// </summary>
        // 11/6/2010 - commented out, we don't currently have "private" posts, just unpublished.
        //[RightDetails(Category = RightCategory.Posts)]
        //ViewPrivatePosts,

        /// <summary>
        /// A user can create new posts. 
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        CreateNewPosts,

        /// <summary>
        /// A user can edit their own posts. 
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        EditOwnPosts,

        /// <summary>
        /// A user can edit posts created by other users.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        EditOtherUsersPosts,

        /// <summary>
        /// A user can delete their own posts.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        DeleteOwnPosts,

        /// <summary>
        /// A user can delete posts created by other users.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        DeleteOtherUsersPosts,

        /// <summary>
        /// A user can set whether or not their own posts are published.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        PublishOwnPosts,

        /// <summary>
        /// A user can set whether or not another user's posts are published.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        PublishOtherUsersPosts,

        #endregion

        #region Pages
        
        /// <summary>
        /// A user can view public, published pages.
        /// </summary>
        [RightDetails(Category = RightCategory.Pages)]
        ViewPublicPages,

        /// <summary>
        /// A user can view unpublished pages.
        /// </summary>
        [RightDetails(Category = RightCategory.Pages)]
        ViewUnpublishedPages,

        /// <summary>
        /// A user can create new pages.
        /// </summary>
        [RightDetails(Category = RightCategory.Pages)]
        CreateNewPages,

        /// <summary>
        /// A user can edit pages they've created.
        /// </summary>
        [RightDetails(Category = RightCategory.Pages)]
        EditOwnPages,

        #endregion

        #region "Ratings"

        /// <summary>
        /// A user can view ratings on posts.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        ViewRatingsOnPosts,

        /// <summary>
        /// A user can submit ratings on posts.
        /// </summary>
        [RightDetails(Category = RightCategory.Posts)]
        SubmitRatingsOnPosts,
        #endregion

        #region Roles

        /// <summary>
        /// A user can view roles.
        /// </summary>
        [RightDetails(Category = RightCategory.Roles)]
        ViewRoles,

        /// <summary>
        /// A user can create new roles.
        /// </summary>
        [RightDetails(Category = RightCategory.Roles)]
        CreateNewRoles,

        /// <summary>
        /// A user can edit existing roles.
        /// </summary>
        [RightDetails(Category = RightCategory.Roles)]
        EditRoles,

        /// <summary>
        /// A user can delete existing roles.
        /// </summary>
        [RightDetails(Category = RightCategory.Roles)]
        DeleteRoles,

        /// <summary>
        /// A user is allowed to edit their own roles.
        /// </summary>
        [RightDetails(Category = RightCategory.Roles)]
        EditOwnRoles,

        /// <summary>
        /// A user is allowed to edit the roles of other users.
        /// </summary>
        [RightDetails(Category = RightCategory.Roles)]
        EditOtherUsersRoles,

        #endregion

        #region Users

        /// <summary>
        /// A user is allowed to register/create a new account. 
        /// </summary>
        [RightDetails(Category = RightCategory.Users)]
        CreateNewUsers,

        /// <summary>
        /// A user is allowed to delete their own account.
        /// </summary>
        [RightDetails(Category = RightCategory.Users)]
        DeleteUserSelf,

        /// <summary>
        /// A user is allowed to delete accounts they do not own.
        /// </summary>
        [RightDetails(Category = RightCategory.Users)]
        DeleteUsersOtherThanSelf,

        /// <summary>
        /// A user is allowed to edit their own account information.
        /// </summary>
        [RightDetails(Category = RightCategory.Users)]
        EditOwnUser,

        /// <summary>
        /// A user is allowed to edit the account information of other users.
        /// </summary>
        [RightDetails(Category=RightCategory.Users)]
        EditOtherUsers,

        #endregion

        #region Custom

        /// <summary>
        /// Manage extensions
        /// </summary>
        [RightDetails(Category = RightCategory.Custom)]
        ManageExtensions,

        /// <summary>
        /// A user is allowed to manage widgets.
        /// </summary>
        [RightDetails(Category = RightCategory.Custom)]
        ManageWidgets,

        /// <summary>
        /// Manage themes
        /// </summary>
        [RightDetails(Category = RightCategory.Custom)]
        ManageThemes,

        /// <summary>
        /// Manage NuGet packages
        /// </summary>
        [RightDetails(Category = RightCategory.Custom)]
        ManagePackages

        #endregion
    }


    /// <summary>
    /// Attribute used to provide extra information about a Rights enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public sealed class RightDetailsAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RightDetailsAttribute()
        {

        }

        #region "Properties"

        /// <summary>
        /// Key for grabbing a description from a resource file.
        /// </summary>
        public string DescriptionResourceLabelKey { get; set; }

        /// <summary>
        /// Key for grabbing a name from a resource file.
        /// </summary>
        public string NameResourceLabelKey { get; set; }

        /// <summary>
        /// The category a Right is for.
        /// </summary>
        public RightCategory Category { get; set; }

        #endregion

    }

    /// <summary>
    /// Categories for Rights.
    /// </summary>
    public enum RightCategory
    {
        /// <summary>
        /// No category
        /// </summary>
        None,

        /// <summary>
        /// General category
        /// </summary>
        General,

        /// <summary>
        /// Comments category
        /// </summary>
        Comments,

        /// <summary>
        /// Pages category
        /// </summary>
        Pages,

        /// <summary>
        /// Post category
        /// </summary>
        Posts,

        /// <summary>
        /// Users category
        /// </summary>
        Users,

        /// <summary>
        /// Roles
        /// </summary>
        Roles,

        /// <summary>
        /// Extensions, themes, widgets and packages
        /// </summary>
        Custom
    }
}