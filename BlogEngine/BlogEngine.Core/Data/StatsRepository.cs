using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System.Linq;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Statistics
    /// </summary>
    public class StatsRepository : IStatsRepository
    {
        /// <summary>
        /// Get stats info
        /// </summary>
        /// <returns>Stats counters</returns>
        public Stats Get()
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            var stats = new Stats();

            var postList = Post.ApplicablePosts.Where(p => p.IsVisible).ToList();

            if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                postList = postList.Where(p => p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower()).ToList();

            stats.PublishedPostsCount = postList.Count(p => p.IsPublished == true);
            stats.DraftPostsCount = postList.Count(p => p.IsPublished == false);

            stats.PublishedPagesCount = Page.Pages.Count(p => p.IsPublished == true && p.IsDeleted == false);
            stats.DraftPagesCount = Page.Pages.Count(p => p.IsPublished == false && p.IsDeleted == false);
            
            CountComments(stats);

            stats.CategoriesCount = Category.Categories.Count;
            stats.TagsCount = 2;
            stats.UsersCount = 3;
            Subscribers(stats);

            return stats;
        }

        void Subscribers(Stats stats)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            var filename = System.IO.Path.Combine(Blog.CurrentInstance.StorageLocation, "newsletter.xml");
            filename = System.Web.Hosting.HostingEnvironment.MapPath(filename);

            if (System.IO.File.Exists(filename))
            {
                var doc = new System.Xml.XmlDocument();
                doc.Load(filename);
                System.Xml.XmlNodeList list = doc.GetElementsByTagName("email");
                stats.SubscribersCount += (list.Count).ToString();
            }
        }

        void CountComments(Stats stats)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            foreach (var post in Post.Posts)
            {
                if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOtherUsersPosts))
                    if (post.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                        continue;

                stats.PublishedCommentsCount += post.Comments.Count(c => c.IsPublished == true && c.IsDeleted == false);
                stats.UnapprovedCommentsCount += post.Comments.Count(c => c.IsPublished == false && c.IsSpam == false && c.IsDeleted == false);
                stats.SpamCommentsCount += post.Comments.Count(c => c.IsPublished == false && c.IsSpam == true && c.IsDeleted == false);
            }
        }

    }
}
