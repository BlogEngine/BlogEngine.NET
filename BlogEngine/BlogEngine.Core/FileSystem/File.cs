using System;
using System.Linq;
using System.IO;
using System.Web;
using BlogEngine.Core.Providers;

namespace BlogEngine.Core.FileSystem
{
    /// <summary>
    /// The file is a virtual file that is loaded into memory.
    /// </summary>
    /// <remarks>
    ///     Not to be confused with System.IO.File
    /// </remarks>
    public partial class File : BusinessBase<File, string>, IComparable<File>
    {

        #region Constants and Fields
        /// <summary>
        /// the binary of the file
        /// </summary>
        private byte[] contents;
        
        /// <summary>
        /// the file size, in raw long format
        /// </summary>
        private long fileSize;

        /// <summary>
        /// the parent directory of the file
        /// </summary>
        private Directory parentDirectory;

        /// <summary>
        /// the name of the file without the path
        /// </summary>
        private string name;

        /// <summary>
        /// the full path of the file, internal field only, use file path for external calls. reduces security concerns
        /// while outside of the buisness layer
        /// </summary>
        internal string fullpath;

        /// <summary>
        /// the relative file path to the blog file container
        /// </summary>
        private string filepath;

        /// <summary>
        /// list of valid image extensions
        /// </summary>
        private string[] ImageExtensnios = { ".jpg", ".png", ".jpeg", ".tiff", ".gif", ".bmp" };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the File Contents, Lazy loaded
        /// </summary>
        /// <remarks>
        /// set accessor set to internal
        /// </remarks>
        public byte[] FileContents
        {
            get
            {
                if (this.contents == null)
                    FileContents = BlogService.GetFileContents(this).FileContents;
                return this.contents;
            }
            internal set
            {
                base.SetValue("FileContents", value, ref this.contents);
            }
        }

        /// <summary>
        /// gets the file size, in raw long
        /// </summary>
        /// <remarks>
        /// set accessor set to internal
        /// </remarks>
        public long FileSize
        {
            get
            {
                return this.fileSize;
            }
            internal set
            {
                base.SetValue("FileSize", value, ref this.fileSize);
            }
        }

        /// <summary>
        /// gets the parent directory, uses Lazy loading
        /// </summary>
        /// <remarks>
        /// set accessor set to internal
        /// </remarks>
        public Directory ParentDirectory
        {
            get
            {
                if (this.parentDirectory == null)
                {
                    var parent = this.FullPath.Contains("/") ? this.FullPath.Substring(0,this.FullPath.LastIndexOf("/")) : this.FullPath;
                    ParentDirectory = BlogService.GetDirectory(parent);
                }
                return this.parentDirectory;
            }
            internal set
            {
                base.SetValue("ParentDirectory", value, ref this.parentDirectory);
            }
        }

        /// <summary>
        /// gets the file name
        /// </summary>
        /// <remarks>
        /// set accessor set to the internal
        /// </remarks>
        public string Name
        {
            get
            {
                return this.name;
            }
            internal set
            {
                base.SetValue("Name", value, ref this.name);
            }
        }

        /// <summary>
        /// gets the full path, internale set. To change the path use rename methods
        /// </summary>
        internal string FullPath
        {
            get
            {
                return this.fullpath;
            }
            set
            {
                base.SetValue("FullPath", value, ref this.fullpath);
            }
        }

        /// <summary>
        /// gets the relative path to the file
        /// </summary>
        /// <remarks>
        /// set accessor set to internal
        /// </remarks>
        public string FilePath
        {
            get
            {
                return this.filepath;
            }
            internal set
            {
                base.SetValue("FilePath", value, ref this.filepath);
            }
        }

        /// <summary>
        /// gets the file extension
        /// </summary>
        public string Extension
        {
            get
            {
                return Path.GetExtension(Name);
            }
        }

        /// <summary>
        /// gets the safe file name, URL encoded version of this.FilePath
        /// </summary>
        public string SafeFilePath
        {
            get
            {
                return HttpUtility.UrlEncode(this.FilePath);
            }
        }

        /// <summary>
        /// gets the file size in string formated
        /// </summary>
        public string FileSizeFormat
        {
            get
            {
                return SizeFormat(this.FileSize, "N");
            }
        }

        /// <summary>
        /// gets the full file description of name and filesize (formated)
        /// </summary>
        public string FileDescription
        {
            get
            {
                return string.Format("{0} ({1})", this.Name, SizeFormat(this.FileSize, "N"));
            }

        }

