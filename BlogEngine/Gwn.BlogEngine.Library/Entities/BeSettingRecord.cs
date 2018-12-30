using System;
using Gwn.BlogEngine.Library.Attributes;
using Gwn.BlogEngine.Library.Interfaces;

namespace Gwn.BlogEngine.Library.Entities
{
    /// <summary>
    /// Wiki record
    /// </summary>
    [Serializable]
    public class BeSettingRecord : IBeExtensionSettingsData
    {
        #region Ordinal indexes
        public const int IdIndex = 0;
        public const int PermaLinkIndex = 1;
        public const int CommandIndex = 2;
        public const int CommandParameterIndex = 3;
        public const int DisplayTemplateIndex = 4;
        #endregion

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [BeSetting("Id", "Id:")]
        public Guid Id { get; set; }                            // 1

        /// <summary>
        /// Gets or sets the perma link.
        /// </summary>
        /// <value>The perma link.</value>
        [BeSetting("PermaLink", "PermaLink:")]
        public string PermaLink { get; set; }                   // 2

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        [BeSetting("Command", "Command:", "200", "true")]
        public string Command { get; set; }			            // 3

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        [BeSetting("CommandParameter", "Command Parameter:", "200","true", "true")]
        public string CommandParameter { get; set; }            // 4

        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        /// <value>The display value.</value>
        [BeSetting("DisplayTemplate", "Display Template:")]
        public string DisplayTemplate { get; set; }             // 5

        /// <summary>
        /// Gets or sets the original.
        /// </summary>
        /// <value>The original.</value>
        public string Original { get; set;}

        /// <summary>
        /// Gets the extension data in the order they are presented
        /// above - important because we reflect over the property
        /// attributes to add them as parameters to extension settings
        /// </summary>
        /// <returns></returns>
        public string[] GetExtensionData()
        {
            return new string[]
                       {
                           Id.ToString(),                       // 1
                           PermaLink ?? "Not Assigned",         // 2
                           Command,                             // 3
                           CommandParameter,                    // 4
                           DisplayTemplate ?? "Not Assigned"    // 5
                       };
        }

        /// <summary>
        /// Gets the key field
        /// </summary>
        /// <value>The key field.</value>
        public string KeyField {
            get { return CommandParameter; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Command: [{0}]  CommandParameter: [{1}]  DisplayValue: [{2}]  Original: [{3}]",
                                 Command,
                                 CommandParameter,
                                 DisplayTemplate,
                                 Original);
        }



    }

}
