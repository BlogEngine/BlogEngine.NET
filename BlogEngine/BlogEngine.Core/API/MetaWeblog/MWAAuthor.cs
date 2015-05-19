namespace BlogEngine.Core.API.MetaWeblog
{
    /// <summary>
    /// wp Author struct
    /// </summary>
    internal struct MWAAuthor
    {
        #region Constants and Fields

        /// <summary>
        ///     display name
        /// </summary>
        public string display_name;

        /// <summary>
        ///     nothing to see here.
        /// </summary>
        public string meta_value;

        /// <summary>
        ///     user email
        /// </summary>
        public string user_email;

        /// <summary>
        ///     userID - Specs call for a int, but our ID is a string.
        /// </summary>
        public string user_id;

        /// <summary>
        ///     user login name
        /// </summary>
        public string user_login;

        #endregion
    }
}