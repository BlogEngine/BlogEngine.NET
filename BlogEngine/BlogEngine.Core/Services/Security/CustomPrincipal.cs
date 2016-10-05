namespace BlogEngine.Core
{
    using System.Security.Principal;
    using System.Web.Security;

    /// <summary>
    /// Custom IPrincipal class used for authorization.
    /// </summary>
    public class CustomPrincipal : IPrincipal
    {
        private IIdentity _identity;

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        public IIdentity Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="roleName">The name of the role for which to check membership.</param>
        /// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
        public bool IsInRole(string roleName)
        { 
            if (Identity == null || !Identity.IsAuthenticated || string.IsNullOrEmpty(Identity.Name))
                return false;

            // Note: Cannot use "Security.CurrentUser.IsInRole" or anything similar since
            // Security.CurrentUser.IsInRole will look to this IsInRole() method here --
            // resulting in an endless loop.  Need to query the role provider directly.

            return Roles.IsUserInRole(Identity.Name, roleName);
        }

        /// <summary>
        /// Creates a new instance using the given IIdentity object.
        /// </summary>
        /// <param name="identity"></param>
        public CustomPrincipal(IIdentity identity)
        {
            _identity = identity;
        }
    }
}
