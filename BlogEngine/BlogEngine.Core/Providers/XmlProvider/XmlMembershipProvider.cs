namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Globalization;
    using System.Linq;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Security;
    using System.Web.Configuration;
    using System.Xml;
    using System.IO;

    /// <summary>
    /// The xml membership provider.
    /// </summary>
    public class XmlMembershipProvider : MembershipProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The password format.
        /// </summary>
        private MembershipPasswordFormat passwordFormat;

        /// <summary>
        /// The users.
        /// </summary>
        private Dictionary<Guid, Dictionary<string, MembershipUser>> users;

        /// <summary>
        /// The xml file name.
        /// </summary>
        private string xmlFileName;

        #endregion

        // MembershipProvider Properties
        #region Properties

        private string XmlFullyQualifiedPath
        {
            get
            {
                string location = Blog.CurrentInstance.StorageLocation;

                string fullyQualifiedPath =
                    HostingEnvironment.MapPath(
                        VirtualPathUtility.Combine(
                        VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath), location + xmlFileName));

                return fullyQualifiedPath;
            }
        }

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        public override string ApplicationName
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        public override bool EnablePasswordReset
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum length required for a password.
        /// </returns>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        public override int PasswordAttemptWindow
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.
        /// </returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return this.passwordFormat;
            }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <value></value>
        /// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
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
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);
            var nodes = doc.GetElementsByTagName("User");
            foreach (XmlNode node in nodes)
            {
                if (!node["UserName"].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!this.CheckPassword(node["Password"].InnerText, oldPassword))
                {
                    continue;
                }
                
                string passwordPrep = this.passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(newPassword) : newPassword;

                node["Password"].InnerText = passwordPrep;
                doc.Save(XmlFullyQualifiedPath);

                this.users = null;
                this.ReadMembershipDataStore();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(
            string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="passwordQuestion">The password question.</param>
        /// <param name="passwordAnswer">The password answer.</param>
        /// <param name="approved">if set to <c>true</c> [approved].</param>
        /// <param name="providerUserKey">The provider user key.</param>
        /// <param name="status">The status.</param>
        /// <returns>A Membership User.</returns>
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
            this.ReadMembershipDataStore();

            if (this.users[Blog.CurrentInstance.Id].ContainsKey(username))
            {
                throw new NotSupportedException("The username is already in use. Please choose another username.");
            }

            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);

            XmlNode xmlUserRoot = doc.CreateElement("User");
            XmlNode xmlUserName = doc.CreateElement("UserName");
            XmlNode xmlPassword = doc.CreateElement("Password");
            XmlNode xmlEmail = doc.CreateElement("Email");
            XmlNode xmlLastLoginTime = doc.CreateElement("LastLoginTime");

            xmlUserName.InnerText = username;

            string passwordPrep = this.passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(password) : password;

            xmlPassword.InnerText = passwordPrep;

            xmlEmail.InnerText = email;
            xmlLastLoginTime.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            xmlUserRoot.AppendChild(xmlUserName);
            xmlUserRoot.AppendChild(xmlPassword);
            xmlUserRoot.AppendChild(xmlEmail);
            xmlUserRoot.AppendChild(xmlLastLoginTime);

            doc.SelectSingleNode("Users").AppendChild(xmlUserRoot);
            doc.Save(XmlFullyQualifiedPath);

            status = MembershipCreateStatus.Success;
            var user = new MembershipUser(
                this.Name, 
                username, 
                username, 
                email, 
                passwordQuestion, 
                passwordPrep, 
                approved, 
                false, 
                DateTime.Now, 
                DateTime.Now, 
                DateTime.Now, 
                DateTime.Now, 
                DateTime.MaxValue);
            this.users[Blog.CurrentInstance.Id].Add(username, user);
            return user;
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            this.ReadMembershipDataStore();

            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);

            foreach (XmlNode node in
                doc.GetElementsByTagName("User").Cast<XmlNode>().Where(node => node.ChildNodes[0].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                doc.SelectSingleNode("Users").RemoveChild(node);
                doc.Save(XmlFullyQualifiedPath);
                this.users[Blog.CurrentInstance.Id].Remove(username);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
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
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
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
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves a collection of all the users.
        /// This implementation ignores pageIndex and pageSize,
        /// and it doesn't sort the MembershipUser objects returned.
        /// </summary>
        /// <param name="pageIndex">The page Index.</param>
        /// <param name="pageSize">The page Size.</param>
        /// <param name="totalRecords">The total Records.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            this.ReadMembershipDataStore();
            var membershipUserCollection = new MembershipUserCollection();

            if (Blog.CurrentInstance.IsSiteAggregation)
            {
                foreach (Blog b in Blog.Blogs)
                {
                    if (!b.IsDeleted && b.IsActive)
                    {
                        foreach (var pair in this.users[b.Id])
                        {
                            if (membershipUserCollection[pair.Key] == null)
                            {
                                membershipUserCollection.Add(pair.Value);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var pair in this.users[Blog.CurrentInstance.Id])
                {
                    membershipUserCollection.Add(pair.Value);
                }
            }

            totalRecords = membershipUserCollection.Count;
            return membershipUserCollection;
        }

        /// <summary>
        /// The get number of users online.
        /// </summary>
        /// <returns>
        /// The get number of users online.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the password for the user with the given username.
        /// </summary>
        /// <param name="username">
        /// the given username
        /// </param>
        /// <returns>
        /// the password if user is found, null otherwise.
        /// </returns>
        public string GetPassword(string username)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">
        /// The user to retrieve the password for.
        /// </param>
        /// <param name="answer">
        /// The password answer for the user.
        /// </param>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves a user based on his/hers username.
        ///     the userIsOnline parameter is ignored.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="userIsOnline">
        /// The user Is Online.
        /// </param>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            this.ReadMembershipDataStore();

            // Retrieve the user from the data source
            MembershipUser user;
            return this.users[Blog.CurrentInstance.Id].TryGetValue(username, out user) ? user : null;
        }

        /// <summary>
        /// Get a user based on the username parameter.
        ///     the userIsOnline parameter is ignored.
        /// </summary>
        /// <param name="providerUserKey">
        /// The provider User Key.
        /// </param>
        /// <param name="userIsOnline">
        /// The user Is Online.
        /// </param>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }

            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);

            return (from XmlNode node in doc.SelectNodes("//User")
                    where node.ChildNodes[0].InnerText.Equals(providerUserKey.ToString(), StringComparison.OrdinalIgnoreCase)
                    let userName = node.ChildNodes[0].InnerText
                    let password = node.ChildNodes[1].InnerText
                    let email = node.ChildNodes[2].InnerText
                    let lastLoginTime = DateTime.Parse(node.ChildNodes[3].InnerText, CultureInfo.InvariantCulture)
                    select new MembershipUser(this.Name, providerUserKey.ToString(), providerUserKey, email, string.Empty, password, true, false, DateTime.Now, lastLoginTime, DateTime.Now, DateTime.Now, DateTime.MaxValue)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a username based on a matching email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The get user name by email.
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);

            return doc.GetElementsByTagName("User").Cast<XmlNode>().Where(
                node => node.ChildNodes[2].InnerText.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)).Select(
                    node => node.ChildNodes[0].InnerText).FirstOrDefault();
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The name of the provider is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The name of the provider has a length of zero.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.string,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.
        /// </exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "XmlMembershipProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "XML membership provider");
            }

            base.Initialize(name, config);

            // Initialize _XmlFileName and make sure the path
            // is app-relative
            var filename = config["xmlFileName"];

            if (string.IsNullOrEmpty(filename))
            {
                filename = "users.xml";
            }
            else
            {
                if (!VirtualPathUtility.IsAppRelative(filename))
                {
                    throw new ArgumentException("xmlFileName must be app-relative");
                }
            }

            xmlFileName = filename;
            config.Remove("xmlFileName");

            // Make sure we have permission to read the XML data source and
            // throw an exception if we don't
            var permission = new FileIOPermission(FileIOPermissionAccess.Write, this.XmlFullyQualifiedPath);
            permission.Demand();

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
                    throw new ProviderException(string.Format("Unrecognized attribute: {0}", attr));
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
            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);
            var nodes = doc.GetElementsByTagName("User");
            var newPassword = Utils.RandomPassword();

            foreach (var node in
                nodes.Cast<XmlNode>().Where(
                    node =>
                    node["UserName"].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(node["Password"].InnerText)))
            {
                string passwordPrep;
                if (this.passwordFormat == MembershipPasswordFormat.Hashed)
                {
                    passwordPrep = Utils.HashPassword(newPassword);
                }
                else
                {
                    passwordPrep = newPassword;
                }

                node["Password"].InnerText = passwordPrep;
                doc.Save(XmlFullyQualifiedPath);

                this.users = null;
                this.ReadMembershipDataStore();
                return newPassword;
            }

            return string.Empty;
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        public override bool UnlockUser(string userName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Updates a user. The username will not be changed.
        /// </summary>
        /// <param name="user">
        /// The membership user.
        /// </param>
        public override void UpdateUser(MembershipUser user)
        {
            this.ReadMembershipDataStore();
            var doc = new XmlDocument();
            doc.Load(XmlFullyQualifiedPath);

            foreach (var node in
                doc.GetElementsByTagName("User").Cast<XmlNode>().Where(node => node.ChildNodes[0].InnerText.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                if (user.Comment.Length > 30)
                {
                    node.ChildNodes[1].InnerText = user.Comment;
                }

                node.ChildNodes[2].InnerText = user.Email;
                node.ChildNodes[3].InnerText = user.LastLoginDate.ToString(
                    "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                doc.Save(XmlFullyQualifiedPath);
                this.users[Blog.CurrentInstance.Id][user.UserName] = user;
            }
        }

        /// <summary>
        /// Returns true if the username and password match an exsisting user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The validate user.
        /// </returns>
        public override bool ValidateUser(string username, string password)
        {
            var validated = false;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            try
            {
                this.ReadMembershipDataStore();

                // Validate the user name and password
                MembershipUser user;
                if (this.users[Blog.CurrentInstance.Id].TryGetValue(username, out user))
                {
                    validated = this.CheckPassword(user.Comment, password);
                }

                return validated;
            }
            catch (Exception)
            {
                return validated;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check password.
        /// </summary>
        /// <param name="storedPassword">
        /// The stored password.
        /// </param>
        /// <param name="inputPassword">
        /// The input password.
        /// </param>
        /// <returns>
        /// The checked password.
        /// </returns>
        private bool CheckPassword(string storedPassword, string inputPassword)
        {
            var validated = false;
            if (storedPassword == string.Empty)
            {
                // This is a special case used for resetting.
                if (string.Compare(inputPassword, "admin", true) == 0)
                {
                    validated = true;
                }
            }
            else
            {
                if (this.passwordFormat == MembershipPasswordFormat.Hashed)
                {
                    if (storedPassword == Utils.HashPassword(inputPassword))
                    {
                        validated = true;
                    }
                }
                else if (inputPassword == storedPassword)
                {
                    validated = true;
                }
            }

            return validated;
        }

        /// <summary>
        /// Builds the internal cache of users.
        /// </summary>
        private void ReadMembershipDataStore()
        {
            lock (this)
            {
                if (this.users == null)
                {
                    this.users = new Dictionary<Guid, Dictionary<string, MembershipUser>>();
                }

                ReadFromFile(XmlFullyQualifiedPath, Blog.CurrentInstance.Id);

                if (Blog.CurrentInstance.IsSiteAggregation)
                {
                    foreach (Blog b in Blog.Blogs)
                    {
                        if (b.IsPrimary || b.IsDeleted || !b.IsActive)
                            continue;

                        string path = HostingEnvironment.MapPath(Path.Combine(Blog.CurrentInstance.StorageLocation, "blogs", b.Name, "users.xml"));

                        if (!File.Exists(path))
                        {
                            Utils.Log(string.Format("XmlMembershipProvider: can not read users from file \"{0}\"", path));
                        }

                        ReadFromFile(path, b.Id);
                    }
                }
            }
        }

        private void ReadFromFile(string fileName, Guid blogId)
        {
            if (!this.users.ContainsKey(blogId))
            {
                this.users[blogId] = new Dictionary<string, MembershipUser>(16, StringComparer.OrdinalIgnoreCase);
                var doc = new XmlDocument();
                doc.Load(fileName);
                var nodes = doc.GetElementsByTagName("User");

                foreach (var user in
                    nodes.Cast<XmlNode>().Select(
                        node => new MembershipUser(
                                    this.Name,
                            // Provider name
                                    node["UserName"].InnerText,
                            // Username
                                    node["UserName"].InnerText,
                            // providerUserKey
                                    node["Email"].InnerText,
                            // Email
                                    string.Empty,
                            // passwordQuestion
                                    node["Password"].InnerText,
                            // Comment
                                    true,
                            // approved
                                    false,
                            // isLockedOut
                                    DateTime.Now,
                            // creationDate
                                    DateTime.Parse(node["LastLoginTime"].InnerText, CultureInfo.InvariantCulture),
                            // lastLoginDate
                                    DateTime.Now,
                            // lastActivityDate
                                    DateTime.Now,
                            // lastPasswordChangedDate
                                    new DateTime(1980, 1, 1))))
                {
                    this.users[blogId].Add(user.UserName, user);
                }
            }
        }

        #endregion
    }
}