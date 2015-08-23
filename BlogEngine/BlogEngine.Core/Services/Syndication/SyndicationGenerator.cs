/****************************************************************************
Modification History:
*****************************************************************************
Date        Author      Description
*****************************************************************************
08/29/2007  brian.kuhn		Created SyndicationGenerator Class
02/12/2008  rtur.net
14/03/2011  CreepinJesus    Updated comment links for use with Disqus
****************************************************************************/
namespace BlogEngine.Core
{
    using BlogEngine.Core.Data.Services;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Generates syndication feeds for blog entities.
    /// </summary>
    public class SyndicationGenerator
    {
        #region Constants and Fields

        /// <summary>
        ///     Private member to hold the name of the syndication generation utility.
        /// </summary>
        private const string GeneratorName = "BlogEngine.Net Syndication Generator";

        /// <summary>
        ///     Private member to hold the URI of the syndication generation utility.
        /// </summary>
        private static readonly Uri GeneratorUri = new Uri("http://dotnetblogengine.net/");

        /// <summary>
        ///     Private member to hold the version of the syndication generation utility.
        /// </summary>
        private static readonly Version GeneratorVersion = new Version("1.0.0.0");

        /// <summary>
        /// Whether the file exists.
        /// </summary>
        private static bool fileExists;

        /// <summary>
        /// The file size.
        /// </summary>
        private static long fileSize;

        /// <summary>
        ///     Private member to hold a collection of the XML namespaces that define supported syndication extensions.
        /// </summary>
        private static Dictionary<string, string> xmlNamespaces;

        /// <summary>
        ///     Private member to hold a collection of <see cref = "Category" /> objects used to categorize the web log content.
        /// </summary>
        private List<Category> blogCategories;

        /// <summary>
        ///     Private member to hold the <see cref = "BlogSettings" /> to use when generating syndication results.
        /// </summary>
        private BlogSettings blogSettings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyndicationGenerator"/> class using the supplied <see cref="BlogSettings"/> and collection of <see cref="Category"/> objects.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="BlogSettings"/> to use when generating syndication results.
        /// </param>
        /// <param name="categories">
        /// A collection of <see cref="Category"/> objects used to categorize the web log content.
        /// </param>
        public SyndicationGenerator(BlogSettings settings, List<Category> categories)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (categories == null)
            {
                throw new ArgumentNullException("categories");
            }

            // ------------------------------------------------------------
            // Initialize class state
            // ------------------------------------------------------------
            this.Settings = settings;

            if (categories.Count <= 0)
            {
                return;
            }

            var values = new Category[categories.Count];
            categories.CopyTo(values);

