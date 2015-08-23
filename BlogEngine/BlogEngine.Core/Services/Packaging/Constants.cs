namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Incapsulate all packaging constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Root allery url
        /// </summary>
        public const string GalleryUrl = "http://dnbegallery.org";
        /// <summary>
        /// Gallery application url
        /// </summary>
        public const string GalleryAppUrl = "http://dnbegallery.org/cms";
        /// <summary>
        /// Package type Theme
        /// </summary>
        public const string Theme = "Theme";
        /// <summary>
        /// Package type Widget
        /// </summary>
        public const string Widget = "Widget";
        /// <summary>
        /// Package type Extension
        /// </summary>
        public const string Extension = "Extension";
        /// <summary>
        /// Page size for admin/plugins lists
        /// </summary>
        public const int PageSize = 15;
        /// <summary>
        /// Installed packages XML
        /// </summary>
        public const string InstalledPackagesXml = "packages.xml";
        /// <summary>
        /// Installed package files XML
        /// </summary>
        public const string InstalledPackageFilesXml = "packagefiles.xml";
        /// <summary>
        /// Cache key for gallery packages
        /// </summary>
        public const string CacheKey = "Gallery-Packages";
    }
}
