using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using BlogEngine.Core.Packaging;

namespace BlogEngine.Core.Providers
{
    public partial class XmlBlogProvider : BlogProvider
    {
        string dataFolder = HttpContext.Current.Server.MapPath(BlogConfig.StorageLocation);

        /// <summary>
        /// Log of all installed packages
        /// </summary>
        /// <param name="package">Intalled package</param>
        public override void SavePackage(InstalledPackage package)
        {
            var fileName = Path.Combine(dataFolder, Constants.InstalledPackagesXml);
            
            var packages = FillPackages() ?? new List<InstalledPackage>();
            int pkgIndex = -1;

            for (int index = 0; index < packages.Count; index++)
            {
                var p = packages[index];
                if (p.PackageId == package.PackageId)
                {
                    pkgIndex = index;
                    break;
                }
            }

            if(pkgIndex >= 0)
                packages.RemoveAt(pkgIndex);

            packages.Add(package);

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("InstalledPackages");

                foreach (var p in packages)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", p.PackageId);
                    writer.WriteAttributeString("version", p.Version);
                    writer.WriteEndElement();
                }
                    
                writer.WriteEndElement();
            }

        }

        /// <summary>
        /// Log of all files for installed package
        /// </summary>
        /// <param name="packageFiles">List of intalled package files</param>
        public override void SavePackageFiles(List<PackageFile> packageFiles)
        {
            if(packageFiles == null || packageFiles.Count == 0)
                return;

            var fileName = dataFolder + Constants.InstalledPackageFilesXml;
            var pkgId = packageFiles.First().PackageId;

            var installedFiles = AllInstalledFiles(pkgId);

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("InstalledPackageFiles");

                // write to file old package files
                foreach (var f in installedFiles)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", f.PackageId);
                    writer.WriteAttributeString("fileorder", f.FileOrder.ToString());
                    writer.WriteAttributeString("filepath", f.FilePath);
                    writer.WriteAttributeString("isdirectory", f.IsDirectory.ToString());
                    writer.WriteEndElement();
                }

                // add new package files
                foreach (var pf in packageFiles)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", pf.PackageId);
                    writer.WriteAttributeString("fileorder", pf.FileOrder.ToString());
                    writer.WriteAttributeString("filepath", pf.FilePath);
                    writer.WriteAttributeString("isdirectory", pf.IsDirectory.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Gets all installed from gallery packages
        /// </summary>
        /// <returns>List of installed packages</returns>
        public override List<InstalledPackage> FillPackages()
        {
            var fileName = Path.Combine(dataFolder, Constants.InstalledPackagesXml);

            if (!File.Exists(fileName))
            {
                return null;
            }

            var doc = new XmlDocument();
            doc.Load(fileName);

            return (from XmlNode node in doc.SelectNodes("InstalledPackages/item")
                select new InstalledPackage
                {
                    PackageId = node.Attributes["id"].InnerText,
                    Version = node.Attributes["version"].InnerText
                }).ToList();
        }

        /// <summary>
        /// Gets list of files for installed package
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <returns>List of files for installed package</returns>
        public override List<PackageFile> FillPackageFiles(string packageId)
        {
            var fileName = Path.Combine(dataFolder, Constants.InstalledPackageFilesXml);
            var packageFiles = new List<PackageFile>();

            if (!File.Exists(fileName))
            {
                return null;
            }

            var doc = new XmlDocument();
            doc.Load(fileName);

            var nodes = doc.SelectNodes("InstalledPackageFiles/item");

            if(nodes != null && nodes.Count > 0)
            {
                packageFiles.AddRange(from XmlNode node in nodes
                    where node.Attributes != null && node.Attributes.Count > 0
                    where node.Attributes["id"].InnerText == packageId
                    select new PackageFile
                    {
                        PackageId = node.Attributes["id"].InnerText, 
                        FileOrder = int.Parse(node.Attributes["fileorder"].InnerText), 
                        FilePath = node.Attributes["filepath"].InnerText, 
                        IsDirectory = bool.Parse(node.Attributes["isdirectory"].InnerText)
                    });
            }

            return packageFiles;
        }

        /// <summary>
        /// Should delete package and remove all package files
        /// </summary>
        /// <param name="packageId">Package ID</param>
        public override void DeletePackage(string packageId)
        {        
            var files = Path.Combine(dataFolder, Constants.InstalledPackageFilesXml);
            var packages = Path.Combine(dataFolder, Constants.InstalledPackagesXml);
            var xmlDoc = new XmlDocument();

            // remove files from packagefiles.xml
            
            xmlDoc.Load(files);
            var packageNodes = xmlDoc.SelectNodes("InstalledPackageFiles/item");

            if(packageNodes != null && packageNodes.Count > 0)
            {
                for (int i = 0; i < packageNodes.Count; i++)
                {
                    if (packageNodes[i].Attributes != null && packageNodes[i].Attributes["id"] != null)
                    {
                        if (packageId == packageNodes[i].Attributes["id"].InnerText)
                        {
                            if (packageNodes[i].ParentNode != null)
                                packageNodes[i].ParentNode.RemoveChild(packageNodes[i]);
                        }
                    }
                }
            }
            xmlDoc.Save(files);

            // remove packages from packages.xml

            xmlDoc = new XmlDocument();
            xmlDoc.Load(packages);
            var pkg = xmlDoc.SelectSingleNode("//InstalledPackages/item[@id='" + packageId + "']");

            if (pkg != null && pkg.ParentNode != null)
                pkg.ParentNode.RemoveChild(pkg);

            xmlDoc.Save(packages);
        }

        private IEnumerable<PackageFile> AllInstalledFiles(string pkgId)
        {
            var allInstalledFiles = new List<PackageFile>();
            var packages = FillPackages();

            if (packages != null && packages.Count > 0)
            {
                foreach (var p in packages)
                {
                    if (p.PackageId != pkgId)
                    {
                        var packageFiles = FillPackageFiles(p.PackageId);
                        if (packageFiles != null && packageFiles.Count > 0)
                        {
                            foreach (var pf in packageFiles)
                            {
                                allInstalledFiles.Add(pf);
                            }
                        }
                    }
                }
            }
            return allInstalledFiles;
        }

    }
}
