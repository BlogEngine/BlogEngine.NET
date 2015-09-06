using System;
using System.Collections.Generic;
using BlogEngine.Core.Providers;

namespace BlogEngine.Core.FileSystem
{
    /// <summary>
    /// A directory is a virtual data store directory,
    ///     the directory may contain files, and directories
    /// </summary>
    /// <remarks>
    ///     Not to be confused with System.IO.Directory, this is a virtual directory that is part of a blog based filesystem
    /// </remarks>
    public partial class Directory : BusinessBase<Directory, Guid>, IComparable<Directory>
    {
        #region Fields & Constants

        /// <summary>
        /// directory name
        /// </summary>
        private string name;

        /// <summary>
        /// directory full path
        /// </summary>
        private string fullpath;

        /// <summary>
        /// is root directory?
        /// </summary>
        private bool isroot;
        
        /// <summary>
        /// parent directory
        /// </summary>
        private Directory parent;

        /// <summary>
        /// directories in the directory, lazy loaded
        /// </summary>
        private IEnumerable<Directory> directories;

        /// <summary>
        /// files in the directory, lazy loaded
        /// </summary>
        private IEnumerable<File> files;
        
        #endregion

        #region Properties

        /// <summary>
        /// gets the Directory name
        /// </summary>
        /// <remarks>
        /// set accessor interal
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
        /// gets the full path to the directory.
        /// </summary>
        /// <remarks>
        /// set accessor marked internal
        /// </remarks>
        public string FullPath
        {
            get
            {
                return this.fullpath;
            }
            internal set
            {
                base.SetValue("FullPath", value, ref this.fullpath);
            }
        }

        /// <summary>
        /// gets the flag wether or not this is the root directory
        /// </summary>
        /// <remarks>
        /// set accessor marked internal
        /// </remarks>
        public bool IsRoot
        {
            get
            {
                return this.isroot;
            }
            internal set
            {
                base.SetValue("IsRoot", value, ref this.isroot);
            }
        }

        /// <summary>
        /// gets the parent directory for the direct
        /// </summary>
        /// <remarks>
        /// set accessor marked as internal
        /// </remarks>
        public Directory Parent
        {
            get
            {
                if (this.parent == null && !this.isroot)
                {
                    var ppath = this.FullPath.Contains("/") ? this.FullPath.Substring(0, this.FullPath.LastIndexOf("/")) : string.Empty;
                    Parent = BlogService.GetDirectory(ppath);
                }
                return this.parent;
            }
            internal set
            {
                base.SetValue("Parent", value, ref this.parent);
            }
        }

        /// <summary>
        /// gets directories contained within this directory, Lazy loaded
        /// </summary>
        /// <remarks>
        /// set marked as internal
        /// </remarks>
        public IEnumerable<Directory> Directories
        {
            get
            {
                if (this.directories == null)
                    Directories = BlogService.GetDirectories(this);
                return this.directories;
            }
            internal set
            {
                base.SetValue("Directories", value, ref this.directories);
            }
        }

        /// <summary>
        /// gets the Files in this directory, Lazy loaded
        /// </summary>
        /// <remarks>
        /// set accessor marked as internal
        /// </remarks>
        public IEnumerable<File> Files
        {
            get
            {
                if (this.files == null)
                    Files = BlogService.GetFiles(this);
                return this.files;
            }
            internal set
            {
                base.SetValue("Files", value, ref this.files);
            }
        }

        /// <summary>
        /// gets the last access time
        /// </summary>
        /// <remarks>
        /// set marked as internal
        /// </remarks>
        public DateTime LastAccessTime { get; internal set; }
        
        #endregion

        #region Methods

        /// <summary>
        /// Creates a subdirectory in the current directory
        /// </summary>
        /// <param name="Name">Name of the new directory</param>
        /// <returns>the new directory</returns>
        public Directory CreateSubDirectory(string Name)
        {
            return BlogService.CreateDirectory(this.FullPath + "/" + Name);
        }

        /// <summary>
        /// Delets a subdirectory by name
        /// </summary>
        /// <param name="Name"></param>
        public void DeleteSubDirectory(string Name)
        {
            BlogService.DeleteDirectory(this.FullPath + "/" + Name);
        }

        /// <summary>
        /// Deletes the current directory, and all subsequent files
        /// </summary>
        public void Delete()
        {
            BlogService.DeleteDirectory(this.FullPath);
            this.Dispose();
        }

        /// <summary>
        /// Deletes all files in the directory.
        /// </summary>
        public void DeleteAllFiles()
        {
            foreach (var file in this.Files)
                file.Delete();
            this.files = null;
        }

        /// <summary>
        /// Deletes all the directories, including subdirectories &amp; files.
        /// </summary>
        public void DeleteAllDirectories()
        {
            foreach (var directory in this.Directories)
                directory.Delete();
            this.directories = null;
        }

        #endregion

        #region Interfaces (not implemented)

        /// <summary>
        /// Not implemented. Throws a NotImplementedException.
        /// </summary>
        /// <param name="other">unused</param>
        /// <returns>Nothing</returns>
        public int CompareTo(Directory other)
        {
            throw new NotImplementedException();
        }

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
        /// <para>unused</para>
        /// <returns>Nothing</returns>
        protected override Directory DataSelect(Guid id)
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
        
        #endregion
    }
}
