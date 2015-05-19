using System.Collections.Generic;

namespace BlogEngine.Core.FileSystem
{
    public class FileResponse
    {
        public FileResponse()
        {
            Files = new List<FileInstance>();
            Path = string.Empty;
        }
        public IEnumerable<FileInstance> Files { get; set; }
        public string Path { get; set; }
    }

    public class FileInstance
    {
        public bool IsChecked { get; set; }
        public string Created { get; set; }
        public string Name { get; set; }
        public string FileSize { get; set; }
        public FileType FileType { get; set; }
        public string FullPath { get; set; }
    }

    public enum FileType
    {
        Directory,
        File,
        Image,
        None
    }
}
