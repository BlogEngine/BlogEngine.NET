using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Providers;
using BlogEngine.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Class for packaging IO
    /// </summary>
    public class FileSystem
    {
        private static int fileOrder;

        /// <summary>
        /// Load themes from /themes directory
        /// </summary>
        /// <returns>Lit of installed themes</returns>
        public static List<Package> LoadThemes()
        {
            try
            {
                return GetThemes();
            }
            catch (Exception ex)
            {
                Utils.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                return null;
            }
        }

        /// <summary>
        /// Load widgets from /widgets directory
        /// </summary>
        /// <returns>List of installed widgets</returns>
        public static List<Package> LoadWidgets()
        {
            try
            {
                return GetWidgets();
            }
            catch (Exception ex)
            {
                Utils.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                return null;
            }
        }

        /// <summary>
        /// Load extensions from extensions manager
        /// </summary>
        /// <returns>List of installed extensions</returns>
        public static List<Package> LoadExtensions()
        {
            var extensions = ExtensionManager.Extensions.Where(x => x.Key != "MetaExtension").ToList();
            var packages = new List<Package>();

            foreach (KeyValuePair<string, ManagedExtension> ext in extensions)
            {
                var x = ExtensionManager.GetExtension(ext.Key);
                var adminPage = string.IsNullOrEmpty(x.AdminPage) ?
                string.Format(Utils.RelativeWebRoot + "admin/Extensions/Settings.aspx?ext={0}&enb={1}", x.Name, x.Enabled) :
                string.Format(x.AdminPage, x.Name, x.Enabled);

                var onlineVersion = GetInstalledVersion(x.Name);
                var p = new Package
                {
                    Id = x.Name,
                    PackageType = "Extension",
                    Title = x.Name,
                    Description = x.Description,
                    LocalVersion = x.Version,
                    OnlineVersion = onlineVersion,
                    Authors = x.Author,
                    IconUrl = string.Format("{0}Content/images/blog/ext.png", Utils.ApplicationRelativeWebRoot),
                    Enabled = x.Enabled,
                    Priority = x.Priority,
                    SettingsUrl = x.Settings.Count > 0 ? adminPage : ""
                };
                if (!string.IsNullOrEmpty(onlineVersion))
                {
                    var extra = Gallery.GetPackageExtra(x.Name + "." + onlineVersion);
                    p.DownloadCount = extra.DownloadCount;
                    p.Rating = extra.Rating;
                }
                packages.Add(p);
            }
            return packages;
        }

        /// <summary>
        /// Copy uncompressed package files
        /// to application directories
        /// </summary>
        /// <param name="package">Package</param>
        public static List<PackageFile> InstallPackage(NuGet.IPackage package)
        {
            var packageFiles = new List<PackageFile>();

            var content = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot +
                string.Format("App_Data/packages/{0}.{1}/content", package.Id, package.Version));

            var lib = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot +
                string.Format("App_Data/packages/{0}.{1}/lib", package.Id, package.Version));

            var root = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot);
            var bin = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot + "bin");

            // copy content files
            var source = new DirectoryInfo(content);
            var target = new DirectoryInfo(root);
            
            fileOrder = 0;

            if (Directory.Exists(content))
            {
                CopyDirectory(source, target, package.Id, packageFiles);

                CreateManifestIfNotExists(package, packageFiles);

                // clear after install
                ForceDeleteDirectory(content);
            }

            // copy DLLs from lib to bin
            if (Directory.Exists(lib))
            {
                source = new DirectoryInfo(lib);
                target = new DirectoryInfo(bin);

                fileOrder = 0;
                CopyDirectory(source, target, package.Id, packageFiles);

                // clear after install
                ForceDeleteDirectory(lib);
            }

            return packageFiles;
        }

        /// <summary>
        /// Remove package files
        /// </summary>
        /// <param name="pkgId">Package Id</param>
        public static void UninstallPackage(string pkgId)
        {
            var installedFiles = BlogService.InstalledFromGalleryPackageFiles(pkgId);

            if (installedFiles.Count == 0)
            {
                Utils.Log(string.Format("Can not find any files installed for package: {0}", pkgId));
                throw new ApplicationException("No files to uninstall");
            }

            var repo = new BlogEngine.Core.Data.PackageRepository();
            var pkg = repo.FindById(pkgId);
            
            foreach (var file in installedFiles.OrderByDescending(f => f.FileOrder))
            {
                var fullPath = HttpContext.Current.Server.MapPath(Path.Combine(Utils.RelativeWebRoot, file.FilePath));

                if(file.IsDirectory)
                {
                    var folder = new DirectoryInfo(fullPath);
                    if (folder.Exists)
                    {
                        if(folder.GetFileSystemInfos().Length == 0)
                        {
                            ForceDeleteDirectory(fullPath);
                        }
                        else
                        {
                            Utils.Log(string.Format("Package Uninstaller: can not remove directory if it is not empty ({0})", fullPath));
                        }
                    }

                }
                else if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            if (pkg != null && !string.IsNullOrWhiteSpace(pkg.OnlineVersion))
            {
                var pkgDir = string.Format("{0}.{1}", pkgId, pkg.OnlineVersion);

                // clean up removing installed version
                pkgDir = HttpContext.Current.Server.MapPath(string.Format("{0}App_Data/packages/{1}", Utils.ApplicationRelativeWebRoot, pkgDir));
                if (Directory.Exists(pkgDir))
                {
                    ForceDeleteDirectory(pkgDir);
                }
            }
        }

        #region Private methods

        static List<Package> GetThemes()
        {
            var installedThemes = new List<Package>();
            var path = HttpContext.Current.Server.MapPath(string.Format("{0}Custom/Themes/", Utils.ApplicationRelativeWebRoot));

            foreach (var p in from d in Directory.GetDirectories(path)
                let index = d.LastIndexOf(Path.DirectorySeparatorChar) + 1
                select d.Substring(index)
                into themeId select GetPackageManifest(themeId, Constants.Theme) ?? 
                new Package {Id = themeId, PackageType = Constants.Theme}
                into p where p.Id != "RazorHost" select p)
            {
                if (string.IsNullOrEmpty(p.IconUrl))
                    p.IconUrl = DefaultIconUrl(p);

                if (string.IsNullOrEmpty(p.Title))
                    p.Title = p.Id;

                p.OnlineVersion = GetInstalledVersion(p.Id);

                if (!string.IsNullOrEmpty(p.OnlineVersion))
                {
                    var extra = Gallery.GetPackageExtra(p.Id + "." + p.OnlineVersion);
                    if (extra != null)
                    {
                        p.DownloadCount = extra.DownloadCount;
                        p.Rating = extra.Rating;
                    }
                }

                installedThemes.Add(p);
            }
            return installedThemes;
        }

        static List<Package> GetWidgets()
        {
            var installedWidgets = new List<Package>();
            var path = HttpContext.Current.Server.MapPath(string.Format("{0}Custom/Widgets/", Utils.ApplicationRelativeWebRoot));

            foreach (var p in from d in Directory.GetDirectories(path)
                let index = d.LastIndexOf(Path.DirectorySeparatorChar) + 1
                select d.Substring(index)
                into widgetId
                select GetPackageManifest(widgetId, Constants.Widget) ??
                new Package { Id = widgetId, PackageType = Constants.Widget })
            {
                if (string.IsNullOrEmpty(p.IconUrl))
                    p.IconUrl = DefaultIconUrl(p);

                if (string.IsNullOrEmpty(p.Title))
                    p.Title = p.Id;

                p.OnlineVersion = GetInstalledVersion(p.Id);

                if (!string.IsNullOrEmpty(p.OnlineVersion))
                {
                    var extra = Gallery.GetPackageExtra(p.Id + "." + p.OnlineVersion);
                    if (extra != null)
                    {
                        p.DownloadCount = extra.DownloadCount;
                        p.Rating = extra.Rating;
                    }
                }

                installedWidgets.Add(p);
            }
            return installedWidgets;
        }

        static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, string pkgId, List<PackageFile> installedFiles)
        {
            var rootPath = HttpContext.Current.Server.MapPath(Utils.RelativeWebRoot);

            foreach (var dir in source.GetDirectories())
            {
                var dirPath = Path.Combine(target.FullName, dir.Name);

                // Files moved to Custom folder
                dirPath = dirPath.Replace("App_Code", "Custom");
                dirPath = dirPath.Replace("\\themes", "\\Custom\\Themes");
                dirPath = dirPath.Replace("\\widgets", "\\Custom\\Widgets");
                dirPath = dirPath.Replace("\\User controls", "\\Custom\\Controls");

                var relPath = dirPath.Replace(rootPath, "");

                // save directory if it is created by package
                // so we can remove it on package uninstall
                if (!Directory.Exists(dirPath))
                {
                    fileOrder++;
                    var fileToCopy = new PackageFile
                    {
                        FilePath = relPath,
                        PackageId = pkgId,
                        FileOrder = fileOrder,
                        IsDirectory = true
                    };
                    installedFiles.Add(fileToCopy);
                }

                CopyDirectory(dir, Directory.CreateDirectory(dirPath), pkgId, installedFiles);
            }
             
            foreach (var file in source.GetFiles())
            {
                var filePath = Path.Combine(target.FullName, file.Name);

                // Files moved to Custom folder
                filePath = filePath.Replace("App_Code", "Custom");
                filePath = filePath.Replace("\\themes", "\\Custom\\Themes");
                filePath = filePath.Replace("\\widgets", "\\Custom\\Widgets");
                filePath = filePath.Replace("\\User controls", "\\Custom\\Controls");

                var relPath = filePath.Replace(rootPath, "");

                file.CopyTo(filePath);

                fileOrder++;
                var fileToCopy = new PackageFile
                {
                    FileOrder = fileOrder,
                    IsDirectory = false,
                    FilePath = relPath,
                    PackageId = pkgId
                };

                // fix known interface changes
                if (filePath.ToLower().EndsWith(".cs") ||
                    filePath.ToLower().EndsWith(".aspx") ||
                    filePath.ToLower().EndsWith(".ascx") ||
                    filePath.ToLower().EndsWith(".master"))
                {
                    ReplaceInFile(filePath, "BlogSettings.Instance.StorageLocation", "Blog.CurrentInstance.StorageLocation");
                    ReplaceInFile(filePath, "BlogSettings.Instance.FileExtension", "BlogConfig.FileExtension");
                    ReplaceInFile(filePath, "\"login.aspx", "\"account/login.aspx");

                    // fix older themes having reference to /themes/ folder
                    ReplaceInFile(filePath, "/themes", "/Custom/Themes");
                    ReplaceInFile(filePath, "themes/", "Custom/Themes/");
                    ReplaceInFile(filePath, "}themes", "}Custom/Themes");
                    ReplaceInFile(filePath, "themes{", "Custom/Themes{");

                    ReplaceInFile(filePath, "/User controls", "/Custom/Controls");
                    ReplaceInFile(filePath, "User controls/", "Custom/Controls/");
                }

                installedFiles.Add(fileToCopy);
            }   
        }

        static void ForceDeleteDirectory(string path)
        {
            DirectoryInfo fol;
            var fols = new Stack<DirectoryInfo>();
            var root = new DirectoryInfo(path);
            fols.Push(root);
            while (fols.Count > 0)
            {
                fol = fols.Pop();
                fol.Attributes = fol.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                foreach (DirectoryInfo d in fol.GetDirectories())
                {
                    fols.Push(d);
                }
                foreach (FileInfo f in fol.GetFiles())
                {
                    f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                    f.Delete();
                }
            }
            root.Delete(true);
        }

        static string DefaultIconUrl(Package pkg)
        {
            var validImages = new List<string> {"icon.jpg", "icon.png", "icon.gif", "screenshot.jpg", "screenshot.png", "theme.jpg", "theme.png"};
            var pkgDir = pkg.PackageType == "Widget" ? "Custom/Widgets" : "Custom/Themes";

            foreach (var img in validImages)
            {
                var url = string.Format("{0}{1}/{2}/{3}",
                Utils.ApplicationRelativeWebRoot, pkgDir, pkg.Id, img);

                url = url.Replace("/themes", "/Custom/Themes");
                url = url.Replace("/widgets", "/Custom/Widgets");

                var path = HttpContext.Current.Server.MapPath(url);

                if (File.Exists(path)) return url;
            }

            if (pkg.PackageType == "Widget")
                return Utils.ApplicationRelativeWebRoot + "Content/images/blog/Widget.png";

            return Utils.ApplicationRelativeWebRoot + "Content/images/blog/Theme.png";
        }

        static void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            var cnt = 0;
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            cnt = content.Length;
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            if (cnt > 0 && cnt != content.Length)
            {
                Utils.Log(string.Format("Package Installer: replacing in {0} from {1} to {2}", filePath, searchText, replaceText));
            }

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        static string PackageType(List<PackageFile> files)
        {
            foreach (var pkg in files)
            {
                if (pkg.FilePath.Contains(@"Custom\Themes"))
                    return "theme";
                if (pkg.FilePath.Contains(@"Custom\Widgets"))
                    return "widget";
                if (pkg.FilePath.Contains(@"Custom\Extensions"))
                    return "extension";
            }
            return "extension";
        }

        /// <summary>
        /// Get online version number
        /// </summary>
        /// <param name="pkgId">Package ID</param>
        /// <returns>Version number</returns>
        public static string GetInstalledVersion(string pkgId)
        {
            var pkg = BlogService.InstalledFromGalleryPackages().Where(p => p.PackageId == pkgId).FirstOrDefault();
            return pkg == null ? "" : pkg.Version;
        }

        static Package GetPackageManifest(string id, string pkgType)
        {
            var jp = new Package { Id = id, PackageType = pkgType };

            var pkgUrl = pkgType == "Theme" ?
                string.Format("{0}Custom/Themes/{1}/theme.xml", Utils.ApplicationRelativeWebRoot, id) :
                string.Format("{0}Custom/Widgets/{1}/widget.xml", Utils.ApplicationRelativeWebRoot, id);

            var pkgPath = HttpContext.Current.Server.MapPath(pkgUrl);
            try
            {
                if (File.Exists(pkgPath))
                {
                    using (var textReader = new XmlTextReader(pkgPath))
                    {
                        textReader.Read();

                        while (textReader.Read())
                        {
                            textReader.MoveToElement();

                            if (textReader.Name == "description")
                                jp.Description = textReader.ReadString();

                            if (textReader.Name == "authors")
                                jp.Authors = textReader.ReadString();

                            if (textReader.Name == "website")
                                jp.Website = textReader.ReadString();

                            if (textReader.Name == "version")
                                jp.LocalVersion = textReader.ReadString();

                            if (textReader.Name == "iconurl")
                                jp.IconUrl = textReader.ReadString();
                        }
                        textReader.Close();
                    }
                    return jp;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Packaging.FileSystem.GetPackageManifest", ex);
            }
            return null;
        }

        /// <summary>
        /// Creates manifest file for theme or widget to save package metadata
        /// </summary>
        /// <param name="package">Package</param>
        /// <param name="packageFiles">Installed files</param>
        public static void CreateManifestIfNotExists(NuGet.IPackage package, List<PackageFile> packageFiles)
        {
            var shortPath = "";
            var type = PackageType(packageFiles);

            if (type == "theme")
                shortPath = string.Format(@"Custom\Themes\{0}\theme.xml", package.Id);

            if (type == "widget")
                shortPath = string.Format(@"Custom\Widgets\{0}\widget.xml", package.Id);

            if (string.IsNullOrEmpty(shortPath))
                return;

            var path = Path.Combine(HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot), shortPath);

            if (File.Exists(path))
                return;

            using (var writer = new XmlTextWriter(path, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("metadata");

                writer.WriteElementString("id", package.Id);
                writer.WriteElementString("description", package.Description);

                writer.WriteElementString("authors", string.Join(", ", package.Authors));

                writer.WriteElementString("website", package.ProjectUrl == null ? "" : package.ProjectUrl.ToString());
                writer.WriteElementString("version", package.Version.ToString());
                writer.WriteElementString("iconurl", package.IconUrl == null ? "" : package.IconUrl.ToString());

                writer.WriteEndElement();
            }
            var idx = packageFiles.Count + 1;
            packageFiles.Add(new PackageFile { PackageId = package.Id, FileOrder = idx, FilePath = shortPath, IsDirectory = false });
        }

        #endregion
    }
}
