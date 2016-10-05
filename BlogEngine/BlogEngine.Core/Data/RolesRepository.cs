using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Security;
using System.Reflection;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Roles repository
    /// </summary>
    public class RolesRepository : IRolesRepository
    {
        /// <summary>
        /// Post list
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns>List of roles</returns>
        public IEnumerable<RoleItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewRoles))
                throw new System.UnauthorizedAccessException();

            var roles = new List<RoleItem>();

            if (string.IsNullOrEmpty(filter)) filter = "1 == 1";
            if (string.IsNullOrEmpty(order)) order = "RoleName";

            roles.AddRange(System.Web.Security.Roles.GetAllRoles().Select(r => new RoleItem 
                { RoleName = r, IsSystemRole = Security.IsSystemRole(r) }));

            roles.Sort((r1, r2) => string.Compare(r1.RoleName, r2.RoleName));

            return roles;
        }

        /// <summary>
        /// Get single role
        /// </summary>
        /// <param name="id">Role name</param>
        /// <returns>User object</returns>
        public RoleItem FindById(string id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewRoles))
                throw new System.UnauthorizedAccessException();

            var roles = new List<Data.Models.RoleItem>();
            roles.AddRange(System.Web.Security.Roles.GetAllRoles().Select(r => new Data.Models.RoleItem { RoleName = r, IsSystemRole = Security.IsSystemRole(r) }));

            return roles.FirstOrDefault(r => r.RoleName.ToLower() == id.ToLower());
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <param name="role">Blog user</param>
        /// <returns>Saved user</returns>
        public RoleItem Add(Data.Models.RoleItem role)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewRoles))
            {
                throw new System.UnauthorizedAccessException();
            }
            else if (String.IsNullOrWhiteSpace(role.RoleName))
            {
                throw new ApplicationException("Role name is required");
            }
            else if (Roles.RoleExists(role.RoleName))
            {
                throw new ApplicationException("Role already exists");
            }
            else
            {
                try
                {
                    Roles.CreateRole(role.RoleName);
                    return FindById(role.RoleName);
                }
                catch (Exception ex)
                {
                    Utils.Log(string.Format("Error adding role", ex));
                    throw new ApplicationException("Error adding new role");
                }
            }
        }

        /// <summary>
        /// Update role
        /// </summary>
        /// <param name="role">New role</param>
        /// <param name="oldRole">Role to update</param>
        /// <returns>True on success</returns>
        public bool Update(RoleItem role, string oldRole)
        {
            if (!Security.IsAuthorizedTo(Rights.EditRoles))
                throw new System.UnauthorizedAccessException();

            var updateRole = Roles.GetAllRoles().FirstOrDefault(r => r.ToString() == oldRole);

            if (updateRole == null)
                throw new ApplicationException("Role not found");

            updateRole = role.RoleName;

            return true;
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True on success</returns>
        public bool Remove(string id)
        {
            if (!Security.IsAuthorizedTo(Rights.DeleteRoles))
                throw new System.UnauthorizedAccessException();

            if (String.IsNullOrWhiteSpace(id))
                throw new ApplicationException("Role name is required");

            try
            {
                Right.OnRoleDeleting(id);
                Roles.DeleteRole(id);
                return true;
            }
            catch (Exception ex)
            {
                Utils.Log("Error deleting role", ex);
                return false;
            }
        }

        /// <summary>
        /// Get role rights
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>Collection of rights</returns>
        public IEnumerable<Group> GetRoleRights(string role)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewRoles))
                throw new System.UnauthorizedAccessException();

            var groups = new List<Group>();

            // store the category for each Rights.
            var rightCategories = new Dictionary<BlogEngine.Core.Rights, string>();
            var roleRights = BlogEngine.Core.Right.GetRights(role);

            foreach (FieldInfo fi in typeof(BlogEngine.Core.Rights).GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                BlogEngine.Core.Rights right = (BlogEngine.Core.Rights)fi.GetValue(null);

                if (right != BlogEngine.Core.Rights.None)
                {
                    RightDetailsAttribute rightDetails = null;

                    foreach (Attribute attrib in fi.GetCustomAttributes(true))
                    {
                        if (attrib is RightDetailsAttribute)
                        {
                            rightDetails = (RightDetailsAttribute)attrib;
                            break;
                        }
                    }

                    var category = rightDetails == null ? RightCategory.General : rightDetails.Category;             

                    var group = groups.FirstOrDefault(g => g.Title == category.ToString());

                    var prm = new Permission();
                    var rt = Right.GetRightByName(right.ToString());

                    prm.Id = right.ToString();
                    prm.Title = rt.DisplayName;
                    prm.IsChecked = roleRights.Contains(rt);

                    if (group == null)
                    {
                        var newGroup = new Group(category.ToString());
                        newGroup.Permissions.Add(prm);
                        groups.Add(newGroup);
                    }
                    else
                    {
                        group.Permissions.Add(prm);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// Roles for user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Roles</returns>
        public IEnumerable<Data.Models.RoleItem> GetUserRoles(string id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewRoles))
                throw new System.UnauthorizedAccessException();

            var roles = new List<Data.Models.RoleItem>();

            roles.AddRange(System.Web.Security.Roles.GetAllRoles()
                .Where(r => r != "Anonymous")
                .Select(r => new Data.Models.RoleItem { 
                    RoleName = r, 
                    IsSystemRole = Security.IsSystemRole(r), 
                    IsChecked = Roles.IsUserInRole(id, r) 
                })); 
           
            roles.Sort((r1, r2) => string.Compare(r1.RoleName, r2.RoleName));

            return roles;
        }

        /// <summary>
        /// Save rights
        /// </summary>
        /// <param name="rights">Rights</param>
        /// <param name="id">Role id</param>
        /// <returns>True if success</returns>
        public bool SaveRights(List<Data.Models.Group> rights, string id)
        {
            if (!Security.IsAuthorizedTo(Rights.EditRoles))
            {
                throw new System.UnauthorizedAccessException();
            }
            else if (String.IsNullOrWhiteSpace(id))
            {
                throw new ApplicationException("Invalid role name");
            }
            else if (rights == null)
            {
                throw new ApplicationException("Rights can not be null");
            }
            else
            {
                var rightsCollection = new Dictionary<string, bool>();

                foreach (var g in rights)
                {
                    foreach (var r in g.Permissions)
                    {
                        if (r.IsChecked)
                        {
                            rightsCollection.Add(r.Id, r.IsChecked);
                        }
                    }
                }

                foreach (var right in Right.GetAllRights())
                {
                    if (right.Flag != Rights.None)
                    {
                        if (rightsCollection.ContainsKey(right.Name))
                        {
                            right.AddRole(id);
                        }
                        else
                        {
                            right.RemoveRole(id);
                        }
                    }
                }
                BlogEngine.Core.Providers.BlogService.SaveRights();
                return true;
            }
        }
    }
}
