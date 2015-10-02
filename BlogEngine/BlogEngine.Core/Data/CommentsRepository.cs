using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Services;
using BlogEngine.Core.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Security;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Comments repository
    /// </summary>
    public class CommentsRepository : ICommentsRepository
    {
        /// <summary>
        /// Comments list
        /// </summary>
        /// <param name="commentType">Comment type</param>
        /// <param name="take">Items to take</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns>List of comments</returns>
        public CommentsVM Get()
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicComments))
                throw new UnauthorizedAccessException();

            var vm = new CommentsVM();
            var comments = new List<Comment>();
            var items = new List<CommentItem>();

            var all = Security.IsAuthorizedTo(Rights.EditOtherUsersPosts);
            foreach (var p in Post.Posts)
            {
                if (all || p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower())
                {
                    comments.AddRange(p.Comments);
                }
            }  
            foreach (var c in comments)
            {
                items.Add(Json.GetComment(c, comments));               
            }
            vm.Items = items;

            vm.Detail = new CommentDetail();
            vm.SelectedItem = new CommentItem();

            return vm;
        }
        /// <summary>
        /// Find by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommentDetail FindById(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicComments))
                throw new UnauthorizedAccessException();

            return (from p in Post.Posts
                    from c in p.AllComments
                    where c.Id == id
                    select Json.GetCommentDetail(c)).FirstOrDefault();
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item">Comment</param>
        /// <returns>Comment object</returns>
        public CommentItem Add(CommentDetail item)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateComments))
                throw new UnauthorizedAccessException();

            var c = new Comment();
            try
            {
                var post = Post.Posts.Where(p => p.Id == item.PostId).FirstOrDefault();

                c.Id = Guid.NewGuid();
                c.ParentId = item.ParentId;
                c.IsApproved = true;
                c.Content = HttpUtility.HtmlAttributeEncode(item.Content);

                c.Author = Security.CurrentUser.Identity.Name;
                var profile = AuthorProfile.GetProfile(c.Author);
                if (profile != null && !string.IsNullOrEmpty(profile.DisplayName))
                {
                    c.Author = profile.DisplayName;
                }

                c.Email = Membership.Provider.GetUser(Security.CurrentUser.Identity.Name, true).Email;
                c.IP = Utils.GetClientIP();
                c.DateCreated = DateTime.Now;
                c.Parent = post;

                post.AddComment(c);
                post.Save();

                var newComm = post.Comments.Where(cm => cm.Content == c.Content).FirstOrDefault();
                return Json.GetComment(newComm, post.Comments);
            }
            catch (Exception ex)
            {
                Utils.Log("Core.Data.CommentsRepository.Add", ex);
                return null;
            }
        }

        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <param name="action">Action</param>
        /// <returns>True on success</returns>
        public bool Update(CommentItem item, string action)
        {
            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
                throw new UnauthorizedAccessException();

            foreach (var p in Post.Posts.ToArray())
            {
                foreach (var c in p.Comments.Where(c => c.Id == item.Id).ToArray())
                {
                    if (action == "approve")
                    {
                        c.IsApproved = true;
                        c.IsSpam = false;
                        p.DateModified = DateTime.Now;
                        p.Save();
                        return true;
                    }

                    if (action == "unapprove")
                    {
                        c.IsApproved = false;
                        c.IsSpam = true;
                        p.DateModified = DateTime.Now;
                        p.Save();
                        return true;
                    }

                    //c.Content = item.Content;
                    c.Author = item.Author;
                    c.Email = item.Email;
                    //c.Website = string.IsNullOrEmpty(item.Website) ? null : new Uri(item.Website);

                    if (item.IsPending)
                    {
                        c.IsApproved = false;
                        c.IsSpam = false;
                    }
                    if (item.IsApproved)
                    {
                        c.IsApproved = true;
                        c.IsSpam = false;
                    }
                    if (item.IsSpam)
                    {
                        c.IsApproved = false;
                        c.IsSpam = true;
                    }
                    // need to mark post as "dirty"
                    p.DateModified = DateTime.Now;
                    p.Save();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
                throw new UnauthorizedAccessException();

            foreach (var p in Post.Posts.ToArray())
            {
                Comment item = (from cmn in p.AllComments
                    where cmn.Id == id select cmn).FirstOrDefault();

                if (item != null)
                {
                    p.RemoveComment(item);
                    p.DateModified = DateTime.Now;
                    p.Save();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete all comments
        /// </summary>
        /// <param name="commentType">Pending or spam</param>
        /// <returns>True on success</returns>
        public bool DeleteAll(string commentType)
        {
            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
                throw new System.UnauthorizedAccessException();

            if (commentType == "pending")
                DeletePending();

            if (commentType == "spam")
                DeleteSpam();

            return true;
        }

        #region Private methods

        // delete all pending comments
        private void DeletePending()
        {
            var posts = Post.ApplicablePosts.Where(p => !p.IsDeleted && p.IsPublished);
            foreach (var p in posts.ToArray())
            {
                foreach (var c in p.NotApprovedComments.Where(c => !c.IsSpam && !c.IsDeleted))
                {
                    p.RemoveComment(c, false);
                }

                p.DateModified = DateTime.Now;
                p.Save();
            }
        }

        // delete all spam comments
        private void DeleteSpam()
        {
            var posts = Post.ApplicablePosts.Where(p => !p.IsDeleted && p.IsPublished);
            foreach (var p in posts.ToArray())
            {
                foreach (var c in p.SpamComments.Where(c => !c.IsDeleted))
                {
                    p.RemoveComment(c, false);
                }

                p.DateModified = DateTime.Now;
                p.Save();
            }
        }

        #endregion
    }
}