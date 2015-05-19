namespace BlogEngine.Core.API.MetaWeblog
{
    using System;

    /// <summary>
    /// wp Page Struct
    /// </summary>
    internal struct MWAPage
    {
        #region Constants and Fields

        /// <summary>
        ///     Content of Blog Post
        /// </summary>
        public string description;

        /// <summary>
        ///     Link to Blog Post
        /// </summary>
        public string link;

        /// <summary>
        ///     Convert Breaks
        /// </summary>
        public string mt_convert_breaks;

        /// <summary>
        ///     Page keywords
        /// </summary>
        public string mt_keywords;

        /// <summary>
        ///     Display date of Blog Post (DateCreated)
        /// </summary>
        public DateTime pageDate;

        /// <summary>
        ///     PostID Guid in string format
        /// </summary>
        public string pageID;

        /// <summary>
        ///     Page Parent ID
        /// </summary>
        public string pageParentID;

        /// <summary>
        ///     Title of Blog Post
        /// </summary>
        public string title;

        #endregion
    }
}