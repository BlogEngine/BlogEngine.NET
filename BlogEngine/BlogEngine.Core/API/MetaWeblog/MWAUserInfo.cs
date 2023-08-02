namespace BlogEngine.Core.API.MetaWeblog
{
#pragma warning disable 649
    /// <summary>
    /// MetaWeblog UserInfo struct
    /// returned from GetUserInfo call
    /// </summary>
    /// <remarks>
    /// Not used currently, but here for completeness.
    /// </remarks>
    internal struct MWAUserInfo
    {
        /// <summary>
        /// User Name Proper
        /// </summary>
        public string nickname;
        
        /// <summary>
        /// Login ID
        /// </summary>
        public string userID;
        // q: how can I disable a warning for a single line?
// a: http://stackoverflow.com/questions/281457/what-is-the-c-sharp-version-of-pragma-warning-disable-xxxx-in-visual-studio
        /// <summary>
        /// Url to User Blog?
        /// </summary>
        public string url;
        
        /// <summary>
        /// Email address of User
        /// </summary>
        public string email;
        
        /// <summary>
        /// User LastName
        /// </summary>
        public string lastName;
        
        /// <summary>
        /// User First Name
        /// </summary>
        public string firstName;
    }
#pragma warning restore 649
}