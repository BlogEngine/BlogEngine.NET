// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.VisitorInfo
{
    using System;
    using System.Collections.Generic;

    using App_Code.Controls;

    using BlogEngine.Core;

    /// <summary>
    /// The widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Constants and Fields

        /// <summary>
        /// The number of comments.
        /// </summary>
        private int numberOfComments;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsEditable.
        /// </summary>
        public override bool IsEditable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets Name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Visitor info";
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
            Visible = false;
            var cookie = Request.Cookies["comment"];

            if (cookie == null)
            {
                return;
            }

            var name = cookie.Values["name"];
            var email = cookie.Values["email"];
            var website = cookie.Values["url"];

            if (name == null)
            {
                return;
            }

            name = name.Replace("+", " ");
            WriteHtml(name, email, website);

            Uri url;
            if (Request.QueryString["apml"] == null && Uri.TryCreate(website, UriKind.Absolute, out url))
            {
                phScript.Visible = true;

                // ltWebsite.Text = url.ToString();
            }

            Visible = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the commented posts.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="website">The website.</param>
        /// <returns>A list of Post</returns>
        private List<Post> GetCommentedPosts(string email, string website)
        {
            var list = new List<Post>();
            foreach (var post in Post.Posts)
            {
                var comments = post.Comments.FindAll(
                    c => email.Equals(c.Email, StringComparison.OrdinalIgnoreCase) ||
                         (c.Website != null &&
                          c.Website.ToString().Equals(website, StringComparison.OrdinalIgnoreCase)));

                if (comments.Count <= 0)
                {
                    continue;
                }
                
                numberOfComments += comments.Count;
                var index = post.Comments.IndexOf(comments[comments.Count - 1]);
                if (index < post.Comments.Count - 1 &&
                    post.Comments[post.Comments.Count - 1].DateCreated > DateTime.Now.AddDays(-7))
                {
                    list.Add(post);
                }
            }

            return list;
        }

        /// <summary>
        /// Writes the HTML.
        /// </summary>
        /// <param name="name">The name to write.</param>
        /// <param name="email">The email.</param>
        /// <param name="website">The website.</param>
        private void WriteHtml(string name, string email, string website)
        {
            if (name.Contains(" "))
            {
                name = name.Substring(0, name.IndexOf(" "));
            }

            var avatar = BlogEngine.Core.Data.Services.Avatar.GetSrc(email);
            var avatarLink = avatar == null || avatar == null ? string.Empty : avatar;
            Title = string.Format(
                String.Concat("<img src=\"{0}\" alt=\"{1}\" align=\"top\" /> ", Resources.labels.visitorHi, " {1}"), avatarLink, Server.HtmlEncode(name));
            pName.InnerHtml = "<strong>" + Resources.labels.welcomeBack + "</strong>";
            var list = GetCommentedPosts(email, website);

            if (list.Count > 0)
            {
                var link = string.Format(
                    "<a href=\"{0}\">{1}</a>", list[0].RelativeLink, Server.HtmlEncode(list[0].Title));
                pComment.InnerHtml = string.Format(Resources.labels.commentsAddedSince, link);
            }

            if (numberOfComments > 0)
            {
                pComment.InnerHtml += string.Format(Resources.labels.writtenCommentsTotal, numberOfComments);
            }
        }

        #endregion
    }
}