using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Contracts;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Trash repository
    /// </summary>
    public class TrashRepository : ITrashRepository
    {
        /// <summary>
        /// Get trash list
        /// </summary>
        /// <param name="trashType">Type (post, page, comment)</param>
        /// <param name="take">Take for a page</param>
        /// <param name="skip">Items to sckip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns></returns>
        public TrashVM GetTrash(TrashType trashType, int take = 10, int skip = 0, string filter = "1 == 1", string order = "DateCreated descending")
        {
            if (!Security.IsAuthorizedTo(Rights.ViewDashboard))
                throw new UnauthorizedAccessException();

            var trash = new TrashVM();
            var comments = new List<Comment>();
            var posts = new List<Post>();
            var pages = new List<Page>();
            var trashList = new List<TrashItem>();
            var trashPage = new List<TrashItem>();
            var query = trashList.AsQueryable().Where(filter);

            // comments
            if (trashType == TrashType.All || trashType == TrashType.Comment)
            {

                foreach (var p in Post.Posts)
                {
                    if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                        if (p.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                            continue;

                    comments.AddRange(p.DeletedComments);
                }
            }

            if (comments.Count > 0)
            {
                foreach (var c in comments)
                {
                    TrashItem t1 = new TrashItem
                    {
                        Id = c.Id,
                        Title = c.Author + ": " + c.Teaser,
                        RelativeUrl = c.RelativeLink,
                        ObjectType = "Comment",
                        DateCreated = c.DateCreated.ToString("MM/dd/yyyy HH:mm")
                    };

                    trashList.Add(t1);
                }
            }

            // posts
            if (trashType == TrashType.All || trashType == TrashType.Post)
            {
                posts = (from x in Post.DeletedPosts orderby x.DateCreated descending select x).ToList();
            }

            if (posts.Count > 0)
            {
                foreach (var p in posts)
                {
                    if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                        if (p.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                            continue;

                    TrashItem t2 = new TrashItem
                    {
                        Id = p.Id,
                        Title = System.Web.HttpContext.Current.Server.HtmlEncode(p.Title),
                        RelativeUrl = p.RelativeLink,
                        ObjectType = "Post",
                        DateCreated = p.DateCreated.ToString("MM/dd/yyyy HH:mm")
                    };

                    trashList.Add(t2);
                }
            }

            // pages
            if (trashType == TrashType.All || trashType == TrashType.Page)
            {
                pages = (from x in Page.DeletedPages orderby x.DateCreated descending select x).ToList();
            }

            if (pages.Count > 0)
            {
                foreach (var p in pages)
                {
                    TrashItem t3 = new TrashItem
                    {
                        Id = p.Id,
                        Title = System.Web.HttpContext.Current.Server.HtmlEncode(p.Title),
                        RelativeUrl = p.RelativeLink,
                        ObjectType = "Page",
                        DateCreated = p.DateCreated.ToString("MM/dd/yyyy HH:mm")
                    };

                    trashList.Add(t3);
                }
            }

            trash.TotalCount = trashList.Count;

            // if take passed in as 0, return all
            if (take == 0) take = trashList.Count;

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                trashPage.Add(item);
            
            trash.Items = trashPage;

            return trash;
        }

        /// <summary>
        /// Restore
        /// </summary>
        /// <param name="trashType">Trash type</param>
        /// <param name="id">Id</param>
        public bool Restore(string trashType, Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewDashboard))
                throw new UnauthorizedAccessException();

            switch (trashType)
            {
                case "Comment":
                    foreach (var p in Post.Posts.ToArray())
                    {
                        var cmnt = p.DeletedComments.FirstOrDefault(c => c.Id == id);
                        if (cmnt != null)
                        {
                            p.RestoreComment(cmnt);
                            break;
                        }
                    }
                    break;
                case "Post":
                    var delPost = Post.DeletedPosts.Where(p => p.Id == id).FirstOrDefault();
                    if (delPost != null) delPost.Restore();
                    break;
                case "Page":
                    var delPage = Page.DeletedPages.Where(pg => pg.Id == id).FirstOrDefault();
                    if (delPage != null) delPage.Restore();
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// Purge
        /// </summary>
        /// <param name="trashType">Trash type</param>
        /// <param name="id">Id</param>
        public bool Purge(string trashType, Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewDashboard))
                throw new UnauthorizedAccessException();

            switch (trashType)
            {
                case "Comment":
                    foreach (var p in Post.Posts.ToArray())
                    {
                        var cmnt = p.DeletedComments.FirstOrDefault(c => c.Id == id);
                        if (cmnt != null)
                        {
                            p.PurgeComment(cmnt);
                            break;
                        }
                    }
                    break;
                case "Post":
                    var delPost = Post.DeletedPosts.Where(p => p.Id == id).FirstOrDefault();
                    if (delPost != null) delPost.Purge();
                    break;
                case "Page":
                    var delPage = Page.DeletedPages.Where(pg => pg.Id == id).FirstOrDefault();
                    if (delPage != null) delPage.Purge();
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// Purge all
        /// </summary>
        public bool PurgeAll()
        {
            if (!Security.IsAuthorizedTo(Rights.ViewDashboard))
                throw new UnauthorizedAccessException();

            // remove deleted comments
            foreach (var p in Post.Posts.ToArray())
            {
                if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                    if (p.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                        continue;

                foreach (var c in p.DeletedComments.ToArray())
                {
                    p.PurgeComment(c);
                }
            }

            // remove deleted posts
            foreach (var p in Post.DeletedPosts.ToArray())
            {
                if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                    if (p.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                        continue;

                p.Purge();
            }

            // remove deleted pages
            foreach (var pg in Page.DeletedPages.ToArray())
            {
                pg.Purge();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResponse PurgeLogfile()
        {
            if (!Security.IsAuthorizedTo(Rights.ViewDashboard))
                throw new UnauthorizedAccessException();

            string fileLocation = System.Web.Hosting.HostingEnvironment.MapPath(System.IO.Path.Combine(BlogConfig.StorageLocation, "logger.txt"));
            if (System.IO.File.Exists(fileLocation))
            {
                System.IO.StreamWriter sw = System.IO.File.CreateText(fileLocation);

                sw.WriteLine("Purged at " + DateTime.Now);
                sw.Close();
                return new JsonResponse { Success = true, Message = "Log file purged" };
            }
            return new JsonResponse { Success = false, Message = "Log file not found" };
        }
    }
}
