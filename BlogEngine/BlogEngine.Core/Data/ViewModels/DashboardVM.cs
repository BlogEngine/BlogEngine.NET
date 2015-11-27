using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Services;
using BlogEngine.Core.Notes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Hosting;

namespace BlogEngine.Core.Data.ViewModels
{
    /// <summary>
    /// Dashboard view model
    /// </summary>
    public class DashboardVM
    {
        /// <summary>
        /// Dashboard vm
        /// </summary>
        public DashboardVM()
        {
            _posts = new List<PostItem>();
            _pages = new List<PageItem>();
            _comments = new List<Comment>();
            _trash = new List<TrashItem>();

            LoadProperties();
        }

        #region Properties

        private List<PostItem> _posts;
        private List<PageItem> _pages;
        private List<Comment> _comments;
        private List<TrashItem> _trash;

        /// <summary>
        /// Draft posts
        /// </summary>
        public List<PostItem> DraftPosts { get; set; }
        /// <summary>
        /// Post published counter
        /// </summary>
        public int PostPublishedCnt { get; set; }
        /// <summary>
        /// Post drafts counter
        /// </summary>
        public int PostDraftCnt { get; set; }
        /// <summary>
        /// Latest comments
        /// </summary>
        public List<CommentItem> Comments
        {
            get
            {
                var comments = new List<CommentItem>();
                foreach(var c in _comments.AsQueryable().OrderBy("DateCreated desc").Take(5).ToList())
                {
                    comments.Add(Json.GetComment(c, _comments));
                }
                return comments;
            }
        }
        /// <summary>
        /// Approved comments counter
        /// </summary>
        public int ApprovedCommentsCnt { get; set; }
        /// <summary>
        /// Pending comments counter
        /// </summary>
        public int PendingCommentsCnt { get; set; }
        /// <summary>
        /// Spam comments counter
        /// </summary>
        public int SpamCommentsCnt { get; set; }
        /// <summary>
        /// Draft pages
        /// </summary>
        public List<PageItem> DraftPages { get; set; }
        /// <summary>
        /// Published pages counter
        /// </summary>
        public int PagePublishedCnt { get; set; }
        /// <summary>
        /// Draft pages counter
        /// </summary>
        public int PageDraftCnt { get; set; }
        /// <summary>
        /// Trash items counter
        /// </summary>
        public List<TrashItem> Trash { get; set; }
        /// <summary>
        /// Quick notes counter
        /// </summary>
        public List<QuickNote> Notes
        {
            get
            {
                return new QuickNotes(Security.CurrentUser.Identity.Name).Notes;
            }
        }
        /// <summary>
        /// Log items counter
        /// </summary>
        public string Logs
        {
            get
            {
                return GetLogFile();
            }
        }

        #endregion

        #region Private methods

        private void LoadProperties()
        {
            LoadPosts();
            LoadPages();
            LoadTrash();
        }

        private void LoadPosts()
        {
            var posts = Post.ApplicablePosts.Where(p => p.IsVisible);
            DraftPosts = new List<PostItem>();
            foreach (var p in posts.Where(p => p.IsPublished == false && p.IsDeleted == false).ToList())
            {
                DraftPosts.Add(Json.GetPost(p));
            }
            PostDraftCnt = DraftPosts == null ? 0 : DraftPosts.Count;
            PostPublishedCnt = posts.Where(p => p.IsPublished).ToList().Count;

            foreach (var p in posts)
            {
                ApprovedCommentsCnt += p.Comments.Where(c => c.IsPublished && !c.IsDeleted).Count();
                PendingCommentsCnt += p.Comments.Where(c => !c.IsPublished && !c.IsSpam && !c.IsDeleted).Count();
                SpamCommentsCnt += p.Comments.Where(c => !c.IsPublished && c.IsSpam && !c.IsDeleted).Count();
                _comments.AddRange(p.Comments);
            }
        }

        private void LoadPages()
        {
            var pages = Page.Pages.Where(p => p.IsVisible);
            DraftPages = new List<PageItem>();
            foreach (var p in pages.Where(p => p.IsPublished == false && p.IsDeleted == false).ToList())
            {
                DraftPages.Add(Json.GetPage(p));
            }
            PageDraftCnt = DraftPages == null ? 0 : DraftPages.Count;
            PagePublishedCnt = pages.Where(p => p.IsPublished).ToList().Count;
        }

        private void LoadTrash()
        {
            var posts = Post.DeletedPosts;
            _trash = new List<TrashItem>();
            if (posts.Count() > 0)
            {
                foreach (var p in posts)
                {
                    _trash.Add(new TrashItem
                        {
                            Id = p.Id,
                            Title = System.Web.HttpContext.Current.Server.HtmlEncode(p.Title),
                            RelativeUrl = p.RelativeLink,
                            ObjectType = "Post",
                            DateCreated = p.DateCreated.ToString("MM/dd/yyyy HH:mm")
                        }
                    );
                }
            }
            var pages = Page.DeletedPages;
            if (pages.Count() > 0)
            {
                foreach (var page in pages)
                {
                    _trash.Add(new TrashItem
                        {
                            Id = page.Id,
                            Title = System.Web.HttpContext.Current.Server.HtmlEncode(page.Title),
                            RelativeUrl = page.RelativeLink,
                            ObjectType = "Page",
                            DateCreated = page.DateCreated.ToString("MM/dd/yyyy HH:mm")
                        }
                    );
                }
            }

            var comms = new List<Comment>();
            foreach (var p in Post.Posts)
            {
                if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                    if (p.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                        continue;

                comms.AddRange(p.DeletedComments);
            }
            if (comms.Count() > 0)
            {
                foreach (var c in comms)
                {
                    _trash.Add(new TrashItem
                        {
                            Id = c.Id,
                            Title = c.Author + ": " + c.Teaser,
                            RelativeUrl = c.RelativeLink,
                            ObjectType = "Comment",
                            DateCreated = c.DateCreated.ToString("MM/dd/yyyy HH:mm")
                        }
                    );
                }
            }
            Trash = _trash;
        }

        IEnumerable<SelectOption> GetLogs()
        {
            string fileLocation = HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "logger.txt"));
            var items = new List<SelectOption>();

            if (File.Exists(fileLocation))
            {
                using (var sw = new StreamReader(fileLocation))
                {
                    string line;
                    string logItem = "";
                    int count = 1;
                    while ((line = sw.ReadLine()) != null)
                    {
                        if (line.Contains("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*"))
                        {
                            // new log item
                            if (!string.IsNullOrEmpty(logItem))
                            {
                                var item = new SelectOption();
                                item.OptionName = "Line" + count.ToString();
                                item.OptionValue = logItem;
                                items.Add(item);
                                logItem = "";
                                count++;
                            }
                        }
                        else
                        {
                            // append line to log item
                            logItem = logItem + line + "<br/>";
                        }
                    }
                    sw.Close();
                    return items;
                }
            }
            else
            {
                return new List<SelectOption>();
            }
        }

        string GetLogFile()
        {
            string fileLocation = HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "logger.txt"));
            var items = new List<SelectOption>();

            if (File.Exists(fileLocation))
            {
                using (var sw = new StreamReader(fileLocation))
                {
                    return sw.ReadToEnd();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
