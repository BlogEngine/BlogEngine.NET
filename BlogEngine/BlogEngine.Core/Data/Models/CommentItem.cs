using System;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Blog comments
    /// </summary>
    public class CommentItem
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        ///     Gets or sets the Comment Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        ///     If comment is pending
        /// </summary>
        public bool IsPending { get; set; }
        /// <summary>
        ///     If comment is approved
        /// </summary>
        public bool IsApproved { get; set; }
        /// <summary>
        ///     Whether comment is spam
        /// </summary>
        public bool IsSpam { get; set; }
        /// <summary>
        ///     Gets or sets the Author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        ///     Author email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        ///     Gets the avatar image
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        ///     Whether this comment has nested comments
        /// </summary>
        public bool HasChildren { get; set; }
        /// <summary>
        ///     Gets or sets the comment title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        ///     Gets or sets the date published
        /// </summary>
        public string DateCreated { get; set; }
        /// <summary>
        /// Comment link
        /// </summary>
        public string RelativeLink { get; set; }
    }
}
