namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// The xml blog provider.
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// The delete profile.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public override void DeleteProfile(AuthorProfile profile)
        {
            var fileName = $"{GetFolder(profile.Blog)}profiles{Path.DirectorySeparatorChar}{profile.Id}.xml";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            if (AuthorProfile.Profiles.Contains(profile))
            {
                AuthorProfile.Profiles.Remove(profile);
            }

            // remove profile picture
            var dir = BlogService.GetDirectory("/avatars");
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

        /// <summary>
        /// The fill profiles.
        /// </summary>
        /// <returns>
        /// A list of AuthorProfile.
        /// </returns>
        public override List<AuthorProfile> FillProfiles()
        {
            var profiles = new List<AuthorProfile>();
            var blogs = new List<Blog>();

            if (Blog.CurrentInstance.IsSiteAggregation)
                blogs = Blog.Blogs;
            else
                blogs.Add(Blog.CurrentInstance);

            foreach (Blog blog in blogs)
            {
                var folder = $"{GetFolder(blog)}profiles{Path.DirectorySeparatorChar}";

                if (!Directory.Exists(folder))
                    continue;

                profiles.AddRange(from file in Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly)
                                  select new FileInfo(file)
                                  into info
                                  select info.Name.Replace(".xml", string.Empty)
                                  into username
                                  select AuthorProfile.Load(username));
            }

            return profiles;
        }

        /// <summary>
        /// The insert profile.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public override void InsertProfile(AuthorProfile profile)
        {
            if (!Directory.Exists($"{GetFolder(profile.Blog)}profiles"))
            {
                Directory.CreateDirectory($"{GetFolder(profile.Blog)}profiles");
            }

            var fileName = $"{GetFolder(profile.Blog)}profiles{Path.DirectorySeparatorChar}{profile.Id}.xml";
            var settings = new XmlWriterSettings { Indent = true };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("profileData");

                writer.WriteElementString("DisplayName", profile.DisplayName);
                writer.WriteElementString("FirstName", profile.FirstName);
                writer.WriteElementString("MiddleName", profile.MiddleName);
                writer.WriteElementString("LastName", profile.LastName);

                writer.WriteElementString("CityTown", profile.CityTown);
                writer.WriteElementString("RegionState", profile.RegionState);
                writer.WriteElementString("Country", profile.Country);

                writer.WriteElementString("Birthday", profile.Birthday.ToString("yyyy-MM-dd"));
                writer.WriteElementString("AboutMe", profile.AboutMe);
                writer.WriteElementString("PhotoURL", profile.PhotoUrl);

                writer.WriteElementString("Company", profile.Company);
                writer.WriteElementString("EmailAddress", profile.EmailAddress);
                writer.WriteElementString("PhoneMain", profile.PhoneMain);
                writer.WriteElementString("PhoneMobile", profile.PhoneMobile);
                writer.WriteElementString("PhoneFax", profile.PhoneFax);

                writer.WriteElementString("IsPrivate", profile.Private.ToString());

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Retrieves a Page from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>An AuthorProfile.</returns>
        public override AuthorProfile SelectProfile(string id)
        {
            var profile = new AuthorProfile(id);

            if (Blog.CurrentInstance.IsSiteAggregation)
            {
                foreach (Blog blog in Blog.Blogs)
                {
                    if (blog.IsActive && !blog.IsDeleted)
                    {
                        profile = SelectProfile(id, blog);

                        if (profile != null)
                            return profile;
                    }
                }
            }
            else
            {
                return SelectProfile(id, Blog.CurrentInstance);
            }

            return profile;
        }

        /// <summary>
        /// The update profile.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public override void UpdateProfile(AuthorProfile profile)
        {
            this.InsertProfile(profile);
        }

        #endregion

        AuthorProfile SelectProfile(string id, Blog blog)
        {
            var fileName = $"{GetFolder(blog)}profiles{Path.DirectorySeparatorChar}{id}.xml";

            if (blog.IsSiteAggregation && !blog.IsPrimary)
                fileName = Path.Combine(BlogConfig.StorageLocation, "blogs", blog.Name, "profiles", id + ".xml");

            var doc = new XmlDocument();

            if (!File.Exists(fileName))
            {
                Utils.Log($"XmlBlogProvider: can not load profile from \"{fileName}\"");
                return null;
            }

            doc.Load(fileName);

            var profile = new AuthorProfile(id);

            if (doc.SelectSingleNode("//DisplayName") != null)
            {
                profile.DisplayName = doc.SelectSingleNode("//DisplayName").InnerText;
            }

            if (doc.SelectSingleNode("//FirstName") != null)
            {
                profile.FirstName = doc.SelectSingleNode("//FirstName").InnerText;
            }

            if (doc.SelectSingleNode("//MiddleName") != null)
            {
                profile.MiddleName = doc.SelectSingleNode("//MiddleName").InnerText;
            }

            if (doc.SelectSingleNode("//LastName") != null)
            {
                profile.LastName = doc.SelectSingleNode("//LastName").InnerText;
            }

            if (doc.SelectSingleNode("//CityTown") != null)
            {
                profile.CityTown = doc.SelectSingleNode("//CityTown").InnerText;
            }

            if (doc.SelectSingleNode("//RegionState") != null)
            {
                profile.RegionState = doc.SelectSingleNode("//RegionState").InnerText;
            }

            if (doc.SelectSingleNode("//Country") != null)
            {
                profile.Country = doc.SelectSingleNode("//Country").InnerText;
            }

            if (doc.SelectSingleNode("//Birthday") != null)
            {
                DateTime date;
                if (DateTime.TryParse(doc.SelectSingleNode("//Birthday").InnerText, out date))
                {
                    profile.Birthday = date;
                }
            }

            if (doc.SelectSingleNode("//AboutMe") != null)
            {
                profile.AboutMe = doc.SelectSingleNode("//AboutMe").InnerText;
            }

            if (doc.SelectSingleNode("//PhotoURL") != null)
            {
                profile.PhotoUrl = doc.SelectSingleNode("//PhotoURL").InnerText;
            }

            if (doc.SelectSingleNode("//Company") != null)
            {
                profile.Company = doc.SelectSingleNode("//Company").InnerText;
            }

            if (doc.SelectSingleNode("//EmailAddress") != null)
            {
                profile.EmailAddress = doc.SelectSingleNode("//EmailAddress").InnerText;
            }

            if (doc.SelectSingleNode("//PhoneMain") != null)
            {
                profile.PhoneMain = doc.SelectSingleNode("//PhoneMain").InnerText;
            }

            if (doc.SelectSingleNode("//PhoneMobile") != null)
            {
                profile.PhoneMobile = doc.SelectSingleNode("//PhoneMobile").InnerText;
            }

            if (doc.SelectSingleNode("//PhoneFax") != null)
            {
                profile.PhoneFax = doc.SelectSingleNode("//PhoneFax").InnerText;
            }

            if (doc.SelectSingleNode("//IsPrivate") != null)
            {
                profile.Private = doc.SelectSingleNode("//IsPrivate").InnerText == "true";
            }
            return profile;
        }
    }
}