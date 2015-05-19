using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using BlogEngine.Core.Providers;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using BlogEngine.Core.Providers.CacheProvider;

namespace BlogEngine.Core
{
    /// <summary>
    /// Represents a blog instance.
    /// </summary>
    public class Blog : BusinessBase<Blog, Guid>, IComparable<Blog>
    {
        /// <summary>
        ///     Whether the blog is deleted.
        /// </summary>
        private bool isDeleted;

        /// <summary>
        ///     Blog name
        /// </summary>
        private string blogName;

        /// <summary>
        ///     Whether the blog is the primary blog instance
        /// </summary>
        private bool isPrimary;

        /// <summary>
        ///     Whether the blog is active
        /// </summary>
        private bool isActive;

        /// <summary>
        ///     The hostname of the blog instance.
        /// </summary>
        private string hostname;

        /// <summary>
        ///     Whether any text before the hostname is accepted.
        /// </summary>
        private bool isAnyTextBeforeHostnameAccepted;

        /// <summary>
        ///     The storage container name of the blog's data
        /// </summary>
        private string storageContainerName;

        /// <summary>
        ///     The virtual path to the blog instance
        /// </summary>
        private string virtualPath;

        /// <summary>
        ///     The relative web root.
        /// </summary>
        private string relativeWebRoot;

        /// <summary>
        ///     Whether this blog instance is an aggregration of data across all blog instances.
        /// </summary>
        private bool isSiteAggregation;

        /// <summary>
        ///     Flag used when blog is deleted on whether the storage container will be deleted too.
        /// </summary>
        private bool deleteStorageContainer;

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The blogs.
        /// </summary>
        private static List<Blog> blogs;

        /// <summary>
        ///     Gets or sets a value indicating whether or not the blog is deleted.
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return this.isDeleted;
            }

