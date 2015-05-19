namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using System.Security.Principal;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System.Security;

    /// <summary>
    /// IIdentity class used for authentication.
    /// </summary>
    /// <remarks>
    /// Need to inherit from MarshalByRefObject to prevent runtime errors with Cassini when using a custom identity.
    /// </remarks>
    public class CustomIdentity : MarshalByRefObject, IIdentity
    {
        /// <summary>
        /// Gets the type of authentication used by BlogEngine.NET.
        /// </summary>
        public string AuthenticationType
        {
            get { return "BlogEngine.NET Custom Identity"; }
        }

        private bool _isAuthenticated;

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        private string _name;

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Creates a new intance using the specified username and isAuthenticated bit.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="isAuthenticated">Whether or not the user is authenticated.</param>
        public CustomIdentity(string username, bool isAuthenticated)
        {
            _name = username;
            _isAuthenticated = isAuthenticated;
        }

        /// <summary>
        /// Creates a new instance, authenticating the user using the given credentials.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        public CustomIdentity(string username, string password)
        {
            if (Utils.StringIsNullOrWhitespace(username))
                throw new ArgumentNullException("username");

            if (Utils.StringIsNullOrWhitespace(password))
                throw new ArgumentNullException("password");

            if (!Membership.ValidateUser(username, password)) { return; }

            _isAuthenticated = true;
            _name = username;
        }
    }
}
