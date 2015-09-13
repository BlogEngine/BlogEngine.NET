using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Notes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Hosting;

namespace BlogEngine.Core.Data.ViewModels
{
    public class DashboardVM
    {
        private List<Post> _posts;
        private List<Page> _pages;
        private List<Comment> _comments;
        private List<TrashItem> _trash;

        public DashboardVM()
        {
            _posts = new List<Post>();
            _pages = new List<Page>();
            _comments = new List<Comment>();
            _trash = new List<TrashItem>();

            LoadProperties();
        }

        #region Properties

        public List<Post> DraftPosts { get; set; }
        public int PostPublishedCnt { get; set; }
        public int PostDraftCnt { get; set; }

        public List<Comment> Comments
        {
            get
            {
                return _comments.AsQueryable().OrderBy("DateCreated desc").Take(5).ToList();
            }
        }
        public int ApprovedCommentsCnt { get; set; }
        public int PendingCommentsCnt { get; set; }
        public int SpamCommentsCnt { get; set; }

        public List<Page> DraftPages { get; set; }
        public int PagePublishedCnt { get; set; }
        public int PageDraftCnt { get; set; }

        public List<TrashItem> Trash { get; set; }

        public List<QuickNote> Notes
        {
            get
            {
                return new QuickNotes(Security.CurrentUser.Identity.Name).Notes;
            }
        }

        public List<SelectOption> Logs
        {
            get
            {
                return GetLogFile().ToList();
            }
        }

        #endregion

        private void LoadProperties()
        {
            LoadPosts();
            LoadPages();
            LoadTrash();
        }

        private void LoadPosts()
        {
            var posts = Post.ApplicablePosts.Where(p => p.IsVisible);
            DraftPosts = posts.Where(p => p.IsPublished == false).ToList();
            PostDraftCnt = DraftPosts.Count;
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
            DraftPages = pages.Where(p => p.IsPublished == false).ToList();
            PageDraftCnt = DraftPages.Count;
            PagePublishedCnt = pages.Where(p => p.IsPublished).ToList().Count;
        }

        private void LoadTrash()
        {
            var posts = Post.ApplicablePosts.Where(p => p.IsDeleted);
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
            var pages = Page.Pages.Where(p => p.IsDeleted);
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

            var comms = _comments.Where(c => c.IsDeleted);
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
        }

        IEnumerable<SelectOption> GetLogFile()
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
    }
}
