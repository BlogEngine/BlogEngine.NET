using System;

namespace BlogEngine.Core.Data.Models
{
    public class CommentFilterItem
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets the Comment Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Block, delete etc.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Email, IP etc.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Equals, contains etc.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Content of the filter, like email or IP address
        /// </summary>
        public string Filter { get; set; }
    }
}
