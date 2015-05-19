using BlogEngine.Core.Data.Models;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Lookups repository
    /// </summary>
    public interface ILookupsRepository
    {
        /// <summary>
        /// Get lookups
        /// </summary>
        /// <returns>Lookups</returns>
        Lookups GetLookups();
    }
}
