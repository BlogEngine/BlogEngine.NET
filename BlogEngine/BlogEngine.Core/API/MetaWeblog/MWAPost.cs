namespace BlogEngine.Core.API.MetaWeblog
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// MetaWeblog Post struct
    ///     used in newPost, editPost, getPost, recentPosts
    ///     not all properties are used everytime.
    /// </summary>
    internal struct MWAPost
    {
        #region Constants and Fields

        /// <summary>
        ///     wp_author_id
        /// </summary>
        public string author;

        /// <summary>
        ///     List of Categories assigned for Blog Post
        /// </summary>
        public List<string> categories;

        /// <summary>
        ///     CommentPolicy (Allow/Deny)
        /// </summary>
        public string commentPolicy;

        /// <summary>
        ///     Content of Blog Post
        /// </summary>
        public string description;

        /// <summary>
        ///     Excerpt
        /// </summary>
        public string excerpt;

        /// <summary>
        ///     Link to Blog Post
        /// </summary>
        public string link;

        /// <summary>
        ///     Display date of Blog Post (DateCreated)
        /// </summary>
        public DateTime postDate;

        /// <summary>
        ///     PostID Guid in string format
        /// </summary>
        public string postID;

        /// <summary>
        ///     Whether the Post is published or not.
        /// </summary>
        public bool publish;

        /// <summary>
        ///     Slug of post
        /// </summary>
        public string slug;

        /// <summary>
        ///     List of Tags assinged for Blog Post
        /// </summary>
        public List<string> tags;

        /// <summary>
        ///     Title of Blog Post
        /// </summary>
        public string title;

        #endregion
    }
}