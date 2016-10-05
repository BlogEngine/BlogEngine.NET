using System;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Category repository
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Find items in collection
        /// </summary>
        /// <param name="take">Items per page, default 10, 0 to return all</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter, for example filter=IsPublished,true,Author,Admin</param>
        /// <param name="order">Sort order, for example order=DateCreated,desc</param>
        /// <returns>List of items</returns>
        IEnumerable<Data.Models.CategoryItem> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Get single item
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns>Object</returns>
        Data.Models.CategoryItem FindById(Guid id);
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="item">Post</param>
        /// <returns>Saved item with new ID</returns>
        Data.Models.CategoryItem Add(Data.Models.CategoryItem item);
        /// <summary>
        /// Update post
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <returns>True on success</returns>
        bool Update(Data.Models.CategoryItem item);
        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>True on success</returns>
        bool Remove(Guid id);
    }
}
