using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Tag repository
    /// </summary>
    public class TagRepository : ITagRepository
    {
        /// <summary>
        /// Get tag list
        /// </summary>
        /// <param name="take">Items per page, default 10, 0 to return all</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="postId">Post id</param>
        /// <param name="order">Sort order, for example order=DateCreated,desc</param>
        /// <returns>List of tags</returns>
        public IEnumerable<Data.Models.TagItem> Find(int take = 10, int skip = 0, string postId = "", string order = "TagName")
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewPublicPosts))
                throw new System.UnauthorizedAccessException();

            var tags = new List<TagItem>();

            foreach (var p in BlogEngine.Core.Post.ApplicablePosts)
            {
                // if blogId passed in, only for this blog
                if (!string.IsNullOrEmpty(postId))
                {
                    Guid gId;
                    if (Guid.TryParse(postId, out gId))
                        if (p.Id != gId) continue;
                }

                foreach (var t in p.Tags)
                {
                    // for every tag either add to collection
                    // or increment counter if already added
                    var tmp = tags.FirstOrDefault(tag => tag.TagName == t);
                    if (tmp == null)
                        tags.Add(new TagItem { TagName = t, TagCount = 1 });
                    else
                        tmp.TagCount++;
                }
            }

            var query = tags.AsQueryable();

            if (take == 0)
                take = tags.Count;

            if (string.IsNullOrEmpty(order))
                order = "TagName";

            return query.OrderBy(order).Skip(skip).Take(take);
        }

        /// <summary>
        /// Updates tag
        /// </summary>
        /// <param name="updateFrom">Value from</param>
        /// <param name="updateTo">Value to</param>
        /// <returns>True on success</returns>
        public bool Save(string updateFrom, string updateTo)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();
            try
            {
                foreach (var p in BlogEngine.Core.Post.Posts.ToArray())
                {
                    var tg = p.Tags.FirstOrDefault(tag => tag == updateFrom);
                    if (tg != null)
                    {
                        p.Tags.Remove(tg);
                        p.Tags.Add(updateTo);
                        p.DateModified = DateTime.Now;
                        p.Save();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("TagRepository.Update: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Delete tag in all posts
        /// </summary>
        /// <param name="id">Tag</param>
        /// <returns>True on success</returns>
        public bool Delete(string id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();
            try
            {
                foreach (var p in BlogEngine.Core.Post.Posts.ToArray())
                {
                    var tg = p.Tags.FirstOrDefault(tag => tag == id);
                    if (tg != null)
                    {
                        p.Tags.Remove(tg);
                        p.DateModified = DateTime.Now;
                        p.Save();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Tags.Delete: {0}", ex.Message));
                return false;
            }
        }
    }
}
