namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
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
        /// Loads the ping services.
        /// </summary>
        /// <returns>A StringCollection.</returns>
        public override StringCollection LoadPingServices()
        {
            var fileName = this.Folder + "pingservices.xml";
            if (!File.Exists(fileName))
            {
                return new StringCollection();
            }

            var col = new StringCollection();
            var doc = new XmlDocument();
            doc.Load(fileName);

            foreach (XmlNode node in
                doc.SelectNodes("services/service").Cast<XmlNode>().Where(node => !col.Contains(node.InnerText)))
            {
                col.Add(node.InnerText);
            }

            return col;
        }

        /// <summary>
        /// Saves the ping services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public override void SavePingServices(StringCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            var fileName = this.Folder + "pingservices.xml";

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("services");

                foreach (var service in services)
                {
                    writer.WriteElementString("service", service);
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}