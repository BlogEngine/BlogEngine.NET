using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Security;
using System.Xml.Serialization;

namespace BlogEngine.Core
{
    /// <summary>
    /// This business object is to handle the profiles of users
    /// </summary>
    [XmlRoot("profile")]
    public class Profile : BusinessBase<Profile, string>
    {
        #region Fields

        private string aboutMe;
        private string birthDate;
        private string cityState;
        private string country;
        private string displayName;
        private string firstName;
        private string gender;
        private string interests;
        private bool isPrivate;
        private string lastName;
        private string photoURL;
        private string regionState;
        private string userName;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("displayName", DataType = "string")]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("interests", DataType = "string")]
        public string Interests
        {
            get { return interests; }
            set { interests = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("userName", DataType = "string")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("aboutme", DataType = "string")]
        public string AboutMe
        {
            get { return aboutMe; }
            set { aboutMe = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("firstName", DataType = "string")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("lastName", DataType = "string")]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("isPrivate", DataType = "bool")]
        public bool IsPrivate
        {
            get { return isPrivate; }
            set { isPrivate = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("photoURL", DataType = "string")]
        public string PhotoURL
        {
            get { return photoURL; }
            set { photoURL = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("gender", DataType = "string")]
        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("birthDate", DataType = "string")]
        public string BirthDate
        {
            get { return birthDate; }
            set { birthDate = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("cityState", DataType = "string")]
        public string CityState
        {
            get { return cityState; }
            set { cityState = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("regionState", DataType = "string")]
        public string RegionState
        {
            get { return regionState; }
            set { regionState = value; }
        }

        ///<summary>
        ///</summary>
        [XmlElement("country", DataType = "string")]
        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        #endregion

        #region Helpers

        ///<summary>
        ///</summary>
        ///<param name="username"></param>
        ///<returns></returns>
        public static Profile GetProfile(string username)
        {
            Profile profile = new Profile();
            try
            {
                // Deserialize the specified file to a Theater object.
                XmlSerializer xs = new XmlSerializer(typeof (Profile));
                FileStream fs =
                    new FileStream(BlogSettings.Instance.StorageLocation + username.ToLowerInvariant() + ".xml",
                                   FileMode.Open);
                profile = (Profile) xs.Deserialize(fs);
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception: " + x.Message);
            }
            return profile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userProfile"></param>
        public static void SaveProfile(Profile userProfile)
        {
            try
            {
                // Serialize the Profile object to an XML file.
                XmlSerializer xs = new XmlSerializer(typeof (Profile));
                FileStream fs =
                    new FileStream(
                        BlogSettings.Instance.StorageLocation + userProfile.userName.ToLowerInvariant() + ".xml",
                        FileMode.Create);
                xs.Serialize(fs, userProfile);
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception: " + x.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Profile> GetProfiles()
        {
            List<Profile> profiles = new List<Profile>();
            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                Profile userProfile = GetProfile(user.UserName);
                profiles.Add(userProfile);
            }
            return profiles;
        }

        #endregion

        #region businessBase

        /// <summary>
        /// Reinforces the business rules by adding additional rules to the 
        /// broken rules collection.
        /// </summary>
        /// 
        /// 
        protected override void ValidationRules()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the object from the data store and populates it.
        /// </summary>
        /// <param name="id">The unique identifier of the object.</param>
        /// <returns>True if the object exists and is being populated successfully</returns>
        protected override Profile DataSelect(string id)
        {
            Profile profile = GetProfile(id);
            return profile;
        }

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected override void DataUpdate()
        {
            SaveProfile(this);
        }

        /// <summary>
        /// Inserts a new object to the data store.
        /// </summary>
        protected override void DataInsert()
        {
            SaveProfile(this);
        }

        /// <summary>
        /// Deletes the object from the data store.
        /// </summary>
        protected override void DataDelete()
        {
            string path = BlogSettings.Instance.StorageLocation + userName.ToLowerInvariant() + ".xml";
            if (IsDeleted)
            {
                try
                {
                    File.Delete(path);
                    Dispose();
                }
                catch (FileNotFoundException)
                {
                    throw new FileNotFoundException("File was not found", path);
                }
                catch (FieldAccessException e)
                {
                    throw new FieldAccessException("File was not found", e.InnerException);
                }
            }
        }

        #endregion
    }
}