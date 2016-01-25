using BlogEngine.Core.Data.ViewModels;
using BlogEngine.Core.Data.Contracts;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Widgets repository
    /// </summary>
    public class WidgetsRepository : IWidgetsRepository
    {
        /// <summary>
        /// Get theme widgets
        /// </summary>
        /// <returns>Widgets view model</returns>
        public WidgetsVM Get()
        {
            if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
                throw new System.UnauthorizedAccessException();

            return new WidgetsVM();
        }
    }
}
