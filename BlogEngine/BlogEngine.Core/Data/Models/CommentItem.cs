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
        ///     Parent comment Id
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        ///     Comment post ID
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Is spam
        /// </summary>
        public bool IsSpam { get; set; }

        /// <summary>
        /// Comment pending approval
        /// </summary>
        public bool IsPending { get; set; }

        /// <summary>
        /// Can be: approved, pending, spam, pingback
        /// </summary>
        public string CommentType { get; set; }

        /// <summary>
        ///     Gets or sets the Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Gets the avatar image
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this comment has nested comments
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        ///     Gets or sets the comment title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Comment content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Gets or sets the author's website
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Static avatar image
        /// </summary>
        public string AuthorAvatar { get; set; }

        /// <summary>
        ///     Gets or sets the ip
        /// </summary>
        public string Ip { get; set; }

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
