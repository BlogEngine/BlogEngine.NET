// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   A storage provider for BlogEngine that uses XML files.
//   <remarks>
//   To build another provider, you can just copy and modify
//   this one. Then add it to the web.config's BlogEngine section.
//   </remarks>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Providers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using System.Xml.Serialization;

    using BlogEngine.Core.DataStore;

    /// <summary>
    /// A storage provider for BlogEngine that uses XML files.
    ///     <remarks>
    /// To build another provider, you can just copy and modify
    ///         this one. Then add it to the web.config's BlogEngine section.
    ///     </remarks>
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// Loads settings from generic data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <returns>
        /// Stream Settings
        /// </returns>
        public override object LoadFromDataStore(ExtensionType extensionType, string extensionId)
        {
            Stream str = null;
            if (!Directory.Exists(StorageLocation(extensionType)))
            {
                Directory.CreateDirectory(StorageLocation(extensionType));
            }

            string fileName = ExtensionLocation(extensionType, extensionId);
            if (File.Exists(fileName))
            {
                var reader = new StreamReader(fileName);
                str = reader.BaseStream;
            }

            return str;
        }

        /// <summary>
        /// Removes settings from data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        public override void RemoveFromDataStore(ExtensionType extensionType, string extensionId)
        {
            string fileName = ExtensionLocation(extensionType, extensionId);
            File.Delete(fileName);
        }

        /// <summary>
        /// Save settings to generic data store
        /// </summary>
        /// <param name="extensionType">
        /// Type of extension
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <param name="settings">
        /// Stream Settings
        /// </param>
        public override void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings)
        {
            if (!Directory.Exists(StorageLocation(extensionType)))
            {
                Directory.CreateDirectory(StorageLocation(extensionType));
            }

            string fileName = ExtensionLocation(extensionType, extensionId);
            using (TextWriter writer = new StreamWriter(fileName))
            {
                var x = new XmlSerializer(settings.GetType());
                x.Serialize(writer, settings);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Data Store Location
        /// </summary>
        /// <param name="extensionType">
        /// Type of extension
        /// </param>
        /// <returns>
        /// Path to storage directory
        /// </returns>
        private static string StorageLocation(ExtensionType extensionType)
        {
            string result;
            switch (extensionType)
            {
                case ExtensionType.Extension:
                    result = Path.Combine(Blog.CurrentInstance.StorageLocation, "datastore", "extensions");
                    break;
                case ExtensionType.Widget:
                    result = Path.Combine(Blog.CurrentInstance.StorageLocation, "datastore", "widgets");
                    break;
                case ExtensionType.Theme:
                    result = Path.Combine(Blog.CurrentInstance.StorageLocation, "datastore", "themes");
                    break;
                default:
                    throw new NotSupportedException(string.Format("Unknown extension type: {0}", extensionType));
            }

            string mappedResult = HostingEnvironment.MapPath(result);
            if (string.IsNullOrEmpty(mappedResult) && result.StartsWith(BlogConfig.DefaultStorageLocation))
            {
                // this can only happen in Mono. We'll try again with AppDomain but it will only work if BlogConfig.StorageLocation == "~/App_Data/" (which is the default value)
				string appDataPhysical = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
				mappedResult = Path.Combine(appDataPhysical, result.Substring(BlogConfig.DefaultStorageLocation.Length));
            }

            if (string.IsNullOrEmpty(mappedResult))
            {
                throw new InvalidOperationException(string.Format("Could not map folder {0} for extension type {1}", result, extensionType));
            }

            return mappedResult;
        }

        /// <summary>
        /// Data Store Location for an extension.
        /// </summary>
        /// <param name="extensionType">
        /// Type of extension
        /// </param>
        /// <param name="extensionId">The id of the extension</param>
        /// <returns>
        /// XML file of extension data.
        /// </returns>
        private static string ExtensionLocation(ExtensionType extensionType, string extensionId)
        {
            return Path.Combine(StorageLocation(extensionType), string.Format("{0}.xml", extensionId));
        }

        #endregion
    }
}