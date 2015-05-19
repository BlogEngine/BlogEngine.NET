namespace Admin
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The Menu control.
    /// </summary>
    public partial class Menu : UserControl
    {
        #region Public Methods

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="text">
        /// The text string.
        /// </param>
        /// <param name="url">
        /// The URL string.
        /// </param>
        public void AddItem(string text, string url)
        {
            var a = new HtmlAnchor { InnerHtml = string.Format("<span>{0}</span>", text), HRef = url };

            System.Diagnostics.Debug.WriteLine("AddItem: " + a.HRef.ToString());

            var li = new HtmlGenericControl("li");
            li.Controls.Add(a);
            ulMenu.Controls.Add(li);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsCallback)
            {
                BindMenu();
            }
        }

        /// <summary>
        /// The bind menu.
        /// </summary>
        private void BindMenu()
        {
            var sitemap = SiteMap.Providers["SecuritySiteMap"];
            if (sitemap != null)
            {
                string adminRootFolder = string.Format("{0}admin", Utils.RelativeWebRoot);

                var root = sitemap.RootNode;
                if (root != null)
                {
                    foreach (
                        var adminNode in root.ChildNodes.Cast<SiteMapNode>().Where(
                            adminNode => adminNode.IsAccessibleToUser(HttpContext.Current)).Where(
                            adminNode => Security.IsAuthenticated ))
                    {
                        if (adminNode.Url.EndsWith("#/blogs") && !Blog.CurrentInstance.IsPrimary)
                            continue;

                        if (adminNode.Url.EndsWith("#/blogs") && !Security.IsAdministrator)
                            continue;

                        var a = new HtmlAnchor
                        {
                            // replace the RelativeWebRoot in adminNode.Url with the RelativeWebRoot of the current
                            // blog instance.  So a URL like /admin/Dashboard.aspx becomes /blog/admin/Dashboard.aspx.
                            HRef = Utils.RelativeWebRoot + adminNode.Url.Substring(Utils.ApplicationRelativeWebRoot.Length), 
                            InnerHtml = string.Format("<span>{0}</span>", Utils.Translate(adminNode.Title, adminNode.Title))
                        };

                        var li = new HtmlGenericControl("li");
                        li.Controls.Add(a);
                        ulMenu.Controls.Add(li);
                    }
                }
            }

            if (Security.IsAuthenticated)
            {
                AddItem(labels.myProfile, string.Format("{0}admin/#/users/profile", Utils.RelativeWebRoot));
                AddItem(labels.changePassword, string.Format("{0}Account/change-password.aspx", Utils.RelativeWebRoot));
            }
        }

        #endregion
    }
}