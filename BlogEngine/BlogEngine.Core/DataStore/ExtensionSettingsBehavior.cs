namespace BlogEngine.Core.DataStore
{
    using BlogEngine.Core.Providers;

    /// <summary>
    /// Incapsulates behavior for saving and retreaving
    ///     extension settings
    /// </summary>
    public class ExtensionSettingsBehavior : ISettingsBehavior
    {
        #region Implemented Interfaces

        #region ISettingsBehavior

        /// <summary>
        /// Retreaves extension object from database or file system
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <returns>
        /// Extension object as Stream
        /// </returns>
        public object GetSettings(ExtensionType extensionType, string extensionId)
        {
            return BlogService.LoadFromDataStore(extensionType, extensionId);
        }

        /// <summary>
        /// Saves extension to database or file system
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <param name="settings">
        /// Extension object
        /// </param>
        /// <returns>
        /// True if saved
        /// </returns>
        public bool SaveSettings(ExtensionType extensionType, string extensionId, object settings)
        {
            BlogService.SaveToDataStore(extensionType, extensionId, settings);
            return true;
        }

        #endregion

        #endregion
    }
}