using System;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.ViewModels;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Comments repository
    /// </summary>
    public interface ICommentsRepository
    {
        /// <summary>
        /// Comments view model
        /// </summary>
        /// <returns></returns>
        CommentsVM Get();
        /// <summary>
        /// Comment by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CommentDetail FindById(Guid id);
        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item">Comment</param>
        /// <returns>Comment object</returns>
        CommentItem Add(CommentDetail item);
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
