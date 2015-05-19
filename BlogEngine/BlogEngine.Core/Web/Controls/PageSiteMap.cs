namespace BlogEngine.Core.Web.Controls
{
    using System;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// A site map provider for the Pages in BlogEngine.
    /// </summary>
    public class PageSiteMap : SiteMapProvider
    {
        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, retrieves a <see cref="T:System.Web.SiteMapNode"></see> object that represents the page at the specified URL.
        /// </summary>
        /// <param name="rawUrl">
        /// A URL that identifies the page for which to retrieve a <see cref="T:System.Web.SiteMapNode"></see>.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the page identified by rawURL; otherwise, null, if no corresponding <see cref="T:System.Web.SiteMapNode"></see> is found or if security trimming is enabled and the <see cref="T:System.Web.SiteMapNode"></see> cannot be returned for the current user.
        /// </returns>
        public override SiteMapNode FindSiteMapNode(string rawUrl)
        {
            var i = rawUrl.IndexOf('?');
            var url = rawUrl;
            if (i > 0)
            {
                url = rawUrl.Substring(0, i);
            }

            var start = url.LastIndexOf('/') + 1;
            var stop = url.LastIndexOf('.');
            url = url.Substring(start, stop - start);

            return Page.Pages
                .Where(page => page.IsVisible && url.Equals(Utils.RemoveIllegalCharacters(page.Title), StringComparison.OrdinalIgnoreCase))
                .Select(page => new SiteMapNode(this, page.Id.ToString(), page.RelativeLink, page.Title, page.Description)).FirstOrDefault();
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the child nodes of a specific <see cref="T:System.Web.SiteMapNode"></see>.
        /// </summary>
        /// <param name="node">
        /// The <see cref="T:System.Web.SiteMapNode"></see> for which to retrieve all child nodes.
        /// </param>
        /// <returns>
        /// A read-only <see cref="T:System.Web.SiteMapNodeCollection"></see> that contains the immediate child nodes of the specified <see cref="T:System.Web.SiteMapNode"></see>; otherwise, null or an empty collection, if no child nodes exist.
        /// </returns>
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            var col = new SiteMapNodeCollection();
            var id = new Guid(node.Key);
            foreach (var page in Page.Pages.Where(page => page.IsVisible && page.Parent == id && page.ShowInList))
            {
                col.Add(new SiteMapNode(this, page.Id.ToString(), page.RelativeLink, page.Title, page.Description));
            }

            return col;
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the parent node of a specific <see cref="T:System.Web.SiteMapNode"></see> object.
        /// </summary>
        /// <param name="node">
        /// The <see cref="T:System.Web.SiteMapNode"></see> for which to retrieve the parent node.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the parent of node; otherwise, null, if the <see cref="T:System.Web.SiteMapNode"></see> has no parent or security trimming is enabled and the parent node is not accessible to the current user.
        /// </returns>
        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            var id = new Guid(node.Key);
            var parentId = Page.GetPage(id).Parent;
            if (parentId != Guid.Empty && parentId != id)
            {
                var parent = Page.GetPage(parentId);
                if (parent.IsVisible)
                {
                    return new SiteMapNode(
                        this, parent.Id.ToString(), parent.RelativeLink, parent.Title, parent.Description);
                }
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overidden in a derived class, retrieves the root node of all the nodes that are currently managed by the current provider.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the root node of the set of nodes that the current provider manages.
        /// </returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            var page = Page.GetFrontPage();
            return page != null ? new SiteMapNode(this, page.Id.ToString(), page.RelativeLink, page.Title) : new SiteMapNode(this, Guid.Empty.ToString(), "~/", "Home");
        }

        #endregion
    }
}