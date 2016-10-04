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
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The profiles.
        /// </summary>
        private static Dictionary<Guid, List<AuthorProfile>> profiles;

        /// <summary>
        /// The about me.
        /// </summary>
        private string aboutMe;

        /// <summary>
        /// The birthday.
        /// </summary>
        private DateTime birthday;

        /// <summary>
        /// The city town.
        /// </summary>
        private string cityTown;

        /// <summary>
        /// The company.
        /// </summary>
        private string company;

        /// <summary>
        /// The country.
        /// </summary>
        private string country;

        /// <summary>
        /// The display name.
        /// </summary>
        private string displayName;

        /// <summary>
        /// The email address.
        /// </summary>
        private string emailAddress;

        /// <summary>
        /// The first name.
        /// </summary>
        private string firstName;

        /// <summary>
        /// The is private.
        /// </summary>
        private bool isprivate;

        /// <summary>
        /// The last name.
        /// </summary>
        private string lastName;

        /// <summary>
        /// The middle name.
        /// </summary>
        private string middleName;

        /// <summary>
        /// The phone fax.
        /// </summary>
        private string phoneFax;

        /// <summary>
        /// The phone main.
        /// </summary>
        private string phoneMain;

        /// <summary>
        /// The phone mobile.
        /// </summary>
        private string phoneMobile;

        /// <summary>
        /// The photo url.
        /// </summary>
        private string photoUrl;

        /// <summary>
        /// The region state.
        /// </summary>
        private string regionState;

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

        #region Properties

        /// <summary>
        /// Gets or sets AboutMe.
        /// </summary>
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

        /// <summary>
        /// Gets or sets Birthday.
        /// </summary>
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

        /// <summary>
        /// Gets or sets CityTown.
        /// </summary>
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

        /// <summary>
        /// Gets or sets Company.
        /// </summary>
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

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
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

        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
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

        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
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

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
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

        /// <summary>
        /// Gets FullName.
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Private.
        /// </summary>
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

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
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

        /// <summary>
        /// Gets or sets MiddleName.
        /// </summary>
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

        /// <summary>
        /// Gets or sets PhoneFax.
        /// </summary>
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

        /// <summary>
        /// Gets or sets PhoneMain.
        /// </summary>
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

        /// <summary>
        /// Gets or sets PhoneMobile.
        /// </summary>
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

        /// <summary>
        /// Gets or sets PhotoURL.
        /// </summary>
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

        /// <summary>
        /// Gets or sets RegionState.
        /// </summary>
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

        /// <summary>
        /// Gets RelativeLink.
        /// </summary>
        public string RelativeLink
        {
            get
            {
                return $"{Utils.RelativeWebRoot}author/{Id}{BlogConfig.FileExtension}";
            }
        }

        /// <summary>
        /// Gets UserName.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.Id;
            }
        }

        /// <summary>
        /// Custom fields
        /// </summary>
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

        #endregion

        #region Public Methods

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
        /// Returns the author profile as a JsonProfile object for json serialization.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Profile ToJson(string username)
        {
            var j = new Profile();
            var p = Profiles.Find(ap => ap.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (p != null)
            {
                j.AboutMe = string.IsNullOrEmpty(p.AboutMe) ? "" : p.aboutMe;
                j.Birthday = p.Birthday.ToShortDateString();
                j.CityTown = string.IsNullOrEmpty(p.CityTown) ? "" : p.CityTown;
                j.Country = string.IsNullOrEmpty(p.Country) ? "" : p.Country;
                j.DisplayName = p.DisplayName;
                j.EmailAddress = p.EmailAddress;
                j.PhoneFax = string.IsNullOrEmpty(p.PhoneFax) ? "" : p.PhoneFax;
                j.FirstName = string.IsNullOrEmpty(p.FirstName) ? "" : p.FirstName;
                j.Private = p.Private;
                j.LastName = string.IsNullOrEmpty(p.LastName) ? "" : p.LastName;
                j.MiddleName = string.IsNullOrEmpty(p.MiddleName) ? "" : p.MiddleName;
                j.PhoneMobile = string.IsNullOrEmpty(p.PhoneMobile) ? "" : p.PhoneMobile;
                j.PhoneMain = string.IsNullOrEmpty(p.PhoneMain) ? "" : p.PhoneMain;
                j.PhotoUrl = string.IsNullOrEmpty(p.PhotoUrl) ? "" : p.PhotoUrl;
                j.RegionState = string.IsNullOrEmpty(p.RegionState) ? "" : p.RegionState;
            }
            else
            {
                j.AboutMe = "";
                j.Birthday = "01/01/1900";
                j.CityTown = "";
                j.Country = "";
                j.DisplayName = username;
                j.EmailAddress = Utils.GetUserEmail(username);
                j.PhoneFax = "";
                j.FirstName = username;
                j.Private = true;
                j.LastName = "";
                j.MiddleName = "";
                j.PhoneMobile = "";
                j.PhoneMain = "";
                j.PhotoUrl = "";
                j.RegionState = "";
            }

            return j;
        }

        /// <summary>
        /// Removes profile for a specific blog
        /// </summary>
        /// <param name="blogId">Blog ID</param>
        public static void RemoveProfile(Guid blogId)
        {
            profiles.Remove(blogId);
        }

        #endregion

        #region Methods

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

        #endregion
    }
}