using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Comment filters repository
    /// </summary>
    public interface ICommentFilterRepository
    {
        /// <summary>
        /// Get item list
        /// </summary>
        /// <param name="take">Number of items to take</param>
        /// <param name="skip">Number of items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <returns>List of items</returns>
        IEnumerable<CommentFilterItem> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Find item by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Item</returns>
        CommentFilterItem FindById(Guid id);
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="item">New item</param>
        /// <returns>Saved item with new ID</returns>
        CommentFilterItem Add(CommentFilterItem item);
        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <returns>True on success</returns>
        bool Update(CommentFilterItem item);
        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>True on success</returns>
        bool Remove(Guid id);
        /// <summary>
        /// Remove all
        /// </summary>
        /// <returns>True on success</returns>
        bool RemoveAll();
    }
}
