using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace BlogEngine.Core.Web
{
    /// <summary>
    /// URL rewrite rules
    /// </summary>
    public class UrlRules
    {
        #region Constants and Fields

        /// <summary>
        /// The Year Regex.
        /// </summary>
        private static readonly Regex YearRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The Year Month Regex.
        /// </summary>
        private static readonly Regex YearMonthRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/([0-1][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The Year Month Day Regex.
        /// </summary>
        private static readonly Regex YearMonthDayRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/([0-1][0-9])/([0-3][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Rules

        /// <summary>
        /// Rewrites the post.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewritePost(HttpContext context, string url)
        {
            int year, month, day;

            var haveDate = ExtractDate(context, out year, out month, out day);
            var slug = ExtractTitle(context, url);

            // Allow for Year/Month only dates in URL (in this case, day == 0), as well as Year/Month/Day dates.
            // first make sure the Year and Month match.
            // if a day is also available, make sure the Day matches.
            var post = Post.ApplicablePosts.Find(
                p =>
                (!haveDate || (p.DateCreated.Year == year && p.DateCreated.Month == month)) &&
                ((!haveDate || (day == 0 || p.DateCreated.Day == day)) &&
                 slug.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase)));

            if (post == null)
            {
                return;
            }

            var q = GetQueryString(context);
            if (q.Contains("id=" + post.Id, StringComparison.OrdinalIgnoreCase))
                q = string.Format("{0}post.aspx?{1}", Utils.ApplicationRelativeWebRoot, q);
            else
                q = string.Format("{0}post.aspx?id={1}{2}", Utils.ApplicationRelativeWebRoot, post.Id, q);

            context.RewritePath(
                url.Contains("/FEED/")
                    ? string.Format("syndication.axd?post={0}{1}", post.Id, GetQueryString(context))
                    : q,
                false);
        }

        /// <summary>
        /// Rewrites the page.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewritePage(HttpContext context, string url)
        {
            var slug = ExtractTitle(context, url);
            var page =
                Page.Pages.Find(
                    p => slug.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase));

            if (page != null)
            {
                context.RewritePath(string.Format("{0}page.aspx?id={1}{2}", Utils.ApplicationRelativeWebRoot, page.Id, GetQueryString(context)), false);
            }
        }

        /// <summary>
        /// Rewrites the contact page.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteContact(HttpContext context, string url)
        {
            RewritePhysicalPageGeneric(context, url, "contact.aspx");
        }

        /// <summary>
        /// Rewrites the archive page.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteArchive(HttpContext context, string url)
        {
            RewritePhysicalPageGeneric(context, url, "archive.aspx");
        }

        /// <summary>
        /// Rewrites the search page.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteSearch(HttpContext context, string url)
        {
            RewritePhysicalPageGeneric(context, url, "search.aspx");
        }

        /// <summary>
        /// Generic routing to rewrite to a physical page, e.g. contact.aspx, archive.aspx, when RemoveExtensionsFromUrls is turned on.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        /// <param name="relativePath">The relative path to the page including the physical page name, e.g. archive.aspx, folder/somepage.aspx</param>
        private static void RewritePhysicalPageGeneric(HttpContext context, string url, string relativePath)
        {
            string query = GetQueryString(context);
            if (query.Length > 0 && query.StartsWith("&"))
            {
                query = "?" + query.Substring(1);
            }
            context.RewritePath(string.Format("{0}{1}{2}", Utils.ApplicationRelativeWebRoot, relativePath, query), false);
        }

        /// <summary>
        /// Rewrites the category.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteCategory(HttpContext context, string url)
        {
            var title = ExtractTitle(context, url);
            foreach (var cat in from cat in Category.ApplicableCategories
                                let legalTitle = Utils.RemoveIllegalCharacters(cat.Title).ToLowerInvariant()
                                where title.Equals(legalTitle, StringComparison.OrdinalIgnoreCase)
                                select cat)
            {
                if (url.Contains("/FEED/"))
                {
                    context.RewritePath(string.Format("syndication.axd?category={0}{1}", cat.Id, GetQueryString(context)), false);
                }
                else
                {
                    context.RewritePath(
                        string.Format("{0}default.aspx?id={1}{2}", Utils.ApplicationRelativeWebRoot, cat.Id, GetQueryString(context)), false);
                    break;
                }
            }
        }

        /// <summary>
        /// Rewrites the tag.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteTag(HttpContext context, string url)
        {
            var tag = ExtractTitle(context, url);

            if (url.Contains("/FEED/"))
            {
                tag = string.Format("syndication.axd?tag={0}{1}", tag, GetQueryString(context));
            }
            else
            {
                tag = string.Format("{0}default.aspx?tag=/{1}{2}", Utils.ApplicationRelativeWebRoot, tag, GetQueryString(context));
            }
            context.RewritePath(tag, false);
        }

        /// <summary>
        /// Page with large calendar
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteCalendar(HttpContext context, string url)
        {
            context.RewritePath(string.Format("{0}default.aspx?calendar=show", Utils.ApplicationRelativeWebRoot), false);
        }

        /// <summary>
        /// Posts for author
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteAuthor(HttpContext context, string url)
        {
            var author = UrlRules.ExtractTitle(context, url);

            var path = string.Format("{0}default.aspx?name={1}{2}",
                Utils.ApplicationRelativeWebRoot,
                author,
                GetQueryString(context));

            context.RewritePath(path, false);
        }

        /// <summary>
        /// Rewrites /blog.aspx path
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        public static void RewriteBlog(HttpContext context, string url)
        {
            var path = string.Format("{0}default.aspx?blog=true{1}",
                Utils.ApplicationRelativeWebRoot,
                GetQueryString(context));

            context.RewritePath(path, false);
        }

        /// <summary>
        /// Rewrites the incoming file request to the actual handler
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="url">the url string</param>
        public static void RewriteFilePath(HttpContext context, string url)
        {
            var wr = url.Substring(0, url.IndexOf("/FILES/") + 6);
            url = url.Replace(wr, "");
            url = url.Substring(0, url.LastIndexOf(System.IO.Path.GetExtension(url)));
            var npath = string.Format("{0}file.axd?file={1}", Utils.ApplicationRelativeWebRoot, url);
            context.RewritePath(npath);
        }

        /// <summary>
        /// Rewrites the incoming image request to the actual handler
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="url">the url string</param>
        public static void RewriteImagePath(HttpContext context, string url)
        {
            var wr = url.Substring(0, url.IndexOf("/IMAGES/") + 7);
            url = url.Replace(wr, "");
            url = url.Substring(0, url.LastIndexOf(System.IO.Path.GetExtension(url)));
            var npath = string.Format("{0}image.axd?picture={1}", Utils.ApplicationRelativeWebRoot, url);
            context.RewritePath(npath);
        }

        /// <summary>
        /// The rewrite default.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public static void RewriteDefault(HttpContext context)
        {
            var url = GetUrlWithQueryString(context);
            var page = string.Format("&page={0}", context.Request.QueryString["page"]);

            if (string.IsNullOrEmpty(context.Request.QueryString["page"]))
            {
                page = null;
            }

            if (YearMonthDayRegex.IsMatch(url))
            {
                var match = YearMonthDayRegex.Match(url);
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var day = match.Groups[3].Value;
                var date = string.Format("{0}-{1}-{2}", year, month, day);
                url = string.Format("{0}default.aspx?date={1}{2}", Utils.ApplicationRelativeWebRoot, date, page);
            }
            else if (YearMonthRegex.IsMatch(url))
            {
                var match = YearMonthRegex.Match(url);
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var path = string.Format("default.aspx?year={0}&month={1}", year, month);
                url = Utils.ApplicationRelativeWebRoot + path + page;
            }
            else if (YearRegex.IsMatch(url))
            {
                var match = YearRegex.Match(url);
                var year = match.Groups[1].Value;
                var path = string.Format("default.aspx?year={0}", year);
                url = Utils.ApplicationRelativeWebRoot + path + page;
            }
            else
            {
                string newUrl = url.Replace("Default.aspx", "default.aspx");  // fixes a casing oddity on Mono
                int defaultStart = url.IndexOf("default.aspx", StringComparison.OrdinalIgnoreCase);
                url = Utils.ApplicationRelativeWebRoot + url.Substring(defaultStart);
            }

            //if (string.IsNullOrEmpty(BlogConfig.FileExtension) && url.Contains("page="))
            //    url = url.Replace("default.aspx?", "");

            context.RewritePath(url, false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if default.aspx was requested
        /// </summary>
        /// <param name="context">http context</param>
        /// <returns>True if default</returns>
        public static bool DefaultPageRequested(HttpContext context)
        {
            var url = context.Request.Url.ToString();
            var match = string.Format("{0}DEFAULT{1}", Utils.AbsoluteWebRoot, BlogConfig.FileExtension);

            var u = GetUrlWithQueryString(context);
            var m = YearMonthDayRegex.Match(u);

            // case when month/day clicked in the calendar widget/control
            // default page will be like site.com/2012/10/15/default.aspx
            if (!m.Success)
            {
                // case when month clicked in the month list
                // default page will be like site.com/2012/10/default.aspx
                m = YearMonthRegex.Match(u);
            }

            if (m.Success)
            {
                var s = string.Format("{0}{1}DEFAULT{2}", Utils.AbsoluteWebRoot, m.ToString().Substring(1), BlogConfig.FileExtension);

                //Utils.Log("Url: " + url + "; s: " + s);

                if (url.Contains(s, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return url.Contains(match, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Extracts the year and month from the requested URL and returns that as a DateTime.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="year">
        /// The year number.
        /// </param>
        /// <param name="month">
        /// The month number.
        /// </param>
        /// <param name="day">
        /// The day number.
        /// </param>
        /// <returns>
        /// Whether date extraction succeeded.
        /// </returns>
        private static bool ExtractDate(HttpContext context, out int year, out int month, out int day)
        {
            year = 0;
            month = 0;
            day = 0;

            if (!BlogSettings.Instance.TimeStampPostLinks)
            {
                return false;
            }

            var match = YearMonthDayRegex.Match(GetUrlWithQueryString(context));
            if (match.Success)
            {
                year = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                day = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                return true;
            }

            match = YearMonthRegex.Match(GetUrlWithQueryString(context));
            if (match.Success)
            {
                year = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts the title from the requested URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="url">
        /// The url string.
        /// </param>
        /// <returns>
        /// The extract title.
        /// </returns>
        public static string ExtractTitle(HttpContext context, string url)
        {
            url = url.ToLowerInvariant().Replace("---", "-");

            if (string.IsNullOrEmpty(BlogConfig.FileExtension))
            {
                if (url.Contains("?"))
                    url = url.Substring(0, url.LastIndexOf("?"));

                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                if (url.Contains("/"))
                    url = url.Substring(url.LastIndexOf("/") + 1);

                return context.Server.HtmlEncode(url);
            }

            if (url.Contains(BlogConfig.FileExtension) && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
                context.Response.AppendHeader("location", url);
                context.Response.StatusCode = 301;
            }

            if (url.Contains(BlogConfig.FileExtension))
                url = url.Substring(0, url.IndexOf(BlogConfig.FileExtension));

            var index = url.LastIndexOf("/") + 1;
            var title = url.Substring(index);
            return context.Server.HtmlEncode(title);
        }

        /// <summary>
        /// Gets the query string from the requested URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The query string.
        /// </returns>
        public static string GetQueryString(HttpContext context)
        {
            var query = context.Request.QueryString.ToString();
            return !string.IsNullOrEmpty(query) ? string.Format("&{0}", query) : string.Empty;
        }

        /// <summary>
        /// Gets query string portion of URL
        /// </summary>
        /// <param name="context">http context</param>
        /// <returns>Query string</returns>
        public static string GetUrlWithQueryString(HttpContext context)
        {
            return string.Format(
                "{0}?{1}", context.Request.Path, context.Request.QueryString.ToString());
        }

        #endregion
    }
}
