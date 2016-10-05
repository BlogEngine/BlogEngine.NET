namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Security.Permissions;

    /// <summary>
    /// A storage provider for BlogEngine that uses XML files.
    ///     <remarks>
    /// To build another provider, you can just copy and modify
    ///         this one. Then add it to the web.config's BlogEngine section.
    ///     </remarks>
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        /// <summary>
        /// Returns a dictionary of right names and the roles associated with them.
        /// </summary>
        /// <returns></returns>
        public override IDictionary<string, IEnumerable<string>> FillRights()
        {
            var fullyQualifiedPath = $"{Folder}rights.xml";

            //var fullyQualifiedPath =
            //    VirtualPathUtility.Combine(
            //        VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath), path);

            //fullyQualifiedPath = HostingEnvironment.MapPath(fullyQualifiedPath);

            var rightsDict = new Dictionary<string, IEnumerable<string>>();

            if (File.Exists(fullyQualifiedPath))
            {
                // Make sure we have permission to read the XML data source and
                // throw an exception if we don't
                var permission = new FileIOPermission(FileIOPermissionAccess.Write, fullyQualifiedPath);
                permission.Demand();

                var doc = new XmlDocument();
                doc.Load(fullyQualifiedPath);


                foreach (XmlNode rightNode in doc.GetElementsByTagName("right"))
                {
                    var rightName = rightNode.Attributes["name"].Value;
                    var roleList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (XmlNode roleNode in rightNode.ChildNodes)
                    {
                        if (roleNode.Name.Equals("role", StringComparison.OrdinalIgnoreCase))
                        {
                            var roleName = roleNode.Attributes["name"].Value;
                            roleName = roleName.Trim();
                            roleList.Add(roleName);
                        }
                    }

                    rightsDict.Add(rightName, roleList);
                }

            }

       
            return rightsDict;

        }

        /// <summary>
        /// Saves the current BlogEngine rights.
        /// </summary>
        /// <param name="rights"></param>
        public override void SaveRights(IEnumerable<Right> rights)
        {
            var fileName = $"{Folder}rights.xml";

            var settings = new XmlWriterSettings { Indent = true };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("rights");

                foreach (var right in rights)
                {
                    writer.WriteStartElement("right");
                    writer.WriteAttributeString("name", right.Name);

                    foreach (var role in right.Roles)
                    {
                        writer.WriteStartElement("role");
                        writer.WriteAttributeString("name", role);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }


                writer.WriteEndElement();
            }


        }

    }
}
