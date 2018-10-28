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

        public string UserId { get; set; }

        public string Information { get; set; }

        public string Title { get; set; }

        public string DisplayName { get; set; }     // *

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string OtherName { get; set; }

        public string FirstName { get; set; }       // *

        public string LastName { get; set; }        // *

        public string MiddleName { get; set; }      // *

        public string Spouse { get; set; }          // *

        public string AboutMe { get; set; }

        public string PhotoUrl { get; set; }        // *

        public string Birthday { get; set; }        // *

        public string EmailAddress { get; set; }    // *

        public string Address { get; set; }

        public string AddressAlt { get; set; }

        public string CityTown { get; set; }        // *

        public string RegionState { get; set; }     // *

        public string Zip { get; set; }

        public string Country { get; set; }         // *

        public string WorkPhone { get; set; }

        public string PhoneFax { get; set; }        // *

        public string PhoneMobile { get; set; }     // *

        public string PhoneMain { get; set; }       // *

        public string Company { get; set; }         // *

        public string Organization { get; set; }

        public string Website { get; set; }

        public string NewsLetter { get; set; }
               
        public string Code { get; set; }

        public bool Private { get; set; }           // *


        public Profile() {
        }

        public Profile(AuthorProfile authorProfile)
        {
            var srcProps = authorProfile.GetType().GetProperties();
            var dstProps = this.GetType().GetProperties();

            foreach(var source in srcProps)
            {
                // Ignore baseclass properties
                if (source.DeclaringType.Name != nameof(AuthorProfile))
                    continue;

                var name = source.Name;
                var value = source.GetValue(authorProfile);

                foreach(var dest in dstProps)
                {
                    if (dest.Name == name)
                    {
                        if (dest.PropertyType.Name == "Boolean")
                            dest.SetValue(this, value);
                        else
                            dest.SetValue(this, $"{value}");
                        break;
                    }
                }
            }
        }

        public Profile(string comment)
        {

            // TODO: use database field names here since we did a select JSON (what we're passing in)
            //       ensure we don't also pass in a comment string that contains RecordId
            var record = JObject.Parse(comment);

            var props = this.GetType().GetProperties();
            foreach(var prop in props)
            {
                if (prop.DeclaringType.Name != "Profile")
                    continue;

                var name = prop.Name;
                var type = prop.PropertyType.Name;

                switch (type)
                {
                    case "Boolean":
                        prop.SetValue(this, GetBoolean(GetField(record, name)));
                        break;
                    default:
                        prop.SetValue(this, GetField(record, name));
                        break;
                }
            }
        }

        private bool GetBoolean(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            bool retValue;
            bool.TryParse(value, out retValue);
            return retValue;
        }

        private string GetField(JObject record, string field)
        {
            var result = record[field];
            if (result == null) return null;
            return record[field].ToString();
        }
    }

}
