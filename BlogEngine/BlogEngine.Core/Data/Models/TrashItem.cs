using System;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Trash item
    /// </summary>
    public class TrashItem
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        ///     Deleted item ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Deleted item title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Relative URL
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        ///     Type of deleted object
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Date created (deleted)
        /// </summary>
        public string DateCreated { get; set; }
    }
}
