using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Security;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Users repository
    /// </summary>
    public class UsersRepository : IUsersRepository
    {
        /// <summary>
        /// Post list
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns>List of users</returns>
        public IEnumerable<BlogUser> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminPages))
                throw new UnauthorizedAccessException();

            var users = new List<BlogUser>();
            int count;
            var userCollection = Membership.Provider.GetAllUsers(0, 999, out count);
            var members = userCollection.Cast<MembershipUser>().ToList();

            foreach (var m in members)
            {
                users.Add(new BlogUser { 
                    IsChecked = false, 
                    UserName = m.UserName, 
                    Email = m.Email,
                    Profile = GetProfile(m.UserName),
                    Roles = GetRoles(m.UserName)
                });
            }

            var query = users.AsQueryable().Where(filter);

            // if take passed in as 0, return all
            if (take == 0) take = users.Count;

            return query.OrderBy(order).Skip(skip).Take(take);
        }

        /// <summary>
        /// Get single post
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User object</returns>
        public BlogUser FindById(string id)
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminPages))
                throw new UnauthorizedAccessException();

            var users = new List<BlogUser>();
            int count;
            var userCollection = Membership.Provider.GetAllUsers(0, 999, out count);
            var members = userCollection.Cast<MembershipUser>().ToList();

            foreach (var m in members)
            {
                users.Add(new BlogUser
                {
                    IsChecked = false,
                    UserName = m.UserName,
                    Email = m.Email,
                    Profile = GetProfile(m.UserName),
                    Roles = GetRoles(m.UserName)
                });
            }
            return users.AsQueryable().Where("UserName.ToLower() == \"" + id.ToLower() + "\"").FirstOrDefault();
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="user">Blog user</param>
        /// <returns>Saved user</returns>
        public BlogUser Add(BlogUser user)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewUsers))
                throw new UnauthorizedAccessException();

            if (user == null || string.IsNullOrEmpty(user.UserName)
                || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                throw new ApplicationException("Error adding new user; Missing required fields");
            }

            if (!Security.IsAuthorizedTo(Rights.CreateNewUsers))
                throw new UnauthorizedAccessException();

            // create user
            var usr = Membership.CreateUser(user.UserName, user.Password, user.Email);
            if (usr == null)
                throw new ApplicationException("Error creating new user");

            UpdateUserProfile(user);

            UpdateUserRoles(user);

            user.Password = "";
            return user;
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">User to update</param>
        /// <returns>True on success</returns>
        public bool Update(BlogUser user)
        {
            if (!Security.IsAuthorizedTo(Rights.EditOwnUser))
                throw new UnauthorizedAccessException();

            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email))
                throw new ApplicationException("Error adding new user; Missing required fields");

            // update user
            var member = Membership.GetUser(user.UserName);

            if (member == null)
                return false;

            member.Email = user.Email;
            Membership.UpdateUser(member);

            // Ensure that if email is changed that the profile email is changed as well
            user.Profile.EmailAddress = member.Email;
			
			//change user password
            if (!string.IsNullOrEmpty(user.OldPassword) && !string.IsNullOrEmpty(user.Password))
                ChangePassword(member, user.OldPassword, user.Password);

            UpdateUserProfile(user);

            UpdateUserRoles(user);

            return true;
        }

        /// <summary>
        /// Save user profile
        /// </summary>
        /// <param name="user">Blog user</param>
        /// <returns>True on success</returns>
        public bool SaveProfile(BlogUser user)
        {
            if (Self(user.UserName) && !Security.IsAuthorizedTo(Rights.EditOwnUser))
                throw new UnauthorizedAccessException();

            if (!Self(user.UserName) && !Security.IsAuthorizedTo(Rights.EditOtherUsers))
                    throw new UnauthorizedAccessException();

            return UpdateUserProfile(user);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True on success</returns>
        public bool Remove(string id){

            if (string.IsNullOrEmpty(id))
                return false;

            if (Self(id) && !Security.IsAuthorizedTo(Rights.DeleteUserSelf))
                throw new UnauthorizedAccessException();

            else if (!Self(id) && !Security.IsAuthorizedTo(Rights.DeleteUsersOtherThanSelf))
                throw new UnauthorizedAccessException();

            // Last check - it should not be possible to remove the last use who has the right to Add and/or Edit other user accounts. If only one of such a 
            // user remains, that user must be the current user, and can not be deleted, as it would lock the user out of the BE environment, left to fix
            // it in XML or SQL files / commands. See issue 11990
            bool adminsExist = false;
            MembershipUserCollection users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
            {
                string[] roles = Roles.GetRolesForUser(user.UserName);

                // look for admins other than 'id' 
                if (!Self(id) && (Right.HasRight(Rights.EditOtherUsers, roles) || Right.HasRight(Rights.CreateNewUsers, roles)))
                {
                    adminsExist = true;
                    break;
                }
            }

            if (!adminsExist)
                throw new ApplicationException("Can not delete last admin");

            string[] userRoles = Roles.GetRolesForUser(id);

            try
            {
                if (userRoles.Length > 0)
                {
                    Roles.RemoveUsersFromRoles(new string[] { id }, userRoles);
                }

                Membership.DeleteUser(id);

                var pf = AuthorProfile.GetProfile(id);
                if (pf != null)
                {
                    Providers.BlogService.DeleteProfile(pf);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Error deleting user", ex.Message);
                return false;
            }
            return true;
        }

        #region Private methods

        static Profile GetProfile(string id)
        {
            // BillKrat.2018.09.02 moved into AuthorProfile to encapsulate
            var result = AuthorProfile.GetPopulatedProfile(id);
            return result;
        }

        static List<RoleItem> GetRoles(string id)
        {
            var roles = new List<RoleItem>();
            var userRoles = new List<RoleItem>();

            roles.AddRange(Roles.GetAllRoles().Select(r => new RoleItem { RoleName = r, IsSystemRole = Security.IsSystemRole(r) }));
            roles.Sort((r1, r2) => string.Compare(r1.RoleName, r2.RoleName));

            foreach (var r in roles)
            {
                if (Roles.IsUserInRole(id, r.RoleName))
                {
                    userRoles.Add(r);
                }
            }
            return userRoles;
        }

        static bool UpdateUserProfile(BlogUser user)
        {
            // If the profile email changed be sure to update membership to match
            if (user.Email != user.Profile.EmailAddress)
            {
                // update user
                var member = Membership.GetUser(user.UserName);
                if (member != null)
                {
                    member.Email = user.Profile.EmailAddress;
                    Membership.UpdateUser(member);
                    user.Email = member.Email;
                }
            }

            // BillKrat.2018.09.02 moved into AuthorProfile to encapsulate
            var result = AuthorProfile.UpdateUserProfile(user);
            

            return result;
        }

        static bool UpdateUserRoles(BlogUser user)
        {
            try
            {
                // remove all user roles and add only checked
                string[] currentRoles = Roles.GetRolesForUser(user.UserName);
                if (currentRoles.Length > 0)
                    Roles.RemoveUserFromRoles(user.UserName, currentRoles);

                if (user.Roles.Count > 0)
                {
                    string[] roles = user.Roles.Where(ur => ur.IsChecked).Select(r => r.RoleName).ToArray();

                    if(roles.Length > 0)
                        Roles.AddUsersToRoles(new string[] { user.UserName }, roles);
                    else
                        Roles.AddUsersToRoles(new string[] { user.UserName }, new string[] { BlogConfig.AnonymousRole });
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.Log("Error updating user roles", ex);
                return false;
            }
        }


        bool ChangePassword(MembershipUser user, string password, string newPassword)
        {
            return user.ChangePassword(password, newPassword);
        }

        bool Self(string id)
        {
            return id.Equals(Security.CurrentUser.Identity.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
