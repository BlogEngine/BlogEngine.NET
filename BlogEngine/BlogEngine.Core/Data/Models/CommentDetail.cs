using System;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Blog comments
    /// </summary>
    public class CommentDetail
    {
        /// <summary>
        ///     Gets or sets the Comment Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        ///     Parent comment Id
        /// </summary>
        public Guid ParentId { get; set; }
        /// <summary>
        ///     Comment post ID
        /// </summary>
        public Guid PostId { get; set; }
        /// <summary>
        ///     Gets or sets the author's website
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        ///     Gets or sets the ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        ///     Comment content
        /// </summary>
        public string Content { get; set; }
    }
}
