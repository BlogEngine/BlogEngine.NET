namespace BlogEngine.Core.API.MetaWeblog
{
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
}