using System;

namespace BlogEngine.Core.Notes
{
    /// <summary>
    /// Settings
    /// </summary>
    public class QuickSetting
    {
        /// <summary>
        /// Blog ID
        /// </summary>
        public Guid BlogId { get; set; }
        /// <summary>
        /// Each author can have individual settings
        /// driving quick notes behavior
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Setting name
        /// </summary>
        public string SettingName { get; set; }
        /// <summary>
        /// Setting value
        /// </summary>
        public string SettingValue { get; set; }
    }
}