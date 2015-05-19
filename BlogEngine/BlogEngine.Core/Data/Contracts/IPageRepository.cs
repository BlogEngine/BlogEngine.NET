using System;
using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Page repository
    /// </summary>
    public interface IPageRepository
    {
        /// <summary>
        /// Find page item
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns></returns>
        IEnumerable<PageItem> Find(int take = 10, int skip = 0, string filter = "", string order = "");
        /// <summary>
        /// Find by id
        /// </summary>
        /// <param name="id">Page id</param>
        /// <returns>Page object</returns>
        PageDetail FindById(Guid id);
        /// <summary>
        /// Add new page
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>Saved page with new ID</returns>
        PageDetail Add(PageDetail page);
        /// <summary>
        /// Update page
        /// </summary>
        /// <param name="page">Page to update</param>
        /// <returns>True on success</returns>
        bool Update(PageDetail page, string action);
        /// <summary>
        /// Delete page
        /// </summary>
        /// <param name="id">Page ID</param>
        /// <returns>True on success</returns>
        bool Remove(Guid id);
    }
}
