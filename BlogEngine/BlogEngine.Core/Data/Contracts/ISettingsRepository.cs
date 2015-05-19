using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Settings repository
    /// </summary>
    public interface ISettingsRepository
    {
        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns>List of settings</returns>
        Settings Get();
        /// <summary>
        /// Update settings
        /// </summary>
        /// <param name="item">Settings item</param>
        /// <returns>True if success</returns>
        bool Update(Settings item);
    }
}
