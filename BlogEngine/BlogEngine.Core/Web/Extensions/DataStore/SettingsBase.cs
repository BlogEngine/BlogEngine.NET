namespace BlogEngine.Core.DataStore
{
    /// <summary>
    /// Base class for extension settings
    /// </summary>
    public abstract class SettingsBase
    {
        #region Constants and Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        protected SettingsBase()
        {
            this.SettingId = string.Empty;
            this.ExType = ExtensionType.Extension;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the type of extension (extension, widget or theme)
        /// </summary>
        public ExtensionType ExType { get; set; }

        /// <summary>
        ///     Gets or sets the Setting ID
        /// </summary>
        public string SettingId { get; set; }

        /// <summary>
        ///     Gets or sets the Settings behavior
        /// </summary>
        public ISettingsBehavior SettingsBehavior { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get settings object from data storage
        /// </summary>
        /// <returns>
        /// Stream representing extension object
        /// </returns>
        public object GetSettings()
        {
            return this.SettingsBehavior.GetSettings(this.ExType, this.SettingId);
        }

        /// <summary>
        /// Saves setting object to data storage
        /// </summary>
        /// <param name="settings">
        /// Settings object
        /// </param>
        /// <returns>
        /// True if saved
        /// </returns>
        public bool SaveSettings(object settings)
        {
            return this.SettingsBehavior.SaveSettings(this.ExType, this.SettingId, settings);
        }

        #endregion
    }
}