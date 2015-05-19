// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.RecentPosts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Linq;

    using App_Code.Controls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Constants and Fields

        /// <summary>
        /// The default number of posts.
        /// </summary>
        private const int DefaultNumberOfPosts = 10;

        /// <summary>
        /// The default show comments.
        /// </summary>
        private const bool DefaultShowComments = true;

        /// <summary>
        /// The default show rating.
        /// </summary>
        private const bool DefaultShowRating = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Widget"/> class.
        /// </summary>
        static Widget()
        {
            Post.Saved += ClearCache;
            Post.CommentAdded += ClearCache;
            Post.CommentRemoved += ClearCache;
            Post.Rated += ClearCache;
            BlogSettings.Changed += ClearCache;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsEditable.
        /// </summary>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets Name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RecentPosts";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        /// data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            var settings = this.GetSettings();
            var numberOfPosts = DefaultNumberOfPosts;
            if (settings.ContainsKey("numberofposts"))
            {
                numberOfPosts = int.Parse(settings["numberofposts"]);
            }

            if (Blog.CurrentInstance.Cache["widget_recentposts"] == null)
            {   
                var visiblePosts = Post.ApplicablePosts.FindAll(p => p.IsVisibleToPublic);

                var max = Math.Min(visiblePosts.Count, numberOfPosts);
                var list = visiblePosts.GetRange(0, max);
                Blog.CurrentInstance.Cache.Insert("widget_recentposts", list);
            }

            var content = RenderPosts((List<Post>)Blog.CurrentInstance.Cache["widget_recentposts"], settings);

            var html = new LiteralControl(content);
                
                // new LiteralControl((string)Blog.CurrentInstance.Cache["widget_recentposts"]);
            this.phPosts.Controls.Add(html);
        }

        #endregion

        #region Methods

        private static void ClearCache(object sender, EventArgs e)
        {
            Blog.CurrentInstance.Cache.Remove("widget_recentposts");

            Blog siteAggregationBlog = Blog.SiteAggregationBlog;
            if (siteAggregationBlog != null)
            {
                siteAggregationBlog.Cache.Remove("widget_recentposts");
            }
        }

        /// <summary>
        /// Renders the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The rendered html.</returns>
        private static string RenderPosts(List<Post> posts, StringDictionary settings)
        {
            if (posts.Count == 0)
            {
                // Blog.CurrentInstance.Cache.Insert("widget_recentposts", "<p>" + Resources.labels.none + "</p>");
                return string.Format("<p>{0}</p>", labels.none);
            }

            var sb = new StringBuilder();
            sb.Append("<ul class=\"recentPosts\" id=\"recentPosts\">");

            var showComments = DefaultShowComments;
            var showRating = DefaultShowRating;
            if (settings.ContainsKey("showcomments"))
            {
                bool.TryParse(settings["showcomments"], out showComments);
            }

            if (settings.ContainsKey("showrating"))
            {
                bool.TryParse(settings["showrating"], out showRating);
            }

            foreach (var post in posts)
            {
                if (!post.IsVisibleToPublic)
                {
                    continue;
                }

                // in rear case when aggregated blog has cached list of posts
                // and child blog was deleted, skip this post
                // TODO: remove posts from aggregated cache on child blog delete
                try { var link = post.RelativeLink; } catch { continue; }

                var rating = Math.Round(post.Rating, 1).ToString(CultureInfo.InvariantCulture);

                const string LinkFormat = "<li><a href=\"{0}\">{1}</a>{2}{3}</li>";

                var comments = string.Format("<span>{0}: {1}</span>", labels.comments, post.ApprovedComments.Count);

                if (BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus)
                {
                    comments = string.Format(
                        "<span><a href=\"{0}#disqus_thread\">{1}</a></span>", post.PermaLink, labels.comments);
                }

                var rate = string.Format("<span>{0}: {1} / {2}</span>", labels.rating, rating, post.Raters);

                if (!showComments || !BlogSettings.Instance.IsCommentsEnabled)
                {
                    comments = null;
                }

                if (!showRating || !BlogSettings.Instance.EnableRating)
                {
                    rate = null;
                }
                else if (post.Raters == 0)
                {
                    rate = string.Format("<span>{0}</span>", labels.notRatedYet);
                }

                sb.AppendFormat(LinkFormat, post.RelativeOrAbsoluteLink, HttpUtility.HtmlEncode(post.Title), comments, rate);
            }

            sb.Append("</ul>");

            // Blog.CurrentInstance.Cache.Insert("widget_recentposts", sb.ToString());
            return sb.ToString();
        }

        #endregion
    }
}