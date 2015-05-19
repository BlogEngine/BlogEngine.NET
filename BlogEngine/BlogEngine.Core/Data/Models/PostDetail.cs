using System;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Blog post detail
    /// </summary>
    public class PostDetail
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// Post ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Post title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Post author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Post content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        ///     Gets or sets the date portion of published date
        /// </summary>
        public string DateCreated { get; set; }
        /// <summary>
        /// Slub
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// Relative link
        /// </summary>
        public string RelativeLink { get; set; }
        /// <summary>
        /// Comma separated list of post categories
        /// </summary>
        public List<CategoryItem> Categories { get; set; }
        /// <summary>
        /// Comma separated list of post tags
        /// </summary>
        public List<TagItem> Tags { get; set; }
        /// <summary>
        /// Comment counts for the post
        /// </summary>
        public string[] Comments { get; set; }
        /// <summary>
        /// Post comments enabled
        /// </summary>
        public bool HasCommentsEnabled { get; set; }
        /// <summary>
        /// Gets or sets post status
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// If post marked for deletion
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// If the current user can delete this page.
        /// </summary>
        public bool CanUserDelete { get; set; }
        /// <summary>
        /// If the current user can edit this page.
        /// </summary>
        public bool CanUserEdit { get; set; }
    }
}
