namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Used for dropdowns
    /// </summary>
    public class SelectOption
    {
        /// <summary>
        /// Option name
        /// </summary>
        public string OptionName { get; set; }
        /// <summary>
        /// Option value
        /// </summary>
        public string OptionValue { get; set; }
        /// <summary>
        /// Option Summary
        /// </summary>
        public string OptionSummary { get; set; }
        /// <summary>
        /// Is option selected
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
