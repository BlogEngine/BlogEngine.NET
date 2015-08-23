namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    using BlogEngine.Core.Providers;

    /// <summary>
    /// Searches the post collection and returns a result based on a search term.
    ///     <remarks>
    /// It is used for related posts and the in-site search feature.
    ///     </remarks>
    /// </summary>
    public static class Search
    {
        #region Constants and Fields

        /// <summary>
        /// The entries.
        /// </summary>
        private static readonly Dictionary<Guid, List<Entry>> _entries = new Dictionary<Guid, List<Entry>>();

        /// <summary>
        /// The stop words.
        /// </summary>
        private static readonly Dictionary<Guid, StringCollection> _stopWords = new Dictionary<Guid, StringCollection>();

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object _syncRoot = new object();

        #endregion

        #region Properties

        private static StringCollection StopWords
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;
                StringCollection stopWords;
                lock (_syncRoot)
                {
                    if (!_stopWords.TryGetValue(blogId, out stopWords))
                    {
                        stopWords = BlogService.LoadStopWords();
                        _stopWords.Add(blogId, stopWords);
                    }
                }

                return stopWords;
            }
        }

        private static List<Entry> Entries
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;
                List<Entry> entries;
                lock (_syncRoot)
                {
                    if (!_entries.TryGetValue(blogId, out entries))
                    {
                        entries = new List<Entry>();
                        _entries.Add(blogId, entries);
                        BuildEntries();
                    }
                }

                return entries;
            }
        }

        private static List<Entry> AllBlogsEntries
        {
            get
            {
                List<Blog> blogs = Blog.Blogs.Where(b => b.IsActive).ToList();
                Guid originalBlogInstanceIdOverride = Blog.InstanceIdOverride;
                List<Entry> entriesAcrossAllBlogs = new List<Entry>();

                // Entries are not loaded for a blog instance until that blog
                // instance is first accessed.  For blog instances where the
                // entries have not yet been loaded, using InstanceIdOverride to
                // temporarily switch the blog CurrentInstance blog so the Entries
                // for that blog instance can be loaded.
                //
                for (int i = 0; i < blogs.Count; i++)
                {
                    List<Entry> blogEntries;
                    if (!_entries.TryGetValue(blogs[i].Id, out blogEntries))
                    {
                        // temporarily override the Current BlogId to the
                        // blog Id we need posts to be loaded for.
                        Blog.InstanceIdOverride = blogs[i].Id;
                        blogEntries = Entries;
                        Blog.InstanceIdOverride = originalBlogInstanceIdOverride;
                    }

                    entriesAcrossAllBlogs.AddRange(blogEntries);
                }

                return entriesAcrossAllBlogs;
            }
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Search"/> class.
        /// </summary>
        static Search()
        {
            Post.Saved += Post_Saved;
            Page.Saved += Page_Saved;
            BlogSettings.Changed += delegate { BuildEntries(); };
            Post.CommentAdded += Post_CommentAdded;
            Post.CommentRemoved += delegate { BuildEntries(); };
            Comment.Approved += Post_CommentAdded;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs after the index has been build.
        /// </summary>
        public static event EventHandler<EventArgs> IndexBuild;

        /// <summary>
        ///     Occurs just before the search index is being build.
        /// </summary>
        public static event EventHandler<EventArgs> IndexBuilding;

        /// <summary>
        ///     Occurs when a search is performed. (The search term is the sender).
        /// </summary>
        public static event EventHandler<EventArgs> Searching;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an IPublishable item to the search entries. 
        ///     That will make it immediately searchable.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public static void AddItem(IPublishable item)
        {
            var entry = new Entry
            {
                Item = item,
                Title = CleanContent(item.Title, false),
                Content = HttpUtility.HtmlDecode(CleanContent(item.Content, true))
            };
            if (item is Comment)
            {
                entry.Content += HttpUtility.HtmlDecode(CleanContent(item.Author, false));
            }

            Entries.Add(entry);
        }

        // public static List<IPublishable> ApmlMatches(Uri url, int maxInterests)
        // {
        // using (System.Net.WebClient client = new System.Net.WebClient())
        // {
        // client.UseDefaultCredentials = true;
        // client.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1;)");
        // using (StreamReader reader = new StreamReader(client.OpenRead(url)))
        // {
        // XmlDocument doc = new XmlDocument();
        // string content = reader.ReadToEnd();
        // string upper = content.ToUpperInvariant();

        // if (upper.Contains("<HTML") && upper.Contains("</HTML"))
        // {
        // List<Uri> urls = FindLinks("apml", content);
        // if (urls.Count > 0)
        // {
        // LoadDocument(url, doc, urls[0]);
        // }
        // else
        // {
        // throw new NotSupportedException("No APML link on page");
        // }
        // }
        // else
        // {
        // doc.LoadXml(content);
        // }

        // return Search.ApmlMatches(doc, 10);
        // }
        // }
        // }

        // private static void LoadDocument(Uri url, XmlDocument doc, Uri ApmlUrl)
        // {
        // if (url.IsAbsoluteUri)
        // {
        // doc.Load(ApmlUrl.ToString());
        // }
        // else
        // {
        // string absoluteUrl = null;
        // if (!url.ToString().StartsWith("/"))
        // absoluteUrl = (url + ApmlUrl.ToString());
        // else
        // absoluteUrl = url.Scheme + "://" + url.Authority + ApmlUrl;

        // doc.Load(absoluteUrl);
        // }
        // }

        /// <summary>
        /// The apml matches.
        /// </summary>
        /// <param name="apmlFile">
        /// The apml file.
        /// </param>
        /// <param name="maxInterests">
        /// The max interests.
        /// </param>
        /// <returns>
        /// A list of IPublishable.
        /// </returns>
        public static List<IPublishable> ApmlMatches(XmlDocument apmlFile, int maxInterests)
        {
            var concepts = new Dictionary<string, float>();
            var nodes = apmlFile.SelectNodes("//Concept");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes == null)
                    {
                        continue;
                    }

                    var key = node.Attributes["key"].InnerText.ToLowerInvariant().Trim();
                    var value = float.Parse(node.Attributes["value"].InnerText, CultureInfo.InvariantCulture);
                    if (!concepts.ContainsKey(key))
                    {
                        concepts.Add(key, value);
                    }
                    else if (concepts[key] < value)
                    {
                        concepts[key] = value;
                    }
                }
            }

            concepts = SortDictionary(concepts);
            var max = Math.Min(concepts.Count, maxInterests);
            var counter = 0;
            var resultSet = new List<Result>();
            foreach (var key in concepts.Keys)
            {
                counter++;
                var results = BuildResultSet(key, false);

                // results = results.FindAll(delegate(Result r) { return r.Rank > 1; });
                resultSet.AddRange(results);
                if (counter == max)
                {
                    break;
                }
            }

            resultSet.Sort();
            var aggregatedResults = new List<Result>();
            foreach (var r in resultSet)
            {
                if (!aggregatedResults.Contains(r))
                {
                    aggregatedResults.Add(r);
                }
                else
                {
                    var r1 = r;
                    var existingResult =
                        aggregatedResults.Find(res => res.GetHashCode() == r1.GetHashCode());
                    existingResult.Rank += r.Rank;
                }
            }

            aggregatedResults = aggregatedResults.FindAll(r => r.Rank > 1);
            var items = aggregatedResults.ConvertAll(ResultToPost);
            var uniqueItems = new List<IPublishable>();

            foreach (var item in items.Where(item => !uniqueItems.Contains(item)))
            {
                uniqueItems.Add(item);
            }

            return uniqueItems;
        }

        /// <summary>
        /// Returns a list of posts that is related to the specified post.
        /// </summary>
        /// <param name="post">The IPublishable post.</param>
        /// <returns>A list of IPublishable.</returns>
        public static List<IPublishable> FindRelatedItems(IPublishable post)
        {
            return FindRelatedItems(post.Title);
        }

        /// <summary>
        /// Returns a list of posts that is related to the specified post.
        /// </summary>
        /// <param name="postTitle">The post title.</param>
        /// <returns>A list of IPublishable.</returns>
        public static List<IPublishable> FindRelatedItems(string postTitle)
        {
            var term = CleanContent(postTitle, false);
            return Hits(term, false);
        }

        /// <summary>
        /// Searches all the posts and returns a ranked result set.
        /// </summary>
        /// <param name="searchTerm">The term to search for</param>
        /// <param name="includeComments">True to include a post's comments and their authors in search</param>
        /// <returns>A list of IPublishable.</returns>
        public static List<IPublishable> Hits(string searchTerm, bool includeComments)
        {
            lock (_syncRoot)
            {
                var results = BuildResultSet(searchTerm, includeComments);
                var items = results.ConvertAll(ResultToPost);
                results.Clear();
                OnSearcing(searchTerm);
                return items;
            }
        }

        #endregion

        // private const string PATTERN = "<head.*<link( [^>]*title=\"{0}\"[^>]*)>.*</head>";
        // private static readonly Regex HREF = new Regex("href=\"(.*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        ///// <summary>
        ///// Finds semantic links in a given HTML document.
        ///// </summary>
        ///// <param name="type">The type of link. Could be foaf, apml or sioc.</param>
        ///// <param name="html">The HTML to look through.</param>
        ///// <returns></returns>
        // public static List<Uri> FindLinks(string type, string html)
        // {
        // MatchCollection matches = Regex.Matches(html, string.Format(PATTERN, type), RegexOptions.IgnoreCase | RegexOptions.Singleline);
        // List<Uri> urls = new List<Uri>();

        // foreach (Match match in matches)
        // {
        // if (match.Groups.Count == 2)
        // {
        // string link = match.Groups[1].Value;
        // Match hrefMatch = HREF.Match(link);

        // if (hrefMatch.Groups.Count == 2)
        // {
        // Uri url;
        // string value = hrefMatch.Groups[1].Value;
        // if (Uri.TryCreate(value, UriKind.Absolute, out url))
        // {
        // urls.Add(url);
        // }
        // }
        // }
        // }

        // return urls;
        // }
        #region Methods

        /// <summary>
        /// Builds the entries so they can be searched.
        /// </summary>
        private static void BuildEntries()
        {
            OnIndexBuilding();

            lock (_syncRoot)
            {
                Entries.Clear();
                foreach (var post in Post.Posts.Where(post => post.IsVisibleToPublic))
                {
                    AddItem(post);
                    if (!BlogSettings.Instance.EnableCommentSearch)
                    {
                        continue;
                    }

                    foreach (var comment in post.Comments.Where(comment => comment.IsApproved))
                    {
                        AddItem(comment);
                    }
                }

                foreach (var page in Page.Pages.Where(page => page.IsVisibleToPublic))
                {
                    AddItem(page);
                }
            }

            OnIndexBuild();
        }

        /// <summary>
        /// Builds the results set and ranks it.
        /// </summary>
        /// <param name="searchTerm">
        /// The search Term.
        /// </param>
        /// <param name="includeComments">
        /// The include Comments.
        /// </param>
        private static List<Result> BuildResultSet(string searchTerm, bool includeComments)
        {
            var results = new List<Result>();
            var term = CleanContent(searchTerm.ToLowerInvariant().Trim(), false);
            var terms = term.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var regex = new Regex(string.Format(CultureInfo.InvariantCulture, "({0})", string.Join("|", terms)));

            List<Entry> entries = Blog.CurrentInstance.IsSiteAggregation ? AllBlogsEntries : Entries;

            foreach (var entry in entries)
            {
                if (entry.Item == null)
                    continue;

                var result = new Result();

                if (!(entry.Item is Comment))
                {
                    var titleMatches = regex.Matches(entry.Title).Count;
                    result.Rank = titleMatches * 20;

                    var postMatches = regex.Matches(entry.Content).Count;
                    result.Rank += postMatches;

                    var descriptionMatches = regex.Matches(entry.Item.Description).Count;
                    result.Rank += descriptionMatches * 2;
                }
                else if (includeComments)
                {
                    var commentMatches = regex.Matches(entry.Content + entry.Title).Count;
                    result.Rank += commentMatches;
                }

                if (result.Rank > 0)
                {
                    result.Item = entry.Item;
                    results.Add(result);
                }
            }

            results.Sort();
            return results;
        }

        /// <summary>
        /// Removes stop words and HTML from the specified string.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="removeHtml">
        /// The remove Html.
        /// </param>
        /// <returns>
        /// The clean content.
        /// </returns>
        private static string CleanContent(string content, bool removeHtml)
        {
            if (removeHtml)
            {
                content = Utils.StripHtml(content);
            }

            content =
                content.Replace("\\", string.Empty).Replace("|", string.Empty).Replace("(", string.Empty).Replace(
                    ")", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("*", string.Empty).
                    Replace("?", string.Empty).Replace("}", string.Empty).Replace("{", string.Empty).Replace(
                        "^", string.Empty).Replace("+", string.Empty);

            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var word in
                words.Select(t => t.ToLowerInvariant().Trim()).Where(word => word.Length > 1 && !StopWords.Contains(word)))
            {
                sb.AppendFormat("{0} ", word);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Raises the event in a safe way
        /// </summary>
        private static void OnIndexBuild()
        {
            if (IndexBuild != null)
            {
                IndexBuild(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the event in a safe way
        /// </summary>
        private static void OnIndexBuilding()
        {
            if (IndexBuilding != null)
            {
                IndexBuilding(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the event in a safe way
        /// </summary>
        /// <param name="searchTerm">
        /// The search Term.
        /// </param>
        private static void OnSearcing(string searchTerm)
        {
            if (Searching != null)
            {
                Searching(searchTerm, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the Saved event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        private static void Page_Saved(object sender, SavedEventArgs e)
        {
            lock (_syncRoot)
            {
                if (e.Action == SaveAction.Insert)
                {
                    AddItem(sender as Page);
                }
                else
                {
                    BuildEntries();
                }
            }
        }

        /// <summary>
        /// Handles the CommentAdded event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void Post_CommentAdded(object sender, EventArgs e)
        {
            if (!BlogSettings.Instance.EnableCommentSearch)
            {
                return;
            }

            var comment = (Comment)sender;
            if (comment.IsApproved)
            {
                AddItem(comment);
            }
        }

        /// <summary>
        /// Handles the Saved event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        private static void Post_Saved(object sender, SavedEventArgs e)
        {
            lock (_syncRoot)
            {
                if (e.Action == SaveAction.Insert)
                {
                    AddItem(sender as Post);
                }
                else
                {
                    BuildEntries();
                }
            }
        }

        /// <summary>
        /// A converter delegate used for converting Results to Posts.
        /// </summary>
        /// <param name="result">The IPublishable result.</param>
        /// <returns>An IPublishable.</returns>
        private static IPublishable ResultToPost(Result result)
        {
            return result.Item;
        }

        /// <summary>
        /// The sort dictionary.
        /// </summary>
        /// <param name="dic">
        /// The dictionary of string keys with float values.
        /// </param>
        /// <returns>
        /// A dictionary of string keys with float values.
        /// </returns>
        private static Dictionary<string, float> SortDictionary(Dictionary<string, float> dic)
        {
            var list = dic.Keys.Select(key => new KeyValuePair<string, float>(key, dic[key])).ToList();

            list.Sort((obj1, obj2) => obj2.Value.CompareTo(obj1.Value));

            return list.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        #endregion
    }

    #region Entry and Result structs

    /// <summary>
    /// A search optimized post object cleansed from HTML and stop words.
    /// </summary>
    internal struct Entry
    {
        #region Constants and Fields

        /// <summary>
        ///     The content of the post cleansed for stop words and HTML
        /// </summary>
        internal string Content;

        /// <summary>
        ///     The post object reference
        /// </summary>
        internal IPublishable Item;

        /// <summary>
        ///     The title of the post cleansed for stop words
        /// </summary>
        internal string Title;

        #endregion
    }

    /// <summary>
    /// A result is a search result which contains a post and its ranking.
    /// </summary>
    internal class Result : IComparable<Result>
    {
        #region Constants and Fields

        /// <summary>
        ///     The post of the result.
        /// </summary>
        internal IPublishable Item;

        /// <summary>
        ///     The rank of the post based on the search term. The higher the rank, the higher the post is in the result set.
        /// </summary>
        internal int Rank;

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Item.Id.GetHashCode();
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable<Result>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value 
        ///     has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero 
        ///     This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(Result other)
        {
            return other.Rank.CompareTo(this.Rank);
        }

        #endregion

        #endregion
    }

    #endregion
}