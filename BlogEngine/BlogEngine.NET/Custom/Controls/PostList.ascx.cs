namespace UserControls
{
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    /// <summary>
    /// The post list user control.
    /// </summary>
    public partial class PostList : UserControl
    {
        #region Constants and Fields

        /// <summary>
        ///     The posts.
        /// </summary>
        private List<IPublishable> publishables;

        /// <summary>
        ///     Initializes a new instance of the <see cref = "PostList" /> class.
        /// </summary>
        public PostList()
        {
            this.ContentBy = ServingContentBy.AllContent;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the criteria by which the content is being served (by tag, category, author, etc).
        /// </summary>
        public ServingContentBy ContentBy { get; set; }

        /// <summary>
        ///     Gets or sets the list of posts to display.
        /// </summary>
        public List<IPublishable> Posts
        {
            get
            {
                return this.publishables;
            }

            set
            {
                this.publishables = value;
                this.pager1.Posts = value;
            }
        }

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
            {
                this.Visible = false;
            }

        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.Page.IsCallback)
            {
                return;
            }

            this.BindPosts();
            //this.InitPaging();
        }

        /// <summary>
        /// Binds the list of posts to individual postview.ascx controls
        ///     from the current theme.
        /// </summary>
        private void BindPosts()
        {
			if (this.Posts == null) {
				// no posts provided, load all posts by default
				Posts = Post.ApplicablePosts.ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; }));
			}
			
            if (this.Posts.Count == 0)
            {
                this.hlPrev.Visible = false;
                return;
            }

            var visiblePosts = this.Posts.FindAll(p => p.IsVisible);

            var count = Math.Min(BlogSettings.Instance.PostsPerPage, visiblePosts.Count);
            var page = this.GetPageIndex();
            var index = page * count;
            var stop = count;
            if (index + count > visiblePosts.Count)
            {
                stop = visiblePosts.Count - index;
            }

            if (stop < 0 || stop + index > visiblePosts.Count)
            {
                this.hlPrev.Visible = false;
                this.hlNext.Visible = false;
                return;
            }

            var theme = Request.QueryString["theme"];
            if(!string.IsNullOrEmpty(theme))
                theme = theme.SanitizePath();

            var path = string.Format("{0}Custom/Themes/{1}/PostView.ascx", Utils.ApplicationRelativeWebRoot, BlogSettings.Instance.GetThemeWithAdjustments(theme));
            var counter = 0;

            if (!System.IO.File.Exists(Server.MapPath(path)))
                path = string.Format("{0}Custom/Controls/Defaults/PostView.ascx", Utils.ApplicationRelativeWebRoot);

            foreach (Post post in visiblePosts.GetRange(index, stop))
            {
                if (counter == stop)
                {
                    break;
                }

                var postView = (PostViewBase)this.LoadControl(path);
                postView.ShowExcerpt = ShowExcerpt();
                postView.Post = post;
                postView.ID = post.Id.ToString().Replace("-", string.Empty);
                postView.Location = ServingLocation.PostList;
                postView.Index = counter;
                this.posts.Controls.Add(postView);
                counter++;
            }

            if (index + stop >= visiblePosts.Count)
            {
                this.hlPrev.Visible = false;
            }
        }

        /// <summary>
        /// Retrieves the current page index based on the QueryString.
        /// </summary>
        /// <returns>
        /// The get page index.
        /// </returns>
        private int GetPageIndex()
        {
            int index;
            if (int.TryParse(this.Request.QueryString["page"], out index))
            {
                index--;
            }

            return index;
        }

        /// <summary>
        /// Initializes the Next and Previous links
        /// </summary>
        private void InitPaging()
        {
            var path = this.Request.RawUrl.Replace("Default.aspx", string.Empty);

            path = path.Contains("?")
                       ? (path.Contains("page=")
                              ? path.Substring(0, path.IndexOf("page="))
                              : $"{path}&")
                       : $"{path}?";

            var page = this.GetPageIndex();
            var url = string.Format("{0}page={{0}}", path);

            // if (page != 1)
            this.hlNext.HRef = string.Format(url, page);

            // else
            // hlNext.HRef = path.Replace("?", string.Empty);
            this.hlPrev.HRef = string.Format(url, page + 2);

            if (page == 0)
            {
                this.hlNext.Visible = false;
            }
            else
            {
                if(this.hlNext.Visible)
                    ((BlogBasePage)this.Page).AddGenericLink("next", "Next page", this.hlNext.HRef);
            }

            if (this.hlPrev.Visible)
            {
                ((BlogBasePage)this.Page).AddGenericLink("prev", "Previous page", string.Format(url, page + 2));
            }
        }

        /// <summary>
        /// Whether or not to show the entire post or just the excerpt/description
        /// in the post list 
        /// </summary>
        /// <returns></returns>
        private bool ShowExcerpt()
        {
            string url = this.Request.RawUrl.ToUpperInvariant();
            bool tagOrCategory = url.Contains("/CATEGORY/") || url.Contains("?TAG=/");

            return BlogSettings.Instance.ShowDescriptionInPostList ||
                (BlogSettings.Instance.ShowDescriptionInPostListForPostsByTagOrCategory && tagOrCategory);
        }

        #endregion
    }
}
