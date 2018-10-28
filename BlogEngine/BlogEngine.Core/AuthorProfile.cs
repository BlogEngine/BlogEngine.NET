/// <summary>
/// Mirrors Profile 
/// </summary>
namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BlogEngine.Core.Providers;
    using BlogEngine.Core.Data.Models;

    /// <summary>
    /// The author profile.
    /// </summary>
    public class AuthorProfile : BusinessBase<AuthorProfile, string>
    {
        private string code;
        #region Constants and Fields

        private static readonly object SyncRoot = new object();
        private static Dictionary<Guid, List<AuthorProfile>> profiles;
        private string zip;
        private string website;
        private string aboutMe;
        private string address;
        private string addressAlt; 
        private string recordId;
        private DateTime birthday;
        private string cityTown;
        private string company;
        private string country;
        private string displayName;
        private string emailAddress;
        private string firstName;
        private bool isprivate;
        private string lastName;
        private string middleName;
        private string phoneFax;
        private string phoneMain;
        private string phoneMobile;
        private string photoUrl;
        private string regionState;
        private string userId;
        private string newsletter;
        private string otherName;
        private string organization;
        private string title;
        private string spouse;
        private string workPhone;
        private string fullName;
        private string information;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorProfile"/> class.
        /// </summary>
        public AuthorProfile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorProfile"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        public AuthorProfile(string username)
        {
            this.Id = username;
        }

        static AuthorProfile()
        {
            Blog.Saved += (s, e) =>
            {
                if (e.Action == SaveAction.Delete)
                {
                    Blog blog = s as Blog;
                    if (blog != null)
                    {
                        // remove deleted blog from static 'profiles'

                        if (profiles != null && profiles.ContainsKey(blog.Id))
                            profiles.Remove(blog.Id);
                    }
                }
            };
        }

        /// <summary>
        ///     Gets an unsorted list of all pages.
        /// </summary>
        public static List<AuthorProfile> Profiles
        {
            get
            {
                if (profiles == null)
                    profiles = new Dictionary<Guid, List<AuthorProfile>>();

                if (!profiles.ContainsKey(Blog.CurrentInstance.Id))
                    profiles.Add(Blog.CurrentInstance.Id, BlogService.FillProfiles());

                return profiles[Blog.CurrentInstance.Id];
            }
        }

        #endregion

        public string RecordId
        {
            get
            {
                return this.recordId;
            }

            set
            {
                base.SetValue("RecordId", value, ref this.recordId);
            }
        }

        public string UserId
        {
            get
            {
                return this.userId;
            }

            set
            {
                base.SetValue("UserId", value, ref this.userId);
            }
        }

        public string Information
        {
            get
            {
                return this.information;
            }
            set
            {
                base.SetValue("Information", value, ref this.information);
            }
        }

        public string Title
        {
            get { return this.title; }
            set { base.SetValue("Title", value, ref this.title); }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                base.SetValue("DisplayName", value, ref this.displayName);
            }
        }

        public string UserName
        {
            get
            {
                return this.Id;
            }
            set
            {
                // do nothing;
            }
        }

        public string FullName
        {
            get
            {
                return this.fullName;
            }
            set
            {
                base.SetValue("FullName", value, ref this.fullName);
            }
        }

        public string OtherName
        {
            get { return this.otherName; }
            set { base.SetValue("OtherName", value, ref this.otherName); }
        }

        public string FirstName
        {
            get
            {
                return this.firstName;
            }

            set
            {
                base.SetValue("FirstName", value, ref this.firstName);
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }

            set
            {
                base.SetValue("LastName", value, ref this.lastName);
            }
        }

        public string MiddleName
        {
            get
            {
                return this.middleName;
            }

            set
            {
                base.SetValue("MiddleName", value, ref this.middleName);
            }
        }

        public string Spouse
        {
            get { return this.spouse; }
            set { base.SetValue("Spouse", value, ref this.spouse); }
        }

        public string AboutMe
        {
            get
            {
                return this.aboutMe;
            }

            set
            {
                base.SetValue("AboutMe", value, ref this.aboutMe);
            }
        }

        public string PhotoUrl
        {
            get
            {
                return this.photoUrl;
            }

            set
            {
                base.SetValue("PhotoUrl", value, ref this.photoUrl);
            }
        }

        public DateTime Birthday
        {
            get
            {
                return this.birthday;
            }

            set
            {
                base.SetValue("Birthday", value, ref this.birthday);
            }
        }

        public string EmailAddress
        {
            get
            {
                return this.emailAddress;
            }

            set
            {
                base.SetValue("EmailAddress", value, ref this.emailAddress);
            }
        }

        public string Address
        {
            get
            {
                return this.address;
            }

            set
            {
                base.SetValue("Address", value, ref this.address);
            }
        }

        public string AddressAlt
        {
            get
            {
                return this.addressAlt;
            }

            set
            {
                base.SetValue("AddressAlt", value, ref this.addressAlt);
            }
        }

        public string CityTown
        {
            get
            {
                return this.cityTown;
            }

            set
            {
                base.SetValue("CityTown", value, ref this.cityTown);
            }
        }

        public string RegionState
        {
            get
            {
                return this.regionState;
            }

            set
            {
                base.SetValue("RegionState", value, ref this.regionState);
            }
        }

        public string Zip
        {
            get { return this.zip; }
            set { base.SetValue("Zip", value, ref this.zip); }
        }

        public string Country
        {
            get
            {
                return this.country;
            }

            set
            {
                base.SetValue("Country", value, ref this.country);
            }
        }

        public string WorkPhone
        {
            get { return this.workPhone; }
            set { base.SetValue("WorkPhone", value, ref this.workPhone); }
        }

        public string PhoneFax
        {
            get
            {
                return this.phoneFax;
            }

            set
            {
                base.SetValue("PhoneFax", value, ref this.phoneFax);
            }
        }

        public string PhoneMobile
        {
            get
            {
                return this.phoneMobile;
            }

            set
            {
                base.SetValue("PhoneMobile", value, ref this.phoneMobile);
            }
        }

        public string PhoneMain
        {
            get
            {
                return this.phoneMain;
            }

            set
            {
                base.SetValue("PhoneMain", value, ref this.phoneMain);
            }
        }

        public string Company
        {
            get
            {
                return this.company;
            }

            set
            {
                base.SetValue("Company", value, ref this.company);
            }
        }

        public string Organization
        {
            get { return this.organization; }
            set { base.SetValue("Organization", value, ref this.organization); }
        }

        public string Website
        {
            get { return this.website; }
            set { base.SetValue("Website", value, ref this.website); }
        }

        public string Newsletter
        {
            get { return this.newsletter; }
            set { base.SetValue("Newsletter", value, ref this.newsletter); }
        }

        public string Code
        {
            get { return this.code; }
            set { base.SetValue("Code", value, ref this.code); }
        }

        public bool Private
        {
            get
            {
                return this.isprivate;
            }

            set
            {
                base.SetValue("Private", value, ref this.isprivate);
            }
        }

        public string RelativeLink
        {
            get
            {
                return $"{Utils.RelativeWebRoot}author/{Id}{BlogConfig.FileExtension}";
            }
            set
            {
                // Do nothing
            }
        }

        public Dictionary<String, CustomField> CustomFields
        {
            get
            {
                var profileFields = BlogService.Provider.FillCustomFields().Where(f =>
                    f.CustomType == "PROFILE" &&
                    f.ObjectId == Security.CurrentUser.Identity.Name).ToList();

                if (profileFields == null || profileFields.Count < 1)
                    return null;

                var fields = new Dictionary<String, CustomField>();

                foreach (var item in profileFields)
                {
                    fields.Add(item.Key, item);
                }
                return fields;
            }
        }

        public static Profile GetPopulatedProfile(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                var pf = GetProfile(id);
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
                var profile = new Profile(pf);
                return profile;
            }
            return null;
        }

        public static bool UpdateUserProfile(BlogUser user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
                return false;

            var authorProfile = GetProfile(user.UserName) ?? new AuthorProfile(user.UserName);
            try
            {
                if (user.Profile == null)
                {
                    user.Profile = new Profile();
                    user.Profile.EmailAddress = user.Email;
                    user.Profile.UserName = user.UserName;
                }

                var srcProps = user.Profile.GetType().GetProperties();
                var dstProps = authorProfile.GetType().GetProperties();

                foreach (var source in srcProps)
                {
                    var name = source.Name;
                    var value = source.GetValue(user.Profile);

                    foreach (var dest in dstProps)
                    {
                        if (dest.Name == name)
                        {
                            switch (dest.PropertyType.Name)
                            {
                                case "DateTime":
                                    DateTime date = DateTime.MinValue;
                                    if (DateTime.TryParse($"{value}", out date))
                                        dest.SetValue(authorProfile, date);
                                    break;

                                case "Boolean":
                                    dest.SetValue(authorProfile, value);
                                    break;

                                default:
                                        dest.SetValue(authorProfile, $"{value}");
                                    break;
                            }
                        }
                    }
                }

                authorProfile.Save();
                UpdateProfileImage(authorProfile);
            }
            catch (Exception ex)
            {
                Utils.Log("Error editing profile", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update profile image
        /// </summary>
        /// <param name="profile"></param>
        static void UpdateProfileImage(AuthorProfile profile)
        {
            var dir = BlogEngine.Core.Providers.BlogService.GetDirectory("/avatars");

            if (string.IsNullOrEmpty(profile.PhotoUrl))
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

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The AuthorProfile.</returns>
        public static AuthorProfile GetProfile(string username)
        {
            return
                Profiles.Find(p => p.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets profile by email address
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>Author profile</returns>
        public static AuthorProfile GetProfileByEmail(string email)
        {
            return
                Profiles.Find(p => String.IsNullOrWhiteSpace(p.EmailAddress) ? false : p.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.FullName;
        }

        /// <summary>
        /// Removes profile for a specific blog
        /// </summary>
        /// <param name="blogId">Blog ID</param>
        public static void RemoveProfile(Guid blogId)
        {
            profiles.Remove(blogId);
        }

        /// <summary>
        /// Datas the delete.
        /// </summary>
        protected override void DataDelete()
        {
            BlogService.DeleteProfile(this);
            if (Profiles.Contains(this))
            {
                Profiles.Remove(this);
            }
        }

        /// <summary>
        /// Datas the insert.
        /// </summary>
        protected override void DataInsert()
        {
            BlogService.InsertProfile(this);

            if (this.New)
            {
                Profiles.Add(this);
            }
        }

        /// <summary>
        /// Datas the select.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>The AuthorProfile.</returns>
        protected override AuthorProfile DataSelect(string id)
        {
            return BlogService.SelectProfile(id);
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        protected override void DataUpdate()
        {
            BlogService.UpdateProfile(this);
        }

        /// <summary>
        /// Validates based on rules.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule(
                "Id",
                "Id must be set to the username of the user who the profile belongs to",
                string.IsNullOrEmpty(this.Id));
        }

    }
}