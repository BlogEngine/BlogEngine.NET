using System.Configuration;

namespace BlogEngine.Core.Providers
{
    /// <summary>
    /// Enables a custom configuration section for the BE provider in Web.config.
    /// </summary>
    public class BlogFileSystemProviderSection : ConfigurationSection
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the name of the default provider
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider", DefaultValue = "XmlBlogProvider")]
        public string DefaultProvider
        {
            get
            {
                return (string)base["defaultProvider"];
            }

            set
            {
                base["defaultProvider"] = value;
            }
        }
        
        /// <summary>
        ///     Gets a collection of registered providers.
        /// </summary>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["providers"];
            }
        }

        #endregion
    }
}
