namespace App_Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using BlogEngine.Core;

    public static class WebUtils
    {
        /// <summary>
        /// Checks to see if the current user has the rights to access an
        /// admin settings page.
        /// </summary>
        /// <param name="checkOnly">
        /// If true, check only. If false and rights are insufficient, user
        /// will be redirected to the login page.
        /// </param>
        /// <returns>True if user has sufficient rights</returns>
        public static bool CheckRightsForAdminSettingsPage(bool checkOnly)
        {
            if (checkOnly)
            {
                return
                    Security.IsAuthorizedTo(AuthorizationCheck.HasAll,
                        BlogEngine.Core.Rights.AccessAdminSettingsPages);
            }
            else
            {
                Security.DemandUserHasRight(AuthorizationCheck.HasAll, true,
                    BlogEngine.Core.Rights.AccessAdminSettingsPages);
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the current user has the rights to moderate comments pages.
        /// </summary>
        /// <param name="checkOnly">
        /// If true, check only. If false and rights are insufficient, user
        /// will be redirected to the login page.
        /// </param>
        /// <returns>True if user has sufficient rights</returns>
        public static bool CheckRightsForAdminCommentsPages(bool checkOnly)
        {
            if (checkOnly)
            {
                return
                    Security.IsAuthorizedTo(AuthorizationCheck.HasAll,
                        BlogEngine.Core.Rights.ModerateComments);
            }
            else
            {
                Security.DemandUserHasRight(AuthorizationCheck.HasAll, true,
                    BlogEngine.Core.Rights.ModerateComments);
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the current user has the rights to view Pages pages.
        /// </summary>
        /// <param name="checkOnly">
        /// If true, check only. If false and rights are insufficient, user
        /// will be redirected to the login page.
        /// </param>
        /// <returns>True if user has sufficient rights</returns>
        public static bool CheckRightsForAdminPagesPages(bool checkOnly)
        {
            Rights[] rights =
            {
                Rights.CreateNewPages,
                Rights.EditOwnPages
            };

            if (checkOnly)
            {
                return Security.IsAuthorizedTo(AuthorizationCheck.HasAny, rights);
            }
            else
            {
                Security.DemandUserHasRight(AuthorizationCheck.HasAny, true, rights);
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the current user has the rights to view Post pages.
        /// </summary>
        /// <param name="checkOnly">
        /// If true, check only. If false and rights are insufficient, user
        /// will be redirected to the login page.
        /// </param>
        /// <returns>True if user has sufficient rights</returns>
        public static bool CheckRightsForAdminPostPages(bool checkOnly)
        {
            Rights[] rights =
            {
                Rights.CreateNewPosts,
                Rights.EditOwnPosts,
                Rights.EditOtherUsersPosts,
                Rights.PublishOwnPosts,
                Rights.PublishOtherUsersPosts
            };

            if (checkOnly)
            {
                return Security.IsAuthorizedTo(AuthorizationCheck.HasAny, rights);
            }
            else
            {
                Security.DemandUserHasRight(AuthorizationCheck.HasAny, true, rights);
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the current blog is the primary blog.
        /// </summary>
        /// <param name="checkOnly">
        /// If true, check only. If false and is not the primary blog, user
        /// will be redirected to the login page.
        /// </param>
        /// <returns>True if user has sufficient rights</returns>
        public static bool CheckIfPrimaryBlog(bool checkOnly)
        {
            if (checkOnly)
            {
                return Blog.CurrentInstance.IsPrimary;
            }
            else
            {
                if (!Blog.CurrentInstance.IsPrimary)
                {
                    Security.RedirectForUnauthorizedRequest();
                    return false;
                }
            }

            return true;
        }
    
    }
}