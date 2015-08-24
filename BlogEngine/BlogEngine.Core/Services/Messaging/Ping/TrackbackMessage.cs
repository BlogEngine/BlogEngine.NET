namespace BlogEngine.Core.Ping
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The trackback message.
    /// </summary>
    public class TrackbackMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackbackMessage"/> class.
        /// </summary>
        /// <param name="item">
        /// The publishable item.
        /// </param>
        /// <param name="urlToNotifyTrackback">
        /// The URL to notify trackback.
        /// </param>
        /// <param name="itemUrl">
        /// The item Url.
        /// </param>
        public TrackbackMessage(IPublishable item, Uri urlToNotifyTrackback, Uri itemUrl)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.Title = item.Title;
            this.PostUrl = itemUrl;
            this.Excerpt = item.Title;
            this.BlogName = BlogSettings.Instance.Name;
            this.UrlToNotifyTrackback = urlToNotifyTrackback;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the blog.
        /// </summary>
        /// <value>The name of the blog.</value>
        public string BlogName { get; set; }

        /// <summary>
        ///     Gets or sets the excerpt.
        /// </summary>
        /// <value>The excerpt.</value>
        public string Excerpt { get; set; }

        /// <summary>
        ///     Gets or sets the post URL.
        /// </summary>
        /// <value>The post URL.</value>
        public Uri PostUrl { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the URL to notify trackback.
        /// </summary>
        /// <value>The URL to notify trackback.</value>
        public Uri UrlToNotifyTrackback { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, 
                "title={0}&url={1}&excerpt={2}&blog_name={3}", 
                this.Title, 
                this.PostUrl, 
                this.Excerpt, 
                this.BlogName);
        }

        #endregion
    }
}