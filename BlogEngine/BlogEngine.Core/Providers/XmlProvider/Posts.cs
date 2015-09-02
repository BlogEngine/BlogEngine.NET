namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// A storage provider for BlogEngine that uses XML files.
    ///     <remarks>
    /// To build another provider, you can just copy and modify
    ///         this one. Then add it to the web.config's BlogEngine section.
    ///     </remarks>
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Properties

        /// <summary>
        ///     Gets the storage folder of the current blog instance.
        /// </summary>
        internal string Folder
        {
            get
            {
                return GetFolder(Blog.CurrentInstance);
            }
        }

        /// <summary>
        ///     Gets the storage folder for the blog.
        /// </summary>
        internal string GetFolder(Blog blog)
        {
            // if "blog" == null, this means it's the primary instance being asked for -- which
            // is in the root of BlogConfig.StorageLocation.
            string location = blog == null ? BlogConfig.StorageLocation : blog.StorageLocation;
            var p = location.Replace("~/", string.Empty);
            return Path.Combine(HttpRuntime.AppDomainAppPath, p);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes a post from the data store.
        /// </summary>
        /// <param name="post">
        /// The post to delete.
        /// </param>
        public override void DeletePost(Post post)
        {
            var fileName = string.Format("{0}posts{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, post.Id);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Retrieves all posts from the data store
        /// </summary>
        /// <returns>
        /// List of Posts
        /// </returns>
        public override List<Post> FillPosts()
        {
            var folder = this.Folder + "posts" + Path.DirectorySeparatorChar;
            if (Directory.Exists(folder))
            {
                var posts = (from file in Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly)
                             select new FileInfo(file)
                             into info
                             select info.Name.Replace(".xml", string.Empty)
                             into id
                             select Post.Load(new Guid(id))).ToList();

                posts.Sort();
                return posts;
            }

            return new List<Post>();
        }

        /// <summary>
        /// Inserts a new Post to the data store.
        /// </summary>
        /// <param name="post">
        /// The post to insert.
        /// </param>
        public override void InsertPost(Post post)
        {
            if (!Directory.Exists(string.Format("{0}posts", this.Folder)))
                Directory.CreateDirectory(string.Format("{0}posts", this.Folder));

            var fileName = string.Format("{0}posts{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, post.Id);
            var settings = new XmlWriterSettings { Indent = true };

            var ms = new MemoryStream();

            using (var writer = XmlWriter.Create(ms, settings))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("post");

                var x = post.DateCreated;
                var b = new DateTime();
                var c = x == b;

                writer.WriteElementString("author", post.Author);
                writer.WriteElementString("title", post.Title);
                writer.WriteElementString("description", post.Description);
                writer.WriteElementString("content", post.Content);
                writer.WriteElementString("ispublished", post.IsPublished.ToString());
                writer.WriteElementString("isdeleted", post.IsDeleted.ToString());
                writer.WriteElementString("iscommentsenabled", post.HasCommentsEnabled.ToString());

                writer.WriteElementString("pubDate", BlogSettings.Instance.ServerTime(post.DateCreated).
                    ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

                writer.WriteElementString("lastModified", BlogSettings.Instance.ServerTime(post.DateModified).
                    ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

                writer.WriteElementString("raters", post.Raters.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("rating", post.Rating.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("slug", post.Slug);

                // Tags
                writer.WriteStartElement("tags");
                foreach (var tag in post.Tags)
                {
                    writer.WriteElementString("tag", tag);
                }

                writer.WriteEndElement();

                // comments
                writer.WriteStartElement("comments");
                foreach (var comment in post.AllComments)
                {
                    writer.WriteStartElement("comment");
                    writer.WriteAttributeString("id", comment.Id.ToString());
                    writer.WriteAttributeString("parentid", comment.ParentId.ToString());
                    writer.WriteAttributeString("approved", comment.IsApproved.ToString());
                    writer.WriteAttributeString("spam", comment.IsSpam.ToString());
                    writer.WriteAttributeString("deleted", comment.IsDeleted.ToString());

                    writer.WriteElementString("date", BlogSettings.Instance.ServerTime(comment.DateCreated)
                        .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

                    writer.WriteElementString("author", comment.Author);
                    writer.WriteElementString("email", comment.Email);
                    writer.WriteElementString("country", comment.Country);
                    writer.WriteElementString("ip", comment.IP);

                    if (comment.Website != null)
                    {
                        writer.WriteElementString("website", comment.Website.ToString());
                    }

                    if (!string.IsNullOrEmpty(comment.ModeratedBy))
                    {
                        writer.WriteElementString("moderatedby", comment.ModeratedBy);
                    }

                    if (comment.Avatar != null)
                    {
                        writer.WriteElementString("avatar", comment.Avatar);
                    }

                    writer.WriteElementString("content", comment.Content);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                // categories
                writer.WriteStartElement("categories");
                foreach (var cat in post.Categories)
                {
                    // if (cat.Id = .Instance.ContainsKey(key))
                    // writer.WriteElementString("category", key.ToString());
                    writer.WriteElementString("category", cat.Id.ToString());
                }

                writer.WriteEndElement();

                // Notification e-mails
                writer.WriteStartElement("notifications");
                foreach (var email in post.NotificationEmails)
                {
                    writer.WriteElementString("email", email);
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                ms.WriteTo(fs);
                ms.Dispose();
            }
        }

        /// <summary>
        /// Retrieves a Post from the provider based on the specified id.
        /// </summary>
        /// <param name="id">
        /// The Post id.
        /// </param>
        /// <returns>
        /// A Post object.
        /// </returns>
        public override Post SelectPost(Guid id)
        {
            var fileName = string.Format("{0}posts{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, id);
            var post = new Post();
            var doc = new XmlDocument();
            doc.Load(fileName);

            post.Title = doc.SelectSingleNode("post/title").InnerText;
            post.Description = doc.SelectSingleNode("post/description").InnerText;
            post.Content = doc.SelectSingleNode("post/content").InnerText;

            if (doc.SelectSingleNode("post/pubDate") != null)
            {
                post.DateCreated = DateTime.Parse(
                    doc.SelectSingleNode("post/pubDate").InnerText, CultureInfo.InvariantCulture);

                post.DateCreated = BlogSettings.Instance.ClientTime(post.DateCreated);
            }

            if (doc.SelectSingleNode("post/lastModified") != null)
            {
                post.DateModified = DateTime.Parse(
                    doc.SelectSingleNode("post/lastModified").InnerText, CultureInfo.InvariantCulture);

                post.DateModified = BlogSettings.Instance.ClientTime(post.DateModified);
            }

            if (doc.SelectSingleNode("post/author") != null)
            {
                post.Author = doc.SelectSingleNode("post/author").InnerText;
            }

            if (doc.SelectSingleNode("post/ispublished") != null)
            {
                post.IsPublished = bool.Parse(doc.SelectSingleNode("post/ispublished").InnerText);
            }

            if (doc.SelectSingleNode("post/isdeleted") != null)
            {
                post.IsDeleted = bool.Parse(doc.SelectSingleNode("post/isdeleted").InnerText);
            }

            if (doc.SelectSingleNode("post/iscommentsenabled") != null)
            {
                post.HasCommentsEnabled = bool.Parse(doc.SelectSingleNode("post/iscommentsenabled").InnerText);
            }

            if (doc.SelectSingleNode("post/raters") != null)
            {
                post.Raters = int.Parse(doc.SelectSingleNode("post/raters").InnerText, CultureInfo.InvariantCulture);
            }

            if (doc.SelectSingleNode("post/rating") != null)
            {
                post.Rating = float.Parse(
                    doc.SelectSingleNode("post/rating").InnerText, CultureInfo.GetCultureInfo("en-gb"));
            }

            if (doc.SelectSingleNode("post/slug") != null)
            {
                post.Slug = doc.SelectSingleNode("post/slug").InnerText;
            }

            // Tags
            foreach (var node in
                doc.SelectNodes("post/tags/tag").Cast<XmlNode>().Where(node => !string.IsNullOrEmpty(node.InnerText)))
            {
                post.Tags.Add(node.InnerText);
            }

            // comments
            foreach (XmlNode node in doc.SelectNodes("post/comments/comment"))
            {
                var comment = new Comment
                    {
                        Id = new Guid(node.Attributes["id"].InnerText), 
                        ParentId =
                            (node.Attributes["parentid"] != null)
                                ? new Guid(node.Attributes["parentid"].InnerText)
                                : Guid.Empty, 
                        Author = node.SelectSingleNode("author").InnerText, 
                        Email = node.SelectSingleNode("email").InnerText, 
                        Parent = post
                    };

                if (node.SelectSingleNode("country") != null)
                {
                    comment.Country = node.SelectSingleNode("country").InnerText;
                }

                if (node.SelectSingleNode("ip") != null)
                {
                    comment.IP = node.SelectSingleNode("ip").InnerText;
                }

                if (node.SelectSingleNode("website") != null)
                {
                    Uri website;
                    if (Uri.TryCreate(node.SelectSingleNode("website").InnerText, UriKind.Absolute, out website))
                    {
                        comment.Website = website;
                    }
                }

                if (node.SelectSingleNode("moderatedby") != null)
                {
                    comment.ModeratedBy = node.SelectSingleNode("moderatedby").InnerText;
                }

                comment.IsApproved = node.Attributes["approved"] == null ||
                                     bool.Parse(node.Attributes["approved"].InnerText);

                if (node.SelectSingleNode("avatar") != null)
                {
                    comment.Avatar = node.SelectSingleNode("avatar").InnerText;
                }

                comment.IsSpam = node.Attributes["spam"] == null ? false : 
                                    bool.Parse(node.Attributes["spam"].InnerText);

                comment.IsDeleted = node.Attributes["deleted"] == null ? false : 
                                    bool.Parse(node.Attributes["deleted"].InnerText);

                comment.Content = node.SelectSingleNode("content").InnerText;

                comment.DateCreated = DateTime.Parse(
                    node.SelectSingleNode("date").InnerText, CultureInfo.InvariantCulture);

                comment.DateCreated = BlogSettings.Instance.ClientTime(comment.DateCreated);

                post.AllComments.Add(comment);
            }

            post.AllComments.Sort();

            // categories
            foreach (var cat in from XmlNode node in doc.SelectNodes("post/categories/category")
                                select new Guid(node.InnerText)
                                into key select Category.GetCategory(key)
                                into cat where cat != null select cat)
            {
                // CategoryDictionary.Instance.ContainsKey(key))
                post.Categories.Add(cat);
            }

            // Notification e-mails
            foreach (XmlNode node in doc.SelectNodes("post/notifications/email"))
            {
                post.NotificationEmails.Add(node.InnerText);
            }

            return post;
        }

        /// <summary>
        /// Updates an existing Post in the data store specified by the provider.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        public override void UpdatePost(Post post)
        {
            this.InsertPost(post);
        }

        #endregion
    }
}