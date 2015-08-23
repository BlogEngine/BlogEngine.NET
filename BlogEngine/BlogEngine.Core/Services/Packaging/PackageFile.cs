namespace BlogEngine.Core.Packaging
{
    ///<summary>
    /// Installed package
    ///</summary>
    public class PackageFile
    {
        ///<summary>
        /// Package ID
        ///</summary>
        public string PackageId { get; set; }
        /// <summary>
        /// Order in wich files were created
        /// To uninstall need to go reverse order
        /// </summary>
        public int FileOrder { get; set; }
        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// True if file is directory
        /// </summary>
        public bool IsDirectory { get; set; }
    }
}
