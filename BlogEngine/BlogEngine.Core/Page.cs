namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BlogEngine.Core.Providers;
    using Data.Models;
    /// <summary>
    /// A page is much like a post, but is not part of the
    ///     blog chronology and is more static in nature.
    ///     <remarks>
    /// Pages can be used for "About" pages or other static
    ///         information.
    ///     </remarks>
    /// </summary>
    public sealed class Page : BusinessBase<Page, Guid>, IPublishable
    {
        #region Constants and Fields

        /// <summary>
        /// The _ sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The pages that not deleted.
        /// </summary>
        private static Dictionary<Guid, List<Page>> pages;

        /// <summary>
        /// The deleted pages.
        /// </summary>
        private static Dictionary<Guid, List<Page>> deletedpages;

        /// <summary>
        /// The _ content.
        /// </summary>
        private string content;

        /// <summary>
        /// The _ description.
        /// </summary>
        private string description;

        /// <summary>
        /// The _ keywords.
        /// </summary>
        private string keywords;

        /// <summary>
        /// The _ parent.
        /// </summary>
        private Guid parent;

        /// <summary>
        /// The _ show in list.
        /// </summary>
        private bool showInList;

        /// <summary>
        /// The _ slug.
        /// </summary>
        private string slug;

        /// <summary>
        /// The _ title.
        /// </summary>
        private string title;

        /// <summary>
        /// The front page.
        /// </summary>
        private bool frontPage;

        /// <summary>
        /// The published.
        /// </summary>
        private bool isPublished;

        /// <summary>
        /// The deleted.
        /// </summary>
        private bool isDeleted;

        /// <summary>
        /// The sort order
        /// </summary>
        private int sortOrder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class. 
        ///     The contructor sets default values.
        /// </summary>
        public Page()
        {
            this.Id = Guid.NewGuid();
            this.DateCreated = BlogSettings.Instance.FromUtc();
        }

        static Page()
        {
            Blog.Saved += (s, e) =>
            {
                if (e.Action == SaveAction.Delete)
                {
                    Blog blog = s as Blog;
                    if (blog != null)
                    {
                        // remove deleted blog from static 'pages' and 'deletedpages'

                        if (pages != null && pages.ContainsKey(blog.Id))
                            pages.Remove(blog.Id);

                        if (deletedpages != null && deletedpages.ContainsKey(blog.Id))
                            deletedpages.Remove(blog.Id);
                    }
                }
            };
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when the page is being served to the output stream.
        /// </summary>
        public static event EventHandler<ServingEventArgs> Serving;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets an unsorted list of all pages excluding deleted.
        /// </summary>
        public static List<Page> Pages
        {
            get
            {
                Blog blog = Blog.CurrentInstance;

                if (pages == null || !pages.ContainsKey(blog.Id))
                {
                    lock (SyncRoot)
                    {
                        if (pages == null || !pages.ContainsKey(blog.Id))
                        {
                            if (pages == null)
                                pages = new Dictionary<Guid, List<Page>>();

                            pages[blog.Id] = BlogService.FillPages().Where(p => p.IsDeleted == false).ToList();
                            pages[blog.Id].Sort((p1, p2) => String.Compare(p1.Title, p2.Title));
                        }
                    }
                }

                return pages[blog.Id];
            }
        }

        /// <summary>
        ///     Gets an unsorted list of deleted pages.
        /// </summary>
        public static List<Page> DeletedPages
        {
            get
            {
                Blog blog = Blog.CurrentInstance;

                if (deletedpages == null || !deletedpages.ContainsKey(blog.Id))
                {
                    lock (SyncRoot)
                    {
                        if (deletedpages == null || !deletedpages.ContainsKey(blog.Id))
                        {
                            if (deletedpages == null)
                                deletedpages = new Dictionary<Guid, List<Page>>();

                            deletedpages[blog.Id] = BlogService.FillPages().Where(p => p.IsDeleted == true).ToList();
                            deletedpages[blog.Id].Sort((p1, p2) => String.Compare(p1.Title, p2.Title));
                        }
                    }
                }

                return deletedpages[blog.Id];
            }
        }

        /// <summary>
        ///     Gets the absolute link to the page.
        /// </summary>
        public Uri AbsoluteLink
        {
            get
            {
                return Utils.ConvertToAbsolute(this.RelativeLink);
            }
        }

        /// <summary>
        ///     Gets or sets the Description or the object.
        /// </summary>
        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                base.SetValue("Content", value, ref this.content);
            }
        }

        /// <summary>
        ///     Gets or sets the Description or the object.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                base.SetValue("Description", value, ref this.description);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not this page should be displayed on the front page.
        /// </summary>
        public bool IsFrontPage
        {
            get
            {
                return this.frontPage;
            }

            set
            {
                base.SetValue("IsFrontPage", value, ref this.frontPage);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the has child pages.
        /// </summary>
        /// Does this page have child pages
        public bool HasChildPages
        {
            get
            {
                return Pages.Any(p => p.Parent == this.Id);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the has parent page.
        /// </summary>
        /// Does this page have a parent page?
        public bool HasParentPage
        {
            get
            {
                return this.Parent != Guid.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the Keywords or the object.
        /// </summary>
        public string Keywords
        {
            get
            {
                return this.keywords;
            }

            set
            {
                base.SetValue("Keywords", value, ref this.keywords);
            }
        }

        /// <summary>
        ///     Gets or sets the parent of the Page. It is used to construct the 
        ///     hierachy of the pages.
        /// </summary>
        public Guid Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                base.SetValue("Parent", value, ref this.parent);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not this page should be published.
        /// </summary>
        public bool IsPublished
        {
            get
            {
                return this.isPublished;
            }

            set
            {
                base.SetValue("IsPublished", value, ref this.isPublished);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not this page is deleted.
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
        ///     Gets a relative-to-the-site-root path to the page.
        ///     Only for in-site use.
        /// </summary>
        public string RelativeLink
        {
            get
            {
                var theslug = Utils.RemoveIllegalCharacters(this.Slug) + BlogConfig.FileExtension;
                return $"{Utils.RelativeWebRoot}page/{theslug}";
            }
        }

        /// <summary>
        ///     Returns a relative link if possible if the hostname of this blog instance matches the
        ///     hostname of the site aggregation blog.  If the hostname is different, then the
        ///     absolute link is returned.
        /// </summary>
        public string RelativeOrAbsoluteLink
        {
            get
            {
                if (this.Blog.DoesHostnameDifferFromSiteAggregationBlog)
                    return this.AbsoluteLink.ToString();
                else
                    return this.RelativeLink;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not this page should be in the sitemap list.
        /// </summary>
        public bool ShowInList
        {
            get
            {
                return this.showInList;
            }

            set
            {
                base.SetValue("ShowInList", value, ref this.showInList);
            }
        }

        /// <summary>
        ///     Gets or sets the Slug of the Page.
        ///     A Slug is the relative URL used by the pages.
        /// </summary>
        public string Slug
        {
            get
            {
                if (string.IsNullOrEmpty(this.slug))
                {
                    return Utils.RemoveIllegalCharacters(this.Title);
                }

                return this.slug;
            }

            set
            {
                base.SetValue("Slug", value, ref this.slug);
            }
        }

        /// <summary>
        ///     Gets or sets the Title or the object.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                base.SetValue("Title", value, ref this.title);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this page should be shown
        /// </summary>
        /// <value></value>
        public bool IsVisible
        {
            get
            {
                if (this.isDeleted)
                    return false;
                else if (this.IsPublished)
                    return true;
                else if (Security.IsAuthorizedTo(Rights.ViewUnpublishedPages))
                    return true;

                return false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this page is visible to visitors not logged into the blog.
        /// </summary>
        /// <value></value>
        public bool IsVisibleToPublic
        {
            get
            {
                return this.IsPublished && !this.IsDeleted;
            }
        }

        /// <summary>
        /// Gets Author.
        /// </summary>
        string IPublishable.Author
        {
            get
            {
                return BlogSettings.Instance.AuthorName;
            }
        }

        /// <summary>
        /// Gets whether or not the current user owns this page.
        /// </summary>
        /// <returns></returns>
        public override bool CurrentUserOwns
        {
            get
            {
                // Because we are not storing an author name for each page,
                // any user that have edit page access is an owner
                return Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOwnPages);
            }
        }

        /// <summary>
        /// Gets whether the current user can delete this page.
        /// </summary>
        /// <returns></returns>
        public override bool CanUserDelete
        {
            get
            {
                return Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOwnPages);
            }
        }

        /// <summary>
        /// Gets whether the current user can edit this page.
        /// </summary>
        /// <returns></returns>
        public override bool CanUserEdit
        {
            get
            {
                return Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOwnPages);
            }
        }

        /// <summary>
        /// Gets Categories.
        /// </summary>
        /// <remarks>
        /// 
        /// 10/21/10
        /// This was returning null. I'm not sure what the purpose of that is. An IEnumerable should return
        /// an empty collection rather than null to indicate that there's nothing there.
        /// 
        /// </remarks>
        StateList<Category> IPublishable.Categories
        {
            get
            {
                return this.categories;
            }
        }
        private StateList<Category> categories = new StateList<Category>();

        StateList<string> IPublishable.Tags
        {
            get
            {
                return tags;
            }
        }

        /// <summary>
        /// The sort order of the page
        /// </summary>
        public int SortOrder
        {
            get { return sortOrder; }
            set { SetValue("SortOrder", value, ref sortOrder); }
        }

        private StateList<string> tags = new StateList<string>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether the current user can publish this page.
        /// </summary>
        /// <returns></returns>
        public bool CanPublish()
        {
            return Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOwnPages);
        }

        /// <summary>
        /// Returns the front page if any is available.
        /// </summary>
        /// <returns>The front Page.</returns>
        public static Page GetFrontPage()
        {
            return Pages.Find(page => page.IsFrontPage);
        }

        /// <summary>
        /// Returns a page based on the specified id.
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <returns>The Page requested.</returns>
        public static Page GetPage(Guid id)
        {
            return Pages.FirstOrDefault(page => page.Id == id);
        }

        /// <summary>
        /// Called when [serving].
        /// </summary>
        /// <param name="page">The page being served.</param>
        /// <param name="arg">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
        public static void OnServing(Page page, ServingEventArgs arg)
        {
            if (Serving != null)
            {
                Serving(page, arg);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.Title;
        }

        #endregion

        #region Implemented Interfaces

        #region IPublishable

        /// <summary>
        /// Raises the Serving event
        /// </summary>
        /// <param name="eventArgs">
        /// The event Args.
        /// </param>
        public void OnServing(ServingEventArgs eventArgs)
        {
            if (Serving != null)
            {
                Serving(this, eventArgs);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Deletes the page from the current BlogProvider.
        /// </summary>
        protected override void DataDelete()
        {
            this.IsDeleted = true;
            this.DateModified = DateTime.Now;
            DataUpdate();
           
            Pages.Remove(this);

            if(!DeletedPages.Contains(this))
                DeletedPages.Add(this);
        }

        /// <summary>
        /// Deletes the Page from the current BlogProvider.
        /// </summary>
        public void Purge()
        {
            BlogService.DeletePage(this);
            DeletedPages.Remove(this);
        }

        /// <summary>
        /// Restores the deleted page.
        /// </summary>
        public void Restore()
        {
            this.IsDeleted = false;
            this.DateModified = DateTime.Now;
            DataUpdate();
            
            DeletedPages.Remove(this);
            Pages.Add(this);
        }

        /// <summary>
        /// Inserts a new page to current BlogProvider.
        /// </summary>
        protected override void DataInsert()
        {
            BlogService.InsertPage(this);

            if (this.New)
            {
                Pages.Add(this);
            }
        }

        /// <summary>
        /// Retrieves a page form the BlogProvider
        /// based on the specified id.
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <returns>The Page requested.</returns>
        protected override Page DataSelect(Guid id)
        {
            return BlogService.SelectPage(id);
        }

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected override void DataUpdate()
        {
            BlogService.UpdatePage(this);
        }

        /// <summary>
        /// Validates the properties on the Page.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule("Title", "Title must be set", string.IsNullOrEmpty(this.Title));
            this.AddRule("Content", "Content must be set", string.IsNullOrEmpty(this.Content));
        }

        #endregion

        #region Custom Fields

        /// <summary>
        /// Custom fields
        /// </summary>
        public Dictionary<String, CustomField> CustomFields
        {
            get
            {
                var pageFields = BlogService.Provider.FillCustomFields().Where(f =>
                    f.CustomType == "PAGE" &&
                    f.ObjectId == this.Id.ToString()).ToList();

                if (pageFields == null || pageFields.Count < 1)
                    return null;

                var fields = new Dictionary<String, CustomField>();

                foreach (var item in pageFields)
                {
                    fields.Add(item.Key, item);
                }
                return fields;
            }
        }

        #endregion
    }
}