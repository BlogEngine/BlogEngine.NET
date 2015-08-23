namespace BlogEngine.Core
{
    using System;

    /// <summary>
    /// The location where the serving takes place
    /// </summary>
    public enum ServingLocation
    {
        /// <summary>
        ///     Is used to indicate that a location hasn't been chosen.
        /// </summary>
        None,

        /// <summary>
        ///     Is used when a single post is served from post.aspx.
        /// </summary>
        SinglePost,

        /// <summary>
        ///     Is used when multiple posts are served using postlist.ascx.
        /// </summary>
        PostList,

        /// <summary>
        ///     Is used when a single page is displayed on page.aspx.
        /// </summary>
        SinglePage,

        /// <summary>
        ///     Is used when content is served from a feed (RSS or ATOM).
        /// </summary>
        Feed,

        /// <summary>
        ///     Is used when content is being sent by e-mail.
        /// </summary>
        Email,

        /// <summary>
        ///     Is used when content is served on a custom location.
        /// </summary>
        Other
    }

    /// <summary>
    /// The criteria by which the content is being served (by tag, category, author, etc)
    /// </summary>
    public enum ServingContentBy
    {
        /// <summary>
        ///     The criteria by which content is being served is unspecified.
        /// </summary>
        Unspecified,

        /// <summary>
        ///     All content is being served.
        /// </summary>
        AllContent,

        /// <summary>
        ///     Content is being served by tag.
        /// </summary>
        Tag,

        /// <summary>
        ///     Content is being served by category.
        /// </summary>
        Category,

        /// <summary>
        ///     Content is being served by author.
        /// </summary>
        Author,

        /// <summary>
        ///     Content is being served by date range.
        /// </summary>
        DateRange,

        /// <summary>
        ///     Content is being served by APML filtering.
        /// </summary>
        Apml
    }

    /// <summary>
    /// Used when a post is served to the output stream.
    /// </summary>
    public class ServingEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServingEventArgs"/> class. 
        ///     Creates a new instance of the class and applies the specified body and serving location.
        /// </summary>
        /// <param name="body">
        /// The body string.
        /// </param>
        /// <param name="location">
        /// The location.
        /// </param>
        public ServingEventArgs(string body, ServingLocation location)
            : this(body, location, ServingContentBy.Unspecified)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServingEventArgs"/> class. 
        ///     Creates a new instance of the class and applies the specified body, serving location and serving content by.
        /// </summary>
        /// <param name="body">
        /// The body string.
        /// </param>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <param name="contentBy">
        /// The content By.
        /// </param>
        public ServingEventArgs(string body, ServingLocation location, ServingContentBy contentBy)
        {
            this.Body = body;
            this.Location = location;
            this.ContentBy = contentBy;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the body of the post. If you change the Body, 
        ///     then that change will be shown on the web page.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to cancel the serving of the content.
        ///     <remarks>
        ///         If the serving is cancelled then the item will not be displayed.
        ///     </remarks>
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///     Gets or sets the criteria by which the content is being served (by tag, category, author, etc).
        /// </summary>
        public ServingContentBy ContentBy { get; set; }

        /// <summary>
        ///     Gets or sets the location where the serving takes place.
        /// </summary>
        public ServingLocation Location { get; set; }

        #endregion
    }
}