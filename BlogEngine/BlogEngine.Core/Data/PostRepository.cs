using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Post repository
    /// </summary>
    public class PostRepository : IPostRepository
    {
        /// <summary>
        /// Post list
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns>List of posts</returns>
        public IEnumerable<PostItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewPublicPosts))
                throw new System.UnauthorizedAccessException();

            var posts = new List<PostItem>();

            var postList = Post.ApplicablePosts.Where(p => p.IsVisible).ToList();

            if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                postList = postList.Where(p => p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower()).ToList();

            if (take == 0) take = postList.Count;
            if (string.IsNullOrEmpty(filter)) filter = "1==1";
            if (string.IsNullOrEmpty(order)) order = "DateCreated desc";

            var query = postList.AsQueryable().Where(filter);

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                posts.Add(Json.GetPost(item));

            return posts;
        }

        /// <summary>
        /// Get single post
        /// </summary>
        /// <param name="id">Post id</param>
        /// <returns>Post object</returns>
        public PostDetail FindById(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
                throw new UnauthorizedAccessException();
            try
            {
                return Json.GetPostDetail((from p in Post.Posts.ToList() where p.Id == id select p).FirstOrDefault());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Add new post
        /// </summary>
        /// <param name="detail">Post</param>
        /// <returns>Saved post with new ID</returns>
        public PostDetail Add(PostDetail detail)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
                throw new UnauthorizedAccessException();

            var post = new Post();
                        
            Save(post, detail);
            return Json.GetPostDetail(post);
        }

        /// <summary>
        /// Update post
        /// </summary>
        /// <param name="detail">Post to update</param>
        /// <param name="action">Action</param>
        /// <returns>True on success</returns>
        public bool Update(PostDetail detail, string action)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOwnPosts))
                throw new System.UnauthorizedAccessException();

            var post = (from p in Post.Posts.ToList() where p.Id == detail.Id select p).FirstOrDefault();

            if (post != null)
            {
                if (action == "publish")
                {
                    post.IsPublished = true;
                    post.DateModified = DateTime.Now;
                    post.Save();
                }
                else if (action == "unpublish")
                {
                    post.IsPublished = false;
                    post.DateModified = DateTime.Now;
                    post.Save();
                }
                else
                {
                    Save(post, detail);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.DeleteOwnPosts))
                throw new System.UnauthorizedAccessException();
            try
            {
                var post = (from p in Post.Posts.ToList() where p.Id == id select p).FirstOrDefault();
                post.Delete();
                post.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Private Methods

        static void Save(Post post, PostDetail detail)
        {
            post.Title = detail.Title;
            post.Author = string.IsNullOrEmpty(detail.Author) ? Security.CurrentUser.Identity.Name : detail.Author;
            post.Description = GetDescription(detail.Description, detail.Content);
            post.Content = detail.Content;
            post.IsPublished = detail.IsPublished;
            post.HasCommentsEnabled = detail.HasCommentsEnabled;
            post.IsDeleted = detail.IsDeleted;
            post.DateCreated = DateTime.ParseExact(detail.DateCreated, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            // if changing slug, should be unique
            if (post.Slug != detail.Slug)
                post.Slug = GetUniqueSlug(detail.Slug);

            UpdatePostCategories(post, detail.Categories);
            UpdatePostTags(post, FilterTags(detail.Tags));

            post.Save();
        }

        static void UpdatePostCategories(Post post, List<CategoryItem> categories)
        {
            post.Categories.Clear();

            if (categories == null)
                return;

            foreach (var cat in categories)
            {
                // add if category does not exist
                var existingCat = Category.Categories.Where(c => c.Title == cat.Title).FirstOrDefault();
                if (existingCat == null)
                {
                    var repo = new CategoryRepository();
                    post.Categories.Add(Category.GetCategory(repo.Add(cat).Id));
                }
                else
                {
                    post.Categories.Add(Category.GetCategory(existingCat.Id));
                }
                
            }
        }

        static void UpdatePostTags(Post post, List<TagItem> tags)
        {
            post.Tags.Clear();

            if (tags == null)
                return;

            foreach (var t in tags)
            {
                post.Tags.Add(t.TagName);
            }
        }

        static List<TagItem> FilterTags(List<TagItem> tags)
        {
            var uniqueTags = new List<TagItem>();

            if (tags == null)
                return uniqueTags;

            foreach (var t in tags)
            {
                if (!uniqueTags.Any(u => u.TagName == t.TagName))
                {
                    uniqueTags.Add(t);
                }
            }
            return uniqueTags;
        }

        static string GetUniqueSlug(string slug)
        {
            string s = Utils.RemoveIllegalCharacters(slug.Trim());

            // will do for up to 100 unique post titles
            for (int i = 1; i < 101; i++)
            {
                if (IsUniqueSlug(s))
                    break;

                s = $"{slug}{i}";
            }
            return s;
        }

        static bool IsUniqueSlug(string slug)
        {
            return Post.ApplicablePosts.Where(p => p.Slug != null && p.Slug.ToLower() == slug.ToLower())
                .FirstOrDefault() == null ? true : false;
        }

        // if description not set, use first 100 chars in the post
        static string GetDescription(string desc, string content)
        {
            if (string.IsNullOrEmpty(desc))
            {
                var p = Utils.StripHtml(content);

                if (p.Length > 100)
                    return p.Substring(0, 100);

                return p;
            }
            return desc;
        }

        #endregion
    }
}