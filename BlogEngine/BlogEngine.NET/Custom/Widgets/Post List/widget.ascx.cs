using App_Code.Controls;
using BlogEngine.Core;
using Resources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Widgets.PostList
{
    public partial class Widget : WidgetBase
    {
        private const int DefaultNumberOfPosts = 10;

        static Widget()
        {
            Post.Saved += ClearCache;
            Post.CommentAdded += ClearCache;
            Post.CommentRemoved += ClearCache;
            BlogSettings.Changed += ClearCache;
        }

        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        public override string Name
        {
            get
            {
                return "Post List";
            }
        }

        public override void LoadWidget()
        {
            var settings = this.GetSettings();
            var id = "widget_postlist_" + this.ID;

            var numberOfPosts = DefaultNumberOfPosts;
            if (settings.ContainsKey("numberofposts"))
            {
                numberOfPosts = int.Parse(settings["numberofposts"]);
            }

            if (Blog.CurrentInstance.Cache[id] == null)
            {   
                var visiblePosts = Post.ApplicablePosts.FindAll(p => p.IsVisibleToPublic);

                // filtering
                var category = settings != null ? settings["cutegory"] : "All";
                if (category != "All")
                {
                    var x = Category.Categories.Where(c => c.Title == "Blog").FirstOrDefault();

                    var cat = Category.Categories.Where(c => c.Title == category).FirstOrDefault();
                    visiblePosts = visiblePosts.Where(p => p.Categories.Contains(cat)).ToList();
                }
                if (settings != null && settings["author"] != "All")
                {
                    visiblePosts = visiblePosts.Where(p => p.Author == settings["author"]).ToList();
                }

                // sorting
                var sortOrder = settings != null ? settings["sortorder"] : "Published";

                if (sortOrder == "Published")
                    visiblePosts = visiblePosts.OrderByDescending(p => p.DateCreated).ToList();

                if (sortOrder == "Alphabetical")
                    visiblePosts = visiblePosts.OrderBy(p => p.Title).ToList();

                if (sortOrder == "Comments")
                    visiblePosts = visiblePosts.OrderByDescending(p => p.Comments.Count).ToList();

                // take
                var max = Math.Min(visiblePosts.Count, numberOfPosts);
                var list = visiblePosts.GetRange(0, max);
                
                Blog.CurrentInstance.Cache.Insert(id, list);
            }

            var content = RenderPosts((List<Post>)Blog.CurrentInstance.Cache[id], settings);
            var html = new LiteralControl(content);    
            phPosts.Controls.Add(html);
        }

        static void ClearCache(object sender, EventArgs e)
        {
            Blog siteAggregationBlog = Blog.SiteAggregationBlog;

            foreach (var item in Blog.CurrentInstance.Cache)
            {
                var i = (System.Collections.DictionaryEntry)item;
                if (i.Key.ToString().Contains("widget_postlist_"))
                {
                    var theKey = i.Key.ToString().Replace(Blog.CurrentInstance.Id.ToString() + "_", "");
                    Blog.CurrentInstance.Cache.Remove(theKey);
                }
            }
            if (siteAggregationBlog != null)
            {
                foreach (var item in siteAggregationBlog.Cache)
                {
                    var i = (System.Collections.DictionaryEntry)item;
                    if (i.Key.ToString().Contains("widget_postlist_"))
                    {
                        var theKey = i.Key.ToString().Replace(siteAggregationBlog.Id.ToString() + "_", "");
                        Blog.CurrentInstance.Cache.Remove(theKey);
                    }
                }
            }
        }

        private static string RenderPosts(List<Post> posts, StringDictionary settings)
        {
            if (posts.Count == 0)
                return string.Format("<p>{0}</p>", labels.none);

            var showImg = false;
            if (settings.ContainsKey("showimg"))
                bool.TryParse(settings["showimg"], out showImg);

            var showDesc = false;
            if (settings.ContainsKey("showdesc"))
                bool.TryParse(settings["showdesc"], out showDesc);

            var cat = "";
            if (settings.ContainsKey("cutegory"))
                cat = settings["cutegory"];

            var sb = new StringBuilder();
            sb.Append("<ul>");

            foreach (var post in posts)
            {
                if (!post.IsVisibleToPublic)
                    continue;

                const string LinkFormat = "<li class=\"post-item-list\"><a href=\"{0}\">{1}</a></li>";
                var linkBody = "";

                if (showImg)
                    linkBody += GetImg(post.Content, post.Title);

                linkBody += "<h3 class=\"post-item-title\">" + post.Title + "</h3>";

                if(showDesc)
                    linkBody += "<p class=\"post-item-desc\">" + HttpUtility.HtmlEncode(post.Description) + "</p>";

                linkBody += "<span class=\"post-item-date\">" + post.DateCreated.ToString("d. MMMM yyyy") + "</span>";

                if(cat != "" && cat != "All")
                    linkBody += "<span class=\"post-item-cat\">" + cat + "</span>";

                sb.AppendFormat(LinkFormat, post.RelativeOrAbsoluteLink, linkBody);
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string GetImg(string post, string title)
        {
            var img = "";
            int idx = post.IndexOf("<img src=");
            if (idx > 0)
            {
                try
                {
                    var idxEnd = post.IndexOf(">", idx);
                    if (idxEnd > idx)
                    {
                        var len = idxEnd - idx;
                        img = post.Substring(idx, len) + " />";

                        // add class and alt
                        if (!img.Contains("class="))
                        {
                            img = img.Replace("img src=", "img class=\"post-item-img\" src=");
                        }
                        if (!img.Contains("alt="))
                        {
                            img = img.Replace(" />", "alt=\"" + title + "\" />");
                        }
                    }
                }
                catch (Exception)
                {
                    img = "";
                }
            }
            return img;
        }

    }
}