using System;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Post item
    /// </summary>
    public class PostItem
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
        /// List of post categories
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
        /// Gets or sets post status
        /// </summary>
        public bool IsPublished { get; set; }
    }
}
