using System.Collections.Generic;

namespace BlogEngine.Core.FileSystem
{
    /// <summary>
    /// UNDONE: Doc: Description.
    /// </summary>
    public class FileResponse
    {
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public FileResponse()
        {
            Files = new List<FileInstance>();
            Path = string.Empty;
        }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public IEnumerable<FileInstance> Files { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Path { get; set; }
    }

    public class FileInstance
    {
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string FileSize { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public FileType FileType { get; set; }
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// Image assosiated with file extension
        /// </summary>
        public string ImgPlaceholder
        {
            get
            {
                if(FileType == FileType.File)
                {
                    return getPlaceholder(Name);
                }
                return string.Empty;
            }
        }

        static string getPlaceholder(string name)
        {
            var file = name.ToLower().Trim();

            if (file.EndsWith(".zip") || file.EndsWith(".gzip") || file.EndsWith(".7zip") || file.EndsWith(".rar"))
            {
                return "fa fa-file-archive-o";
            }
            if (file.EndsWith(".doc") || file.EndsWith(".docx"))
            {
                return "fa fa-file-word-o";
            }
            if (file.EndsWith(".xls") || file.EndsWith(".xlsx"))
            {
                return "fa fa-file-excel-o";
            }
            if (file.EndsWith(".pdf"))
            {
                return "fa fa-file-pdf-o";
            }
            if (file.EndsWith(".txt"))
            {
                return "fa fa-file-text-o";
            }

            return "fa fa-file-o";
        }
    }

    /// <summary>
    /// UNDONE: Doc: Description.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        Directory,
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        File,
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        Image,
        /// <summary>
        /// UNDONE: Doc: Description.
        /// </summary>
        None
    }
}
