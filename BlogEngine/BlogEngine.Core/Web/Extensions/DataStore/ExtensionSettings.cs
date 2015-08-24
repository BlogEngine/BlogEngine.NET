namespace BlogEngine.Core.DataStore
{
    /// <summary>
    /// Extension settings implementation
    /// </summary>
    public class ExtensionSettings : SettingsBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionSettings"/> class.
        /// </summary>
        /// <param name="setId">The set id.</param>
        public ExtensionSettings(string setId)
        {
            this.SettingId = setId;
            this.ExType = ExtensionType.Extension;
            this.SettingsBehavior = new ExtensionSettingsBehavior();
        }

        #endregion
    }
}