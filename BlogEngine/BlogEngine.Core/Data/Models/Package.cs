using System;
namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Install package
    /// </summary>
    public class Package
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// Package Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Package type (extension, theme, widget)
        /// </summary>
        public string PackageType { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Local Version
        /// </summary>
        public string LocalVersion { get; set; }
        /// <summary>
        /// Online version
        /// </summary>
        public string OnlineVersion { get; set; }
        /// <summary>
        /// Desctiption
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Download count
        /// </summary>
        public int DownloadCount { get; set; }
        /// <summary>
        /// Last updated
        /// </summary>
        public string LastUpdated { get; set; }
        /// <summary>
        /// Project Website
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// Package url in the server gallery
        /// </summary>
        public string PackageUrl { get; set; }
        /// <summary>
        /// Icon URL
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        ///  Authors
        /// </summary>
        public string Authors { get; set; }
        /// <summary>
        /// Tags
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// Package rating
        /// </summary>
        public double Rating { get; set; }
        /// <summary>
        /// Enabled flag
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Priority (estensions order of execution)
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Url for extension settings/admin page
        /// </summary>
        public string SettingsUrl { get; set; }
        /// <summary>
        /// If update available in the gallery
        /// </summary>
        public bool UpdateAvailable
        {
            get
            {
                return ConvertVersion(LocalVersion) < ConvertVersion(OnlineVersion);
            }
        }
        /// <summary>
        /// Package extended data
        /// </summary>
        public PackageExtra Extra { get; set; }

        static int ConvertVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
                return 0;

            int numVersion;
            Int32.TryParse(version.Replace(".", ""), out numVersion);
            return numVersion;
        }
    }
}
