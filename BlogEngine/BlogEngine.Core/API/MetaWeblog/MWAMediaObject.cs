namespace BlogEngine.Core.API.MetaWeblog
{
    /// <summary>
    /// MetaWeblog MediaObject struct
    ///     passed in the newMediaObject call
    /// </summary>
    internal struct MWAMediaObject
    {
        #region Constants and Fields

        /// <summary>
        ///     Media
        /// </summary>
        public byte[] bits;

        /// <summary>
        ///     Name of media object (filename)
        /// </summary>
        public string name;

        /// <summary>
        ///     Type of file
        /// </summary>
        public string type;

        #endregion
    }
}