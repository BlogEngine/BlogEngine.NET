using BlogEngine.Core.Packaging;

namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Web.Configuration;

    using BlogEngine.Core.DataStore;
    using BlogEngine.Core.FileSystem;   

    /// <summary>
    /// The proxy class for communication between
    ///     the business objects and the providers.
    /// </summary>
    public static class BlogService
    {
        #region Constants and Fields

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object TheLock = new object();

        /// <summary>
        /// The provider. Don't access this directly. Access it through the property accessor.
        /// </summary>
        private static BlogProvider _provider;

        /// <summary>
        /// the file storage provider. Don't access this directly. Access it the property accessor
        /// </summary>
        private static BlogFileSystemProvider _fileStorageProvider;

        /// <summary>
        /// The providers.
        /// </summary>
        private static BlogProviderCollection _providers;


        private static BlogFileSystemProviderCollection _fileProviders;
        #endregion

        #region Properties

        /// <summary>
        ///     gets the current FileSystem provider
        /// </summary>
        public static BlogFileSystemProvider FileSystemProvider
        {
            get
            {
                LoadProviders();
                return _fileStorageProvider;
            }
        }

        /// <summary>
        ///     Gets a collection of FileSystem providers that are defined in Web.config.
        /// </summary>
        public static BlogFileSystemProviderCollection FileSystemProviders
        {
            get
            {
                LoadProviders();
                return _fileProviders;
            }
        }

        /// <summary>
        ///     Gets the current provider.
        /// </summary>
        public static BlogProvider Provider
        {
            get
            {
                LoadProviders();
                return _provider;
            }
        }

        internal static void ReloadFileSystemProvider()
        {
            _fileStorageProvider = null;
            LoadProviders();
        }

        /// <summary>
        ///     Gets a collection of all registered providers.
        /// </summary>
        public static BlogProviderCollection Providers
        {
            get
            {
                LoadProviders();
                return _providers;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes the specified BlogRoll from the current provider.
        /// </summary>
        /// <param name="blogRoll">
        /// The blog Roll.
        /// </param>
        public static void DeleteBlogRoll(BlogRollItem blogRoll)
        {
            Provider.DeleteBlogRollItem(blogRoll);
        }

        /// <summary>
        /// Deletes the specified Blog from the current provider.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public static void DeleteBlog(Blog blog)
        {
            Provider.DeleteBlog(blog);
        }

        /// <summary>
        /// Deletes the storage container for the specified Blog from the current provider.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public static bool DeleteBlogStorageContainer(Blog blog)
        {
            return Provider.DeleteBlogStorageContainer(blog);
        }

        /// <summary>
        /// Deletes the specified Category from the current provider.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public static void DeleteCategory(Category category)
        {
            Provider.DeleteCategory(category);
        }

        /// <summary>
        /// Deletes the specified Page from the current provider.
        /// </summary>
        /// <param name="page">
        /// The page to delete.
        /// </param>
        public static void DeletePage(Page page)
        {
            Provider.DeletePage(page);
        }

        /// <summary>
        /// Deletes the specified Post from the current provider.
        /// </summary>
        /// <param name="post">
        /// The post to delete.
        /// </param>
        public static void DeletePost(Post post)
        {
            Provider.DeletePost(post);
        }

        /// <summary>
        /// Deletes the specified Page from the current provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to delete.
        /// </param>
        public static void DeleteProfile(AuthorProfile profile)
        {
            Provider.DeleteProfile(profile);
        }

        /// <summary>
        /// Returns a list of all BlogRolls in the current provider.
        /// </summary>
        /// <returns>
        /// A list of BlogRollItem.
        /// </returns>
        public static List<BlogRollItem> FillBlogRolls()
        {
            return Provider.FillBlogRoll();
        }

        /// <summary>
        /// The fill categories.
        /// </summary>
        /// <returns>
        /// A list of Category.
        /// </returns>
        public static List<Category> FillCategories(Blog blog)
        {
            return Provider.FillCategories(blog);
        }

        /// <summary>
        /// The fill pages.
        /// </summary>
        /// <returns>
        /// A list of Page.
        /// </returns>
        public static List<Page> FillPages()
        {
            return Provider.FillPages();
        }

        /// <summary>
        /// The fill posts.
        /// </summary>
        /// <returns>
        /// A list of Post.
        /// </returns>
        public static List<Post> FillPosts()
        {
            return Provider.FillPosts();
        }

        /// <summary>
        /// The fill blogs.
        /// </summary>
        /// <returns>
        /// A list of Blogs.
        /// </returns>
        public static List<Blog> FillBlogs()
        {
            return Provider.FillBlogs();
        }

        /// <summary>
        /// The fill profiles.
        /// </summary>
        /// <returns>
        /// A list of AuthorProfile.
        /// </returns>
        public static List<AuthorProfile> FillProfiles()
        {
            return Provider.FillProfiles();
        }

        /// <summary>
        /// Returns a list of all Referrers in the current provider.
        /// </summary>
        /// <returns>
        /// A list of Referrer.
        /// </returns>
        public static List<Referrer> FillReferrers()
        {
            return Provider.FillReferrers();
        }

        /// <summary>
        /// Returns a dictionary representing rights and the roles that allow them.
        /// </summary>
        /// <returns>
        /// 
        /// The key must be a string of the name of the Rights enum of the represented Right.
        /// The value must be an IEnumerable of strings that includes only the role names of
        /// roles the right represents.
        /// 
        /// Inheritors do not need to worry about verifying that the keys and values are valid.
        /// This is handled in the Right class.
        /// 
        /// </returns>
        public static IDictionary<string, IEnumerable<string>> FillRights()
        {
            return Provider.FillRights();
        }

        /// <summary>
        /// Persists a new BlogRoll in the current provider.
        /// </summary>
        /// <param name="blogRoll">
        /// The blog Roll.
        /// </param>
        public static void InsertBlogRoll(BlogRollItem blogRoll)
        {
            Provider.InsertBlogRollItem(blogRoll);
        }

        /// <summary>
        /// Persists a new Blog in the current provider.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public static void InsertBlog(Blog blog)
        {
            Provider.InsertBlog(blog);
        }

        /// <summary>
        /// Persists a new Category in the current provider.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public static void InsertCategory(Category category)
        {
            Provider.InsertCategory(category);
        }

        /// <summary>
        /// Persists a new Page in the current provider.
        /// </summary>
        /// <param name="page">
        /// The page to insert.
        /// </param>
        public static void InsertPage(Page page)
        {
            Provider.InsertPage(page);
        }

        /// <summary>
        /// Persists a new Post in the current provider.
        /// </summary>
        /// <param name="post">
        /// The post to insert.
        /// </param>
        public static void InsertPost(Post post)
        {
            Provider.InsertPost(post);
        }

        /// <summary>
        /// Persists a new Page in the current provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to insert.
        /// </param>
        public static void InsertProfile(AuthorProfile profile)
        {
            Provider.InsertProfile(profile);
        }

        /// <summary>
        /// Persists a new Referrer in the current provider.
        /// </summary>
        /// <param name="referrer">
        /// The referrer to insert.
        /// </param>
        public static void InsertReferrer(Referrer referrer)
        {
            Provider.InsertReferrer(referrer);
        }

        /// <summary>
        /// Loads settings from data storage
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <returns>
        /// Settings as stream
        /// </returns>
        public static object LoadFromDataStore(ExtensionType extensionType, string extensionId)
        {
            return Provider.LoadFromDataStore(extensionType, extensionId);
        }

        /// <summary>
        /// Loads the ping services.
        /// </summary>
        /// <returns>A StringCollection.</returns>
        public static StringCollection LoadPingServices()
        {
            return Provider.LoadPingServices();
        }

        /// <summary>
        /// Loads the settings from the provider and returns
        /// them in a StringDictionary for the BlogSettings class to use.
        /// </summary>
        /// <returns>A StringDictionary.</returns>
        public static StringDictionary LoadSettings(Blog blog)
        {
            return Provider.LoadSettings(blog);
        }

        /// <summary>
        /// Loads the stop words from the data store.
        /// </summary>
        /// <returns>A StringCollection.</returns>
        public static StringCollection LoadStopWords()
        {
            return Provider.LoadStopWords();
        }

        /// <summary>
        /// Removes object from data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        public static void RemoveFromDataStore(ExtensionType extensionType, string extensionId)
        {
            Provider.RemoveFromDataStore(extensionType, extensionId);
        }

        /// <summary>
        /// Saves the ping services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public static void SavePingServices(StringCollection services)
        {
            Provider.SavePingServices(services);
        }

        /// <summary>
        /// Saves all of the current BlogEngine rights to the provider.
        /// </summary>
        public static void SaveRights()
        {
            Provider.SaveRights(Right.GetAllRights());

            // This needs to be called after rights are changed.
            Right.RefreshAllRights();
        }

        /// <summary>
        /// Save the settings to the current provider.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public static void SaveSettings(StringDictionary settings)
        {
            Provider.SaveSettings(settings);
        }

        /// <summary>
        /// Saves settings to data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extensio ID
        /// </param>
        /// <param name="settings">
        /// Settings object
        /// </param>
        public static void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings)
        {
            Provider.SaveToDataStore(extensionType, extensionId, settings);
        }

        /// <summary>
        /// Returns a BlogRoll based on the specified id.
        /// </summary>
        /// <param name="id">The BlogRoll id.</param>
        /// <returns>A BlogRollItem.</returns>
        public static BlogRollItem SelectBlogRoll(Guid id)
        {
            return Provider.SelectBlogRollItem(id);
        }

        /// <summary>
        /// Returns a Blog based on the specified id.
        /// </summary>
        /// <param name="id">The Blog id.</param>
        /// <returns>A Blog.</returns>
        public static Blog SelectBlog(Guid id)
        {
            return Provider.SelectBlog(id);
        }

        /// <summary>
        /// Returns a Category based on the specified id.
        /// </summary>
        /// <param name="id">The Category id.</param>
        /// <returns>A Category.</returns>
        public static Category SelectCategory(Guid id)
        {
            return Provider.SelectCategory(id);
        }

        /// <summary>
        /// Returns a Page based on the specified id.
        /// </summary>
        /// <param name="id">The Page id.</param>
        /// <returns>A Page object.</returns>
        public static Page SelectPage(Guid id)
        {
            return Provider.SelectPage(id);
        }

        /// <summary>
        /// Returns a Post based on the specified id.
        /// </summary>
        /// <param name="id">The post id.</param>
        /// <returns>A Post object.</returns>
        public static Post SelectPost(Guid id)
        {
            return Provider.SelectPost(id);
        }

        /// <summary>
        /// Returns a Page based on the specified id.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>An AuthorProfile.</returns>
        public static AuthorProfile SelectProfile(string id)
        {
            return Provider.SelectProfile(id);
        }

        /// <summary>
        /// Returns a Referrer based on the specified id.
        /// </summary>
        /// <param name="id">The Referrer Id.</param>
        /// <returns>A Referrer.</returns>
        public static Referrer SelectReferrer(Guid id)
        {
            return Provider.SelectReferrer(id);
        }

        /// <summary>
        /// Sets up the required storage files/tables for a new Blog instance, from an existing blog instance.
        /// </summary>
        /// <param name="existingBlog">The existing blog instance to base the new blog instance off of.</param>
        /// <param name="newBlog">The new blog instance.</param>
        /// <returns>A boolean indicating if the setup process was successful.</returns>
        public static bool SetupBlogFromExistingBlog(Blog existingBlog, Blog newBlog)
        {
            return Provider.SetupBlogFromExistingBlog(existingBlog, newBlog);
        }

        /// <summary>
        /// Setup new blog
        /// </summary>
        /// <param name="newBlog">New blog</param>
        /// <param name="userName">User name</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns>True if successful</returns>
        public static bool SetupNewBlog(Blog newBlog, string userName, string email, string password)
        {
            return Provider.SetupNewBlog(newBlog, userName, email, password);
        }

        /// <summary>
        /// Updates an exsiting BlogRoll.
        /// </summary>
        /// <param name="blogRoll">
        /// The blog Roll.
        /// </param>
        public static void UpdateBlogRoll(BlogRollItem blogRoll)
        {
            Provider.UpdateBlogRollItem(blogRoll);
        }

        /// <summary>
        /// Updates an exsiting Blog.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public static void UpdateBlog(Blog blog)
        {
            Provider.UpdateBlog(blog);
        }

        /// <summary>
        /// Updates an exsiting Category.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public static void UpdateCategory(Category category)
        {
            Provider.UpdateCategory(category);
        }

        /// <summary>
        /// Updates an exsiting Page.
        /// </summary>
        /// <param name="page">
        /// The page to update.
        /// </param>
        public static void UpdatePage(Page page)
        {
            Provider.UpdatePage(page);
        }

        /// <summary>
        /// Updates an exsiting Post.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        public static void UpdatePost(Post post)
        {
            Provider.UpdatePost(post);
        }

        /// <summary>
        /// Updates an exsiting Page.
        /// </summary>
        /// <param name="profile">
        /// The profile to update.
        /// </param>
        public static void UpdateProfile(AuthorProfile profile)
        {
            Provider.UpdateProfile(profile);
        }

        /// <summary>
        /// Updates an existing Referrer.
        /// </summary>
        /// <param name="referrer">
        /// The referrer to update.
        /// </param>
        public static void UpdateReferrer(Referrer referrer)
        {
            Provider.UpdateReferrer(referrer);
        }

        #region FileSystem Static Methods

        internal static void ClearFileSystem()
        {
            FileSystemProvider.ClearFileSystem();
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
        public static Directory CreateDirectory(string VirtualPath)
        {
            return FileSystemProvider.CreateDirectory(VirtualPath);
        }

        /// <summary>
        /// Deletes a spefic directory from a virtual path
        /// </summary>
        /// <param name="VirtualPath">The path to delete</param>
        /// <remarks>
        /// Virtual path is the path starting from the /files/ containers
        /// The entity is queried against to current blog id
        /// </remarks>
        public static void DeleteDirectory(string VirtualPath)
        {
            FileSystemProvider.DeleteDirectory(VirtualPath);
        }

        /// <summary>
        /// Deletes a directory by passing in the Directory object
        /// </summary>
        /// <param name="DirectoryObj">the DirectoryObj </param>
        public static void DeleteDirectory(Directory DirectoryObj)
        {
            DeleteDirectory(DirectoryObj.FullPath);
        }

        /// <summary>
        /// Returns wether or not the specific directory by virtual path exists
        /// </summary>
        /// <param name="VirtualPath">The virtual path to query</param>
        /// <returns>boolean</returns>
        public static bool DirectoryExists(string VirtualPath)
        {
            return FileSystemProvider.DirectoryExists(VirtualPath);
        }

        /// <summary>
        /// gets a directory by the virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <returns>the directory object or null for no directory found</returns>
        public static Directory GetDirectory(string VirtualPath)
        {
            return FileSystemProvider.GetDirectory(VirtualPath);
        }

        /// <summary>
        /// gets a directory by a basedirectory and a string array of sub path tree
        /// </summary>
        /// <param name="BaseDirectory">the base directory object</param>
        /// <param name="SubPath">the params of sub path</param>
        /// <returns>the directory found, or null for no directory found</returns>
        public static Directory GetDirectory(Directory BaseDirectory, params string[] SubPath)
        {
            return FileSystemProvider.GetDirectory(BaseDirectory, SubPath);
        }

        /// <summary>
        /// gets all the directories underneath a base directory. Only searches one level.
        /// </summary>
        /// <param name="BaseDirectory">the base directory</param>
        /// <returns>collection of Directory objects</returns>
        internal static IEnumerable<Directory> GetDirectories(Directory BaseDirectory)
        {
            return FileSystemProvider.GetDirectories(BaseDirectory);
        }

        /// <summary>
        /// gets all the files in a directory, only searches one level
        /// </summary>
        /// <param name="BaseDirectory">the base directory</param>
        /// <returns>collection of File objects</returns>
        public static IEnumerable<File> GetFiles(Directory BaseDirectory)
        {
            return FileSystemProvider.GetFiles(BaseDirectory);
        }

        /// <summary>
        /// gets a specific file by virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path of the file</param>
        /// <returns></returns>
        public static File GetFile(string VirtualPath)
        {
            return FileSystemProvider.GetFile(VirtualPath);
        }

        /// <summary>
        /// boolean wether a file exists by its virtual path
        /// </summary>
        /// <param name="VirtualPath">the virtual path</param>
        /// <returns>boolean</returns>
        public static bool FileExists(string VirtualPath)
        {
            return FileSystemProvider.FileExists(VirtualPath);
        }

        /// <summary>
        /// deletes a file by virtual path
        /// </summary>
        /// <param name="VirtualPath">virtual path</param>
        public static void DeleteFile(string VirtualPath)
        {
            FileSystemProvider.DeleteFile(VirtualPath);
        }

        /// <summary>
        /// uploads a file to the provider container
        /// </summary>
        /// <param name="FileBinary">file contents as byte array</param>
        /// <param name="FileName">the file name</param>
        /// <param name="BaseDirectory">directory object that is the owner</param>
        /// <returns>the new file object</returns>
        public static FileSystem.File UploadFile(byte[] FileBinary, string FileName, FileSystem.Directory BaseDirectory)
        {
            return FileSystemProvider.UploadFile(FileBinary, FileName, BaseDirectory);
        }

        /// <summary>
        /// uploads a file to the provider container
        /// </summary>
        /// <param name="FileBinary">the contents of the file as a byte array</param>
        /// <param name="FileName">the file name</param>
        /// <param name="BaseDirectory">the directory object that is the owner</param>
        /// <param name="Overwrite">boolean wether to overwrite the file if it exists.</param>
        /// <returns>the new file object</returns>
        public static FileSystem.File UploadFile(byte[] FileBinary, string FileName, FileSystem.Directory BaseDirectory, bool Overwrite)
        {
            return FileSystemProvider.UploadFile(FileBinary, FileName, BaseDirectory, Overwrite);
        }

        /// <summary>
        /// uploads a file to the provider container
        /// </summary>
        /// <param name="FileStream">the file stream of the file being uploaded</param>
        /// <param name="FileName">the file name</param>
        /// <param name="BaseDirectory">the directory object that is the owner</param>
        /// <returns>the new file object</returns>
        public static FileSystem.File UploadFile(System.IO.Stream FileStream, string FileName, FileSystem.Directory BaseDirectory)
        {
            return UploadFile(FileStream, FileName, BaseDirectory, false);
        }

        /// <summary>
        /// uploads a file to the provider container
        /// </summary>
        /// <param name="FileStream">the file stream of the file being uploaded</param>
        /// <param name="FileName">the file name</param>
        /// <param name="BaseDirectory">the directory object that is the owner</param>
        /// <param name="Overwrite">boolean wether to overwrite the file if it exists.</param>
        /// <returns>the new file object</returns>
        public static FileSystem.File UploadFile(System.IO.Stream FileStream, string FileName, FileSystem.Directory BaseDirectory, bool Overwrite)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            FileStream.CopyTo(ms);
            FileStream.Close();
            byte[] binary = ms.ToArray();
            ms.Close();
            return FileSystemProvider.UploadFile(binary, FileName, BaseDirectory, Overwrite);
        }

        /// <summary>
        /// gets the file contents via Lazy load, however in the DbProvider the Contents are loaded when the initial object is created to cut down on DbReads
        /// </summary>
        /// <param name="BaseFile">the baseFile object to fill</param>
        /// <returns>the original file object</returns>
        internal static File GetFileContents(File BaseFile)
        {
            return FileSystemProvider.GetFileContents(BaseFile);
        }
        #endregion

        #region Packaging

        /// <summary>
        /// Save installed gallery package
        /// </summary>
        /// <param name="package">Installed package</param>
        public static void InsertPackage(InstalledPackage package)
        {
            Provider.SavePackage(package);
        }
        /// <summary>
        /// Save package files
        /// </summary>
        /// <param name="packageFiles">List of package files</param>
        public static void InsertPackageFiles(List<PackageFile> packageFiles)
        {
            Provider.SavePackageFiles(packageFiles);
        }

        /// <summary>
        /// Packages installed from online gallery
        /// </summary>
        /// <returns></returns>
        public static List<InstalledPackage> InstalledFromGalleryPackages()
        {
            return Provider.FillPackages();
        }

        /// <summary>
        /// Log of files installed by gallery package
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <returns>List of files</returns>
        public static List<PackageFile> InstalledFromGalleryPackageFiles(string packageId)
        {
            return Provider.FillPackageFiles(packageId);
        }

        /// <summary>
        /// Delete all installed by package files from application
        /// </summary>
        /// <param name="packageId">Package ID</param>
        public static void DeletePackage(string packageId)
        {
            Provider.DeletePackage(packageId);
        }

        #endregion

        #region Custom fields

        /// <summary>
        /// Saves custom field
        /// </summary>
        /// <param name="field">Object custom field</param>
        public static void SaveCustomField(BlogEngine.Core.Data.Models.CustomField field)
        {
            Provider.SaveCustomField(field);
        }

        /// <summary>
        /// Fills list of custom fields for a blog
        /// </summary>
        /// <param name="blog">Current blog</param>
        /// <returns>List of custom fields</returns>
        public static List<BlogEngine.Core.Data.Models.CustomField> FillCustomFields()
        {
            return Provider.FillCustomFields();
        }

        /// <summary>
        /// Deletes custom field
        /// </summary>
        /// <param name="field">Object field</param>
        public static void DeleteCustomField(BlogEngine.Core.Data.Models.CustomField field)
        {
            Provider.DeleteCustomField(field);
        }

        /// <summary>
        /// Clear custom fields for a type (post, theme etc)
        /// </summary>
        /// <param name="blogId">Blog id</param>
        /// <param name="customType">Custom type</param>
        /// <param name="objectType">Custom object</param>
        public static void ClearCustomFields(string blogId, string customType, string objectType)
        {
            Provider.ClearCustomFields(blogId, customType, objectType);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Load the providers from the web.config.
        /// </summary>
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (TheLock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <blogProvider> section
                        var section =
                            (BlogProviderSection)WebConfigurationManager.GetSection("BlogEngine/blogProvider");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new BlogProviderCollection();
                        ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(BlogProvider));
                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                        {
                            throw new ProviderException("Unable to load default BlogProvider");
                        }
                    }
                }
            }
            if (_fileStorageProvider == null)
            {
                lock (TheLock)
                {
                    if (_fileStorageProvider == null)
                    {
                        var section = (BlogFileSystemProviderSection)WebConfigurationManager.GetSection("BlogEngine/blogFileSystemProvider");
                        _fileProviders = new BlogFileSystemProviderCollection();
                        ProvidersHelper.InstantiateProviders(section.Providers, _fileProviders, typeof(BlogFileSystemProvider));
                        _fileStorageProvider = _fileProviders[section.DefaultProvider];
                        if (_fileStorageProvider == null)
                        {
                            throw new ProviderException("unable to load default file system Blog Provider");
                        }
                    }
                }
            }
        }

        #endregion
    }
}