using BlogEngine.Core.Data.ViewModels;

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
    }
}
