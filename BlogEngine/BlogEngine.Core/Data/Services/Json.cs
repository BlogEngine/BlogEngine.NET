using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlogEngine.Core.Data.Services
{
    /// <summary>
    /// Helper class for Json operation
    /// </summary>
    public class Json
    {
        /// <summary>
        /// Get post converted to Json
        /// </summary>
        /// <param name="post">Post</param>
        /// <returns>Json post</returns>
        public static PostItem GetPost(Post post)
        {
            return new PostItem
            {
                Id = post.Id,
                Author = post.Author,
                Title = post.Title,
                Slug = post.Slug,
                RelativeLink = post.RelativeLink,
                DateCreated = post.DateCreated.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                Categories = GetCategories(post.Categories),
                Tags = GetTags(post.Tags),
                Comments = GetComments(post),
                IsPublished = post.IsPublished,
            };
        }
        /// <summary>
        /// Get detailed post
        /// </summary>
        /// <param name="post">Post</param>
        /// <returns>Json post detailed</returns>
        public static PostDetail GetPostDetail(Post post)
        {
            return new PostDetail
            {
                Id = post.Id,
                Author = post.Author,
                Title = post.Title,
                Slug = post.Slug,
                Description = post.Description,
                RelativeLink = post.RelativeLink,
                Content = post.Content,
                DateCreated = post.DateCreated.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                Categories = GetCategories(post.Categories),
                Tags = GetTags(post.Tags),
                Comments = GetComments(post),
                HasCommentsEnabled = post.HasCommentsEnabled,
                IsPublished = post.IsPublished,
                IsDeleted = post.IsDeleted,
                CanUserEdit = post.CanUserEdit,
                CanUserDelete = post.CanUserDelete
            };
        }
        /// <summary>
        /// Get page converted to json
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>Json page</returns>
        public static PageItem GetPage(Page page)
        {
            Page parent = null;
            SelectOption parentOption = null;

            if (page.Parent != Guid.Empty)
            {
                parent = Page.Pages.FirstOrDefault(p => p.Id.Equals(page.Parent));
                parentOption = new SelectOption { IsSelected = false, OptionName = parent.Title, OptionValue = parent.Id.ToString() };
            }
            return new PageItem
            {
                Id = page.Id,
                ShowInList = page.ShowInList,
                Title = page.Title,
                Slug = page.Slug,
                Parent = parentOption,
                Keywords = page.Keywords,
                DateCreated = page.DateCreated.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                HasChildren = page.HasChildPages,
                IsPublished = page.IsPublished,
                IsFrontPage = page.IsFrontPage,
                SortOrder = page.SortOrder,
            };
        }
        /// <summary>
        /// Get page details
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>Json page details</returns>
        public static PageDetail GetPageDetail(Page page)
        {
            Page parent = null;
            SelectOption parentOption = null;

            if (page.Parent != Guid.Empty)
            {
                parent = Page.Pages.FirstOrDefault(p => p.Id.Equals(page.Parent));
                parentOption = new SelectOption { IsSelected = false, OptionName = parent.Title, OptionValue = parent.Id.ToString() };
            }
            return new PageDetail
            {
                Id = page.Id,
                ShowInList = page.ShowInList,
                Title = page.Title,
                Slug = page.Slug,
                RelativeLink = page.RelativeLink,
                Content = page.Content,
                Parent = parentOption,
                Description = page.Description,
                Keywords = page.Keywords,
                DateCreated = page.DateCreated.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                HasChildren = page.HasChildPages,
                IsPublished = page.IsPublished,
                IsFrontPage = page.IsFrontPage,
                IsDeleted = page.IsDeleted,
                SortOrder = page.SortOrder,
            };
        }
        /// <summary>
        /// Get json comment
        /// </summary>
        /// <param name="c">Comment</param>
        /// <param name="postComments">List of comments</param>
        /// <returns>Json comment</returns>
        public static CommentItem GetComment(Comment c, List<Comment> postComments)
        {
            var jc = new CommentItem();
            jc.Id = c.Id;
            jc.IsApproved = c.IsApproved;
            jc.IsSpam = c.IsSpam;
            jc.IsPending = !c.IsApproved && !c.IsSpam;
            jc.Email = c.Email == "trackback" ? "pingback" : c.Email;
            jc.Author = c.Author;
            jc.Title = c.Teaser.Length < 80 ? c.Teaser : c.Teaser.Substring(0, 80) + "...";
            jc.DateCreated = c.DateCreated.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            jc.RelativeLink = c.RelativeLink;
            jc.HasChildren = postComments.FirstOrDefault(pc => pc.ParentId == c.Id) != null;
            jc.Avatar = Gravatar(c);
            return jc;
        }
        /// <summary>
        ///     Get comment detail
        /// </summary>
        /// <param name="c">Comment</param>
        /// <returns>Json comment detail</returns>
        public static CommentDetail GetCommentDetail(Comment c)
        {
            var jc = new CommentDetail();
            jc.Id = c.Id;
            jc.ParentId = c.ParentId;
            jc.PostId = c.Parent.Id;
            jc.Title = c.Teaser.Length < 80 ? c.Teaser : c.Teaser.Substring(0, 80) + "...";
            jc.Content = c.Content;
            jc.Website = c.Website == null ? "" : c.Website.ToString();
            jc.Ip = c.IP;
            return jc;
        }

        /// <summary>
        /// Convert json comment back to BE comment
        /// </summary>
        /// <param name="c">Json comment</param>
        /// <returns>Comment</returns>
        public static Comment SetComment(CommentItem c)
        {
            Comment item = (from p in Post.Posts
                            from cmn in p.AllComments
                            where cmn.Id == c.Id
                            select cmn).FirstOrDefault();

            if (c.IsPending)
            {
                item.IsApproved = false;
                item.IsSpam = false;
            }
            if (c.IsApproved)
            {
                item.IsApproved = true;
                item.IsSpam = false;
            }
            if (c.IsSpam)
            {
                item.IsApproved = false;
                item.IsSpam = true;
            }

            item.Email = c.Email;
            item.Author = c.Author;
            return item;
        }

        #region Private methods

        static List<CategoryItem> GetCategories(ICollection<Category> categories)
        {
            if (categories == null || categories.Count == 0)
                return null;

            //var html = categories.Aggregate("", (current, cat) => current + string.Format
            //("<a href='#' onclick=\"ChangePostFilter('Category','{0}','{1}')\">{1}</a>, ", cat.Id, cat.Title));
            var categoryList = new List<CategoryItem>();
            foreach (var coreCategory in categories)
            {
                var item = new CategoryItem();
                item.Id = coreCategory.Id;
                item.Title = coreCategory.Title;
                item.Description = coreCategory.Description;
                item.Parent = ItemParent(coreCategory.Parent);
                categoryList.Add(item);
            }
            return categoryList;
        }

        static SelectOption ItemParent(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;

            var item = Category.Categories.FirstOrDefault(c => c.Id == id);
            return new SelectOption { OptionName = item.Title, OptionValue = item.Id.ToString() };
        }

        static List<TagItem> GetTags(ICollection<string> tags)
        {
            if (tags == null || tags.Count == 0)
                return null;

            var items = new List<TagItem>();
            foreach (var item in tags)
            {
                items.Add(new TagItem { TagName = item });
            }
            return items;
        }

        static string[] GetComments(Post post)
        {
            if (post.Comments == null || post.Comments.Count == 0)
                return null;

            string[] comments = new string[3];
            comments[0] = post.NotApprovedComments.Count.ToString();
            comments[1] = post.ApprovedComments.Count.ToString();
            comments[2] = post.SpamComments.Count.ToString();
            return comments;
        }

        static string Gravatar(Comment comment)
        {
            var website = comment.Website == null ? "" : comment.Website.ToString();
            return Avatar.GetSrc(comment.Email, website);
        }

        #endregion
    }
}
