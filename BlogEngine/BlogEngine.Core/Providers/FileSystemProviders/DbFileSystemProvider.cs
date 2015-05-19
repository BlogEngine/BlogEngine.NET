using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Collections.Specialized;
using System.Configuration.Provider;

namespace BlogEngine.Core.Providers
{
    /// <summary>
    /// DbBlogProvider Parial class for manageing all FileSystem methods
    /// </summary>
    public partial class DbFileSystemProvider : BlogFileSystemProvider
    {

        /// <summary>
        /// The conn string name.
        /// </summary>
        private string connStringName;

        /// <summary>
        /// The parm prefix.
        /// </summary>
        private string parmPrefix;

        /// <summary>
        /// The table prefix.
        /// </summary>
        private string tablePrefix;


        /// <summary>
        /// init
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = "DbBlogProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Generic Database Blog Provider");
            }

            base.Initialize(name, config);

            if (config["storageVariable"] == null)
            {
                // default to BlogEngine
                config["storageVariable"] = "BlogEngine";
            }

            this.connStringName = config["storageVariable"];
            config.Remove("storageVariable");

            if (config["tablePrefix"] == null)
            {
                // default
                config["tablePrefix"] = "be_";
            }

            this.tablePrefix = config["tablePrefix"];
            config.Remove("tablePrefix");

            if (config["parmPrefix"] == null)
            {
                // default
                config["parmPrefix"] = "@";
            }

            this.parmPrefix = config["parmPrefix"];
            config.Remove("parmPrefix");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                var attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                {
                    throw new ProviderException(string.Format("Unrecognized attribute: {0}", attr));
                }
            }
        }

        #region Contansts & Fields
        /// <summary>
        /// active web.config connection string, defined by the blogProvider sections
        /// </summary>
        private string connectionString { get { return WebConfigurationManager.ConnectionStrings[this.connStringName].ConnectionString; } }
        #endregion

        #region Methods

        /// <summary>
        /// Clears a file system. This will delete all files and folders recursivly.
        /// </summary>
        /// <remarks>
        /// Handle with care... Possibly an internal method?
        /// </remarks>
        public override void ClearFileSystem()
        {
            var root = Blog.CurrentInstance.RootFileStore;
            foreach (var directory in root.Directories)
                directory.Delete();
            foreach (var file in root.Files)
                file.Delete();
            root.Delete();
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
            VirtualPath = VirtualPath.VirtualPathToDbPath();
            if (DirectoryExists(VirtualPath))
                return GetDirectory(VirtualPath);
            var directoryName = "root";
            if (!string.IsNullOrWhiteSpace(VirtualPath))
                directoryName = string.Format("/{0}", VirtualPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last());
            FileSystem.FileStoreDb db = new FileSystem.FileStoreDb(this.connectionString);
            FileSystem.FileStoreDirectory dir = new FileSystem.FileStoreDirectory
            {
                BlogID = Blog.CurrentInstance.Id,
                CreateDate = DateTime.Now,
                FullPath = VirtualPath,
                Id = Guid.NewGuid(),
                LastAccess = DateTime.Now,
                Name = directoryName,
                LastModify = DateTime.Now
            };
            if (!string.IsNullOrWhiteSpace(VirtualPath))
            {
                var parentPath = VirtualPath.Contains("/") ? VirtualPath.Substring(0, VirtualPath.LastIndexOf("/")) : string.Empty;
                dir.ParentID = this.GetDirectory(parentPath).Id;
            }
            db.FileStoreDirectories.InsertOnSubmit(dir);
            db.SubmitChanges();
            db.Dispose();
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
            if (!DirectoryExists(VirtualPath.VirtualPathToDbPath()))
                return;
            FileSystem.FileStoreDb db = new FileSystem.FileStoreDb(this.connectionString);
            var query = db.FileStoreDirectories.Where(x => x.FullPath.ToLower() == VirtualPath.VirtualPathToDbPath().ToLower() && x.BlogID == Blog.CurrentInstance.Id);
            foreach (var item in query)
            {
                var subDirectories = db.FileStoreDirectories.Where(x => x.ParentID == item.Id);
                foreach (var sb in subDirectories)
                    DeleteDirectory(sb.FullPath);
            }
            db.FileStoreDirectories.DeleteAllOnSubmit(query);
            db.SubmitChanges();
            db.Dispose();
        }

        /// <summary>
        /// Returns wether or not the specific directory by virtual path exists
        /// </summary>
        /// <param name="VirtualPath">The virtual path to query</param>
        /// <returns>boolean</returns>
        public override bool DirectoryExists(string VirtualPath)
        {
            VirtualPath = VirtualPath.VirtualPathToDbPath();
            return new FileSystem.FileStoreDb(this.connectionString).FileStoreDirectories.Where(x => x.FullPath.ToLower() == VirtualPath.ToLower() && x.BlogID == Blog.CurrentInstance.Id).Count() > 0;
        }

        /// <summary>
        /// gets a directory by the virtual path, creates the directory path if it does not exist
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <returns>the directory object</returns>
        public override FileSystem.Directory GetDirectory(string VirtualPath)
        {
            return GetDirectory(VirtualPath, true);   
        }

        /// <summary>
        /// gets a directory by the virtual path, with a flag to create if not found
        /// </summary>
        /// <param name="VirtualPath">The virtual path</param>
        /// <param name="CreateNew">bool yes \ no to create the director.</param>
        /// <returns>the directory object, or null if the create flag is set to false</returns>
        public override FileSystem.Directory GetDirectory(string VirtualPath, bool CreateNew)
        {
            VirtualPath = VirtualPath.VirtualPathToDbPath();
            FileSystem.FileStoreDb db = new FileSystem.FileStoreDb(this.connectionString);
            if (string.IsNullOrWhiteSpace(VirtualPath))
            {
                var directory = db.FileStoreDirectories.FirstOrDefault(x => x.BlogID == Blog.CurrentInstance.Id && x.ParentID == null);
                if (directory == null)
                {
                    db.Dispose();
                    return this.CreateDirectory(VirtualPath);
                }
                var obj = directory.CopyToDirectory();
                db.Dispose();
                return obj;
            }
            else
            {
                var dir = db.FileStoreDirectories.FirstOrDefault(x => x.FullPath.ToLower() == VirtualPath.ToLower() && x.BlogID == Blog.CurrentInstance.Id);
                if (dir == null)
                {
                    var newDirectoryPieces = VirtualPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var cPath = string.Empty;
                    foreach (var pieces in newDirectoryPieces)
                    {
                        cPath = string.Format("{0}/{1}", cPath, pieces);
                        if (!DirectoryExists(cPath))
                            CreateDirectory(cPath);
                    }
                    var nObj = GetDirectory(VirtualPath);
                    db.Dispose();
                    return nObj;
                }
                var obj = dir.CopyToDirectory();
                db.Dispose();
                return obj;
            }
        }

        /// <summary>
        /// gets a directory by a basedirectory and a string array of sub path tree
        /// </summary>
        /// <param name="BaseDirectory">the base directory object</param>
        /// <param name="SubPath">the params of sub path</param>
        /// <returns>the directory found, or null for no directory found</returns>
        public override FileSystem.Directory GetDirectory(FileSystem.Directory BaseDirectory, params string[] SubPath)
        {
            return GetDirectory(string.Concat(BaseDirectory.FullPath, string.Join("/", SubPath)));
        }

        /// <summary>
        /// gets a directory by a basedirectory and a string array of sub path tree.
        /// </summary>
        /// <param name="BaseDirectory">the base directory object</param>
        /// <param name="CreateNew">If true, creates the directory if it does not already exist.</param>
        /// <param name="SubPath">the params of sub path</param>
        /// <returns>The directory found, or null for no directory found</returns>
        public override FileSystem.Directory GetDirectory(FileSystem.Directory BaseDirectory, bool CreateNew, params string[] SubPath)
        {
            return GetDirectory(string.Concat(BaseDirectory.FullPath, string.Join("/", SubPath)), CreateNew);
        }

        /// <summary>
        /// gets all the directories underneath a base directory. Only searches one level.
        /// </summary>
        /// <param name="BaseDirectory">the base directory</param>
        /// <returns>collection of Directory objects</returns>
        public override IEnumerable<FileSystem.Directory> GetDirectories(FileSystem.Directory BaseDirectory)
        {
            FileSystem.FileStoreDb db = new FileSystem.FileStoreDb(this.connectionString);
            var dirs = db.FileStoreDirectories.Where(x => x.ParentID == BaseDirectory.Id && x.BlogID == Blog.CurrentInstance.Id);
            List<FileSystem.Directory> directories = new List<FileSystem.Directory>();
            foreach (var d in dirs)
                directories.Add(d.CopyToDirectory());
            db.Dispose();
            return directories;
        }

        /// <summary>
        /// gets all the files in a directory, only searches one level
        /// </summary>
        /// <param name="BaseDirectory">the base directory</param>
        /// <returns>collection of File objects</returns>
        public override IEnumerable<FileSystem.File> GetFiles(FileSystem.Directory BaseDirectory)
        {
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var fileDir = db.FileStoreDirectories.FirstOrDefault(x=>x.Id == BaseDirectory.Id);
            if (fileDir == null)
                return new List<FileSystem.File>();
            var arr = fileDir.FileStoreFiles.Select(x => x.CopyToFile()).ToList();
            db.Dispose();
            return arr;
        }

        /// <summary>
        /// gets a specific file by virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path of the file</param>
        /// <returns></returns>
        public override FileSystem.File GetFile(string VirtualPath)
        {
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var file = db.FileStoreFiles.FirstOrDefault(x => x.FullPath == VirtualPath.VirtualPathToDbPath() && x.FileStoreDirectory.BlogID == Blog.CurrentInstance.Id).CopyToFile();
            db.Dispose();
            return file;
        }

        /// <summary>
        /// boolean wether a file exists by its virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <returns>boolean</returns>
        public override bool FileExists(string VirtualPath)
        {
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var file = db.FileStoreFiles.FirstOrDefault(x => x.FullPath.ToLower() == VirtualPath.VirtualPathToDbPath() && x.FileStoreDirectory.BlogID == Blog.CurrentInstance.Id) == null;
            db.Dispose();
            return file;
        }

        /// <summary>
        /// deletes a file by virtual path
        /// </summary>
        /// <param name="VirtualPath">virtual path</param>
        public override void DeleteFile(string VirtualPath)
        {
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var file = db.FileStoreFiles.Where(x => x.FullPath == VirtualPath.VirtualPathToDbPath() && x.FileStoreDirectory.BlogID == Blog.CurrentInstance.Id);
            db.FileStoreFiles.DeleteAllOnSubmit(file);
            db.SubmitChanges();
            db.Dispose();
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
            var virtualPath = BaseDirectory.FullPath + "/" + FileName;
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var files = db.FileStoreFiles.Where(x => x.FileStoreDirectory.Id == BaseDirectory.Id && x.FullPath.ToLower() == virtualPath);
            if (files.Count() > 0)
            {
                if (!Overwrite)
                {
                    db.Dispose();
                    throw new Exception("File " + FileName + " already exists in this path.");
                }
                db.FileStoreFiles.DeleteAllOnSubmit(files);
                db.SubmitChanges();
            }
            var file = new FileSystem.FileStoreFile()
            {
                Contents = FileBinary,
                CreateDate = DateTime.Now,
                FileID = Guid.NewGuid(),
                FullPath = virtualPath,
                LastAccess = DateTime.Now,
                LastModify = DateTime.Now,
                Name = FileName,
                ParentDirectoryID = BaseDirectory.Id,
                Size = FileBinary.Length
            };
            db.FileStoreFiles.InsertOnSubmit(file);
            db.SubmitChanges();
            db.Dispose();
            return file.CopyToFile();
        }

        /// <summary>
        /// gets the file contents via Lazy load, however in the DbProvider the Contents are loaded when the initial object is created to cut down on DbReads
        /// </summary>
        /// <param name="BaseFile">the baseFile object to fill</param>
        /// <returns>the original file object</returns>
        internal override FileSystem.File GetFileContents(FileSystem.File BaseFile)
        {
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var file = db.FileStoreFiles.FirstOrDefault(x => x.FileID == Guid.Parse(BaseFile.Id));
            if (file == null)
                throw new ArgumentException("File not found in dataset");
            BaseFile.FileContents = file.Contents.ToArray();
            db.Dispose();
            return BaseFile;
        }

        /// <summary>
        /// Returns a thumbnail image at a maximum size. Only one size is provided as the thumbnail will be scaled down. If the thumbnail does not exist the thumbnail is created
        /// </summary>
        /// <param name="VirtualPath">The virtual path of the image</param>
        /// <param name="MaximumSize">The maximum size for the image</param>
        /// <returns>The image with the thumbnail contents</returns>
        /// <remarks>
        /// this is a virtual file and all actual file methods will not be available.
        /// </remarks>
        public override FileSystem.Image ImageThumbnail(string VirtualPath, int MaximumSize)
        {
            var file = GetFile(VirtualPath);
            if (!file.IsImage)
                return null;
            var db = new FileSystem.FileStoreDb(this.connectionString);
            var image = file.AsImage;
            var thumbnail = db.FileStoreFileThumbs.FirstOrDefault(x => x.FileId == Guid.Parse(image.Id));
            if (thumbnail == null)
            {

                FileSystem.FileStoreFileThumb thumb = new FileSystem.FileStoreFileThumb()
                {
                    contents = FileSystem.Image.ResizeImageThumbnail(MaximumSize, image.FileContents),
                    FileId = Guid.Parse(image.Id),
                    size = MaximumSize,
                    thumbnailId = Guid.NewGuid()
                };
                db.FileStoreFileThumbs.InsertOnSubmit(thumb);
                db.SubmitChanges();
                image.FileContents = thumb.contents.ToArray();
            }
            else
                image.FileContents = thumbnail.contents.ToArray();
            db.Dispose();
            return image;
        }

        #endregion
    }

    #region Extension Methods
    /// <summary>
    /// static classes for the DbFileSystem
    /// </summary>
    public static class DbFileSystemExtensions
    {
        /// <summary>
        /// copy's the database directory object to a Directory object
        /// </summary>
        /// <param name="inObj">the database directory to copy</param>
        /// <returns>a new Directory object</returns>
        public static FileSystem.Directory CopyToDirectory(this FileSystem.FileStoreDirectory inObj)
        {
            if (inObj == null)
                return null;
            return new FileSystem.Directory()
            {
                DateCreated = inObj.CreateDate,
                DateModified = inObj.LastModify,
                FullPath = inObj.FullPath,
                Id = inObj.Id,
                LastAccessTime = inObj.LastAccess,
                Name = inObj.Name,
                IsRoot = inObj.ParentID == null,
            };
        }

        /// <summary>
        /// copys a database File object to a File object
        /// </summary>
        /// <param name="inObj">the database file object to copy</param>
        /// <returns>a new File object</returns>
        public static FileSystem.File CopyToFile(this FileSystem.FileStoreFile inObj)
        {
            if (inObj == null)
                return null;
            return new FileSystem.File()
            {
                DateCreated = inObj.CreateDate,
                DateModified = inObj.LastModify,
                FilePath = inObj.FullPath,
                FileSize = inObj.Size,
                Id = inObj.FileID.ToString(),
                LastAccessTime = inObj.LastAccess,
                Name = inObj.Name,
                FileContents = inObj.Contents.ToArray(),
                FullPath = inObj.FullPath
            };

        }

        /// <summary>
        /// removes the virtual path of BlogStorageLocation + files from the virtual path. 
        /// </summary>
        /// <param name="VirtualPath">the virtual path to replace against</param>
        /// <returns>the repleaced string</returns>
        public static string VirtualPathToDbPath(this string VirtualPath)
        {
            return VirtualPath.Replace(Blog.CurrentInstance.StorageLocation + Utils.FilesFolder, "");
        }
    }
    #endregion

}
