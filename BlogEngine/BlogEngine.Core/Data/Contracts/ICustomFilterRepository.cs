using BlogEngine.Core.Data.Models;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Custom filter
    /// </summary>
    public interface ICustomFilterRepository
    {
        /// <summary>
        /// Get list of custom filters
        /// </summary>
        /// <returns>List of filters</returns>
        IEnumerable<CustomFilter> GetCustomFilters();
        /// <summary>
        /// Reset counters for custom filter
        /// </summary>
        /// <param name="filterName">Filter name</param>
        /// <returns>Json response</returns>
        JsonResponse ResetCounters(string filterName);
    }
}