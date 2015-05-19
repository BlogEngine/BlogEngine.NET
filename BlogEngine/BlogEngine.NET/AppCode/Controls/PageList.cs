// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds a page list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;
    using BlogEngine.Core;

    /// <summary>
    /// Builds a page list.
    /// </summary>
    public class PageList : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The html string.
        /// </summary>
        private static Dictionary<Guid, string> blogsHtml = new Dictionary<Guid, string>();

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="PageList"/> class. 
        /// </summary>
        static PageList()
        {
            BlogEngine.Core.Page.Saved += (sender, args) =>
            {
                lock (syncRoot) {
                    // only refresh current blog
                    blogsHtml.Remove(Blog.CurrentInstance.Id);
                }
            };
        }

        #endregion

        #region Properties

        private static string Html {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;
                lock (syncRoot) {
                    
                    if (!blogsHtml.ContainsKey(blogId)) {
                        if (BlogEngine.Core.Page.Pages != null) {
                            var ul = BindPages();
                            blogsHtml[blogId] = BlogEngine.Core.Utils.RenderControl(ul);
                        } else {
                            blogsHtml[blogId] = string.Empty;
                        }
                    }

                    return blogsHtml[blogId];
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(PageList.Html);
           // writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loops through all pages and builds the HTML
        /// presentation.
        /// </summary>
        /// <returns>A list item.</returns>
        private static HtmlGenericControl BindPages()
        {
            var ul = new HtmlGenericControl("ul") { ID = "pagelist" };
            ul.Attributes.Add("class", "pagelist");

            foreach (var page in BlogEngine.Core.Page.Pages.Where(page => page.ShowInList && page.IsVisibleToPublic))
            {
                var li = new HtmlGenericControl("li");
                var href = page.RelativeLink;
                if (BlogSettings.Instance.RemoveExtensionsFromUrls && !string.IsNullOrEmpty(BlogConfig.FileExtension))
                    href = href.Replace(BlogConfig.FileExtension, "");
                
                var anc = new HtmlAnchor { HRef = href, InnerHtml = page.Title, Title = page.Description };

                li.Controls.Add(anc);
                ul.Controls.Add(li);
            }

            return ul;
        }

        #endregion
    }
}