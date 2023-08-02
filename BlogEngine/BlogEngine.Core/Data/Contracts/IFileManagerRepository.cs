using BlogEngine.Core.FileSystem;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Interface for a file manager repository.
    /// </summary>
    public interface IFileManagerRepository
    {
        /// <summary>
        /// Find a file or collection of files. Provides paging interface.
        /// </summary>
        /// <param name="take">How many files to return.</param>
        /// <param name="skip">How many to skip before starting to return.</param>
        /// <param name="path">UNDONE: Doc: More detail. Path of files to return.</param>
        /// <param name="order">Order in which to return files.</param>
        /// <returns>IEnumerable of file instances (FileInstance class).</returns>
        IEnumerable<FileInstance> Find(int take = 10, int skip = 0, string path = "", string order = "");
    }
}
