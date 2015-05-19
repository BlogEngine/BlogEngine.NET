// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds a blogs list.
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
    /// Builds a blog list.
    /// </summary>
    public class BlogList : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The html string.
        /// </summary>
        private static string blogsHtml = "";

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="BlogList"/> class. 
        /// </summary>
        static BlogList()
        {
            BlogEngine.Core.Blog.Saved += (sender, args) =>
            {
                lock (syncRoot) {
                    blogsHtml = "";
                }
            };
        }

        #endregion

        #region Properties

        private static string Html {
            get
            {
                lock (syncRoot) 
                {    
                    if (string.IsNullOrEmpty(blogsHtml)) 
                    {
                        var ul = BindBlogs();
                        blogsHtml = BlogEngine.Core.Utils.RenderControl(ul);
                    }
                    return blogsHtml;
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
            writer.Write(BlogList.Html);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loops through all pages and builds the HTML
        /// presentation.
        /// </summary>
        /// <returns>A list item.</returns>
        private static HtmlGenericControl BindBlogs()
        {
            var ul = new HtmlGenericControl("ul") { ID = "bloglist" };
            ul.Attributes.Add("class", "bloglist");

            var blogs = Blog.Blogs.Where(b => b.IsActive).ToList();

            // blog used for aggregation always on top, then all alphabetically sorted
            foreach (var blog in blogs.OrderByDescending(b => b.IsSiteAggregation).ThenBy(b => b.Name))
            {
                var li = new HtmlGenericControl("li");
                var href = blog.RelativeWebRoot;
               
                var anc = new HtmlAnchor { HRef = href, InnerHtml = blog.Name, Title = blog.Name };

                li.Controls.Add(anc);
                ul.Controls.Add(li);
            }

            return ul;
        }

        #endregion
    }
}