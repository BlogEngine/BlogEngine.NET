using Newtonsoft.Json.Linq;
using System.Diagnostics;
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
        public string RecordId { get; set; }

        /// <summary>
        /// biography
        /// </summary>
        public string AboutMe { get; set; }

        /// <summary>
        /// birthday date
        /// </summary>
        public string Birthday { get; set; }

        public string Address { get; set; }

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

        public Profile() {
            Debug.WriteLine("");
        }

        public Profile(string comment)
        {
            var record = JObject.Parse(comment);
            EmailAddress = GetField(record, "Email");
            Address = GetField(record, "Address");
            CityTown = GetField(record, "City");
            RegionState = GetField(record, "State"); 
            DisplayName = GetField(record, "Name");
            LastName = GetField(record, "LastName");
            FirstName = GetField(record, "FirstName");
            Company = GetField(record, "Company");
            if (Company == "") GetField(record, "Organization");
            PhoneMain = GetField(record, "HomePhone");
            if (PhoneMain == "") PhoneMain = "NA";
            PhoneMobile = GetField(record, "CellPhone");
            if (PhoneMobile == "") PhoneMobile = "NA";
            RecordId = GetField(record, "ContactId");
            Debug.WriteLine("");

        }

        private string GetField(JObject record, string field)
        {
            var result = record[field];
            if (result == null) return "";
            return record[field].ToString();
        }
    }

}
