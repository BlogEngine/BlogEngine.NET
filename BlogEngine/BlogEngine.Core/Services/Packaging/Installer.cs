using System;
using System.Web;
using BlogEngine.Core.Providers;
using NuGet;
using BlogEngine.Core.Data.Services;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Responsible for install/uninstall operations
    /// </summary>
    public static class Installer
    {
        private static IPackageRepository _repository
        {
            get
            {
                return PackageRepositoryFactory.Default.CreateRepository(BlogConfig.GalleryFeedUrl);
            }
        }

        /// <summary>
        /// Install package
        /// </summary>
        /// <param name="pkgId"></param>
        public static bool InstallPackage(string pkgId)
        {
            try
            {
                // if package already installed - uninstall it
                if (BlogService.InstalledFromGalleryPackages() != null)
                {
                    if(BlogService.InstalledFromGalleryPackages().Find(p => p.PackageId == pkgId) != null)
                    {
                        UninstallPackage(pkgId);
                    }
                }
                    
                var packageManager = new PackageManager( _repository,
                    new DefaultPackagePathResolver(BlogConfig.GalleryFeedUrl),
                    new PhysicalFileSystem(HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot + "App_Data/packages"))
                );

                var package = _repository.FindPackage(pkgId);

                packageManager.InstallPackage(package, false, true);

                var iPkg = new InstalledPackage { PackageId = package.Id, Version = package.Version.ToString() };
                BlogService.InsertPackage(iPkg);

                var packageFiles = FileSystem.InstallPackage(package);

                BlogService.InsertPackageFiles(packageFiles);

                Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                CustomFieldsParser.ClearCache();

                Utils.Log(string.Format("Installed package {0} by {1}", pkgId, Security.CurrentUser.Identity.Name));
            }
            catch (Exception ex)
            {
                Utils.Log("BlogEngine.Core.Packaging.Installer.InstallPackage(" + pkgId + ")", ex);
                UninstallPackage(pkgId);
                throw;
            }

            return true;
        }

        /// <summary>
        /// Uninstall package
        /// </summary>
        /// <param name="pkgId"></param>
        /// <returns></returns>
        public static bool UninstallPackage(string pkgId)
        {
            try
            {
                FileSystem.UninstallPackage(pkgId);
                // Utils.Log(string.Format("Uninstalled package {0}: installed package files removed. ", pkgId));

                // remove packagefiles.xml and packages.xml (or DB records)
                BlogService.DeletePackage(pkgId);
                // Utils.Log(string.Format("Uninstalled package {0}: package records removed. ", pkgId));

                UninstallGalleryPackage(pkgId);
                // Utils.Log(string.Format("Uninstalled package {0}: NuGet file removed. ", pkgId));

                // reset cache
                Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                Utils.Log(string.Format("Uninstalled package {0} by {1}", pkgId, Security.CurrentUser.Identity.Name));
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Error unistalling package {0}: {1}"), pkgId, ex.Message);
                throw;
            }

            return true;
        }

        private static void UninstallGalleryPackage(string pkgId)
        {
            // if installed from gallery, also remove NuGet package files
            var packageManager = new PackageManager(
                _repository,
                new DefaultPackagePathResolver(BlogConfig.GalleryFeedUrl),
                new PhysicalFileSystem(HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot + "App_Data/packages"))
            );
            var package = _repository.FindPackage(pkgId);

            if (package != null)
                packageManager.UninstallPackage(package, true);
        }

    }
}
