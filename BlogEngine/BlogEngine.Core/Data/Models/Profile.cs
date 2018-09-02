/// <summary>
/// Mirrors AuthorProfile
/// </summary>
namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Author profile for json serialization
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// biography
        /// </summary>
        public string AboutMe { get; set; }

        /// <summary>
        /// birthday date
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// city
        /// </summary>
        public string CityTown { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// email address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// middle name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// fax
        /// </summary>
        public string PhoneFax { get; set; }

        /// <summary>
        /// main phone
        /// </summary>
        public string PhoneMain { get; set; }

        /// <summary>
        /// mobile phone
        /// </summary>
        public string PhoneMobile { get; set; }

        /// <summary>
        /// photo url
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// private/public
        /// </summary>
        public bool Private { get; set; }

        /// <summary>
        /// state
        /// </summary>
        public string RegionState { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
    }
}
