namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Configuration;
    using System.Xml;

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
        /// Loads the settings from the provider.
        /// </summary>
        /// <returns>A StringDictionary.</returns>
        public override StringDictionary LoadSettings(Blog blog)
        {
            var filename = HttpContext.Current.Server.MapPath($"{blog.StorageLocation}settings.xml");               
            var dic = new StringDictionary();

            var doc = new XmlDocument();
            doc.Load(filename);

            var settings = doc.SelectSingleNode("settings");
            if (settings != null)
            {
                foreach (XmlNode settingsNode in settings.ChildNodes)
                {
                    var name = settingsNode.Name;
                    var value = settingsNode.InnerText;

                    dic.Add(name, value);
                }
            }

            return dic;
        }

        /// <summary>
        /// Saves the settings to the provider.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public override void SaveSettings(StringDictionary settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var filename = string.Format("{0}settings.xml", Folder);
            var writerSettings = new XmlWriterSettings { Indent = true };

            // ------------------------------------------------------------
            // Create XML writer against file path
            // ------------------------------------------------------------
            using (var writer = XmlWriter.Create(filename, writerSettings))
            {
                writer.WriteStartElement("settings");

                foreach (string key in settings.Keys)
                {
                    writer.WriteElementString(key, settings[key]);
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}