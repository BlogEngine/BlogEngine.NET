using BlogEngine.Core.Data.ViewModels;
using BlogEngine.Core.Data.Contracts;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Dashboard repository
    /// </summary>
    public class DashboardRepository : IDashboardRepository
    {
        /// <summary>
        /// Get all dashboard items
        /// </summary>
        /// <returns>Dashboard view model</returns>
        public DashboardVM Get()
        {
            if (!Security.IsAuthorizedTo(Rights.ViewDashboard))
                throw new System.UnauthorizedAccessException();

            return new DashboardVM();
        }
    }
}
