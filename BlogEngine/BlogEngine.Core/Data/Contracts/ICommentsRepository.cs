using System;
using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Comments repository
    /// </summary>
    public interface ICommentsRepository
    {
        /// <summary>
        /// Comments list
        /// </summary>
        /// <param name="commentType">Comment type</param>
        /// <param name="take">Items to take</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns>List of comments</returns>
        IEnumerable<CommentItem> GetComments(CommentType commentType = CommentType.All, int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Single commnet by ID
        /// </summary>
        /// <param name="id">
        /// Comment id
        /// </param>
        /// <returns>
        /// A JSON Comment
        /// </returns>
        CommentItem FindById(Guid id);
        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item">Comment</param>
        /// <returns>Comment object</returns>
        CommentItem Add(CommentItem item);
        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <param name="action">Action (approve/unapprove)</param>
        /// <returns>True on success</returns>
        bool Update(CommentItem item, string action);
        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>True on success</returns>
        bool Remove(Guid id);
        /// <summary>
        /// Delete all comments
        /// </summary>
        /// <param name="commentType">Pending or spam</param>
        /// <returns>True on success</returns>
        bool DeleteAll(string commentType);
    }
}
