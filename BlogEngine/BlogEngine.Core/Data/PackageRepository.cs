using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Services;
using BlogEngine.Core.Packaging;
using BlogEngine.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Caching;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Package repository
    /// </summary>
    public class PackageRepository : IPackageRepository
    {
        /// <summary>
        /// Find packages
        /// </summary>
        /// <param name="take">Items to take</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns>List of packages</returns>
        public IEnumerable<Package> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (string.IsNullOrEmpty(order)) order = "LastUpdated desc";

            var packages = new List<Package>();
            var pkgsToLoad = new List<Package>();

            if (filter.ToLower() == "extensions")
            {
                if (!Security.IsAuthorizedTo(Rights.ManageExtensions))
                    throw new UnauthorizedAccessException();

                var fullList = Packaging.FileSystem.LoadExtensions();
                string[] coreExts = { "bbcode", "logger", "breakpost", "resolvelinks", "sendcommentmail", "sendpings", "smilies" };

                foreach (var pkg in fullList)
                {
                    if(!coreExts.Contains(pkg.Id.ToLower()))
                    {
                        pkgsToLoad.Add(pkg);
                    }
                }
            }
            else if (filter.ToLower() == "themes")
            {
                if (!Security.IsAuthorizedTo(Rights.ManageThemes))
                    throw new UnauthorizedAccessException();
                pkgsToLoad = Packaging.FileSystem.LoadThemes();
            }
            else if (filter.ToLower() == "widgets")
            {
                if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
                    throw new UnauthorizedAccessException();
                pkgsToLoad = Packaging.FileSystem.LoadWidgets();
            }
            else
            {
                if (!Security.IsAuthorizedTo(Rights.ManagePackages))
                    throw new UnauthorizedAccessException();
                pkgsToLoad = CachedPackages;
            }

            if (take == 0) take = pkgsToLoad.Count();

            return pkgsToLoad.AsQueryable().OrderBy(order).Skip(skip).Take(take);
        }

        /// <summary>
        /// Package by ID
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <returns>Package</returns>
        public Package FindById(string id)
        {
            if (!Security.IsAuthorizedTo(Rights.ManagePackages))
                throw new UnauthorizedAccessException();

            Package galPkg = CachedPackages.FirstOrDefault(p => p.Id == id);
            Package locPkg = Packaging.FileSystem.LoadThemes().FirstOrDefault(p => p.Id == id);

            if(locPkg == null)
                locPkg = Packaging.FileSystem.LoadExtensions().FirstOrDefault(p => p.Id == id);

            if (locPkg == null)
                locPkg = Packaging.FileSystem.LoadWidgets().FirstOrDefault(p => p.Id == id);

            if(locPkg != null && galPkg != null)
            {
                // package installed fro gallery
                galPkg.SettingsUrl = locPkg.SettingsUrl;
            }

            return galPkg == null ? locPkg : galPkg;
        }

        /// <summary>
        /// Update package metadata
        /// </summary>
        /// <param name="item">Package object</param>
        /// <returns>True if success</returns>
        public bool Update(Package item)
        {
            if (!Security.IsAdministrator)
                throw new System.UnauthorizedAccessException();

            if (item == null)
                return false;

            switch (item.PackageType)
            {
                case "Extension":
                    if (!string.IsNullOrEmpty(item.Id))
                    {
                        var ext = ExtensionManager.GetExtension(item.Id);

                        if (ext != null)
                        {
                            ext.Priority = item.Priority;
                            ext.Enabled = item.Enabled;
                            ExtensionManager.ChangeStatus(item.Id, item.Enabled);
                            ExtensionManager.SaveToStorage(ext);
                            Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);
                        }
                        else
                        {
                            Utils.Log($"Failed to find extension {item.Id} while trying to update package repository");
                        }
                    }
                    break;
                case "Theme":
                    break;
                case "Widget":
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// Install package
        /// </summary>
        /// <param name="id">Package id</param>
        /// <returns>True if success</returns>
        public bool Install(string id)
        {
            if (!Security.IsAdministrator || !Blog.CurrentInstance.IsPrimary)
                throw new System.UnauthorizedAccessException();

            return Installer.InstallPackage(id);
        }

        /// <summary>
        /// Uninstall package
        /// </summary>
        /// <param name="id">Package id</param>
        /// <returns>True if success</returns>
        public bool Uninstall(string id)
        {
            if (!Security.IsAdministrator || !Blog.CurrentInstance.IsPrimary)
                throw new System.UnauthorizedAccessException();

            DeleteThemeCustomFields(id);

            return Installer.UninstallPackage(id);
        }

        #region Private methods

        static List<Package> CachedPackages
        {
            get
            {
                // uncomment this line to disable gallery caching for debugging
                // Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                if (Blog.CurrentInstance.Cache[Constants.CacheKey] == null)
                {
                    Blog.CurrentInstance.Cache.Add(
                        Constants.CacheKey,
                        LoadPackages(),
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 15, 0),
                        CacheItemPriority.Low,
                        null);
                }
                return (List<Package>)Blog.CurrentInstance.Cache[Constants.CacheKey];
            }
        }

        static List<Package> LoadPackages()
        {
            var packages = new List<Package>();
            Gallery.Load(packages);
            foreach (var p in packages)
            {
                p.LocalVersion = Packaging.FileSystem.GetInstalledVersion(p.Id);
            }
            return packages;
        }

        static void Trace(string msg, List<Package> packages)
        {
            string s = "{0}|{1}|{2}|{3}|{4}|{5}";
            foreach (var p in packages)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(s, msg, p.PackageType, p.Id, p.LocalVersion, p.OnlineVersion));
            }
        }

        static void DeleteThemeCustomFields(string theme)
        {
            var p = CachedPackages.FirstOrDefault(pkg => pkg.Id == theme);
            if (p != null && p.PackageType != null && p.PackageType.ToLower() == "theme")
            {
                var fields = BlogEngine.Core.Providers.BlogService.FillCustomFields();

                foreach (var f in fields)
                {
                    if (f.BlogId == Blog.CurrentInstance.Id && f.CustomType.ToLower() == "theme" && f.ObjectId == theme)
                    {
                        BlogEngine.Core.Providers.BlogService.DeleteCustomField(f);
                    }
                }
                CustomFieldsParser.ClearCache();
            }
        }

        #endregion
    }
}
