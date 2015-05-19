namespace BlogEngine.Core.Providers
{
    using System;
    using System.Configuration.Provider;

    /// <summary>
    /// A collection of all registered providers.
    /// </summary>
    public class BlogProviderCollection : ProviderCollection
    {
        #region Indexers

        /// <summary>
        ///     Gets a provider by its name.
        /// </summary>
        /// <param name="name">The name of the provider.</param>
        public new BlogProvider this[string name]
        {
            get
            {
                return (BlogProvider)base[name];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a provider to the collection.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (!(provider is BlogProvider))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }

            base.Add(provider);
        }

        #endregion
    }
}