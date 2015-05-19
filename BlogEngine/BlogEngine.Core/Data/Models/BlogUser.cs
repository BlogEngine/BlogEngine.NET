using System.Collections.Generic;
namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// The blog user
    /// </summary>
    public class BlogUser
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
		/// <summary>
        /// Old Password
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// User profile
        /// </summary>
        public Profile Profile { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<RoleItem> Roles { get; set; }
    }
}
