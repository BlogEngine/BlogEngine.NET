namespace BlogEngine.Core.DataStore
{
    /// <summary>
    /// WidgetSettings implementation
    /// </summary>
    public class WidgetSettings : SettingsBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetSettings"/> class.
        /// </summary>
        /// <param name="setId">
        /// The set id.
        /// </param>
        public WidgetSettings(string setId)
        {
            this.SettingId = setId;
            this.ExType = ExtensionType.Widget;
            this.SettingsBehavior = new StringDictionaryBehavior();
        }

        #endregion
    }
}