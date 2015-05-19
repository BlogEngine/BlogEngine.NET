// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Shows a chronological list of recent posts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// Shows a chronological list of recent posts.
    /// </summary>
    public class RecentPosts : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// The posts.
        /// </summary>
        private static readonly Dictionary<Guid, List<Post>> blogsPosts = new Dictionary<Guid, List<Post>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="RecentPosts"/> class.
        /// </summary>
        static RecentPosts()
        {
            BuildPostList();
            Post.Saved += PostSaved;
            Post.CommentAdded += ClearCache;
            Post.CommentRemoved += ClearCache;
            Post.Rated += ClearCache;
            BlogSettings.Changed += ClearCache;
        }

        #endregion

        #region Properties

        private static List<Post> Posts
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;

                if (!blogsPosts.ContainsKey(blogId))
                {
                    lock (syncRoot)
                    {
                        if (!blogsPosts.ContainsKey(blogId))
                        {
                            List<Post> posts = BuildPostList();
                            blogsPosts[blogId] = posts;
                        }
                    }
                }

                return blogsPosts[blogId];
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
            if (this.Page.IsCallback)
            {
                return;
            }

            var html = RenderPosts();
            writer.Write(html);
        }

        #endregion

        #region Methods

        private static void ClearCache(object sender, EventArgs e)
        {
            Guid blogId = Blog.CurrentInstance.BlogId;
            blogsPosts.Remove(blogId);
            
            Blog siteAggregationBlog = Blog.SiteAggregationBlog;
            if (siteAggregationBlog != null)
            {
                blogsPosts.Remove(siteAggregationBlog.BlogId);
            }
        }

        /// <summary>
        /// Builds the post list.
        /// </summary>
        private static List<Post> BuildPostList()
        {
            var number = Math.Min(BlogSettings.Instance.NumberOfRecentPosts, Post.ApplicablePosts.Count);

            List<Post> returnPosts = new List<Post>();
            foreach (var post in Post.ApplicablePosts.Where(post => post.IsVisibleToPublic).Take(number))
            {
                returnPosts.Add(post);
            }

            return returnPosts;
        }

        /// <summary>
        /// Handles the Saved event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        private static void PostSaved(object sender, SavedEventArgs e)
        {
            if (e.Action == SaveAction.Update)
            {
                return;
            }

            ClearCache(null, EventArgs.Empty);
        }

        /// <summary>
        /// Renders the posts.
        /// </summary>
        /// <returns>The HTML string.</returns>
        private static string RenderPosts()
        {
            if (Posts.Count == 0)
            {
                return string.Format("<p>{0}</p>", labels.none);
            }

            var sb = new StringBuilder();
            sb.Append("<ul class=\"recentPosts\" id=\"recentPosts\">");

            foreach (var post in Posts.Where(post => post.IsVisibleToPublic))
            {
                var rating = Math.Round(post.Rating, 1).ToString(CultureInfo.InvariantCulture);

                const string Link = "<li><a href=\"{0}\">{1}</a>{2}{3}</li>";
                var comments = string.Format("<span>{0}: {1}</span>", labels.comments, post.ApprovedComments.Count);
                var rate = string.Format("<span>{0}: {1} / {2}</span>", labels.rating, rating, post.Raters);

                if (!BlogSettings.Instance.DisplayCommentsOnRecentPosts || !BlogSettings.Instance.IsCommentsEnabled)
                {
                    comments = null;
                }

                if (!BlogSettings.Instance.DisplayRatingsOnRecentPosts || !BlogSettings.Instance.EnableRating)
                {
                    rate = null;
                }

                if (post.Raters == 0)
                {
                    rate = string.Format("<span>{0}</span>", labels.notRatedYet);
                }

                sb.AppendFormat(Link, post.RelativeOrAbsoluteLink, HttpUtility.HtmlEncode(post.Title), comments, rate);
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        #endregion
    }
}