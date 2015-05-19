using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Blog repository
    /// </summary>
    public interface IBlogRepository
    {
        /// <summary>
        /// Get blog list
        /// </summary>
        /// <param name="take">Number of items to take</param>
        /// <param name="skip">Number of items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <returns>List of blogs</returns>
        IEnumerable<BlogEngine.Core.Data.Models.Blog> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Find blog by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Blog</returns>
        BlogEngine.Core.Data.Models.Blog FindById(Guid id);
        /// <summary>
        /// Add new blog
        /// </summary>
        /// <param name="item">Blog item</param>
        /// <returns>Saved blog with new ID</returns>
        BlogEngine.Core.Data.Models.Blog Add(BlogItem item);
        /// <summary>
        /// Update blog
        /// </summary>
        /// <param name="blog">Blog to update</param>
        /// <returns>True on success</returns>
        bool Update(BlogEngine.Core.Data.Models.Blog blog);
        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>True on success</returns>
        bool Remove(Guid id);
    }
}
