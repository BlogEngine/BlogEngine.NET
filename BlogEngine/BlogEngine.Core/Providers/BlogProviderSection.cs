namespace BlogEngine.Core.Providers
{
    using System.Configuration;

    /// <summary>
    /// A configuration section for web.config.
    /// </summary>
    /// <remarks>
    /// In the config section you can specify the provider you 
    ///     want to use for BlogEngine.NET.
    /// </remarks>
    public class BlogProviderSection : ConfigurationSection
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
        /// Gets or sets the name of the file storage provider, defaults to the XmlBlogProvider if one is not specified
        /// </summary>
        /// <remarks>
        /// This  attribute is not required, however allows for specifying different providers for various operations
        /// </remarks>
        [StringValidator(MinLength = 0)]
        [ConfigurationProperty("fileStoreProvider", IsRequired = false, DefaultValue = "XmlBlogProvider")]
        public string FileStoreProvider
        {
            get
            {
                return (string)base["fileStoreProvider"];
            }

            set
            {
                base["fileStoreProvider"] = value;
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