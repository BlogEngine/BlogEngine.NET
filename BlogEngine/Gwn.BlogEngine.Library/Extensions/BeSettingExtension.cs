using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core.Web.Extensions;
using Gwn.BlogEngine.Library.Attributes;
using Gwn.BlogEngine.Library.Interfaces;

namespace Gwn.BlogEngine.Library.Extensions
{
    public static class BeSettingExtension
    {
        /// <summary>
        /// Initializes the specified extension settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extensionSettings">The extension settings.</param>
        /// <param name="records">The records.</param>
        /// <returns></returns>
        public static ExtensionSettings Initialize<T>(
            this ExtensionSettings extensionSettings, 
            List<T> records) where T : IBeExtensionSettingsData
        {
            var extensionName = extensionSettings.Name;

            // Get the first object in the list - we're only
            // interested in its properties
            var record = records.FirstOrDefault();

            // Get the property list
            var recordInfo = record.GetType().GetProperties();

            // Iterate through each property 
            foreach (var member in recordInfo)
            {
                // We're only interested in the ExtensionsSettingsAttribute
                var attributeList = member.GetCustomAttributes(typeof(BeSettingAttribute), true);
                if (attributeList == null)
                    continue;

                // Retrieve the attribute from the list
                var settingInfo = attributeList.FirstOrDefault().Cast<BeSettingAttribute>();
                if (settingInfo == null)
                    continue;

                // Assign each field to its value (easier to read)
                var keyfield = settingInfo.KeyField.ToBool();
                var label = settingInfo.Label;
                var maxlen = settingInfo.MaxLength.ToInt32();
                var name = settingInfo.Name;
                var parameterType = settingInfo.ParameterType.ToParameterType();
                var required = settingInfo.Required.ToBool();

                // Add the extension setting parameter
                extensionSettings.AddParameter(name, label, maxlen, required, keyfield, parameterType);
            }

            // With the parameters set now we can add each of the
            // default data records to the extensionSettings
            foreach (T data in records)
                extensionSettings.AddValues( data.GetExtensionData());

            // Import settings
            ExtensionManager.ImportSettings(extensionSettings);

            // return ExtensionSettings
            return ExtensionManager.GetSettings(extensionName);
        }
    }
}
