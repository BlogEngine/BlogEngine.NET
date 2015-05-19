using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web.UI;

namespace BlogEngine.Core.Web.Controls
{
    /// <summary>
    /// Base class for related posts control
    /// </summary>
    public class RelatedPostsBase : UserControl
    {
        readonly string _cacheKey = "RELATED_POSTS_FOR_";
        int _postCount = 3;
        int _descLength = 100;

        /// <summary>
        /// Number of posts to display in the list
        /// </summary>
        public int PostCount
        {
            get { return _postCount; }
            set { _postCount = value; }
        }

        /// <summary>
        /// Descriptions maximum length
        /// </summary>
        public int DescriptionMaxLength
        {
            get { return _descLength; }
            set { _descLength = value; }
        }    

        /// <summary>
        /// Current post to find related for
        /// </summary>
        public IPublishable PostItem { get; set; }

        /// <summary>
        /// Related post lists
        /// </summary>
        public List<RelatedPost> RelatedPostList
        {
            get
            {
                try
                {
                    var CacheKey = _cacheKey + PostItem.Id.ToString();
                    if (Blog.CurrentInstance.Cache[CacheKey] == null)
                    {
                        Blog.CurrentInstance.Cache.Add(
                            CacheKey,
                            FindRelated(),
                            null,
                            Cache.NoAbsoluteExpiration,
                            new TimeSpan(0, 15, 0),
                            CacheItemPriority.Low,
                            null);
                    }
                    return (List<RelatedPost>)Blog.CurrentInstance.Cache[CacheKey];
                }
                catch (Exception ex)
                {
                    Utils.Log("Error getting related posts", ex);
                    return new List<RelatedPost>();
                }
            }
        }

        List<RelatedPost> FindRelated()
        {
            var list = new List<RelatedPost>();
            if (PostItem != null)
            {
                var relatedPosts = Search.FindRelatedItems(PostItem.Title);
                var cnt = 1;
                foreach (var item in relatedPosts)
                {
                    list.Add(new RelatedPost { Title = item.Title, Link = item.RelativeLink, Description = GetDescription(item) });
                    if (cnt >= _postCount)
                        break;
                    cnt++;
                }
            }
            return list;
        }

        string GetDescription(IPublishable post)
        {
            var description = Utils.StripHtml(post.Description);
            if (description != null && description.Length > this.DescriptionMaxLength)
            {
                description = string.Format("{0}...", description.Substring(0, this.DescriptionMaxLength));
            }

            if (String.IsNullOrEmpty(description))
            {
                var content = Utils.StripHtml(post.Content);
                description = content.Length > this.DescriptionMaxLength
                    ? string.Format("{0}...", content.Substring(0, this.DescriptionMaxLength))
                    : content;
            }
            return description;
        }
    }

    /// <summary>
    /// Related post item
    /// </summary>
    public class RelatedPost
    {
        /// <summary>
        /// Post title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Post link
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Post description
        /// </summary>
        public string Description { get; set; }
    }
}
