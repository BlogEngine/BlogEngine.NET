namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Packaging source
    /// </summary>
    public class PackagingSource
    {
        /// <summary>
        /// Feed id
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// Feed title
        /// </summary>
        public virtual string FeedTitle { get; set; }
        /// <summary>
        /// Feed url
        /// </summary>
        public virtual string FeedUrl { get; set; }
    }
}
