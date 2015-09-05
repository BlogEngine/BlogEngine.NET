// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds an author list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;
    using BlogEngine.Core;
    using BlogEngine.Core.Data.Services;

    using Resources;

    /// <summary>
    /// Builds an author list.
    /// </summary>
    public class AuthorList : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The show rss icon.
        /// </summary>
        private static Dictionary<Guid, bool> blogsShowRssIcon = new Dictionary<Guid, bool>();

        private static Dictionary<Guid, bool> blogsShowAuthorImg = new Dictionary<Guid, bool>();

        private int authorImgSize = 24;

        private static string widgetCacheKey = "widget_authorlist";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="AuthorList"/> class. 
        /// </summary>
        static AuthorList()
        {
            Post.Saved += ClearCache;
            AuthorProfile.Saved += ClearCache;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether or not to show feed icons next to the category links.
        /// </summary>
        public bool ShowRssIcon
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;

                if (!blogsShowRssIcon.ContainsKey(blogId))
                    blogsShowRssIcon[blogId] = true;

                return blogsShowRssIcon[blogId];
            }

            set
            {
                if (ShowRssIcon == value)
                {
                    return;
                }

                blogsShowRssIcon[Blog.CurrentInstance.Id] = value;
            }
        }

        /// <summary>
        ///     Gets or sets either to show author image in the authors list
        /// </summary>
        public bool ShowAuthorImg
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;

                if (!blogsShowAuthorImg.ContainsKey(blogId))
                    blogsShowAuthorImg[blogId] = true;

                return blogsShowAuthorImg[blogId];
            }

            set
            {
                if (ShowAuthorImg == value)
                {
                    return;
                }

                blogsShowAuthorImg[Blog.CurrentInstance.Id] = value;
            }
        }

        public int AuthorImgSize
        {
            get
            {
                return authorImgSize;
            }

            set
            {
                if (authorImgSize == value)
                {
                    return;
                }

                authorImgSize = value;
            }
        }

        /// <summary>
        ///     Gets the rendered HTML in the private field and first
        ///     updates it when a post has been saved (new or updated).
        /// </summary>
        private string Html
        {
            get
            {
                var widgetHrml = Blog.CurrentInstance.Cache[widgetCacheKey] as string;

                if (string.IsNullOrEmpty(widgetHrml))
                {
                    widgetHrml = Utils.RenderControl(BindAuthors());
                    Blog.CurrentInstance.Cache.Insert(widgetCacheKey, widgetHrml);
                }

                return widgetHrml;
            }
        }

        #endregion

        #region Properties from widget Edit

        public int MaxAuthors { get; set; }

        public string DisplayPattern { get; set; }

        public string PatternAggregated { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(Html);
            writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        private static void ClearCache(object sender, EventArgs e)
        {
            Blog.CurrentInstance.Cache.Remove(widgetCacheKey);

            Blog siteAggregationBlog = Blog.SiteAggregationBlog;
            if (siteAggregationBlog != null)
            {
                siteAggregationBlog.Cache.Remove(widgetCacheKey);
                AuthorProfile.RemoveProfile(siteAggregationBlog.Id);
            }
        }

        /// <summary>
        /// Loops through all users and builds the HTML
        /// presentation.
        /// </summary>
        /// <returns>The authors.</returns>
        private HtmlGenericControl BindAuthors()
        {
            if (Post.Posts.Count == 0)
            {
                var p = new HtmlGenericControl("p") { InnerHtml = labels.none };
                return p;
            }

            var ul = new HtmlGenericControl("ul") { ID = "authorlist" };
            ul.Attributes.Add("class", "authorlist");

            IEnumerable<MembershipUser> users = Membership.GetAllUsers()
                .Cast<MembershipUser>()
                .ToList()
                .OrderBy(a => a.UserName);

            int userCnt = 0;

            foreach (MembershipUser user in users)
            {
                if (userCnt >= MaxAuthors && MaxAuthors > 0)
                    break;

                var blog = Post.GetBlogByAuthor(user.UserName);
                if(blog == null)
                    continue;

                var blogName = blog.IsPrimary ? "" : blog.Name + "/";

                var postCount = Post.GetPostsByAuthor(user.UserName).Count;
                if (postCount == 0)
                    continue;

                var li = new HtmlGenericControl("li");

                if (ShowRssIcon)
                {
                    var img = new HtmlImage
                    {
                        Src = string.Format("{0}Content/images/blog/rssButton.png", Utils.RelativeWebRoot),
                        Alt = string.Format("RSS feed for {0}", user.UserName)
                    };
                    img.Attributes["class"] = "rssButton";

                    var feedAnchor = new HtmlAnchor
                    {
                        HRef =
                            string.Format("{0}{1}syndication.axd?author={2}", Utils.ApplicationRelativeWebRoot, blogName, Utils.RemoveIllegalCharacters(user.UserName))
                    };
                    feedAnchor.Attributes["rel"] = "nofollow";
                    feedAnchor.Controls.Add(img);

                    li.Controls.Add(feedAnchor);
                }

                if (ShowAuthorImg)
                {
                    var img = new HtmlImage
                    {
                        Src = Avatar.GetSrc(user.Email),
                        Alt = "author avatar",
                        Width = authorImgSize,
                        Height = authorImgSize
                    };
                    img.Attributes["class"] = "author-avatar";

                    var authorAnchor = new HtmlAnchor
                    {
                        HRef = string.Format("{0}{1}syndication.axd?author={2}", 
                        Utils.ApplicationRelativeWebRoot, blogName, Utils.RemoveIllegalCharacters(user.UserName))
                    };
                    authorAnchor.Attributes["rel"] = "nofollow";
                    authorAnchor.Controls.Add(img);

                    li.Controls.Add(authorAnchor);
                }

                var innerHtml = "";
                try
                {
                    innerHtml = Blog.CurrentInstance.IsSiteAggregation ?
                    string.Format(PatternAggregated, user.UserName, blog.Name, postCount) :
                    string.Format(DisplayPattern, user.UserName, postCount);
                }
                catch (Exception)
                {
                    innerHtml = Blog.CurrentInstance.IsSiteAggregation ?
                    string.Format("{0}@{1} ({2})", user.UserName, blog.Name, postCount) :
                    string.Format("{0} ({1})", user.UserName, postCount);
                }

                var anc = new HtmlAnchor
                {
                    HRef = string.Format("{0}{1}author/{2}{3}", Utils.ApplicationRelativeWebRoot, blogName, Utils.RemoveIllegalCharacters(user.UserName), BlogConfig.FileExtension),
                    InnerHtml = innerHtml,
                    Title = string.Format("Author: {0}", user.UserName)
                };
                anc.Attributes.Add("class", "authorlink");

                li.Controls.Add(anc);
                ul.Controls.Add(li);
                userCnt++;
            }

            return ul;
        }

        #endregion
    }
}