using System;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Locally installed gallery package
    /// </summary>
    public class InstalledPackage
    {
        /// <summary>
        /// Package ID
        /// </summary>
        public string PackageId { get; set; }
        /// <summary>
        /// Installed package Version
        /// </summary>
        public string Version { get; set; }
    }
}