        /// <summary>
        /// gets the full download path to the file, using the file handler
        /// </summary>
        public string FileDownloadPath
        {
            get
            {
                return string.Format("{0}file.axd?file={1}", Utils.RelativeWebRoot, this.SafeFilePath);
            }
        }

        /// <summary>
        /// gets the last access time for the file
        /// </summary>
        /// <remarks>
        /// set accessor set to internal
        /// </remarks>
        public DateTime LastAccessTime { get; internal set; }

        /// <summary>
        /// valdidates if this object is an image
        /// </summary>
        public bool IsImage
        {
            get
            {
                return ImageExtensnios.Any(x => x.ToLower() == this.Extension.ToLower());
            }
        }

        /// <summary>
        /// converts the object to the Image object and disposes of the original object. An exception will be thrown if the image is not of a file type.
        /// </summary>
        /// <remarks>
        /// always compare the IsImage flag first before attempting a direct call to AsImage
        /// </remarks>
        public Image AsImage
        {
            get
            {
                var img = new Image(this);
                this.Dispose();
                return img;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// deletes the current file object from the storage container
        /// </summary>
        public void Delete()
        {
            BlogService.DeleteFile(this.FullPath);
            this.Dispose();
        }

        /// <summary>
        /// renames the current file
        /// </summary>
        /// <param name="NewName">the new name of the file</param>
        /// <returns>the new file objbect</returns>
        /// <remarks>
        /// methods performs an upload, then deletes itself. The result will dispose of the original object
        /// </remarks>
        public File Rename(string NewName)
        {
            var newFile = BlogService.UploadFile(this.FileContents, NewName, this.ParentDirectory);
            this.Delete();
            this.Dispose();
            return newFile;
        }

        /// <summary>
        /// moves the file to a new directory &amp; new name
        /// </summary>
        /// <param name="NewName">the new name</param>
        /// <param name="NewBaseDirectory">the new directory</param>
        /// <returns>
        /// the new file object
        /// </returns>
        /// <remarks>
        /// methods perfoms an upload, the deletes iteself. The result will dispose of the original object
        /// </remarks>
        public File MoveFile(string NewName, Directory NewBaseDirectory)
        {
            var newFile = BlogService.UploadFile(this.FileContents, NewName, NewBaseDirectory, true);
            this.Delete();
            this.Dispose();
            return newFile;
        }

        /// <summary>
        /// copys a file from one directory to another
        /// </summary>
        /// <param name="NewBaseDirectory">the new directory</param>
        /// <returns>the new file object</returns>
        /// <remarks>
        /// both object will be maintained after the copy. 
        /// </remarks>
        public File CopyFile(Directory NewBaseDirectory)
        {
            return BlogService.UploadFile(this.FileContents, this.Name, NewBaseDirectory, true);
        }

        /// <summary>
        /// copys a file from one directory to another with a new name
        /// </summary>
        /// <param name="NewName">the new name</param>
        /// <param name="NewBaseDirectory">the new directory</param>
        /// <returns>the new file object</returns>
        /// <remarks>
        /// both object will be maintained after the copy. 
        /// </remarks>
        public File CopyFile(string NewName, Directory NewBaseDirectory)
        {
            return BlogService.UploadFile(this.FileContents, NewName, NewBaseDirectory, true);
        }


        #region Unimplemented Methods

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        protected override void ValidationRules()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        protected override void DataUpdate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        /// <param name="id">unused</param>
        /// <returns>Nothing</returns>
        protected override File DataSelect(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        protected override void DataInsert()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        protected override void DataDelete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        /// <param name="other">unused</param>
        /// <returns>Nothing</returns>
        public int CompareTo(File other)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// formats a file to a predetermided size
        /// </summary>
        /// <param name="size">the file size as a float</param>
        /// <param name="formatString">the numeric format string</param>
        /// <returns>the formated string</returns>
        private static string SizeFormat(float size, string formatString)
        {
            if (size < 1024)
            {
                return string.Format("{0} bytes", size.ToString(formatString));
            }

            if (size < Math.Pow(1024, 2))
            {
                return string.Format("{0} kb", (size / 1024).ToString(formatString));
            }

            if (size < Math.Pow(1024, 3))
            {
                return string.Format("{0} mb", (size / Math.Pow(1024, 2)).ToString(formatString));
            }

            if (size < Math.Pow(1024, 4))
            {
                return string.Format("{0} gb", (size / Math.Pow(1024, 3)).ToString(formatString));
            }

            return size.ToString(formatString);
        }
        #endregion

    }
}