            foreach (var category in values)
            {
                this.Categories.Add(category);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a collection of the XML namespaces used to provide support for syndication extensions.
        /// </summary>
        /// <value>The collection of the XML namespaces, keyed by namespace prefix, that are used to provide support for syndication extensions.</value>
        public static Dictionary<string, string> SupportedNamespaces
        {
            get
            {
                // xmlNamespaces.Add("atom", "http://www.w3.org/2005/Atom");
                return xmlNamespaces ??
                       (xmlNamespaces =
                        new Dictionary<string, string>
                            {
                                { "blogChannel", "http://backend.userland.com/blogChannelModule" },
                                { "dc", "http://purl.org/dc/elements/1.1/" },
                                { "pingback", "http://madskills.com/public/xml/rss/module/pingback/" },
                                { "trackback", "http://madskills.com/public/xml/rss/module/trackback/" },
                                { "wfw", "http://wellformedweb.org/CommentAPI/" },
                                { "slash", "http://purl.org/rss/1.0/modules/slash/" },
                                { "geo", "http://www.w3.org/2003/01/geo/wgs84_pos#" },
                                { "betag", "http://dotnetblogengine.net/schemas/tags"}
                            });
            }
        }

        /// <summary>
        ///     Gets a collection of <see cref = "Category" /> objects used to categorize the web log content.
        /// </summary>
        /// <value>A collection of <see cref = "Category" /> objects used to categorize the web log content.</value>
        public List<Category> Categories
        {
            get
            {
                return this.blogCategories ?? (this.blogCategories = new List<Category>());
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref = "BlogSettings" /> used when generating syndication results.
        /// </summary>
        /// <value>The <see cref = "BlogSettings" /> used when generating syndication results.</value>
        /// <exception cref = "ArgumentNullException">The <paramref name = "value" /> is a null reference (Nothing in Visual Basic).</exception>
        public BlogSettings Settings
        {
            get
            {
                return this.blogSettings;
            }

            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                
                this.blogSettings = value;
            }
        }

        /// <summary>
        /// Gets SupportedMedia.
        /// </summary>
        private static Dictionary<string, string> SupportedMedia
        {
            get
            {
                var dic = new Dictionary<string, string>
                    {
                        { ".mp3", "audio/mpeg" },
                        { ".m4a3", "audio/x-m4a" },
                        { ".mp4", "video/mp4" },
                        { ".m4v", "video/x-m4v" },
                        { ".mov", "video/quicktime" },
                        { ".pdf", "application/pdf" }
                    };
                return dic;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the supplied <see cref="DateTime"/> to its equivalent <a href="http://asg.web.cmu.edu/rfc/rfc822.html">RFC-822 DateTime</a> string representation.
        /// </summary>
        /// <param name="dateTime">
        /// The <see cref="DateTime"/> to convert.
        /// </param>
        /// <returns>
        /// The equivalent <a href="http://asg.web.cmu.edu/rfc/rfc822.html">RFC-822 DateTime</a> string representation.
        /// </returns>
        public static string ToRfc822DateTime(DateTime dateTime)
        {
            var offset =
                (int)(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours + BlogSettings.Instance.Timezone);
            var timeZone = string.Format("+{0}", offset.ToString(NumberFormatInfo.InvariantInfo).PadLeft(2, '0'));

            // ------------------------------------------------------------
            // Adjust time zone based on offset
            // ------------------------------------------------------------
            if (offset < 0)
            {
                var i = offset * -1;
                timeZone = string.Format("-{0}", i.ToString(NumberFormatInfo.InvariantInfo).PadLeft(2, '0'));
            }

            return dateTime.ToString(
                string.Format("ddd, dd MMM yyyy HH:mm:ss {0}", timeZone.PadRight(5, '0')), DateTimeFormatInfo.InvariantInfo);
        }

        static string RssDateString(DateTime pubDate)
        {
            pubDate = pubDate.AddHours(BlogSettings.Instance.Timezone);
            var value = pubDate.ToString("ddd',' d MMM yyyy HH':'mm':'ss") + " " +
                pubDate.ToString("zzzz").Replace(":", "");
            return value;
        }

        /// <summary>
        /// Writes a generated syndication feed that conforms to the supplied <see cref="SyndicationFormat"/> using the supplied <see cref="Stream"/> and collection.
        /// </summary>
        /// <param name="format">
        /// A <see cref="SyndicationFormat"/> enumeration value indicating the syndication format to generate.
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which you want to write the syndication feed.
        /// </param>
        /// <param name="publishables">
        /// The collection of <see cref="IPublishable"/> objects used to generate the syndication feed content.
        /// </param>
        /// <param name="title">
        /// The title of the RSS channel
        /// </param>
        public void WriteFeed(SyndicationFormat format, Stream stream, List<IPublishable> publishables, string title)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (publishables == null)
            {
                throw new ArgumentNullException("publishables");
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException(
                    String.Format(
                        null,
                        "Unable to generate {0} syndication feed. The provided stream does not support writing.",
                        format),
                    "stream");
            }

            // ------------------------------------------------------------
            // Write syndication feed based on specified format
            // ------------------------------------------------------------
            switch (format)
            {
                case SyndicationFormat.Atom:
                    this.WriteAtomFeed(stream, publishables, title);
                    break;

                case SyndicationFormat.Rss:
                    this.WriteRssFeed(stream, publishables, title);
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the value of the specified <see cref="TimeSpan"/> to its equivalent string representation.
        /// </summary>
        /// <param name="offset">
        /// The <see cref="TimeSpan"/> to convert.
        /// </param>
        /// <param name="separator">
        /// Separator used to deliminate hours and minutes.
        /// </param>
        /// <returns>
        /// A string representation of the TimeSpan.
        /// </returns>
        private static string FormatW3COffset(TimeSpan offset, string separator)
        {
            var formattedOffset = String.Empty;

            if (offset >= TimeSpan.Zero)
            {
                formattedOffset = "+";
            }

            return String.Concat(
                formattedOffset,
                offset.Hours.ToString("00", CultureInfo.InvariantCulture),
                separator,
                offset.Minutes.ToString("00", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets enclosure for supported media type
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="publishable">The publishable instance.</param>
        /// <returns>The enclosure.</returns>
        private static string GetEnclosure(string content, IPublishable publishable)
        {
            var enclosure = string.Empty;
            fileSize = 0;
            fileExists = false;

            foreach (var media in SupportedMedia)
            {
                enclosure = GetMediaEnclosure(publishable, content, media.Key, media.Value);
                if (enclosure.Length > 0)
                {
                    break;
                }
            }

            return enclosure;
        }

        /// <summary>
        /// Gets enclosure for supported media type
        /// </summary>
        /// <param name="publishable">The publishable instance.</param>
        /// <param name="content">The content.</param>
        /// <param name="media">The media.</param>
        /// <param name="mediatype">The mediatype.</param>
        /// <returns>The enclosure.</returns>
        private static string GetMediaEnclosure(IPublishable publishable, string content, string media, string mediatype)
        {
            const string RegexLink = @"<a href=((.|\n)*?)>((.|\n)*?)</a>";
            var enclosure = "<enclosure url=\"{0}\" length=\"{1}\" type=\"{2}\" />";
            var matches = Regex.Matches(content, RegexLink);

            if (matches.Count > 0)
            {
                string filename;

                foreach (var match in matches.Cast<Match>().Where(match => match.Value.Contains(media)))
                {
                    filename = match.Value.Substring(match.Value.IndexOf("http"));
                    filename = filename.Substring(0, filename.IndexOf(">")).Replace("\"", string.Empty).Trim();
                    filename = ValidateFileName(publishable, filename);

                    if (!fileExists)
                    {
                        continue;
                    }
                    
                    enclosure = string.Format(enclosure, filename, fileSize, mediatype);
                    return enclosure;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates a <see cref="Uri"/> that represents the peramlink for the supplied <see cref="IPublishable"/>.
        /// </summary>
        /// <param name="publishable">
        /// The <see cref="IPublishable"/> used to generate the permalink for.
        /// </param>
        /// <returns>
        /// A <see cref="Uri"/> that represents the peramlink for the supplied <see cref="IPublishable"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="publishable"/> is a null reference (Nothing in Visual Basic).
        /// </exception>
        private static Uri GetPermaLink(IPublishable publishable)
        {
            var post = publishable as Post;
            return post != null ? post.PermaLink : publishable.AbsoluteLink;
        }

        /// <summary>
        /// Converts the supplied <see cref="DateTime"/> to its equivalent <a href="http://www.w3.org/TR/NOTE-datetime">W3C DateTime</a> string representation.
        /// </summary>
        /// <param name="utcDateTime">
        /// The Coordinated Universal Time (UTC) <see cref="DateTime"/> to convert.
        /// </param>
        /// <returns>
        /// The equivalent <a href="http://www.w3.org/TR/NOTE-datetime">W3C DateTime</a> string representation.
        /// </returns>
        private static string ToW3CDateTime(DateTime utcDateTime)
        {
            var utcOffset = TimeSpan.Zero;
            return (utcDateTime + utcOffset).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) +
                   FormatW3COffset(utcOffset, ":");
        }

        /// <summary>
        /// Validates the name of the file.
        /// </summary>
        /// <param name="publishable">The publishable instance.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The validated file name.</returns>
        private static string ValidateFileName(IPublishable publishable, string fileName)
        {
            fileName = fileName.Replace(publishable.Blog.AbsoluteWebRoot.ToString(), string.Empty);

            try
            {
                var physicalPath = HttpContext.Current.Server.MapPath(fileName);
                var info = new FileInfo(physicalPath);
                fileSize = info.Length;
                fileExists = true;
            }
            catch (Exception)
            {
                // if file does not exist - try to strip down leading
                // directory in the path; sometimes it duplicated
                if (fileName.IndexOf("/") > 0)
                {
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                    ValidateFileName(publishable, fileName);
                }
            }

            return publishable.Blog.AbsoluteWebRoot + fileName;
        }

        /// <summary>
        /// Writes the Atom feed entry element information to the specified <see cref="XmlWriter"/> using the supplied <see cref="Page"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write feed entry element information to.
        /// </param>
        /// <param name="publishable">
        /// The <see cref="IPublishable"/> used to generate feed entry content.
        /// </param>
        private static void WriteAtomEntry(XmlWriter writer, IPublishable publishable)
        {
            var post = publishable as Post;
            
            // var comment = publishable as Comment;

            // ------------------------------------------------------------
            // Raise serving event
            // ------------------------------------------------------------                
            var arg = new ServingEventArgs(publishable.Content, ServingLocation.Feed);
            publishable.OnServing(arg);
            if (arg.Cancel)
            {
                return;
            }

            // ------------------------------------------------------------
            // Modify publishable content to make references absolute
            // ------------------------------------------------------------
            var content = Utils.ConvertPublishablePathsToAbsolute(arg.Body, publishable);

            writer.WriteStartElement("entry");

            // ------------------------------------------------------------
            // Write required entry elements
            // ------------------------------------------------------------
            writer.WriteElementString("id", publishable.AbsoluteLink.ToString());
            writer.WriteElementString("title", publishable.Title);
            writer.WriteElementString("updated", ToW3CDateTime(publishable.DateCreated.ToUniversalTime()));

            // ------------------------------------------------------------
            // Write recommended entry elements
            // ------------------------------------------------------------
            writer.WriteStartElement("link");
            writer.WriteAttributeString("rel", "self");
            writer.WriteAttributeString("href", GetPermaLink(publishable).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("link");
            writer.WriteAttributeString("href", publishable.AbsoluteLink.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("author");
            writer.WriteElementString("name", publishable.Author);
            writer.WriteEndElement();

            writer.WriteStartElement("summary");
            writer.WriteAttributeString("type", "html");
            writer.WriteString(content);
            writer.WriteEndElement();

            // ------------------------------------------------------------
            // Write optional entry elements
            // ------------------------------------------------------------
            writer.WriteElementString("published", ToW3CDateTime(publishable.DateCreated.ToUniversalTime()));

            writer.WriteStartElement("link");
            writer.WriteAttributeString("rel", "related");
            writer.WriteAttributeString("href", String.Concat(publishable.AbsoluteLink.ToString(),
				BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus ? "#disqus_thread" : "#comment"));
            writer.WriteEndElement();

            // ------------------------------------------------------------
            // Write enclosure tag for podcasting support
            // ------------------------------------------------------------
            if (BlogSettings.Instance.EnableEnclosures)
            {
                var encloser = GetEnclosure(content, publishable);
                if (!string.IsNullOrEmpty(encloser))
                {
                    writer.WriteRaw(encloser);
                }
            }

            // ------------------------------------------------------------
            // Write entry category elements
            // ------------------------------------------------------------
            if (publishable.Categories != null)
            {
                foreach (var category in publishable.Categories)
                {
                    writer.WriteStartElement("category");
                    writer.WriteAttributeString("term", category.Title);
                    writer.WriteEndElement();
                }
            }

            // ------------------------------------------------------------
            // Write entry tag elements
            // ------------------------------------------------------------
            if (BlogSettings.Instance.EnableTagExport && publishable.Categories != null)
            {
                foreach (var tag in publishable.Tags)
                {
                    writer.WriteElementString("betag", "tag", "http://dotnetblogengine.net/schemas/tags", tag);
                }
            }


            // ------------------------------------------------------------
            // Write Dublin Core syndication extension elements
            // ------------------------------------------------------------
            if (!String.IsNullOrEmpty(publishable.Author))
            {
                writer.WriteElementString("dc", "publisher", "http://purl.org/dc/elements/1.1/", publishable.Author);
            }

            if (!String.IsNullOrEmpty(publishable.Description))
            {
                writer.WriteElementString(
                    "dc", "description", "http://purl.org/dc/elements/1.1/", publishable.Description);
            }

            // ------------------------------------------------------------
            // Write pingback syndication extension elements
            // ------------------------------------------------------------
            Uri pingbackServer;
            if (Uri.TryCreate(
                String.Concat(publishable.Blog.AbsoluteWebRoot.ToString().TrimEnd('/'), "/pingback.axd"),
                UriKind.RelativeOrAbsolute,
                out pingbackServer))
            {
                writer.WriteElementString(
                    "pingback",
                    "server",
                    "http://madskills.com/public/xml/rss/module/pingback/",
                    pingbackServer.ToString());
                writer.WriteElementString(
                    "pingback",
                    "target",
                    "http://madskills.com/public/xml/rss/module/pingback/",
                    GetPermaLink(publishable).ToString());
            }

            // ------------------------------------------------------------
            // Write slash syndication extension elements
            // ------------------------------------------------------------
            if (post != null && post.Comments != null)
            {
                writer.WriteElementString(
                    "slash",
                    "comments",
                    "http://purl.org/rss/1.0/modules/slash/",
                    post.Comments.Count.ToString(CultureInfo.InvariantCulture));
            }

            // ------------------------------------------------------------
            // Write trackback syndication extension elements
            // ------------------------------------------------------------
            if (post != null && post.TrackbackLink != null)
            {
                writer.WriteElementString(
                    "trackback",
                    "ping",
                    "http://madskills.com/public/xml/rss/module/trackback/",
                    post.TrackbackLink.ToString());
            }

            // ------------------------------------------------------------
            // Write well-formed web syndication extension elements
            // ------------------------------------------------------------
            writer.WriteElementString(
                "wfw",
                "comment",
                "http://wellformedweb.org/CommentAPI/",
                String.Concat(publishable.AbsoluteLink.ToString(),
				BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus ? "#disqus_thread" : "#comment"));
            writer.WriteElementString(
                "wfw",
                "commentRss",
                "http://wellformedweb.org/CommentAPI/",
                string.Format("{0}/syndication.axd?post={1}", publishable.Blog.AbsoluteWebRoot.ToString().TrimEnd('/'), publishable.Id));

            // ------------------------------------------------------------
            // Write </entry> element
            // ------------------------------------------------------------
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the RSS channel item element information to the specified <see cref="XmlWriter"/> using the supplied <see cref="Page"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write channel item element information to.
        /// </param>
        /// <param name="publishable">
        /// The <see cref="IPublishable"/> used to generate channel item content.
        /// </param>
        private static void WriteRssItem(XmlWriter writer, IPublishable publishable)
        {
            // ------------------------------------------------------------
            // Cast IPublishable as Post to support comments/trackback
            // ------------------------------------------------------------
            var post = publishable as Post;
            var comment = publishable as Comment;

            // ------------------------------------------------------------
            // Raise serving event
            // ------------------------------------------------------------                
            var arg = new ServingEventArgs(publishable.Content, ServingLocation.Feed);
            publishable.OnServing(arg);
            if (arg.Cancel)
            {
                return;
            }

            // ------------------------------------------------------------
            // Modify post content to make references absolute
            // ------------------------------------------------------------    
            var content = Utils.ConvertPublishablePathsToAbsolute(arg.Body, publishable);

            // handle custom fields in the posts
            content = CustomFieldsParser.GetPageHtml(content);

            if (comment != null)
            {
                content = content.Replace(Environment.NewLine, "<br />");
            }

            writer.WriteStartElement("item");

            // ------------------------------------------------------------
            // Write required channel item elements
            // ------------------------------------------------------------
            if (comment != null)
            {
                writer.WriteElementString("title", publishable.Author + " on " + comment.Parent.Title);
            }
            else
            {
                writer.WriteElementString("title", publishable.Title);
            }

            writer.WriteElementString("description", content);
            writer.WriteElementString("link", publishable.AbsoluteLink.ToString());

            // ------------------------------------------------------------
            // Write enclosure tag for podcasting support
            // ------------------------------------------------------------
            if (BlogSettings.Instance.EnableEnclosures)
            {
                var encloser = GetEnclosure(content, publishable);
                if (!string.IsNullOrEmpty(encloser))
                {
                    writer.WriteRaw(encloser);
                }
            }

            // ------------------------------------------------------------
            // Write optional channel item elements
            // ------------------------------------------------------------
            if (!string.IsNullOrEmpty(BlogSettings.Instance.FeedAuthor))
            {
                writer.WriteElementString("author", BlogSettings.Instance.FeedAuthor);
            }
            if (post != null)
            {
                writer.WriteElementString(
                    "comments", String.Concat(publishable.AbsoluteLink.ToString(),
					BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus ? "#disqus_thread" : "#comment"));
            }

            writer.WriteElementString("guid", GetPermaLink(publishable).ToString());
            writer.WriteElementString("pubDate", RssDateString(publishable.DateCreated));

            // ------------------------------------------------------------
            // Write channel item category elements
            // ------------------------------------------------------------
            if (publishable.Categories != null)
            {
                foreach (var category in publishable.Categories)
                {
                    writer.WriteElementString("category", category.Title);
                }
            }

            // ------------------------------------------------------------
            // Write channel item tag elements
            // ------------------------------------------------------------
            if (BlogSettings.Instance.EnableTagExport && publishable.Categories != null)
            {
                foreach (var tag in publishable.Tags)
                {
                    writer.WriteElementString("betag", "tag", "http://dotnetblogengine.net/schemas/tags", tag);
                }
            }

            // ------------------------------------------------------------
            // Write Dublin Core syndication extension elements
            // ------------------------------------------------------------
            if (!String.IsNullOrEmpty(publishable.Author))
            {
                writer.WriteElementString("dc", "publisher", "http://purl.org/dc/elements/1.1/", publishable.Author);
            }

            // if (!String.IsNullOrEmpty(publishable.Description))
            // {
            //     writer.WriteElementString("dc", "description", "http://purl.org/dc/elements/1.1/", publishable.Description);
            // }

            // ------------------------------------------------------------
            // Write pingback syndication extension elements
            // ------------------------------------------------------------
            Uri pingbackServer;
            if (Uri.TryCreate(
                String.Concat(publishable.Blog.AbsoluteWebRoot.ToString().TrimEnd('/'), "/pingback.axd"),
                UriKind.RelativeOrAbsolute,
                out pingbackServer))
            {
                writer.WriteElementString(
                    "pingback",
                    "server",
                    "http://madskills.com/public/xml/rss/module/pingback/",
                    pingbackServer.ToString());
                writer.WriteElementString(
                    "pingback",
                    "target",
                    "http://madskills.com/public/xml/rss/module/pingback/",
                    GetPermaLink(publishable).ToString());
            }

            // ------------------------------------------------------------
            // Write slash syndication extension elements
            // ------------------------------------------------------------
            if (post != null && post.Comments != null)
            {
                writer.WriteElementString(
                    "slash",
                    "comments",
                    "http://purl.org/rss/1.0/modules/slash/",
                    post.Comments.Count.ToString(CultureInfo.InvariantCulture));
            }

            // ------------------------------------------------------------
            // Write trackback syndication extension elements
            // ------------------------------------------------------------
            if (post != null && post.TrackbackLink != null)
            {
                writer.WriteElementString(
                    "trackback",
                    "ping",
                    "http://madskills.com/public/xml/rss/module/trackback/",
                    post.TrackbackLink.ToString());
            }

            // ------------------------------------------------------------
            // Write well-formed web syndication extension elements
            // ------------------------------------------------------------
            writer.WriteElementString(
                "wfw",
                "comment",
                "http://wellformedweb.org/CommentAPI/",
                String.Concat(publishable.AbsoluteLink.ToString(),
				BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus ? "#disqus_thread" : "#comment"));
            writer.WriteElementString(
                "wfw",
                "commentRss",
                "http://wellformedweb.org/CommentAPI/",
                string.Format("{0}/syndication.axd?post={1}", publishable.Blog.AbsoluteWebRoot.ToString().TrimEnd('/'), publishable.Id));

            // ------------------------------------------------------------
            // Write </item> element
            // ------------------------------------------------------------
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the Atom feed element information to the specified <see cref="XmlWriter"/> using the supplied collection.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write channel element information to.
        /// </param>
        /// <param name="publishables">
        /// The collection of <see cref="IPublishable"/> objects used to generate syndication content.
        /// </param>
        /// <param name="title">
        /// The title of the ATOM content.
        /// </param>
        private void WriteAtomContent(XmlWriter writer, List<IPublishable> publishables, string title)
        {
            // ------------------------------------------------------------
            // Write required feed elements
            // ------------------------------------------------------------
            writer.WriteElementString("id", Utils.AbsoluteWebRoot.ToString());
            writer.WriteElementString("title", title);
            
            var updated = publishables.Count > 0
                              ? ToW3CDateTime(publishables[0].DateModified.ToUniversalTime())
                              : ToW3CDateTime(DateTime.UtcNow);
            
            writer.WriteElementString(
                "updated",
                updated);

            // ------------------------------------------------------------
            // Write recommended feed elements
            // ------------------------------------------------------------
            writer.WriteStartElement("link");
            writer.WriteAttributeString("href", Utils.AbsoluteWebRoot.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("link");
            writer.WriteAttributeString("rel", "self");
            writer.WriteAttributeString("href", string.Format("{0}syndication.axd?format=atom", Utils.AbsoluteWebRoot));
            writer.WriteEndElement();

            // writer.WriteStartElement("link");
            // writer.WriteAttributeString("rel", "alternate");
            // writer.WriteAttributeString("href", Utils.FeedUrl.ToString());
            // writer.WriteEndElement();

            // ------------------------------------------------------------
            // Write optional feed elements
            // ------------------------------------------------------------
            writer.WriteElementString("subtitle", this.Settings.Description);

            // ------------------------------------------------------------
            // Write common/shared feed elements
            // ------------------------------------------------------------
            this.WriteAtomContentCommonElements(writer);

            // ------------------------------------------------------------
            // Skip publishable content if it is not visible
            // ------------------------------------------------------------
            foreach (var publishable in publishables.Where(publishable => publishable.IsVisible))
            {
                // ------------------------------------------------------------
                // Write <entry> element for publishable content
                // ------------------------------------------------------------
                WriteAtomEntry(writer, publishable);
            }
        }

        /// <summary>
        /// Writes the common/shared Atom feed element information to the specified <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write channel element information to.
        /// </param>
        private void WriteAtomContentCommonElements(XmlWriter writer)
        {
            // ------------------------------------------------------------
            // Write optional feed elements
            // ------------------------------------------------------------
            writer.WriteStartElement("author");
            writer.WriteElementString("name", this.Settings.AuthorName);
            writer.WriteEndElement();

            writer.WriteStartElement("generator");
            writer.WriteAttributeString("uri", GeneratorUri.ToString());
            writer.WriteAttributeString("version", GeneratorVersion.ToString());
            writer.WriteString(GeneratorName);
            writer.WriteEndElement();

            // ------------------------------------------------------------
            // Write blogChannel syndication extension elements
            // ------------------------------------------------------------
            Uri blogRoll;
            if (Uri.TryCreate(
                String.Concat(Utils.AbsoluteWebRoot.ToString().TrimEnd('/'), "/opml.axd"),
                UriKind.RelativeOrAbsolute,
                out blogRoll))
            {
                writer.WriteElementString(
                    "blogChannel", "blogRoll", "http://backend.userland.com/blogChannelModule", blogRoll.ToString());
            }

            if (!String.IsNullOrEmpty(this.Settings.Endorsement))
            {
                Uri blink;
                if (Uri.TryCreate(this.Settings.Endorsement, UriKind.RelativeOrAbsolute, out blink))
                {
                    writer.WriteElementString(
                        "blogChannel", "blink", "http://backend.userland.com/blogChannelModule", blink.ToString());
                }
            }

            // ------------------------------------------------------------
            // Write Dublin Core syndication extension elements
            // ------------------------------------------------------------
            if (!String.IsNullOrEmpty(this.Settings.AuthorName))
            {
                writer.WriteElementString("dc", "creator", "http://purl.org/dc/elements/1.1/", this.Settings.AuthorName);
            }

            if (!String.IsNullOrEmpty(this.Settings.Description))
            {
                writer.WriteElementString(
                    "dc", "description", "http://purl.org/dc/elements/1.1/", this.Settings.Description);
            }

            if (!String.IsNullOrEmpty(this.Settings.Language))
            {
                writer.WriteElementString("dc", "language", "http://purl.org/dc/elements/1.1/", this.Settings.Language);
            }

            if (!String.IsNullOrEmpty(this.Settings.Name))
            {
                writer.WriteElementString("dc", "title", "http://purl.org/dc/elements/1.1/", this.Settings.Name);
            }

            // ------------------------------------------------------------
            // Write basic geo-coding syndication extension elements
            // ------------------------------------------------------------
            var decimalFormatInfo = new NumberFormatInfo { NumberDecimalDigits = 6 };

            if (this.Settings.GeocodingLatitude != Single.MinValue)
            {
                writer.WriteElementString(
                    "geo",
                    "lat",
                    "http://www.w3.org/2003/01/geo/wgs84_pos#",
                    this.Settings.GeocodingLatitude.ToString("N", decimalFormatInfo));
            }

            if (this.Settings.GeocodingLongitude != Single.MinValue)
            {
                writer.WriteElementString(
                    "geo",
                    "long",
                    "http://www.w3.org/2003/01/geo/wgs84_pos#",
                    this.Settings.GeocodingLongitude.ToString("N", decimalFormatInfo));
            }
        }

        /// <summary>
        /// Writes a generated Atom syndication feed to the specified <see cref="Stream"/> using the supplied collection.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which you want to write the syndication feed.
        /// </param>
        /// <param name="publishables">
        /// The collection of <see cref="IPublishable"/> objects used to generate the syndication feed content.
        /// </param>
        /// <param name="title">
        /// The title of the ATOM content.
        /// </param>
        private void WriteAtomFeed(Stream stream, List<IPublishable> publishables, string title)
        {
            var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            // ------------------------------------------------------------
            // Create writer against stream using defined settings
            // ------------------------------------------------------------
            using (var writer = XmlWriter.Create(stream, writerSettings))
            {
                writer.WriteStartElement("feed", "http://www.w3.org/2005/Atom");

                // writer.WriteAttributeString("version", "1.0");

                // ------------------------------------------------------------
                // Write XML namespaces used to support syndication extensions
                // ------------------------------------------------------------
                foreach (var prefix in SupportedNamespaces.Keys)
                {
                    writer.WriteAttributeString("xmlns", prefix, null, SupportedNamespaces[prefix]);
                }

                // ------------------------------------------------------------
                // Write feed content
                // ------------------------------------------------------------
                this.WriteAtomContent(writer, publishables, title);

                writer.WriteFullEndElement();
            }
        }

        /// <summary>
        /// Writes the RSS channel element information to the specified <see cref="XmlWriter"/> using the supplied collection.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write channel element information to.
        /// </param>
        /// <param name="publishables">
        /// The collection of <see cref="IPublishable"/> objects used to generate syndication content.
        /// </param>
        /// <param name="title">
        /// The title of the RSS channel.
        /// </param>
        private void WriteRssChannel(XmlWriter writer, IEnumerable<IPublishable> publishables, string title)
        {
            writer.WriteStartElement("channel");

            writer.WriteElementString("title", title);
            writer.WriteElementString("description", this.Settings.Description);
            writer.WriteElementString("link", Utils.AbsoluteWebRoot.ToString());

            // ------------------------------------------------------------
            // Write common/shared channel elements
            // ------------------------------------------------------------
            this.WriteRssChannelCommonElements(writer);

            foreach (var publishable in publishables.Where(publishable => publishable.IsVisible))
            {
                WriteRssItem(writer, publishable);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the common/shared RSS channel element information to the specified <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write channel element information to.
        /// </param>
        private void WriteRssChannelCommonElements(XmlWriter writer)
        {
            // ------------------------------------------------------------
            // Write optional channel elements
            // ------------------------------------------------------------
            
            // var url = Utils.FeedUrl;
            // if (HttpContext.Current != null)
            // {
            //     url = HttpContext.Current.Request.Url.ToString();
            // }
            writer.WriteElementString("docs", "http://www.rssboard.org/rss-specification");
            writer.WriteElementString("generator", string.Format("BlogEngine.NET {0}", BlogSettings.Instance.Version()));

            // writer.WriteRaw("\n<atom:link href=\"" + url + "\" rel=\"self\" type=\"application/rss+xml\" />");
            if (!String.IsNullOrEmpty(this.Settings.Language))
            {
                writer.WriteElementString("language", this.Settings.Language);
            }

            // ------------------------------------------------------------
            // Write blogChannel syndication extension elements
            // ------------------------------------------------------------
            Uri blogRoll;
            if (Uri.TryCreate(
                String.Concat(Utils.AbsoluteWebRoot.ToString().TrimEnd('/'), "/opml.axd"),
                UriKind.RelativeOrAbsolute,
                out blogRoll))
            {
                writer.WriteElementString(
                    "blogChannel", "blogRoll", "http://backend.userland.com/blogChannelModule", blogRoll.ToString());
            }

            if (!String.IsNullOrEmpty(this.Settings.Endorsement))
            {
                Uri blink;
                if (Uri.TryCreate(this.Settings.Endorsement, UriKind.RelativeOrAbsolute, out blink))
                {
                    writer.WriteElementString(
                        "blogChannel", "blink", "http://backend.userland.com/blogChannelModule", blink.ToString());
                }
            }

            // ------------------------------------------------------------
            // Write Dublin Core syndication extension elements
            // ------------------------------------------------------------
            if (!String.IsNullOrEmpty(this.Settings.AuthorName))
            {
                writer.WriteElementString("dc", "creator", "http://purl.org/dc/elements/1.1/", this.Settings.AuthorName);
            }

            // if (!String.IsNullOrEmpty(this.Settings.Description))
            // {
            // writer.WriteElementString("dc", "description", "http://purl.org/dc/elements/1.1/", this.Settings.Description);
            // }
            if (!String.IsNullOrEmpty(this.Settings.Name))
            {
                writer.WriteElementString("dc", "title", "http://purl.org/dc/elements/1.1/", this.Settings.Name);
            }

            // ------------------------------------------------------------
            // Write basic geo-coding syndication extension elements
            // ------------------------------------------------------------
            var decimalFormatInfo = new NumberFormatInfo { NumberDecimalDigits = 6 };

            if (this.Settings.GeocodingLatitude != Single.MinValue)
            {
                writer.WriteElementString(
                    "geo",
                    "lat",
                    "http://www.w3.org/2003/01/geo/wgs84_pos#",
                    this.Settings.GeocodingLatitude.ToString("N", decimalFormatInfo));
            }

            if (this.Settings.GeocodingLongitude != Single.MinValue)
            {
                writer.WriteElementString(
                    "geo",
                    "long",
                    "http://www.w3.org/2003/01/geo/wgs84_pos#",
                    this.Settings.GeocodingLongitude.ToString("N", decimalFormatInfo));
            }
        }

        /// <summary>
        /// Writes a generated RSS syndication feed to the specified <see cref="Stream"/> using the supplied collection.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which you want to write the syndication feed.
        /// </param>
        /// <param name="publishables">
        /// The collection of <see cref="IPublishable"/> objects used to generate the syndication feed content.
        /// </param>
        /// <param name="title">
        /// The title of the RSS channel.
        /// </param>
        private void WriteRssFeed(Stream stream, IEnumerable<IPublishable> publishables, string title)
        {
            var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            // ------------------------------------------------------------
            // Create writer against stream using defined settings
            // ------------------------------------------------------------
            using (var writer = XmlWriter.Create(stream, writerSettings))
            {
                writer.WriteStartElement("rss");
                writer.WriteAttributeString("version", "2.0");

                // ------------------------------------------------------------
                // Write XML namespaces used to support syndication extensions
                // ------------------------------------------------------------
                foreach (var prefix in SupportedNamespaces.Keys)
                {
                    writer.WriteAttributeString("xmlns", prefix, null, SupportedNamespaces[prefix]);
                }

                // ------------------------------------------------------------
                // Write <channel> element
                // ------------------------------------------------------------
                this.WriteRssChannel(writer, publishables, title);

                writer.WriteFullEndElement();
            }
        }

        #endregion
    }
}