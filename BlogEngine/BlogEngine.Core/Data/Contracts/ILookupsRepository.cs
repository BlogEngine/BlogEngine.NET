using BlogEngine.Core.Data.Models;

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

        /// <summary>
        /// Editor options
        /// </summary>
        /// <param name="options">Options</param>
        void SaveEditorOptions(EditorOptions options);
    }
}
