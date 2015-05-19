using System.Web.UI;

namespace BlogEngine.Core.Web.Controls
{
    /// <summary>
    /// Post navigation base
    /// </summary>
    public class PostNavigationBase : UserControl
    {
        Post next
        {
            get
            {
                return GetNextPost(CurrentPost);
            }
        }
        Post prev
        {
            get
            {
                return GetPrevPost(CurrentPost);
            }
        }

        /// <summary>
        /// Current post
        /// </summary>
        public Post CurrentPost { get; set; }

        /// <summary>
        /// Next post relative url
        /// </summary>
        public string NextPostUrl 
        {
            get 
            {
                if (next == null)
                    return string.Empty;

                return next.RelativeLink;
            } 
        }

        /// <summary>
        /// Next post title
        /// </summary>
        public string NextPostTitle
        {
            get
            {
                if (next == null)
                    return string.Empty;

                return next.Title;
            }
        }

        /// <summary>
        /// Previous post relative url
        /// </summary>
        public string PreviousPostUrl
        {
            get
            {
                if (prev == null)
                    return string.Empty;

                return prev.RelativeLink;
            }
        }

        /// <summary>
        /// Previous post title
        /// </summary>
        public string PreviousPostTitle
        {
            get
            {
                if (prev == null)
                    return string.Empty;

                return prev.Title;
            }
        }

        #region Private methods

        /// <summary>
        /// Gets the next post filtered for invisible posts.
        /// </summary>
        private Post GetNextPost(Post post)
        {
            if (post.Next == null)
                return null;

            if (post.Next.IsVisible)
                return post.Next;

            return GetNextPost(post.Next);
        }

        /// <summary>
        /// Gets the prev post filtered for invisible posts.
        /// </summary>
        private Post GetPrevPost(Post post)
        {
            if (post.Previous == null)
                return null;

            if (post.Previous.IsVisible)
                return post.Previous;

            return GetPrevPost(post.Previous);
        }

        #endregion
    }
}