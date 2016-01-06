using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
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
            var usr = Membership.GetUser(user.UserName);

            if (usr == null)
                return false;

            usr.Email = user.Email;
            Membership.UpdateUser(usr);
			
			//change user password
            if (!string.IsNullOrEmpty(user.OldPassword) && !string.IsNullOrEmpty(user.Password))
                ChangePassword(usr, user.OldPassword, user.Password);

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
            if (!Utils.StringIsNullOrWhitespace(id))
            {
                var pf = AuthorProfile.GetProfile(id);
                if (pf == null)
                {
                    pf = new AuthorProfile(id);
                    pf.Birthday = DateTime.Parse("01/01/1900");
                    pf.DisplayName = id;
                    pf.EmailAddress = Utils.GetUserEmail(id);
                    pf.FirstName = id;
                    pf.Private = true;
                    pf.Save();
                }
                
                return new Profile { 
                    AboutMe = string.IsNullOrEmpty(pf.AboutMe) ? "" : pf.AboutMe,
                    Birthday = pf.Birthday.ToShortDateString(),
                    CityTown = string.IsNullOrEmpty(pf.CityTown) ? "" : pf.CityTown,
                    Country = string.IsNullOrEmpty(pf.Country) ? "" : pf.Country,
                    DisplayName = pf.DisplayName,
                    EmailAddress = pf.EmailAddress,
                    PhoneFax = string.IsNullOrEmpty(pf.PhoneFax) ? "" : pf.PhoneFax,
                    FirstName = string.IsNullOrEmpty(pf.FirstName) ? "" : pf.FirstName,
                    Private = pf.Private,
                    LastName = string.IsNullOrEmpty(pf.LastName) ? "" : pf.LastName,
                    MiddleName = string.IsNullOrEmpty(pf.MiddleName) ? "" : pf.MiddleName,
                    PhoneMobile = string.IsNullOrEmpty(pf.PhoneMobile) ? "" : pf.PhoneMobile,
                    PhoneMain = string.IsNullOrEmpty(pf.PhoneMain) ? "" : pf.PhoneMain,
                    PhotoUrl = string.IsNullOrEmpty(pf.PhotoUrl) ? "" : pf.PhotoUrl.Replace("\"", ""),
                    RegionState = string.IsNullOrEmpty(pf.RegionState) ? "" : pf.RegionState
                };
            }
            return null;
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
            if (user == null || string.IsNullOrEmpty(user.UserName))
                return false;

            var pf = AuthorProfile.GetProfile(user.UserName) 
                ?? new AuthorProfile(user.UserName);
            try
            {
                pf.DisplayName = user.Profile.DisplayName;
                pf.FirstName = user.Profile.FirstName;
                pf.MiddleName = user.Profile.MiddleName;
                pf.LastName = user.Profile.LastName;
                pf.EmailAddress = user.Email; // user.Profile.EmailAddress;

                DateTime date;
                if (user.Profile.Birthday.Length == 0)
                    user.Profile.Birthday = "1/1/1001";

                if (DateTime.TryParse(user.Profile.Birthday, out date))
                    pf.Birthday = date;

                pf.PhotoUrl = user.Profile.PhotoUrl.Replace("\"", "");
                pf.Private = user.Profile.Private;

                pf.PhoneMobile = user.Profile.PhoneMobile;
                pf.PhoneMain = user.Profile.PhoneMain;
                pf.PhoneFax = user.Profile.PhoneFax;

                pf.CityTown = user.Profile.CityTown;
                pf.RegionState = user.Profile.RegionState;
                pf.Country = user.Profile.Country;
                pf.AboutMe = user.Profile.AboutMe;

                pf.Save();
                UpdateProfileImage(pf);
            }
            catch (Exception ex)
            {
                Utils.Log("Error editing profile", ex);
                return false;
            }
            return true;
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

        static void UpdateProfileImage(AuthorProfile profile)
        {
            var dir = BlogEngine.Core.Providers.BlogService.GetDirectory("/avatars");

            if(string.IsNullOrEmpty(profile.PhotoUrl))
            {
                foreach (var f in dir.Files)
                {
                    var dot = f.Name.IndexOf(".");
                    var img = dot > 0 ? f.Name.Substring(0, dot) : f.Name;
                    if (profile.UserName == img)
                    {
                        f.Delete();
                    }
                }
            }
            else
            {
                foreach (var f in dir.Files)
                {
                    var dot = f.Name.IndexOf(".");
                    var img = dot > 0 ? f.Name.Substring(0, dot) : f.Name;
                    // delete old profile image saved with different name
                    // for example was admin.jpg and now admin.png
                    if (profile.UserName == img && f.Name != profile.PhotoUrl.Replace("\"", ""))
                    {
                        f.Delete();
                    }
                }
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
