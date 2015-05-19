using System.Collections.Generic;
using BlogEngine.Core.Data.Models;
using System;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Trash repository
    /// </summary>
    public interface ITrashRepository
    {
        /// <summary>
        /// Get trash list
        /// </summary>
        /// <param name="trashType">Type (post, page, comment)</param>
        /// <param name="take">Take for a page</param>
        /// <param name="skip">Items to sckip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns>Trash view model</returns>
        TrashVM GetTrash(TrashType trashType, int take = 10, int skip = 0, string filter = "", string order = "");

        /// <summary>
        /// Restore
        /// </summary>
        /// <param name="trashType">Trash type</param>
        /// <param name="id">Id</param>
        bool Restore(string trashType, Guid id);

        /// <summary>
        /// Purge
        /// </summary>
        /// <param name="trashType">Trash type</param>
        /// <param name="id">Id</param>
        bool Purge(string trashType, Guid id);

        /// <summary>
        /// Purge all
        /// </summary>
        bool PurgeAll();

        /// <summary>
        /// Purge log file
        /// </summary>
        /// <returns></returns>
        JsonResponse PurgeLogfile();
    }
}
