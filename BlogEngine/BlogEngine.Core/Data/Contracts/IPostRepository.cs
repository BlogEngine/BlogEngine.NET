using System;
using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Posts repository
    /// </summary>
    public interface IPostRepository
    {
        /// <summary>
        /// Post list
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns>List of posts</returns>
        IEnumerable<PostItem> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Get single post
        /// </summary>
        /// <param name="id">Post id</param>
        /// <returns>Post object</returns>
        PostDetail FindById(Guid id);
        /// <summary>
        /// Add new post
        /// </summary>
        /// <param name="post">Post</param>
        /// <returns>Saved post with new ID</returns>
        PostDetail Add(PostDetail post);
        /// <summary>
        /// Update post
        /// </summary>
        /// <param name="post">Post to update</param>
        /// <returns>True on success</returns>
        bool Update(PostDetail post, string action);
        /// <summary>
        /// Delete post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>True on success</returns>
        bool Remove(Guid id);
    }
}