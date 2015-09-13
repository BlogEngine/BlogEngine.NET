using BlogEngine.Core.Data.ViewModels;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Dashboard repository
    /// </summary>
    public interface IDashboardRepository
    {
        /// <summary>
        /// Get all dashboard items
        /// </summary>
        /// <returns>Dashboard view model</returns>
        DashboardVM Get();
    }
}
