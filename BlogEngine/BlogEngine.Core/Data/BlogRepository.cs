using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Hosting;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Blog repository
    /// </summary>
    public class BlogRepository : IBlogRepository
    {
        /// <summary>
        /// Get blog list
        /// </summary>
        /// <param name="take">Number of items to take</param>
        /// <param name="skip">Number of items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <returns>List of blogs</returns>
        public IEnumerable<Models.Blog> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            // sub-blogs not allowed to see other blogs
            if (!(Blog.CurrentInstance.IsPrimary && Security.IsAdministrator))
                throw new UnauthorizedAccessException();

            if (take == 0) take = Blog.Blogs.Count;
            if (string.IsNullOrEmpty(filter)) filter = "1==1";
            if (string.IsNullOrEmpty(order)) order = "Name";

            var items = new List<Models.Blog>();
            var query = Blog.Blogs.AsQueryable().Where(filter);

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                items.Add(ToJson((Blog)item));

            return items;
        }

        /// <summary>
        /// Find blog by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Blog</returns>
        public Models.Blog FindById(Guid id)
        {
            // sub-blogs not allowed to see other blogs
            if (!(Blog.CurrentInstance.IsPrimary && Security.IsAdministrator))
                throw new UnauthorizedAccessException();

            var blog = Blog.Blogs.FirstOrDefault(b => b.Id == id);
            return ToJson(blog);
        }

        /// <summary>
        /// Add new blog
        /// </summary>
        /// <param name="item">Blog item</param>
        /// <returns>Saved blog with new ID</returns>
        public Models.Blog Add(BlogItem item)
        {
            // has to be on primary blog and be an admin
            // or blog allows create new on self registration
            if (!(Blog.CurrentInstance.IsPrimary && (Security.IsAdministrator || BlogSettings.Instance.CreateBlogOnSelfRegistration)))
                throw new UnauthorizedAccessException();

            string message;
            if (!BlogGenerator.ValidateProperties(item.Name, item.UserName, item.Email, out message))
                throw new ApplicationException(message);

            var coreBlog = BlogGenerator.CreateNewBlog(item.Name, item.UserName, item.Email, item.Password, out message);

            if (coreBlog == null || !string.IsNullOrWhiteSpace(message))
                throw new ApplicationException("Failed to create the new blog.");

            return ToJson(coreBlog);
        }

        /// <summary>
        /// Update blog
        /// </summary>
        /// <param name="blog">Blog to update</param>
        /// <returns>True on success</returns>
        public bool Update(Models.Blog blog)
        {
            // sub-blogs not allowed to see other blogs
            if (!(Blog.CurrentInstance.IsPrimary && Security.IsAdministrator))
                throw new UnauthorizedAccessException();
            try
            {
                var coreBlog = Blog.Blogs.FirstOrDefault(b => b.Id == blog.Id);
                return Save(coreBlog, blog);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            // sub-blogs not allowed to see other blogs
            if (!(Blog.CurrentInstance.IsPrimary && Security.IsAdministrator))
                throw new UnauthorizedAccessException();
            try
            {
                var blog = Blog.Blogs.FirstOrDefault(b => b.Id == id);
                blog.Delete();
                blog.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Private methods

        static bool Save(Blog coreBlog, BlogEngine.Core.Data.Models.Blog jsonBlog)
        {
            coreBlog.Id = jsonBlog.Id;
            coreBlog.Name = jsonBlog.Name;
            coreBlog.StorageContainerName = jsonBlog.StorageContainerName;
            coreBlog.Hostname = jsonBlog.Hostname;
            coreBlog.IsAnyTextBeforeHostnameAccepted = jsonBlog.IsAnyTextBeforeHostnameAccepted;
            coreBlog.VirtualPath = jsonBlog.VirtualPath;
            coreBlog.IsActive = jsonBlog.IsActive;
            coreBlog.IsSiteAggregation = jsonBlog.IsSiteAggregation;
            coreBlog.IsPrimary = jsonBlog.IsPrimary;
            //coreBlog.StorageLocation = PhysicalStorageLocation = HostingEnvironment.MapPath(blog.StorageLocation);
            coreBlog.Save();
            return true;
        }

        static BlogEngine.Core.Data.Models.Blog ToJson(Blog blog)
        {
            var jb = new BlogEngine.Core.Data.Models.Blog
            {
                Id = blog.Id,
                Name = blog.Name,
                StorageContainerName = blog.StorageContainerName,
                Hostname = blog.Hostname,
                IsAnyTextBeforeHostnameAccepted = blog.IsAnyTextBeforeHostnameAccepted,
                VirtualPath = blog.VirtualPath,
                IsActive = blog.IsActive,
                IsSiteAggregation = blog.IsSiteAggregation,
                IsPrimary = blog.IsPrimary,
                RelativeWebRoot = blog.RelativeWebRoot,
                AbsoluteWebRoot = blog.AbsoluteWebRoot,
                PhysicalStorageLocation = HostingEnvironment.MapPath(blog.StorageLocation),
                CanUserEdit = blog.CanUserEdit,
                CanUserDelete = blog.CanUserDelete
            };

            return jb;
        }

        #endregion
    }
}