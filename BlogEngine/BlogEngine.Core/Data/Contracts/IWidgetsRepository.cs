using BlogEngine.Core.Data.ViewModels;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Widgets repository
    /// </summary>
    public interface IWidgetsRepository
    {
        /// <summary>
        /// Get all widgets items
        /// </summary>
        /// <returns>Widgets view model</returns>
        WidgetsVM Get();
        /// <summary>
        /// Update widget zones
        /// </summary>
        /// <param name="items">List of zones</param>
        /// <returns>True on success</returns>
        bool Update(List<WidgetZone> items);
    }
}
