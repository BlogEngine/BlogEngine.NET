namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Implementation of the XmlSiteMapProvider that is Rights aware.
    /// </summary>
    public class SecuritySiteMapProvider : XmlSiteMapProvider
    {
        /// <summary>
        /// Returns whether the SiteMapNode is accessible to the current user.
        /// </summary>
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            // We are checking Rights, and other custom attributes here.
            // Roles may also be part of the SiteMapNode.  Let the base class
            // check for that.  If false, return false, otherwise, continue
            // with our checks.

            if (!base.IsAccessibleToUser(context, node))
                return false;

            bool primaryBlogInstanceOnly;
            if (!string.IsNullOrWhiteSpace(node["primaryBlogInstanceOnly"]) && bool.TryParse(node["primaryBlogInstanceOnly"], out primaryBlogInstanceOnly))
            {
                if (primaryBlogInstanceOnly && !Blog.CurrentInstance.IsPrimary)
                {
                    return false;
                }
            }

            if (!String.IsNullOrWhiteSpace(node["rights"]))
            {
                // By default, all specified Rights must exist.
                // We allow this to be overridden via the "rightsAuthorizationCheck"
                // attribute.

                AuthorizationCheck authCheck = AuthorizationCheck.HasAll;
                if (!String.IsNullOrWhiteSpace(node["rightsAuthorizationCheck"]))
                {
                    authCheck = Utils.ParseEnum<AuthorizationCheck>(node["rightsAuthorizationCheck"], AuthorizationCheck.HasAll);
                }

                string[] rightsRaw = node["rights"].Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                List<Rights> rightsToCheck = new List<Rights>();
                foreach (string r in rightsRaw)
                {
                    Rights right = Utils.ParseEnum<Rights>(r.Trim(), Rights.None);
                    if (right != Rights.None)
                        rightsToCheck.Add(right);
                }

                if (rightsToCheck.Count > 0)
                {
                    return Security.IsAuthorizedTo(authCheck, rightsToCheck.ToArray());
                }
            }

            return true;
        }
    }
}
