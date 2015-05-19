namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;

    using BlogEngine.Core.Providers;

    /// <summary>
    /// BlogRolls are links to outside blogs.
    /// </summary>
    [Serializable]
    public class BlogRollItem : BusinessBase<BlogRollItem, Guid>, IComparable<BlogRollItem>
    {
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The blog rolls.
        /// </summary>
        private static Dictionary<Guid, List<BlogRollItem>> blogRolls;

        /// <summary>
        /// The blog url.
        /// </summary>
        private Uri blogUrl;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The feed url.
        /// </summary>
        private Uri feedUrl;

        /// <summary>
        /// The sort index.
        /// </summary>
        private int sortIndex;

        /// <summary>
        /// The title.
        /// </summary>
        private string title;

        /// <summary>
        /// The xfn string.
        /// </summary>
        private string xfn;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "BlogRollItem" /> class.
        /// </summary>
        public BlogRollItem()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogRollItem"/> class.
        /// </summary>
        /// <param name="title">
        /// The title of the BlogRollItem.
        /// </param>
        /// <param name="description">
        /// The description of the BlogRollItem.
        /// </param>
        /// <param name="blogUrl">
        /// The <see cref="Uri"/> of the BlogRollItem.
        /// </param>
        public BlogRollItem(string title, string description, Uri blogUrl)
        {
            this.Id = Guid.NewGuid();
            this.Title = title;
            this.Description = description;
            this.BlogUrl = blogUrl;
        }

        static BlogRollItem()
        {
            Blog.Saved += (s, e) =>
            {
                if (e.Action == SaveAction.Delete)
                {
                    Blog blog = s as Blog;
                    if (blog != null)
                    {
                        // remove deleted blog from static 'blogRolls'

                        if (blogRolls != null && blogRolls.ContainsKey(blog.Id))
                            blogRolls.Remove(blog.Id);
                    }
                }
            };
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets all of the BlogRollItems from the data store.
        /// </summary>
        public static List<BlogRollItem> BlogRolls
        {
            get
            {
                Blog blog = Blog.CurrentInstance;

                if (blogRolls == null || !blogRolls.ContainsKey(blog.Id))
                {
                    lock (SyncRoot)
                    {
                        if (blogRolls == null || !blogRolls.ContainsKey(blog.Id))
                        {
                            if (blogRolls == null)
                                blogRolls = new Dictionary<Guid, List<BlogRollItem>>();

                            blogRolls[blog.Id] = BlogService.FillBlogRolls();

                            if(blogRolls[blog.Id] != null)
                                blogRolls[blog.Id].Sort();
                        }
                    }
                }

                return blogRolls[blog.Id];
            }
        }

        /// <summary>
        ///     Gets or sets the BlogUrl of the object.
        /// </summary>
        public Uri BlogUrl
        {
            get
            {
                return this.blogUrl;
            }

            set
            {
                base.SetValue("BlogUrl", value, ref this.blogUrl);
            }
        }

        /// <summary>
        ///     Gets or sets the Description of the object.
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
        ///     Gets or sets the FeedUrl of the object.
        /// </summary>
        public Uri FeedUrl
        {
            get
            {
                return this.feedUrl;
            }

            set
            {
                base.SetValue("FeedUrl", value, ref this.feedUrl);
            }
        }

        /// <summary>
        ///     Gets or sets the SortIndex of the object.
        /// </summary>
        public int SortIndex
        {
            get
            {
                return this.sortIndex;
            }

            set
            {
                base.SetValue("SortIndex", value, ref this.sortIndex);
            }
        }

        /// <summary>
        ///     Gets or sets the Title of the object.
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
        ///     Gets or sets the Xfn of the object.
        /// </summary>
        public string Xfn
        {
            get
            {
                return this.xfn;
            }

            set
            {
                base.SetValue("Xfn", value, ref this.xfn);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the BlogRollItem from the data store.
        /// </summary>
        /// <param name="id">The blogroll item id.</param>
        /// <returns>The blogroll item.</returns>
        public static BlogRollItem GetBlogRollItem(Guid id)
        {
            return BlogRolls.Find(br => br.Id == id);
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

        #region IComparable<BlogRollItem>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
        ///     The return value has the following meanings: Value Meaning Less than zero This object is 
        ///     less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(BlogRollItem other)
        {
            return this.SortIndex.CompareTo(other.SortIndex);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Deletes the object from the data store.
        /// </summary>
        protected override void DataDelete()
        {
            OnSaving(this, SaveAction.Delete);
            if (this.Deleted)
            {
                BlogService.DeleteBlogRoll(this);
            }

            BlogRolls.Remove(this);
            OnSaved(this, SaveAction.Delete);

            this.Dispose();
        }

        /// <summary>
        /// Inserts a new object to the data store.
        /// </summary>
        protected override void DataInsert()
        {
            OnSaving(this, SaveAction.Insert);
            if (this.New)
            {
                BlogService.InsertBlogRoll(this);
            }

            OnSaved(this, SaveAction.Insert);
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
        protected override BlogRollItem DataSelect(Guid id)
        {
            return BlogService.SelectBlogRoll(id);
        }

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected override void DataUpdate()
        {
            OnSaving(this, SaveAction.Update);
            if (this.IsChanged)
            {
                BlogService.UpdateBlogRoll(this);
            }

            OnSaved(this, SaveAction.Update);
        }

        /// <summary>
        /// Reinforces the business rules by adding additional rules to the
        ///     broken rules collection.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule("Title", "Title must be set", string.IsNullOrEmpty(this.Title));
            this.AddRule("BlogUrl", "BlogUrl must be set", this.BlogUrl == null);
        }

        #endregion
    }
}