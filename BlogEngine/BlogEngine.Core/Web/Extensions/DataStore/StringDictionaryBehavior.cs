namespace BlogEngine.Core.DataStore
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Xml.Serialization;

    using BlogEngine.Core.Providers;

    /// <summary>
    /// The string dictionary behavior.
    /// </summary>
    internal class StringDictionaryBehavior : ISettingsBehavior
    {
        #region Constants and Fields

        /// <summary>
        /// The section.
        /// </summary>
        private static readonly BlogProviderSection Section =
            (BlogProviderSection)ConfigurationManager.GetSection("BlogEngine/blogProvider");

        #endregion

        #region Implemented Interfaces

        #region ISettingsBehavior

        /// <summary>
        /// Retreaves StringDictionary object from database or file system
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        /// <returns>
        /// StringDictionary object as Stream
        /// </returns>
        public object GetSettings(ExtensionType extensionType, string extensionId)
        {
            SerializableStringDictionary ssd;
            var sd = new StringDictionary();
            var serializer = new XmlSerializer(typeof(SerializableStringDictionary));

            if (Section.DefaultProvider == "XmlBlogProvider")
            {
                var stm = (Stream)BlogService.LoadFromDataStore(extensionType, extensionId);
                if (stm != null)
                {
                    ssd = (SerializableStringDictionary)serializer.Deserialize(stm);
                    stm.Close();
                    sd = ssd;
                }
            }
            else
            {
                var o = BlogService.LoadFromDataStore(extensionType, extensionId);
                if (!string.IsNullOrEmpty((string)o))
                {
                    using (var reader = new StringReader((string)o))
                    {
                        ssd = (SerializableStringDictionary)serializer.Deserialize(reader);
                    }

                    sd = ssd;
                }
            }

            return sd;
        }

        /// <summary>
        /// Saves String Dictionary to Data Store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        /// <param name="settings">
        /// StringDictionary settings
        /// </param>
        /// <returns>
        /// The save settings.
        /// </returns>
        public bool SaveSettings(ExtensionType extensionType, string extensionId, object settings)
        {
            var sd = (StringDictionary)settings;
            var ssd = new SerializableStringDictionary();

            foreach (DictionaryEntry de in sd)
            {
                ssd.Add(de.Key.ToString(), de.Value.ToString());
            }

            BlogService.SaveToDataStore(extensionType, extensionId, ssd);
            return true;
        }

        #endregion

        #endregion
    }
}