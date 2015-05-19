using System;
namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Class that represents custom field
    /// </summary>
    public class CustomField
    {
        /// <summary>
        /// Custom type, for example "post" or "theme"
        /// </summary>
        public string CustomType { get; set; }
        /// <summary>
        /// Object ID, for example post ID or theme name
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// The blog ID
        /// </summary>
        public Guid BlogId { get; set; }
        /// <summary>
        /// The key in the key/value pair
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The value. Can be simple string or string
        /// representation of object that client can parse
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// custom meta data like "hidden", "style" etc.
        /// </summary>
        public string Attribute { get; set; }
        /// <summary>
        /// Used to assign key to HTML controls when edit field in admin
        /// </summary>
        public string ControlId
        {
            get
            {
                return Utils.RemoveIllegalCharacters(Key);
            }
        }
    }
}