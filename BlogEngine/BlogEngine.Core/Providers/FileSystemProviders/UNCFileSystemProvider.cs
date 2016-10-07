using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.IO;

namespace BlogEngine.Core.Providers
{
    /// <summary>
    /// A class for managing storage on a UNC share.
    /// </summary>
    public partial class UNCFileSystemProvider : BlogFileSystemProvider
    {
        /// <summary>
        /// unc path to store files. This will be the base path for all blogs.
        /// Each blog will then seperate into seperate paths built upon the UNC path by the blog name.
        /// ie. [unc]/primary, [unc]/template
        /// </summary>
        private string uncPath;

        /// <summary>
        /// init
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (String.IsNullOrEmpty(name))
            {
                name = "UNCBlogProvider";
            }
            base.Initialize(name, config);
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Generic UNC File Path Blog Provider");
            }

            if (config["storageVariable"] == null)
            {
                // default to BlogEngine XML provider paths
                config["storageVariable"] = HostingEnvironment.MapPath(Blog.CurrentInstance.StorageLocation);
            }
            else
            {
                if(config["storageVariable"].EndsWith(@"\"))
                    config["storageVariable"] =  config["storageVariable"].Substring(0,  config["storageVariable"].Length - 1);
                if(!System.IO.Directory.Exists(config["storageVariable"]))
                    throw new ArgumentException("storageVariable (as unc path) does not exist. or does not have read\\write permissions");
            }

            this.uncPath = config["storageVariable"];
            config.Remove("storageVariable");
        }


        private string VirtualPathToUNCPath(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            VirtualPath = VirtualPath.Replace("//","/").Replace("/",@"\").Trim();
            VirtualPath = VirtualPath.StartsWith(@"\") ? VirtualPath : string.Concat(@"\", VirtualPath);
            var storageContainerName = (string.IsNullOrWhiteSpace(Blog.CurrentInstance.StorageContainerName) ? Blog.CurrentInstance.Name : Blog.CurrentInstance.StorageContainerName).Replace(" ", "").Trim();
            var fileContainer = string.Concat(this.uncPath, @"\" ,storageContainerName).Trim();
            if(VirtualPath.ToLower().Contains(fileContainer.ToLower()))
                return VirtualPath;
            return string.Concat(fileContainer,VirtualPath);
        }

        private string CleanVirtualPath(string VirtualPath)
        {
            return VirtualPath.Replace(Blog.CurrentInstance.StorageLocation + Utils.FilesFolder, "").Trim();
        }
        
        /// <summary>
        /// Clears a file system. This will delete all files and folders recursivly.
        /// </summary>
        /// <remarks>
        /// Handle with care... Possibly an internal method?
        /// </remarks>
        public override void ClearFileSystem()
        {
            var root = GetDirectory("");
            foreach(var directory in root.Directories)
                directory.Delete();
            foreach(var file in root.Files)
                file.Delete();
        }

        /// <summary>
        /// Creates a directory at a specific path
        /// </summary>
        /// <param name="VirtualPath">The virtual path to be created</param>
        /// <returns>the new Directory object created</returns>
        /// <remarks>
        /// Virtual path is the path starting from the /files/ containers
        /// The entity is created against the current blog id
        /// </remarks>
        public override FileSystem.Directory CreateDirectory(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            var aPath = VirtualPathToUNCPath(VirtualPath);
            if(!System.IO.Directory.Exists(aPath))
                System.IO.Directory.CreateDirectory(aPath);
            return GetDirectory(VirtualPath);
        }

         /// <summary>
        /// Deletes a spefic directory from a virtual path
        /// </summary>
        /// <param name="VirtualPath">The path to delete</param>
        /// <remarks>
        /// Virtual path is the path starting from the /files/ containers
        /// The entity is queried against to current blog id
        /// </remarks>
        public override void DeleteDirectory(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            if (!this.DirectoryExists(VirtualPath))
                return;
            var aPath = VirtualPathToUNCPath(VirtualPath);
            var sysDir = new System.IO.DirectoryInfo(aPath);
            sysDir.Delete(true);
        }

        /// <summary>
        /// Returns wether or not the specific directory by virtual path exists
        /// </summary>
        /// <param name="VirtualPath">The virtual path to query</param>
        /// <returns>boolean</returns>
        public override bool DirectoryExists(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            var aPath = VirtualPathToUNCPath(VirtualPath);
            return System.IO.Directory.Exists(aPath);
        }

        /// <summary>
        /// gets a directory by the virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <returns>the directory object or null for no directory found</returns>
        public override FileSystem.Directory GetDirectory(string VirtualPath)
        {
            return GetDirectory(VirtualPath, true);
        }

        /// <summary>
        /// gets a directory by the virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <param name="CreateNew">unused</param>
        /// <returns>the directory object</returns>
        public override FileSystem.Directory GetDirectory(string VirtualPath, bool CreateNew)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            var aPath = VirtualPathToUNCPath(VirtualPath);
            var sysDir = new System.IO.DirectoryInfo(aPath);
            if (!sysDir.Exists)
                this.CreateDirectory(VirtualPath);
            var dir = new FileSystem.Directory();
            dir.FullPath = VirtualPath;
            dir.Name = sysDir.Name;
            dir.IsRoot = string.IsNullOrWhiteSpace(VirtualPath);
            dir.LastAccessTime = sysDir.LastAccessTime;
            dir.DateModified = sysDir.LastWriteTime;
            dir.DateCreated = sysDir.CreationTime;
            dir.Id = Guid.NewGuid();
            return dir;
        }

        /// <summary>
        /// gets a directory by a basedirectory and a string array of sub path tree
        /// </summary>
        /// <param name="BaseDirectory">the base directory object</param>
        /// <param name="SubPath">the params of sub path</param>
        /// <returns>the directory found, or null for no directory found</returns>
        public override FileSystem.Directory GetDirectory(FileSystem.Directory BaseDirectory, params string[] SubPath)
        {
            return GetDirectory(string.Join("/", BaseDirectory.FullPath, SubPath), true);
        }

        /// <summary>
        /// gets a directory by a basedirectory and a string array of sub path tree
        /// </summary>
        /// <param name="BaseDirectory">the base directory object</param>
        /// <param name="CreateNew">if set will create the directory structure</param>
        /// <param name="SubPath">the params of sub path</param>
        /// <returns>the directory found, or null for no directory found</returns>
        public override FileSystem.Directory GetDirectory(FileSystem.Directory BaseDirectory, bool CreateNew, params string[] SubPath)
        {
            return GetDirectory(string.Join("/", BaseDirectory.FullPath, SubPath), CreateNew);
        }

         /// <summary>
        /// gets all the directories underneath a base directory. Only searches one level.
        /// </summary>
        /// <param name="BaseDirectory">the base directory</param>
        /// <returns>collection of Directory objects</returns>
        public override IEnumerable<FileSystem.Directory> GetDirectories(FileSystem.Directory BaseDirectory)
        {
            var aPath = VirtualPathToUNCPath(BaseDirectory.FullPath);
            var sysDirectory = new System.IO.DirectoryInfo(aPath);
            return sysDirectory.GetDirectories().Select(x => GetDirectory($"{BaseDirectory.FullPath}/{x.Name}"));
        }


        /// <summary>
        /// gets all the files in a directory, only searches one level
        /// </summary>
        /// <param name="BaseDirectory">the base directory</param>
        /// <returns>collection of File objects</returns>
        public override IEnumerable<FileSystem.File> GetFiles(FileSystem.Directory BaseDirectory)
        {

            var aPath = VirtualPathToUNCPath(BaseDirectory.FullPath);
            var sysDirectory = new DirectoryInfo(aPath);
            return sysDirectory.GetFiles().Where(x => x.Name.ToLower() != "thumbs.db").Select(x => GetFile($"{BaseDirectory.FullPath}/{x.Name}"));
        }

        /// <summary>
        /// gets a specific file by virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path of the file</param>
        /// <returns></returns>
        public override FileSystem.File GetFile(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            var aPath = VirtualPathToUNCPath(VirtualPath);
            var sysFile = new FileInfo(aPath);
            if (!sysFile.Exists)
                throw new FileNotFoundException("The file at " + VirtualPath + " was not found.");

            var file = new FileSystem.File
            {
                FullPath = VirtualPath,
                Name = sysFile.Name,
                DateModified = sysFile.LastWriteTime,
                DateCreated = sysFile.CreationTime,
                Id = Guid.NewGuid().ToString(),
                LastAccessTime = sysFile.LastAccessTime,
                ParentDirectory = GetDirectory(VirtualPath.Substring(0, VirtualPath.LastIndexOf("/"))),
                FilePath = VirtualPath,
                FileSize = sysFile.Length,
            };
            return file;
        }

        /// <summary>
        /// boolean wether a file exists by its virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <returns>boolean</returns>
        public override bool FileExists(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            var aPath = VirtualPathToUNCPath(VirtualPath);
            return File.Exists(aPath);
        }

        /// <summary>
        /// deletes a file by virtual path
        /// </summary>
        /// <param name="VirtualPath">virtual path</param>
        public override void DeleteFile(string VirtualPath)
        {
            VirtualPath = CleanVirtualPath(VirtualPath);
            if (!this.FileExists(VirtualPath))
                return;
            var aPath = VirtualPathToUNCPath(VirtualPath);
            var sysFile = new FileInfo(aPath);
            sysFile.Delete();
        }

        /// <summary>
        /// uploads a file to the provider container
        /// </summary>
        /// <param name="FileBinary">file contents as byte array</param>
        /// <param name="FileName">the file name</param>
        /// <param name="BaseDirectory">directory object that is the owner</param>
        /// <returns>the new file object</returns>
        public override FileSystem.File UploadFile(byte[] FileBinary, string FileName, FileSystem.Directory BaseDirectory)
        {
            return UploadFile(FileBinary, FileName, BaseDirectory, false);
        }

        /// <summary>
        /// uploads a file to the provider container
        /// </summary>
        /// <param name="FileBinary">the contents of the file as a byte array</param>
        /// <param name="FileName">the file name</param>
        /// <param name="BaseDirectory">the directory object that is the owner</param>
        /// <param name="Overwrite">boolean wether to overwrite the file if it exists.</param>
        /// <returns>the new file object</returns>
        public override FileSystem.File UploadFile(byte[] FileBinary, string FileName, FileSystem.Directory BaseDirectory, bool Overwrite)
        {
            var virtualPath = $"{BaseDirectory.FullPath}/{FileName}";
            if (FileExists(virtualPath))
                if (Overwrite)
                    DeleteFile(virtualPath);
                else
                    throw new IOException("File " + virtualPath + " already exists. Unable to upload file.");

            var aPath = VirtualPathToUNCPath(virtualPath);
            File.WriteAllBytes(aPath, FileBinary);
            return GetFile(virtualPath);
        }

        /// <summary>
        /// gets the file contents via Lazy load, however in the DbProvider the Contents are loaded when the initial object is created to cut down on DbReads
        /// </summary>
        /// <param name="BaseFile">the baseFile object to fill</param>
        /// <returns>the original file object</returns>
        internal override FileSystem.File GetFileContents(FileSystem.File BaseFile)
        {
            var aPath = VirtualPathToUNCPath(BaseFile.FullPath);
            BaseFile.FileContents = FileToByteArray(aPath);
            return BaseFile;
        }

        /// <summary>
        /// Converts a file path to a byte array for handler processing
        /// </summary>
        /// <param name="FilePath">the file path to process</param>
        /// <returns>a new binary array</returns>
        private static byte[] FileToByteArray(string FilePath)
        {
            byte[] Buffer = null;

            try
            {
                System.IO.FileStream FileStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.BinaryReader BinaryReader = new System.IO.BinaryReader(FileStream);
                long TotalBytes = new System.IO.FileInfo(FilePath).Length;
                Buffer = BinaryReader.ReadBytes((Int32)TotalBytes);
                FileStream.Close();
                FileStream.Dispose();
                BinaryReader.Close();
            }
            catch (Exception ex)
            {
                Utils.Log("File Provider FileToByArray", ex);
            }

            return Buffer;
        }

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        /// <param name="VirtualPath">unused</param>
        /// <param name="MaximumSize">unused</param>
        /// <returns>Nothing</returns>
        public override FileSystem.Image ImageThumbnail(string VirtualPath, int MaximumSize)
        {
            throw new NotImplementedException();
        }
    }
}
