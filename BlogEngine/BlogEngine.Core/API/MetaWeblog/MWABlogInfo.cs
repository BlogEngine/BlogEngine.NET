namespace BlogEngine.Core.API.MetaWeblog
{
    /// <summary>
    /// MetaWeblog BlogInfo struct
    ///     returned as an array from getUserBlogs
    /// </summary>
    internal struct MWABlogInfo
    {
        #region Constants and Fields

        /// <summary>
        ///     Blog ID (Since BlogEngine.NET is single instance this number is always 10.
        /// </summary>
        public string blogID;

        /// <summary>
        ///     Blog Title
        /// </summary>
        public string blogName;

        /// <summary>
        ///     Blog Url
        /// </summary>
        public string url;

        #endregion
    }
}