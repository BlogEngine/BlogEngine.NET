using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Tag repository
    /// </summary>
    public interface ITagRepository
    {
        /// <summary>
        /// Get tag list
        /// </summary>
        /// <param name="take">Items per page, default 10, 0 to return all</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="postId">Post id</param>
        /// <param name="order">Sort order, for example order=DateCreated,desc</param>
        /// <returns>List of items</returns>
        IEnumerable<Data.Models.TagItem> Find(int take = 10, int skip = 0, string postId = "", string order = "");
        /// <summary>
        /// Updates tag
        /// </summary>
        /// <param name="updateFrom">Value from</param>
        /// <param name="updateTo">Value to</param>
        /// <returns>True on success</returns>
        bool Save(string updateFrom, string updateTo);
        /// <summary>
        /// Delete tag in all posts
        /// </summary>
        /// <param name="id">Tag</param>
        /// <returns>True on success</returns>
        bool Delete(string id);
    }
}
