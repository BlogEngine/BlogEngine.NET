namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Web.Security;

    /// <summary>
    /// Generic Db Membership Provider
    /// </summary>
    public class DbMembershipProvider : MembershipProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The application name.
        /// </summary>
        private string applicationName;

        /// <summary>
        /// The conn string name.
        /// </summary>
        private string connStringName;

        /// <summary>
        /// The parm prefix.
        /// </summary>
        private string parmPrefix;

        /// <summary>
        /// The password format.
        /// </summary>
        private MembershipPasswordFormat passwordFormat;

        /// <summary>
        /// The table prefix.
        /// </summary>
        private string tablePrefix;

        #endregion

        #region Properties

        /// <summary>
        ///     Returns the application name as set in the web.config
        ///     otherwise returns BlogEngine.  Set will throw an error.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     Hardcoded to false
        /// </summary>
        public override bool EnablePasswordReset
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Can password be retrieved via email?
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Hardcoded to 5
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        ///     Hardcoded to 0
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        ///     Hardcoded to 4
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        ///     Not implemented
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Password format (Clear or Hashed)
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return this.passwordFormat;
            }
        }

        /// <summary>
        ///     Not Implemented
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Hardcoded to false
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Hardcoded to false
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Change the password if the old password matches what is stored
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>The change password.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var oldPasswordCorrect = false;
            var success = false;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT password FROM {0}Users WHERE BlogID = {1}blogid AND userName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        // Check Old Password

                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), username));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                var actualPassword = rdr.GetString(0);
                                if (actualPassword == string.Empty)
                                {
                                    // This is a special case used for resetting.
                                    if (oldPassword.ToLower() == "admin")
                                    {
                                        oldPasswordCorrect = true;
                                    }
                                }
                                else
                                {
                                    if (this.passwordFormat == MembershipPasswordFormat.Hashed)
                                    {
                                        if (actualPassword == Utils.HashPassword(oldPassword))
                                        {
                                            oldPasswordCorrect = true;
                                        }
                                    }
                                    else if (actualPassword == oldPassword)
                                    {
                                        oldPasswordCorrect = true;
                                    }
                                }
                            }
                        }

                        // Update New Password
                        if (oldPasswordCorrect)
                        {
                            cmd.CommandText = string.Format("UPDATE {0}Users SET password = {1}pwd WHERE BlogID = {1}blogid AND userName = {1}name", this.tablePrefix, this.parmPrefix);

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("pwd"), (this.passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(newPassword) : newPassword)));

                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>The change password question and answer.</returns>
        public override bool ChangePasswordQuestionAndAnswer(
            string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add new user to database
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="approved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(
            string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool approved,
            object providerUserKey,
            out MembershipCreateStatus status)
        {

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Users (blogId, userName, password, emailAddress, lastLoginTime) VALUES ({1}blogid, {1}name, {1}pwd, {1}email, {1}login)", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("name"), username));
                        parms.Add(conn.CreateParameter(FormatParamName("pwd"), (this.passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(password) : password)));
                        parms.Add(conn.CreateParameter(FormatParamName("email"), email));
                        parms.Add(conn.CreateParameter(FormatParamName("login"), DateTime.Now));

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MembershipUser user = this.GetMembershipUser(username, email, DateTime.Now);
            status = MembershipCreateStatus.Success;

            return user;
        }

        /// <summary>
        /// Delete user from database
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>The delete user.</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            bool success = false;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Users WHERE blogId = {1}blogid AND userName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), username));

                        try
                        {
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        catch (Exception)
                        {
                            success = false;
                            throw;
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(
            string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return all users in MembershipUserCollection
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    if (Blog.CurrentInstance.IsSiteAggregation)
                    {
                        using (var cmd = conn.CreateTextCommand($"SELECT username, EmailAddress, lastLoginTime FROM {tablePrefix}Users "))
                        {
                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    users.Add(this.GetMembershipUser(rdr.GetString(0), rdr.GetString(1), rdr.GetDateTime(2)));
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var cmd = conn.CreateTextCommand($"SELECT username, EmailAddress, lastLoginTime FROM {tablePrefix}Users WHERE BlogID = {parmPrefix}blogid "))
                        {
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    users.Add(this.GetMembershipUser(rdr.GetString(0), rdr.GetString(1), rdr.GetDateTime(2)));
                                }
                            }
                        }
                    }
                }
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns>
        /// The get number of users online.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>The get password.</returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return this.GetUser(providerUserKey.ToString(), userIsOnline);
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            MembershipUser user = null;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT username, EmailAddress, lastLoginTime FROM {0}Users WHERE BlogID = {1}blogid AND UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), username));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                user = this.GetMembershipUser(username, rdr.GetString(1), rdr.GetDateTime(2));
                            }
                        }
                    }
                }
            }

            return user;
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            string userName = null;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT userName FROM {0}Users WHERE BlogID = {1}blogid AND emailAddress = {1}email", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("email"), email));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                userName = rdr.GetString(0);
                            }
                        }
                    }
                }
            }

            return userName;
        }

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="name">
        /// Configuration name
        /// </param>
        /// <param name="config">
        /// Configuration settings
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "DbMembershipProvider";
            }

            if (Utils.IsMono)
            {
                // Mono dies with a "Unrecognized attribute: description" if a description is part of the config.
                if (!string.IsNullOrEmpty(config["description"]))
                {
                    config.Remove("description");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(config["description"]))
                {
                    config.Remove("description");
                    config.Add("description", "Generic Database Membership Provider");
                }
            }

            base.Initialize(name, config);

            // Connection String
            if (config["connectionStringName"] == null)
            {
                config["connectionStringName"] = "BlogEngine";
            }

            this.connStringName = config["connectionStringName"];
            config.Remove("connectionStringName");

            // Table Prefix
            if (config["tablePrefix"] == null)
            {
                config["tablePrefix"] = "be_";
            }

            this.tablePrefix = config["tablePrefix"];
            config.Remove("tablePrefix");

            // Parameter character
            if (config["parmPrefix"] == null)
            {
                config["parmPrefix"] = "@";
            }

            this.parmPrefix = config["parmPrefix"];
            config.Remove("parmPrefix");

            // Application Name
            if (config["applicationName"] == null)
            {
                config["applicationName"] = "BlogEngine";
            }

            this.applicationName = config["applicationName"];
            config.Remove("applicationName");

            // Password Format
            if (config["passwordFormat"] == null)
            {
                config["passwordFormat"] = "Hashed";
                this.passwordFormat = MembershipPasswordFormat.Hashed;
            }
            else if (string.Compare(config["passwordFormat"], "clear", true) == 0)
            {
                this.passwordFormat = MembershipPasswordFormat.Clear;
            }
            else
            {
                this.passwordFormat = MembershipPasswordFormat.Hashed;
            }

            config.Remove("passwordFormat");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                var attr = config.GetKey(0);
                if (!string.IsNullOrEmpty(attr))
                {
                    throw new ProviderException($"Unrecognized attribute: {attr}");
                }
            }
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        public override string ResetPassword(string username, string answer)
        {
            if (string.IsNullOrEmpty(username))
            {
                return string.Empty;
            }

            var oldPassword = string.Empty;
            var randomPassword = Utils.RandomPassword();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {

                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT password FROM {0}Users WHERE BlogID = {1}blogid AND userName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        // Check Old Password

                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), username));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                oldPassword = rdr.GetString(0);
                            }
                        }

                        // Update Password
                        if (!string.IsNullOrEmpty(oldPassword))
                        {
                            cmd.CommandText = string.Format("UPDATE {0}Users SET password = {1}pwd WHERE BlogID = {1}blogid AND userName = {1}name", this.tablePrefix, this.parmPrefix);

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("pwd"), (this.passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(randomPassword) : randomPassword)));

                            cmd.ExecuteNonQuery();
                            return randomPassword;
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        /// <returns>The unlock user.</returns>
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update User Data (not password)
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("UPDATE {0}Users SET emailAddress = {1}email WHERE BlogId = {1}blogId AND userName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("name"), user.UserName));
                        parms.Add(conn.CreateParameter(FormatParamName("email"), user.Email));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Check username and password
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>The validate user.</returns>
        public override bool ValidateUser(string username, string password)
        {
            var validated = false;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT password FROM {0}Users WHERE BlogID = {1}blogid AND UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), username));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                var storedPwd = rdr.GetString(0);

                                if (storedPwd == string.Empty)
                                {
                                    // This is a special case used for resetting.
                                    if (password.ToLower() == "admin")
                                    {
                                        validated = true;
                                    }
                                }
                                else
                                {
                                    if (this.passwordFormat == MembershipPasswordFormat.Hashed)
                                    {
                                        if (storedPwd == Utils.HashPassword(password))
                                        {
                                            validated = true;
                                        }
                                    }
                                    else if (storedPwd == password)
                                    {
                                        validated = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return validated;
        }

        #endregion

        #region Methods

        private DbConnectionHelper CreateConnection()
        {
            var settings = ConfigurationManager.ConnectionStrings[this.connStringName];
            return new DbConnectionHelper(settings);
        }


        /// <summary>
        /// Returns a formatted parameter name to include this DbBlogProvider instance's paramPrefix.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private string FormatParamName(string parameterName)
        {
            return $"{parmPrefix}{parameterName}";
        }

        /// <summary>
        /// Gets membership user.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="lastLogin">
        /// The last login.
        /// </param>
        /// <returns>
        /// A MembershipUser.
        /// </returns>
        private MembershipUser GetMembershipUser(string userName, string email, DateTime lastLogin)
        {
            var user = new MembershipUser(
                this.Name, // Provider name
                userName, // Username
                userName, // providerUserKey
                email, // Email
                string.Empty, // passwordQuestion
                string.Empty, // Comment
                true, // approved
                false, // isLockedOut
                DateTime.Now, // creationDate
                lastLogin, // lastLoginDate
                DateTime.Now, // lastActivityDate
                DateTime.Now, // lastPasswordChangedDate
                new DateTime(1980, 1, 1)); // lastLockoutDate
            return user;
        }

        #endregion
    }
}