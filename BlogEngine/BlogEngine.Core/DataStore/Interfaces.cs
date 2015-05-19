namespace BlogEngine.Core.DataStore
{
    /// <summary>
    /// Type of extension
    /// </summary>
    public enum ExtensionType
    {
        /// <summary>
        ///     An Extension.
        /// </summary>
        Extension, 

        /// <summary>
        ///     A Widget Extension.
        /// </summary>
        Widget, 

        /// <summary>
        ///     A Theme Extension.
        /// </summary>
        Theme
    }

    /// <summary>
    /// Public interfaces and enums for DataStore
    ///     ISettingsBehavior incapsulates saving and retreaving
    ///     settings objects to and from data storage
    /// </summary>
    public interface ISettingsBehavior
    {
        #region Public Methods

        /// <summary>
        /// Get settings interface
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        /// <returns>
        /// Settings object
        /// </returns>
        object GetSettings(ExtensionType extensionType, string extensionId);

        /// <summary>
        /// Save settings interface
        /// </summary>
        /// <param name="extensionType">
        /// Extensio Type
        /// </param>
        /// <param name="extensionId">
        /// Extensio Id
        /// </param>
        /// <param name="settings">
        /// Settings object
        /// </param>
        /// <returns>
        /// True if saved
        /// </returns>
        bool SaveSettings(ExtensionType extensionType, string extensionId, object settings);

        #endregion
    }
}