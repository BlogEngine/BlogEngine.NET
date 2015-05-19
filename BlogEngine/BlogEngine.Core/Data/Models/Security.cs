using System.Collections.Generic;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Json friendly Role wrapper
    /// </summary>
    public class RoleItem
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// Role Name
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Is System Role
        /// </summary>
        public bool IsSystemRole { get; set; }
    }

    /// <summary>
    /// Group of permissions
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Empty constructor needed for serialization
        /// </summary>
        public Group() { }
        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="title">Role title</param>
        public Group(string title) 
        {
            Title = title;
            if (Permissions == null)
                Permissions = new List<Permission>();
        }
        /// <summary>
        /// Role title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// List of rights
        /// </summary>
        public List<Permission> Permissions { get; set; }
    }

    /// <summary>
    /// Permission
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Right Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Checked if right allowed for the role
        /// </summary>
        public bool IsChecked { get; set; }
    }
}