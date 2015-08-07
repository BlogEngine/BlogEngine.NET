using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Xml;
using System.Web.Hosting;
using System.Globalization;

namespace BlogEngine.Core.Providers
{
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// Deletes a Blog
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public override void DeleteBlog(Blog blog)
        {
            this.WriteBlogsFile(Blog.Blogs.Where(b => b.Id != blog.Id).ToList());
        }

        /// <summary>
        /// Deletes the blog's storage container.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public override bool DeleteBlogStorageContainer(Blog blog)
        {
            return blog.DeleteBlogFolder();
        }

        /// <summary>
        /// Fills an unsorted list of Blogs.
        /// </summary>
        /// <returns>
        /// A List&lt;Blog&gt; of all Blogs
        /// </returns>
        public override List<Blog> FillBlogs()
        {
            var blogs = new List<Blog>();

            // we want the folder of the primary blog instance, as "Blogs" is site-wide, not per-blog.
            // pass null to GetFolder() to get the base storage location.
            var fileName = this.GetFolder(null) + "blogs.xml";
            if (!File.Exists(fileName))
            {
                return blogs;
            }

            var doc = new XmlDocument();
            doc.Load(fileName);

            var nodes = doc.SelectNodes("blogs/item");
            if (nodes != null)
            {

                foreach (XmlNode node in nodes)
                {
                    var b = new Blog
                    {
                        Id = node.Attributes["id"] == null ? Guid.NewGuid() : new Guid(node.Attributes["id"].Value),
                        Name = node.Attributes["name"] == null ? string.Empty : node.Attributes["name"].Value,
                        StorageContainerName = node.Attributes["storageContainerName"] == null ? string.Empty : node.Attributes["storageContainerName"].Value,
                        Hostname = node.Attributes["hostName"] == null ? string.Empty : node.Attributes["hostName"].Value,
                        IsAnyTextBeforeHostnameAccepted = node.Attributes["isAnyTextBeforeHostnameAccepted"] == null ? false : bool.Parse(node.Attributes["isAnyTextBeforeHostnameAccepted"].Value),
                        VirtualPath = node.Attributes["virtualPath"] == null ? string.Empty : node.Attributes["virtualPath"].Value,
                        IsPrimary = node.Attributes["isPrimary"] == null ? false : bool.Parse(node.Attributes["isPrimary"].Value),
                        IsActive = node.Attributes["isActive"] == null ? false : bool.Parse(node.Attributes["isActive"].Value),
                        IsSiteAggregation = node.Attributes["isSiteAggregation"] == null ? false : bool.Parse(node.Attributes["isSiteAggregation"].Value)
                    };

                    blogs.Add(b);
                    b.MarkOld();   
                }
            }

            return blogs;
        }

        /// <summary>
        /// Inserts a Blog
        /// </summary>
        /// <param name="blog">
        /// The Blog.
        /// </param>
        public override void InsertBlog(Blog blog)
        {
            this.WriteBlogsFile(new List<Blog>(Blog.Blogs).Concat(new List<Blog>() { blog } ).ToList());
        }

        /// <summary>
        /// Gets a Blog based on a Guid.
        /// </summary>
        /// <param name="id">
        /// The Blog's Guid.
        /// </param>
        /// <returns>
        /// A matching Blog
        /// </returns>
        public override Blog SelectBlog(Guid id)
        {
            var blog = Blog.Blogs.Find(b => b.Id == id) ?? new Blog() { Id = id };

            blog.MarkOld();
            return blog;
        }

        /// <summary>
        /// Updates a Blog
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public override void UpdateBlog(Blog blog)
        {
            var blogs = Blog.Blogs;
            this.WriteBlogsFile(blogs);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves blogs to a file.
        /// </summary>
        /// <param name="blogs">
        /// The blogs.
        /// </param>
        private void WriteBlogsFile(List<Blog> blogs)
        {
            // pass null to GetFolder() to get the base storage location.
            var fileName = this.GetFolder(null) + "blogs.xml";

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("blogs");

                foreach (var b in blogs)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", b.Id.ToString());
                    writer.WriteAttributeString("name", (b.Name ?? string.Empty).Trim());
                    writer.WriteAttributeString("hostName", (b.Hostname ?? string.Empty).Trim());
                    writer.WriteAttributeString("isAnyTextBeforeHostnameAccepted", b.IsAnyTextBeforeHostnameAccepted.ToString());
                    writer.WriteAttributeString("storageContainerName", (b.StorageContainerName ?? string.Empty).Trim());
                    writer.WriteAttributeString("virtualPath", (b.VirtualPath ?? string.Empty).Trim());
                    writer.WriteAttributeString("isPrimary", b.IsPrimary.ToString());
                    writer.WriteAttributeString("isActive", b.IsActive.ToString());
                    writer.WriteAttributeString("isSiteAggregation", b.IsSiteAggregation.ToString());

                    writer.WriteEndElement();
                    b.MarkOld();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Sets up the required storage files/tables for a new Blog instance, from an existing blog instance.
        /// </summary>
        /// <param name="existingBlog">
        /// The existing blog to copy from.
        /// </param>
        /// <param name="newBlog">
        /// The new blog to copy to.
        /// </param>
        public override bool SetupBlogFromExistingBlog(Blog existingBlog, Blog newBlog)
        {
            bool copyResult = newBlog.CopyExistingBlogFolderToNewBlogFolder(existingBlog);

            // All we need to do for the XmlBlogProvider is to copy the folders, as done above.

            return copyResult;
        }

        /// <summary>
        /// Setup new blog
        /// </summary>
        /// <param name="newBlog">New blog</param>
        /// <param name="userName">User name</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns>True if successful</returns>
        public override bool SetupNewBlog(Blog newBlog, string userName, string email, string password)
        {
            return BlogGenerator.CopyTemplateBlogFolder(newBlog.Name, userName, email, password);
        }

        #endregion
    }
}
