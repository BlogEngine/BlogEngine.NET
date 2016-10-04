namespace BlogEngine.Core.API.BlogML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Linq;
    using System.Globalization;

    using global::BlogML;
    using global::BlogML.Xml;

    /// <summary>
    /// Class to validate BlogML data and import it into Blog
    /// </summary>
    public class BlogReader : BaseReader
    {
        #region Constants and Fields

        /// <summary>
        ///     The xml data.
        /// </summary>
        private string xmlData = string.Empty;

        /// <summary>
        ///     The category lookup.
        /// </summary>
        private List<Category> categoryLookup = new List<Category>();

        private List<BlogMlExtendedPost> blogsExtended = new List<BlogMlExtendedPost>();

        #endregion

        #region Properties

        /// <summary>
        /// imported posts counter
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        ///     Sets BlogML data uploaded and saved as string
        /// </summary>
        public string XmlData
        {
            set
            {
                xmlData = value;
            }
        }

        /// <summary>
        ///     Gets an XmlReader that converts BlogML data saved as string into XML stream
        /// </summary>
        private XmlTextReader XmlReader
        {
            get
            {
                var byteArray = Encoding.UTF8.GetBytes(this.xmlData);
                var stream = new MemoryStream(byteArray);
                return new XmlTextReader(stream);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Imports BlogML file into blog
        /// </summary>
        /// <returns>
        /// True if successful
        /// </returns>
        public override bool Import()
        {
            Message = string.Empty;
            var blog = new BlogMLBlog();
            try
            {
                blog = BlogMLSerializer.Deserialize(XmlReader);
            }
            catch (Exception ex)
            {
                Message = $"BlogReader.Import: BlogML could not load with 2.0 specs. {ex.Message}";
                Utils.Log(Message);
                return false;
            }

            try
            {
                LoadFromXmlDocument();

                LoadBlogCategories(blog);

                LoadBlogExtendedPosts(blog);

                LoadBlogPosts();

                Message = $"Imported {PostCount} new posts";
            }
            catch (Exception ex)
            {
                Message = $"BlogReader.Import: {ex.Message}";
                Utils.Log(Message);
                return false;
            }

            return true;
        }

        #endregion

        #region Methods

        private Dictionary<string, Dictionary<string, Guid>> _substitueGuids;
        private Guid GetGuid(string type, string value)
        {
            value = (value ?? string.Empty).Trim();

            // Value might be a GUID, or it could be a simple integer.

            if (!String.IsNullOrWhiteSpace(value) &&
                value.Length == 36)
            {
                return new Guid(value);
            }

            // If we've already retrieved a Guid for a particular type/value, then
            // return the same Guid.  This is in case the type/value is referenced by
            // other objects, we would want to use the same Guid to keep the
            // references correct.

            if (_substitueGuids == null)
            {
                _substitueGuids = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
            }

            if (!_substitueGuids.ContainsKey(type))
                _substitueGuids.Add(type, new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase));

            if (!_substitueGuids[type].ContainsKey(value))
                _substitueGuids[type].Add(value, Guid.NewGuid());

            return _substitueGuids[type][value];
        }

        private T GetAttributeValue<T>(XmlAttribute attr)
        {
            if (attr == null)
                return default(T);

            return (T)Convert.ChangeType(attr.Value, typeof(T));
        }

        private DateTime GetDate(XmlAttribute attr)
        { 
            string value = GetAttributeValue<string>(attr);
            DateTime defaultDate = DateTime.Now;

            DateTime dt = defaultDate;
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (!DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dt = defaultDate;
            }

            return dt;
        }

        private Uri GetUri(string value)
        {
            Uri uri;
            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
                return uri;

            return null;
        }

        /// <summary>
        /// BlogML does not support tags - load directly fro XML
        /// </summary>
        private void LoadFromXmlDocument()
        {
            var doc = new XmlDocument();
            doc.Load(XmlReader);
            var posts = doc.GetElementsByTagName("post");

            foreach (XmlNode post in posts)
            {
                var blogX = new BlogMlExtendedPost();

                if (post.Attributes != null)
                    blogX.PostUrl = GetAttributeValue<string>(post.Attributes["post-url"]);

                if (post.ChildNodes.Count <= 0)
                {
                    blogsExtended.Add(blogX);
                    continue;
                }

                foreach (XmlNode child in post.ChildNodes)
                {
                    if (child.Name == "tags")
                    {
                        foreach (XmlNode tag in child.ChildNodes)
                        {
                            if (tag.Attributes != null)
                            {
                                if (blogX.Tags == null) blogX.Tags = new StateList<string>();
                                blogX.Tags.Add(GetAttributeValue<string>(tag.Attributes["ref"]));
                            }
                        }
                    }

                    if (child.Name == "comments")
                        LoadBlogComments(blogX, child);

                    if (child.Name == "trackbacks")
                        LoadBlogTrackbacks(blogX, child);
                }
                blogsExtended.Add(blogX);
            }
        }

        /// <summary>
        /// Lost post comments from xml file
        /// </summary>
        /// <param name="blogX">extended blog</param>
        /// <param name="child">comments xml node</param>
        private void LoadBlogComments(BlogMlExtendedPost blogX, XmlNode child)
        {
            foreach (XmlNode com in child.ChildNodes)
            {
                if(com.Attributes != null)
                {
                    var c = new Comment
                                {
                                    Id = GetGuid("comment", GetAttributeValue<string>(com.Attributes["id"])),
                                    Author = GetAttributeValue<string>(com.Attributes["user-name"]),
                                    Email = GetAttributeValue<string>(com.Attributes["user-email"]),
                                    ParentId = GetGuid("comment", GetAttributeValue<string>(com.Attributes["parentid"])),
                                    IP = GetAttributeValue<string>(com.Attributes["user-ip"]),
                                    DateCreated = GetDate(com.Attributes["date-created"])
                                };

                    if (!string.IsNullOrEmpty(GetAttributeValue<string>(com.Attributes["user-url"])))
                        c.Website = GetUri(GetAttributeValue<string>(com.Attributes["user-url"]));

                    c.IsApproved = GetAttributeValue<bool>(com.Attributes["approved"]);

                    foreach (XmlNode comNode in com.ChildNodes)
                    {
                        if(comNode.Name == "content")
                        {
                            c.Content = comNode.InnerText;
                        }
                    }
                    if(blogX.Comments == null) blogX.Comments = new List<Comment>();
                    blogX.Comments.Add(c);
                }
            }
        }

        /// <summary>
        /// Lost post trackbacks and pingbacks from xml file
        /// </summary>
        /// <param name="blogX">extended blog</param>
        /// <param name="child">comments xml node</param>
        private void LoadBlogTrackbacks(BlogMlExtendedPost blogX, XmlNode child)
        {
            foreach (XmlNode com in child.ChildNodes)
            {
                if (com.Attributes != null)
                {
                    var c = new Comment
                    {
                        Id = GetGuid("comment", GetAttributeValue<string>(com.Attributes["id"])), 
                        IP = "127.0.0.1",
                        IsApproved = GetAttributeValue<bool>(com.Attributes["approved"]),
                        DateCreated = GetDate(com.Attributes["date-created"])
                    };

                    if (!string.IsNullOrEmpty(GetAttributeValue<string>(com.Attributes["url"])))
                        c.Website = GetUri(GetAttributeValue<string>(com.Attributes["url"]));

                    foreach (XmlNode comNode in com.ChildNodes)
                    {
                        if (comNode.Name == "title")
                        {
                            c.Content = comNode.InnerText;
                        }
                    }

                    c.Email = c.Content.ToLowerInvariant().Contains("pingback") ? "pingback" : "trackback";
                    c.Author = c.Email;

                    if (blogX.Comments == null) blogX.Comments = new List<Comment>();
                    blogX.Comments.Add(c);
                }
            }
        }

        /// <summary>
        /// Load blog categories
        /// </summary>
        /// <param name="blog">BlogML blog</param>
        private void LoadBlogCategories(BlogMLBlog blog)
        {
            foreach (var cat in blog.Categories)
            {
                var c = new Category
                {
                    Id = GetGuid("category", cat.ID),
                    Title = cat.Title,
                    Description = string.IsNullOrEmpty(cat.Description) ? "" : cat.Description,
                    DateCreated = cat.DateCreated,
                    DateModified = cat.DateModified
                };

                if (!string.IsNullOrEmpty(cat.ParentRef) && cat.ParentRef != "0")
                    c.Parent = GetGuid("category", cat.ParentRef);

                categoryLookup.Add(c);

                if (Category.GetCategory(c.Id) == null)
                {
                    c.Save();
                }
            }
        }

        /// <summary>
        /// extended post has all BlogML plus fields not supported
        /// by BlogML like tags. here we assign BlogML post
        /// to extended matching on post URL 
        /// </summary>
        /// <param name="blog">BlogML blog</param>
        private void LoadBlogExtendedPosts(BlogMLBlog blog)
        {
            foreach (var post in blog.Posts)
            {
                if (post.PostType == BlogPostTypes.Normal)
                {
                    BlogMLPost p = post;
                    blogsExtended.Where(b => b.PostUrl == p.PostUrl).FirstOrDefault().BlogPost = post;
                }
            }
        }

        /// <summary>
        /// Loads the blog posts.
        /// </summary>
        private void LoadBlogPosts()
        {
            var bi = new BlogImporter();
            Utils.Log("BlogReader.LoadBlogPosts: Start importing posts");

            foreach (BlogMlExtendedPost extPost in blogsExtended)
            {
                try
                {
                    BlogMlExtendedPost post = extPost;

                    if (extPost.BlogPost.Categories.Count > 0)
                    {
                        for (var i = 0; i < extPost.BlogPost.Categories.Count; i++)
                        {
                            int i2 = i;
                            var cId = GetGuid("category", post.BlogPost.Categories[i2].Ref);

                            foreach (var category in categoryLookup)
                            {
                                if (category.Id == cId)
                                {
                                    if (extPost.Categories == null)
                                        extPost.Categories = new StateList<Category>();

                                    extPost.Categories.Add(category);
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(bi.AddPost(extPost)))
                    {
                        PostCount++;
                    }
                    else
                    {
                        Utils.Log("Post '{0}' has been skipped" + extPost.BlogPost.Title);
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log("BlogReader.LoadBlogPosts: " + ex.Message);
                }
            }
            bi.ForceReload();
            Utils.Log($"BlogReader.LoadBlogPosts: Completed importing {PostCount} posts");
        }

        #endregion
    }
}