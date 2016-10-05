using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Roles repository
    /// </summary>
    public interface IRolesRepository
    {
        /// <summary>
        /// Post list
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns>List of roles</returns>
        IEnumerable<Data.Models.RoleItem> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Get single role
        /// </summary>
        /// <param name="id">Role name</param>
        /// <returns>User object</returns>
        Data.Models.RoleItem FindById(string id);
        /// <summary>
        /// Add new role
        /// </summary>
        /// <param name="role">Blog user</param>
        /// <returns>Saved user</returns>
        Data.Models.RoleItem Add(Data.Models.RoleItem role);
        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True on success</returns>
        bool Remove(string id);
        /// <summary>
        /// Get role rights
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>Collection of rights</returns>
        IEnumerable<Group> GetRoleRights(string role);
        /// <summary>
        /// Roles for user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Roles</returns>
        IEnumerable<Data.Models.RoleItem> GetUserRoles(string id);
        /// <summary>
        /// Save rights
        /// </summary>
        /// <param name="rights">Rights</param>
        /// <param name="id">Role id</param>
        /// <returns>True if success</returns>
        bool SaveRights(List<Data.Models.Group> rights, string id);
    }
}