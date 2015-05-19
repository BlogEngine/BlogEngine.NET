// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.RecentComments
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

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
        /// The default number of comments.
        /// </summary>
        private const int DefaultNumberOfComments = 10;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "Widget" /> class.
        /// </summary>
        static Widget()
        {
            Post.CommentAdded += ClearCache;
            Post.CommentRemoved += ClearCache;
            Post.CommentUpdated += ClearCache;
            BlogSettings.Changed += ClearCache;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether IsEditable.
        /// </summary>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets Name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RecentComments";
            }
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            var settings = this.GetSettings();
            var numberOfComments = DefaultNumberOfComments;
            if (settings.ContainsKey("numberofcomments"))
            {
                numberOfComments = int.Parse(settings["numberofcomments"]);
            }

            if (Blog.CurrentInstance.Cache["widget_recentcomments"] == null)
            {
                var comments = new List<Comment>();

                foreach (var post in Post.ApplicablePosts.Where(post => post.IsVisible))
                {
                    comments.AddRange(
                        post.Comments.FindAll(c => c.IsApproved && c.Email.Contains("@")));
                }

                comments.Sort();
                comments.Reverse();

                var max = Math.Min(comments.Count, numberOfComments);
                var list = comments.GetRange(0, max);
                Blog.CurrentInstance.Cache.Insert("widget_recentcomments", list);
            }

            var content = RenderComments((List<Comment>)Blog.CurrentInstance.Cache["widget_recentcomments"]);

            var html = new LiteralControl(content);

            // new LiteralControl((string)Blog.CurrentInstance.Cache["widget_recentcomments"]);
            this.phPosts.Controls.Add(html);
        }

        #endregion

        #region Methods

        private static void ClearCache(object sender, EventArgs e)
        {
            Blog.CurrentInstance.Cache.Remove("widget_recentcomments");

            Blog siteAggregationBlog = Blog.SiteAggregationBlog;
            if (siteAggregationBlog != null)
            {
                siteAggregationBlog.Cache.Remove("widget_recentcomments");
            }
        }

        /// <summary>
        /// Renders the comments.
        /// </summary>
        /// <param name="comments">The comments.</param>
        /// <returns>The rendered html.</returns>
        private static string RenderComments(ICollection<Comment> comments)
        {
            if (comments.Count == 0)
            {
                // Blog.CurrentInstance.Cache.Insert("widget_recentcomments", "<p>" + Resources.labels.none + "</p>");
                return string.Format("<p>{0}</p>", labels.none);
            }

            var ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "recentComments");
            ul.ID = "recentComments";

            foreach (var comment in comments)
            {
                if (comment.IsApproved)
                {
                    var li = new HtmlGenericControl("li");

                    // The post title
                    var title = new HtmlAnchor { HRef = comment.Parent.RelativeOrAbsoluteLink, InnerText = comment.Parent.Title };
                    title.Attributes.Add("class", "postTitle");
                    li.Controls.Add(title);

                    // The comment count on the post
                    var count =
                        new LiteralControl(string.Format(" ({0})<br />", ((Post)comment.Parent).ApprovedComments.Count));
                    li.Controls.Add(count);

                    // The author
                    if (comment.Website != null)
                    {
                        using (var author = new HtmlAnchor())
                        {
                            author.Attributes.Add("rel", "nofollow");
                            author.HRef = comment.Website.ToString();
                            author.InnerHtml = comment.Author;
                            li.Controls.Add(author);
                        }

                        using (var wrote = new LiteralControl(string.Format(" {0}: ", labels.wrote)))
                        {
                            li.Controls.Add(wrote);
                        }
                    }
                    else
                    {
                        using (var author = new LiteralControl(string.Format("{0} {1}: ", comment.Author, labels.wrote)))
                        {
                            li.Controls.Add(author);
                        }
                    }

                    // The comment body
                    var commentBody = Regex.Replace(comment.Content, @"\[(.*?)\]", string.Empty);
                    var bodyLength = Math.Min(commentBody.Length, 50);

                    commentBody = commentBody.Substring(0, bodyLength);
                    if (commentBody.Length > 0)
                    {
                        if (commentBody[commentBody.Length - 1] == '&')
                        {
                            commentBody = commentBody.Substring(0, commentBody.Length - 1);
                        }
                    }

                    commentBody += comment.Content.Length <= 50 ? " " : "... ";
                    var body = new LiteralControl(commentBody);
                    li.Controls.Add(body);

                    // The comment link
                    using (
                        var link = new HtmlAnchor
                            {
                                HRef = string.Format("{0}#id_{1}", comment.Parent.RelativeOrAbsoluteLink, comment.Id), 
                                InnerHtml = string.Format("[{0}]", labels.more)
                            })
                    {
                        link.Attributes.Add("class", "moreLink");
                        li.Controls.Add(link);
                    }

                    ul.Controls.Add(li);
                }
            }

            var sw = new StringWriter();
            ul.RenderControl(new HtmlTextWriter(sw));

            string Ahref =
                String.Concat("<a href=\"{0}syndication.axd?comments=true\">", labels.commentRSS, " <img src=\"{0}Content/images/blog/rssButton.png\" alt=\"\" /></a>");
            var rss = string.Format(Ahref, Utils.RelativeOrAbsoluteWebRoot);
            sw.Write(rss);
            return sw.ToString();

            // Blog.CurrentInstance.Cache.Insert("widget_recentcomments", sw.ToString());
        }

        #endregion
    }
}