            set
            {
                base.SetValue("IsDeleted", value, ref this.isDeleted);
            }
        }

        /// <summary>
        ///     Gets whether the blog is the primary blog instance.
        /// </summary>
        public bool IsPrimary
        {
            get
            {
                return this.isPrimary;
            }
            internal set
            {
                // SetAsPrimaryInstance() exists as a public method to make
                // a blog instance be the primary one -- which makes sure other
                // instances are no longer primary.

                base.SetValue("IsPrimary", value, ref this.isPrimary);
            }
        }

        /// <summary>
        ///     Gets whether this blog instance is an aggregration of data across all blog instances.
        /// </summary>
        public bool IsSiteAggregation
        {
            get
            {
                return this.isSiteAggregation;
            }
            set
            {
                base.SetValue("IsSiteAggregation", value, ref this.isSiteAggregation);
            }
        }

        /// <summary>
        ///     Gets whether the blog instance is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                base.SetValue("IsActive", value, ref this.isActive);
            }
        }

        /// <summary>
        ///     Gets the optional hostname of the blog instance.
        /// </summary>
        public string Hostname
        {
            get
            {
                return this.hostname;
            }
            set
            {
                base.SetValue("Hostname", value, ref this.hostname);
            }
        }

        /// <summary>
        ///     Gets whether any text before the hostname is accepted.
        /// </summary>
        public bool IsAnyTextBeforeHostnameAccepted
        {
            get
            {
                return this.isAnyTextBeforeHostnameAccepted;
            }
            set
            {
                base.SetValue("IsAnyTextBeforeHostnameAccepted", value, ref this.isAnyTextBeforeHostnameAccepted);
            }
        }

        /// <summary>
        ///     Gets or sets the blog name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.blogName;
            }

            set
            {
                base.SetValue("Name", value, ref this.blogName);
            }
        }

        /// <summary>
        ///     Gets or sets the storage container name.
        /// </summary>
        public string StorageContainerName
        {
            get
            {
                return this.storageContainerName;
            }

            set
            {
                base.SetValue("StorageContainerName", value, ref this.storageContainerName);
            }
        }

        /// <summary>
        ///     Gets or sets the virtual path to the blog instance.
        /// </summary>
        public string VirtualPath
        {
            get
            {
                return this.virtualPath;
            }

            set
            {
                // RelativeWebRoot is based on VirtualPath.  Clear relativeWebRoot
                // so RelativeWebRoot is re-generated.
                this.relativeWebRoot = null;

                base.SetValue("VirtualPath", value, ref this.virtualPath);
            }
        }

        /// <summary>
        ///     Gets or sets the unique Identification of the blog instance.
        /// </summary>
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                base.BlogId = value;
            }
        }

        /// <summary>
        ///     Flag used when blog is deleted on whether the storage container will be deleted too.
        ///     This property is not peristed.
        /// </summary>
        public bool DeleteStorageContainer
        {
            get
            {
                return this.deleteStorageContainer;
            }

            set
            {
                base.SetValue("DeleteStorageContainer", value, ref this.deleteStorageContainer);
            }
        }

        /// <summary>
        /// Returns true if the blog resides in a subfolder of the application root directory; 
        /// false otherwise.
        /// </summary>
        public bool IsSubfolderOfApplicationWebRoot
        {
            get
            {
                return this.RelativeWebRoot.Length > Utils.ApplicationRelativeWebRoot.Length;
            }
        }

        /// <summary>
        /// Attempts to delete the current Blog.
        /// </summary>
        public override void Delete()
        {
            if (this.IsPrimary)
            {
                throw new Exception("The primary blog cannot be deleted.");
            }

            base.Delete();
        }

        /// <summary>
        /// Deletes the Blog from the current BlogProvider.
        /// </summary>
        protected override void DataDelete()
        {
            OnSaving(this, SaveAction.Delete);

            if (this.DeleteStorageContainer)
            {
                BlogService.DeleteBlogStorageContainer(this);
            }

            if (this.Deleted)
            {
                BlogService.DeleteBlog(this);
            }

            Blogs.Remove(this);
            SortBlogs();
            OnSaved(this, SaveAction.Delete);

            this.Dispose();
        }

        /// <summary>
        /// Inserts a new blog to the current BlogProvider.
        /// </summary>
        protected override void DataInsert()
        {
            OnSaving(this, SaveAction.Insert);
            if (this.New)
            {
                BlogService.InsertBlog(this);
            }

            Blogs.Add(this);
            SortBlogs();
            OnSaved(this, SaveAction.Insert);
        }

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected override void DataUpdate()
        {
            OnSaving(this, SaveAction.Update);
            if (this.IsChanged)
            {
                BlogService.UpdateBlog(this);
                SortBlogs();
            }

            OnSaved(this, SaveAction.Update);
        }

        /// <summary>
        /// Retrieves the object from the data store and populates it.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the object.
        /// </param>
        /// <returns>
        /// The object that was selected from the data store.
        /// </returns>
        protected override Blog DataSelect(Guid id)
        {
            return BlogService.SelectBlog(id);
        }

        /// <summary>
        /// Reinforces the business rules by adding additional rules to the
        ///     broken rules collection.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule("Name", "Name must be set", string.IsNullOrEmpty(this.Name));
        }

        /// <summary>
        /// Gets whether the current user can delete this object.
        /// </summary>
        public override bool CanUserDelete
        {
            get
            {
                return Security.IsAdministrator && !this.IsPrimary;
            }
        }

        /// <summary>
        /// Sets the current Blog instance as the primary.
        /// </summary>
        public void SetAsPrimaryInstance()
        {   
            for (int i = 0; i < Blogs.Count; i++)
            {
                // Ensure other blogs are not marked as primary.
                if (Blogs[i].Id != this.Id && Blogs[i].IsPrimary)
                {
                    Blogs[i].IsPrimary = false;
                    Blogs[i].Save();
                }
                else if (Blogs[i].Id == this.Id)
                {
                    Blogs[i].IsPrimary = true;
                    Blogs[i].Save();
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Blog" /> class. 
        ///     The default contstructor assign default values.
        /// </summary>
        public Blog()
        {
            this.Id = Guid.NewGuid();
            this.BlogId = this.Id;
            this.DateCreated = DateTime.Now;
            this.DateModified = DateTime.Now;
        }

        /// <summary>
        ///     Gets all blogs.
        /// </summary>
        public static List<Blog> Blogs
        {
            get
            {
                if (blogs == null)
                {
                    lock (SyncRoot)
                    {
                        if (blogs == null)
                        {
                            blogs = BlogService.FillBlogs().ToList();

                            if (blogs.Count == 0)
                            { 
                                // create the primary instance

                                Blog blog = new Blog();
                                blog.Name = "Primary";
                                blog.hostname = string.Empty;
                                blog.VirtualPath = BlogConfig.VirtualPath;
                                blog.StorageContainerName = string.Empty;
                                blog.IsPrimary = true;
                                blog.IsSiteAggregation = false;
                                blog.Save();
                            }

                            SortBlogs();
                        }
                    }
                }

                return blogs;
            }
        }

        private static void SortBlogs()
        {
            blogs.Sort();
        }

        /// <summary>
        /// Marked as ThreadStatic so each thread has its own value.
        /// Need to be careful with this since when using ThreadPool.QueueUserWorkItem,
        /// after a thread is used, it is returned to the thread pool and
        /// any ThreadStatic values (such as this field) are not cleared, they will persist.
        /// 
        /// This value is reset in WwwSubdomainModule.BeginRequest.
        /// 
        /// </summary>
        [ThreadStatic]
        private static Guid _InstanceIdOverride;

        /// <summary>
        /// This is a thread-specific Blog Instance ID to override.
        /// If the current blog instance needs to be overridden,
        /// this property can be used.  A typical use for this is when
        /// using BG/async threads where the current blog instance
        /// cannot be determined since HttpContext will be null.
        /// </summary>
        public static Guid InstanceIdOverride
        {
            get { return _InstanceIdOverride; }
            set { _InstanceIdOverride = value; }
        }

        /// <summary>
        /// The current blog instance.
        /// </summary>
        public static Blog CurrentInstance
        {
            get
            {
                if (_InstanceIdOverride != Guid.Empty)
                {
                    Blog overrideBlog = Blogs.FirstOrDefault(b => b.Id == _InstanceIdOverride);
                    if (overrideBlog != null)
                    {
                        return overrideBlog;
                    }
                }

                const string CONTEXT_ITEM_KEY = "current-blog-instance";
                HttpContext context = HttpContext.Current;

                Blog blog = context.Items[CONTEXT_ITEM_KEY] as Blog;
                if (blog != null) { return blog; }

                List<Blog> blogs = Blogs;
                if (blogs.Count == 0) { return null; }

                if (blogs.Count == 1)
                {
                    blog = blogs[0];
                }
                else
                {
                    // Determine which blog.
                    //

                    // Web service and Page method calls to the server need to be made to the
                    // root level, and cannot be virtual URLs that get rewritten to the correct
                    // physical location.  When attempting to rewrite these URLs, the web
                    // service/page method will throw a "405 Method Not Allowed" error.
                    // For us to determine which blog these AJAX calls are on behalf of,
                    // a request header on the AJAX calls will be appended to tell us which
                    // blog instance they are for.
                    //
                    // The built-in ASP.NET Callback system works correctly even when
                    // the URL is rewritten.  For these, CurrentInstance will be determined
                    // and stored in HttpContext.Items before the rewrite is done -- so even
                    // after the rewrite, CurrentInstance will reference the correct blog
                    // instance.
                    // 

                    string blogIdHeader = context.Request.Headers["x-blog-instance"];
                    if (!string.IsNullOrWhiteSpace(blogIdHeader) && blogIdHeader.Length == 36)
                    {
                        blog = GetBlog(new Guid(blogIdHeader));
                        if (blog != null && !blog.IsActive)
                            blog = null;
                    }

                    if (blog == null)
                    {

                        // Note, this.Blogs is sorted via SortBlogs() so the blogs with longer
                        // RelativeWebRoots come first.  This is important when matching so the
                        // more specific matches are done first.

                        // for the purposes here, adding a trailing slash to RawUrl, even if it's not
                        // a correct URL.  if a blog has a relative root of /blog1, RelativeWebRoot
                        // will be /blog1/ (containing the trailing slash).  for equal comparisons,
                        // make sure rawUrl also has a trailing slash.

                        string rawUrl = VirtualPathUtility.AppendTrailingSlash(context.Request.RawUrl);
                        string hostname = context.Request.Url.Host;

                        for (int i = 0; i < blogs.Count; i++)
                        {
                            Blog checkBlog = blogs[i];

                            if (checkBlog.isActive)
                            {
                                // first check the hostname, if a hostname is specified

                                if (!string.IsNullOrWhiteSpace(checkBlog.hostname))
                                {
                                    bool isMatch = false;

                                    if (checkBlog.IsAnyTextBeforeHostnameAccepted)
                                        isMatch = hostname.EndsWith(checkBlog.hostname, StringComparison.OrdinalIgnoreCase);
                                    else
                                        isMatch = hostname.Equals(checkBlog.hostname, StringComparison.OrdinalIgnoreCase);

                                    // if isMatch, we still need to check the conditions below, to allow
                                    // multiple path variations for a particular hostname.
                                    if (!isMatch)
                                    {
                                        continue;
                                    }
                                }

                                // second check the path.

                                if (rawUrl.StartsWith(checkBlog.RelativeWebRoot, StringComparison.OrdinalIgnoreCase))
                                {
                                    blog = checkBlog;
                                    break;
                                }
                            }
                        }

                        // if all blogs are inactive, or there are no matches for some reason,
                        // select the primary blog.
                        if (blog == null)
                        {
                            blog = blogs.FirstOrDefault(b => b.IsPrimary);
                        }
                    }
                }

                context.Items[CONTEXT_ITEM_KEY] = blog;

                return blog;
            }
        }

        /// <summary>
        /// Returns a blog based on the specified id.
        /// </summary>
        /// <param name="id">
        /// The blog id.
        /// </param>
        /// <returns>
        /// The selected blog.
        /// </returns>
        public static Blog GetBlog(Guid id)
        {
            return Blogs.Find(b => b.Id == id);
        }

        /// <summary>
        ///     Returns the site aggregation blog instance, if one exists.
        /// </summary>
        /// <returns>
        ///     The site aggregation blog instance, if one exists.
        /// </returns>
        public static Blog SiteAggregationBlog
        {
            get
            {
                return Blogs.Find(b => b.IsSiteAggregation);
            }
        }

        /// <summary>
        ///     Gets whether the hostname differs from the Site Aggreation blog.
        /// </summary>
        public bool DoesHostnameDifferFromSiteAggregationBlog
        {
            get
            {
                if (this.IsSiteAggregation)
                    return false;

                Blog siteAggregationBlog = SiteAggregationBlog;
                if (siteAggregationBlog == null)
                    return false;

                return siteAggregationBlog.Hostname != this.Hostname;
            }
        }

        /// <summary>
        ///     Gets a mappable virtual path to the blog instance's storage folder.
        /// </summary>
        public string StorageLocation
        {
            get
            {
                // only the Primary blog instance should have an empty StorageContainerName
                if (string.IsNullOrWhiteSpace(this.StorageContainerName))
                {
                    return BlogConfig.StorageLocation;
                }

                return string.Format("{0}{1}/{2}/", BlogConfig.StorageLocation, BlogConfig.BlogInstancesFolderName, this.StorageContainerName);
            }
        }

        /// <summary>
        /// the root file storage directory for the blog. All File system management should start from the Root file store
        /// </summary>
        public FileSystem.Directory RootFileStore
        {
            get
            {
                return BlogService.GetDirectory(string.Concat(this.StorageLocation, Utils.FilesFolder));
            }
        }

        /// <summary>
        ///     Gets the relative root of the blog instance.
        /// </summary>
        /// <value>A string that ends with a '/'.</value>
        public string RelativeWebRoot
        {
            get
            {
                return relativeWebRoot ??
                       (relativeWebRoot =
                        VirtualPathUtility.ToAbsolute(VirtualPathUtility.AppendTrailingSlash(this.VirtualPath ?? BlogConfig.VirtualPath)));
            }
        }

        /// <summary>
        ///     Gets the "authority" portion of the absolute web root.
        /// </summary>
        public string AbsoluteWebRootAuthority
        {
            get
            {
                return AbsoluteWebRoot.GetLeftPart(UriPartial.Authority);
            }
        }

        /// <summary>
        ///     Gets the absolute root of the blog instance.
        /// </summary>
        public Uri AbsoluteWebRoot
        {
            get
            {
                string contextItemKey = string.Format("{0}-absolutewebroot", this.Id);

                var context = HttpContext.Current;
                if (context == null)
                {
                    throw new WebException("The current HttpContext is null");
                }

                Uri absoluteWebRoot = context.Items[contextItemKey] as Uri;
                if (absoluteWebRoot != null) { return absoluteWebRoot; }

                UriBuilder uri = new UriBuilder();
                if (!string.IsNullOrWhiteSpace(this.Hostname))
                    uri.Host = this.Hostname;
                else
                {
                    uri.Host = context.Request.Url.Host;
                    if (!context.Request.Url.IsDefaultPort)
                    {
                        uri.Port = context.Request.Url.Port;
                    }
                }

                uri.Path = RelativeWebRoot;
                uri.Scheme = context.Request.Url.Scheme; // added for https support

                absoluteWebRoot = uri.Uri;
                context.Items[contextItemKey] = absoluteWebRoot;

                return absoluteWebRoot;
            }
        }


        /// <summary>
        /// Creates a new blog.
        /// </summary>
        public static Blog CreateNewBlog(
            string copyFromExistingBlogId,
            string blogName,
            string hostname,
            bool isAnyTextBeforeHostnameAccepted,
            string storageContainerName,
            string virtualPath,
            bool isActive,
            bool isSiteAggregation,
            out string message)
        {
            message = null;

            if (!ValidateProperties(true, null, blogName, hostname, isAnyTextBeforeHostnameAccepted, storageContainerName, virtualPath, isSiteAggregation, out message))
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Validation for new blog failed.";
                }
                return null;
            }

            if (string.IsNullOrWhiteSpace(copyFromExistingBlogId) || copyFromExistingBlogId.Length != 36)
            {
                message = "An existing blog instance ID must be specified to create the new blog from.";
                return null;
            }

            Blog existingBlog = Blog.GetBlog(new Guid(copyFromExistingBlogId));

            if (existingBlog == null)
            {
                message = "The existing blog instance to create the new blog from could not be found.";
                return null;
            }

            Blog newBlog = new Blog()
            {
                Name = blogName,
                StorageContainerName = storageContainerName,
                Hostname = hostname,
                IsAnyTextBeforeHostnameAccepted = isAnyTextBeforeHostnameAccepted,
                VirtualPath = virtualPath,
                IsActive = isActive,
                IsSiteAggregation = isSiteAggregation
            };

            bool setupResult = false;
            try
            {
                setupResult = newBlog.SetupFromExistingBlog(existingBlog);
            }
            catch (Exception ex)
            {   
                Utils.Log("Blog.CreateNewBlog", ex);
                message = "Failed to create new blog. Error: " + ex.Message;
                return null;
            }

            if (!setupResult)
            {
                message = "Failed during process of setting up the blog from the existing blog instance.";
                return null;
            }

            // save the blog for the first time.
            newBlog.Save();

            return newBlog;
        }

        /// <summary>
        /// Validates the blog properties.
        /// </summary>
        public static bool ValidateProperties(
            bool isNew,
            Blog updateBlog,
            string blogName,
            string hostname,
            bool isAnyTextBeforeHostnameAccepted,
            string storageContainerName,
            string virtualPath,
            bool isSiteAggregation,
            out string message)
        {
            message = null;

            if (string.IsNullOrWhiteSpace(blogName))
            {
                message = "Blog Name is Required.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(hostname))
            {
                if (!Utils.IsHostnameValid(hostname) &&
                    !Utils.IsIpV4AddressValid(hostname) &&
                    !Utils.IsIpV6AddressValid(hostname))
                {
                    message = "Invalid Hostname.  Hostname must be an IP address or domain name.";
                    return false;
                }
            }

            Regex validChars = new Regex("^[a-z0-9-_]+$", RegexOptions.IgnoreCase);

            // if primary is being edited, allow an empty storage container name (bypass check).
            if (updateBlog == null || !updateBlog.IsPrimary)
            {
                if (string.IsNullOrWhiteSpace(storageContainerName))
                {
                    message = "Storage Container Name is Required.";
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(storageContainerName) && !validChars.IsMatch(storageContainerName))
            {
                message = "Storage Container Name contains invalid characters.";
                return false;
            }
            

            if (string.IsNullOrWhiteSpace(virtualPath))
            {
                message = "Virtual Path is Required.";
                return false;
            }
            else
            {
                if (!virtualPath.StartsWith("~/"))
                {
                    message = "Virtual Path must begin with ~/";
                    return false;
                }

                // note: a virtual path of ~/ without anything after it is allowed.  this would
                // typically be for the primary blog, but can also be for blogs that are using
                // subdomains, where each instance might be ~/

                string vPath = virtualPath.Substring(2);

                if (vPath.Length > 0)
                {
                    if (!validChars.IsMatch(vPath))
                    {
                        message = "The Virtual Path contains invalid characters after the ~/";
                        return false;
                    }
                }
            }

            if (isSiteAggregation)
            {
                Blog siteAggregationBlog = Blog.SiteAggregationBlog;
                if ((updateBlog == null && siteAggregationBlog != null) || (updateBlog != null && siteAggregationBlog != null && updateBlog.Id != siteAggregationBlog.Id))
                {
                    message = "Another blog is already marked as being the Site Aggregation blog.  Only one blog instance can be the Site Aggregration instance.";
                    return false;
                }
            }

            if (Blog.Blogs.FirstOrDefault(b => (updateBlog == null || updateBlog.Id != b.Id) && (b.VirtualPath ?? string.Empty).Equals((virtualPath ?? string.Empty), StringComparison.OrdinalIgnoreCase) && (b.Hostname ?? string.Empty).Equals(hostname ?? string.Empty, StringComparison.OrdinalIgnoreCase)) != null)
            {
                message = "Another blog has the same combination of Hostname and Virtual Path.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets up the blog instance using the files and settings from an existing blog instance.
        /// </summary>
        /// <param name="existing">The existing blog instance to use files and settings from.</param>
        /// <returns></returns>
        public bool SetupFromExistingBlog(Blog existing)
        {
            if (existing == null)
                throw new ArgumentException("existing");

            if (string.IsNullOrWhiteSpace(this.StorageContainerName))
                throw new ArgumentException("this.StorageContainerName");

            // allow the blog provider to setup the necessary blog files, etc.
            bool providerResult = BlogService.SetupBlogFromExistingBlog(existing, this);
            if (!providerResult)
                return false;

            //if (Membership.Provider.Name.Equals("DbMembershipProvider", StringComparison.OrdinalIgnoreCase))
            //{ 

            //}

            //if (Roles.Provider.Name.Equals("DbRoleProvider", StringComparison.OrdinalIgnoreCase))
            //{

            //}

            return true;
        }

        internal bool DeleteBlogFolder()
        {
            // This method is called by the blog providers when a blog's storage container
            // is being deleted.  Even the DbBlogProvider will call this method.
            // However, a different type of blog provider (e.g. Azure, etc) may not
            // need to call this method.

            try
            {
                string storagePath = HostingEnvironment.MapPath(this.StorageLocation);
                if (Directory.Exists(storagePath))
                {
                    Directory.Delete(storagePath, true);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Blog.DeleteBlogFolder", ex);
                return false;
            }

            return true;
        }

        internal bool CopyExistingBlogFolderToNewBlogFolder(Blog existingBlog)
        {
            // This method is called by the blog providers when a new blog is being setup.
            // Even the DbBlogProvider will call this method.  However, a different type of
            // blog provider (e.g. Azure, etc) may not need to call this method.

            if (string.IsNullOrWhiteSpace(this.StorageContainerName))
                throw new ArgumentException("this.StorageContainerName");

            string existingBlogStoragePath = null;
            try
            {
                // Ensure the existing blog storage path exists.

                existingBlogStoragePath = HostingEnvironment.MapPath(existingBlog.StorageLocation);
                if (!Directory.Exists(existingBlogStoragePath))
                {
                    throw new Exception(string.Format("Storage folder for existing blog instance to copy from does not exist.  Directory not found is: {0}", existingBlogStoragePath));
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Blog.CreateNewBlogFromExisting", ex);
                throw;  // re-throw error so error message bubbles up.
            }

            // Ensure "BlogInstancesFolderName" exists.
            string blogInstancesFolder = HostingEnvironment.MapPath(string.Format("{0}{1}", BlogConfig.StorageLocation, BlogConfig.BlogInstancesFolderName));
            if (!Utils.CreateDirectoryIfNotExists(blogInstancesFolder))
                return false;

            // If newBlogStoragePath already exists, throw an exception as this may be a mistake
            // and we don't want to overwrite any existing data.
            string newBlogStoragePath = HostingEnvironment.MapPath(this.StorageLocation);
            try
            {
                if (Directory.Exists(newBlogStoragePath))
                {
                    throw new Exception(string.Format("Blog destination folder already exists. {0}", newBlogStoragePath));
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Blog.CopyExistingBlogFolderToNewBlogFolder", ex);
                throw;  // re-throw error so error message bubbles up.
            }
            if (!Utils.CreateDirectoryIfNotExists(newBlogStoragePath))
                return false;

            // Copy the entire directory contents.
            DirectoryInfo source = new DirectoryInfo(existingBlogStoragePath);
            DirectoryInfo target = new DirectoryInfo(newBlogStoragePath);

            try
            {
                // if the primary blog directly in App_Data is the 'source', when all the directories/files are
                // being copied to the new location, we don't want to copy the entire BlogInstancesFolderName
                // (by default ~/App_Data/blogs) to the new location.  Everything except for that can be copied.
                // If the 'source' is a blog instance under ~/App_Data/blogs (e.g. ~/App_Data/blogs/template),
                // then this is not a concern.

                Utils.CopyDirectoryContents(source, target, new List<string>() { BlogConfig.BlogInstancesFolderName });
            }
            catch (Exception ex)
            {
                Utils.Log("Blog.CopyExistingBlogFolderToNewBlogFolder", ex);
                throw;  // re-throw error so error message bubbles up.
            }

            return true;
        }

        /// <summary>
        /// Compares the current Blog instance to another.
        /// </summary>
        /// <param name="other">The other Blog instance to compare to.</param>
        /// <returns>-1, 0, or 1. See ordering rules in the comments.</returns>
        public int CompareTo(Blog other)
        {   
            // order so:
            //   1. active blogs come first
            //   2. blogs with longer Hostnames come first (length DESC)
            //   3. blogs not allowing any text before hostname come first.
            //   4. blogs with longer RelativeWebRoots come first (length DESC)
            //   5. blog name ASC.

            // it is sorted this way so the more specific criteria are evaluated first,
            // and pre-sorted to make CurrentInstance work as fast as possible.

            if (this.IsActive && !other.IsActive)
                return -1;
            else if (!this.IsActive && other.IsActive)
                return 1;

            int otherHostnameLength = other.Hostname.Length;
            int thisHostnameLength = this.hostname.Length;

            if (otherHostnameLength != thisHostnameLength)
            {
                return otherHostnameLength.CompareTo(thisHostnameLength);
            }

            // at this point, otherHostnameLength == thisHostnameLength.
            if (otherHostnameLength > 0)  // if so, thisHostnameLength is also > 0.
            {
                if (this.IsAnyTextBeforeHostnameAccepted && !other.IsAnyTextBeforeHostnameAccepted)
                    return 1;
                else if (!this.IsAnyTextBeforeHostnameAccepted && other.IsAnyTextBeforeHostnameAccepted)
                    return -1;
            }

            int otherRelWebRootLength = other.RelativeWebRoot.Length;
            int thisRelWebRootLength = this.RelativeWebRoot.Length;

            if (otherRelWebRootLength != thisRelWebRootLength)
            {
                return otherRelWebRootLength.CompareTo(thisRelWebRootLength);
            }

            return this.Name.CompareTo(other.Name);
        }

        private CacheProvider _cache;
        /// <summary>
        /// blog instance cache
        /// </summary>
        public CacheProvider Cache
        {
            get { return _cache ?? (_cache = new CacheProvider(HttpContext.Current.Cache)); }
        }

    }
}
