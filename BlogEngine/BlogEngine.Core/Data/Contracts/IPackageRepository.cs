using BlogEngine.Core.Data.Models;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Package repository
    /// </summary>
    public interface IPackageRepository
    {
        /// <summary>
        /// Find packages
        /// </summary>
        /// <param name="take">Items to take</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns>List of packages</returns>
        IEnumerable<Package> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Find by ID
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <returns>Package</returns>
        Package FindById(string id);
        /// <summary>
        /// Install package
        /// </summary>
        /// <param name="id">Package id</param>
        /// <returns>True if success</returns>
        bool Install(string id);
        /// <summary>
        /// Uninstall package
        /// </summary>
        /// <param name="id">Package id</param>
        /// <returns>True if success</returns>
        bool Uninstall(string id);
        /// <summary>
        /// Update package metadata
        /// </summary>
        /// <param name="item">Package object</param>
        /// <returns>True if success</returns>
        bool Update(Package item);
    }
}
