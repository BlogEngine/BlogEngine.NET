namespace BlogEngine.Core
{
    using System;

    /// <summary>
    /// Blog roll item
    /// </summary>
    public class BlogRollItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "BlogRollItem" /> class.
        /// </summary>
        public BlogRollItem()
        {
            Id = Guid.NewGuid();
            Title = "";
            Description = "";
            Xfn = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogRollItem"/> class.
        /// </summary>
        /// <param name="title">
        /// The title of the BlogRollItem.
        /// </param>
        /// <param name="description">
        /// The description of the BlogRollItem.
        /// </param>
        /// <param name="blogUrl">
        /// The <see cref="Uri"/> of the BlogRollItem.
        /// </param>
        public BlogRollItem(string title, string description, Uri blogUrl)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            BlogUrl = blogUrl;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Item ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the BlogUrl of the object.
        /// </summary>
        public Uri BlogUrl { get; set; }

        /// <summary>
        ///     Gets or sets the Description of the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the FeedUrl of the object.
        /// </summary>
        public Uri FeedUrl { get; set; }

        /// <summary>
        ///     Gets or sets the SortIndex of the object.
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        ///     Gets or sets the Title of the object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the Xfn of the object.
        /// </summary>
        public string Xfn { get; set; }

        #endregion
    }
}