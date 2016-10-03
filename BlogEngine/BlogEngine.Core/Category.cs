namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using BlogEngine.Core.Providers;

    /// <summary>
    /// Categories are a way to organize posts. 
    ///     A post can be in multiple categories.
    /// </summary>
    [Serializable]
    public class Category : BusinessBase<Category, Guid>, IComparable<Category>
    {
        #region Constants and Fields

        /// <summary>
        ///     The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        ///     The categories.
        /// </summary>
        private static Dictionary<Guid, List<Category>> categories = new Dictionary<Guid, List<Category>>();

        /// <summary>
        ///     The description.
        /// </summary>
        private string description;

        /// <summary>
        ///     The parent.
        /// </summary>
        private Guid? parent;

        /// <summary>
        ///     The title.
        /// </summary>
        private string title;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Category"/> class. 
        /// </summary>
        static Category()
        {
            Blog.Saved += (s, e) =>
            {
                if (e.Action == SaveAction.Delete)
                {
                    Blog blog = s as Blog;
                    if (blog != null)
                    {
                        // remove deleted blog from static 'categories'

                        if (categories != null && categories.ContainsKey(blog.Id))
                            categories.Remove(blog.Id);
                    }
                }
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Category" /> class.
        /// </summary>
        public Category()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        ///     The category.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public Category(string title, string description)
        {
            this.Id = Guid.NewGuid();
            this.title = title;
            this.description = description;
            this.Parent = null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a sorted list of all Categories.
        /// </summary>
        /// <value>The categories.</value>
        public static List<Category> Categories
        {
            get
            {
                Blog blog = Blog.CurrentInstance;
                List<Category> blogCategories;

                if (!categories.TryGetValue(blog.BlogId, out blogCategories))
                {
                    lock (SyncRoot)
                    {
                        if (!categories.TryGetValue(blog.BlogId, out blogCategories))
                        {
                            categories[blog.Id] = blogCategories = BlogService.FillCategories(blog);

                            if(blogCategories != null)
                                blogCategories.Sort();
                        }
                    }
                }

                return blogCategories;
            }
        }

        /// <summary>
        ///     Gets a sorted list of all Categories across all blog instances.
        /// </summary>
        /// <value>The categories.</value>
        public static List<Category> AllBlogCategories
        {
            get
            {
                List<Blog> blogs = Blog.Blogs.Where(b => b.IsActive).ToList();
                Guid originalBlogInstanceIdOverride = Blog.InstanceIdOverride;
                List<Category> categoriesAcrossAllBlogs = new List<Category>();

                // Categories are not loaded for a blog instance until that blog
                // instance is first accessed.  For blog instances where the
                // categories have not yet been loaded, using InstanceIdOverride to
                // temporarily switch the blog CurrentInstance blog so the Categories
                // for that blog instance can be loaded.
                //
                for (int i = 0; i < blogs.Count; i++)
                {
                    List<Category> blogCategories;
                    if (!categories.TryGetValue(blogs[i].Id, out blogCategories))
                    {
                        // temporarily override the Current BlogId to the
                        // blog Id we need categories to be loaded for.
                        Blog.InstanceIdOverride = blogs[i].Id;
                        blogCategories = Categories;
                        Blog.InstanceIdOverride = originalBlogInstanceIdOverride;
                    }

                    categoriesAcrossAllBlogs.AddRange(blogCategories);
                }

                return categoriesAcrossAllBlogs;
            }
        }

        /// <summary>
        ///     Gets a sorted list of all Categories, taking into account the
        ///     current blog instance's Site Aggregation status in determining if
        ///     categories from just the current instance or all instances should
        ///     be returned.
        /// </summary>
        /// <remarks>
        ///     This logic could be put into the normal 'Categories' property, however
        ///     there are times when a Site Aggregation blog instance may just need
        ///     its own categories.  So ApplicableCategories can be called when data
        ///     across all blog instances may be needed, and Categories can be called
        ///     when data for just the current blog instance is needed.
        /// </remarks>
        public static List<Category> ApplicableCategories
        {
            get
            {
                if (Blog.CurrentInstance.IsSiteAggregation)
                    return AllBlogCategories;
                else
                    return Categories;
            }
        }

        /// <summary>
        ///     Gets the absolute link to the page displaying all posts for this category.
        /// </summary>
        /// <value>The absolute link.</value>
        public Uri AbsoluteLink
        {
            get
            {
                return new Uri(this.Blog.AbsoluteWebRootAuthority + this.RelativeLink);
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
        ///     Gets or sets the Description of the object.
        /// </summary>
        /// <value>The description.</value>
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
        ///     Gets the absolute link to the feed for this category's posts.
        /// </summary>
        /// <value>The feed absolute link.</value>
        public Uri FeedAbsoluteLink
        {
            get
            {
                return Utils.ConvertToAbsolute(this.FeedRelativeLink);
            }
        }

        /// <summary>
        ///     Gets the relative link to the feed for this category's posts.
        /// </summary>
        /// <value>The feed relative link.</value>
        public string FeedRelativeLink
        {
            get
            {
                var root = Blog.CurrentInstance.IsSiteAggregation ? Utils.ApplicationRelativeWebRoot : Blog.RelativeWebRoot;
                return string.Format("{0}category/feed/{1}{2}", root,
                    Utils.RemoveIllegalCharacters(this.Title),
                    BlogConfig.FileExtension);
            }
        }

        /// <summary>
        ///     Gets or sets the Parent ID of the object
        /// </summary>
        /// <value>The parent.</value>
        public Guid? Parent
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
        ///     Gets all posts in this category.
        /// </summary>
        /// <value>The posts.</value>
        public List<Post> Posts
        {
            get
            {
                return Post.GetPostsByCategory(this.Id);
            }
        }

        /// <summary>
        ///     Gets the relative link to the page displaying all posts for this category.
        /// </summary>
        /// <value>The relative link.</value>
        public string RelativeLink
        {
            get
            {
                var root = Blog.CurrentInstance.IsSiteAggregation ? Utils.ApplicationRelativeWebRoot : Blog.RelativeWebRoot;
                return root + "category/" + Utils.RemoveIllegalCharacters(this.Title) +
                       BlogConfig.FileExtension;
            }
        }

        /// <summary>
        ///     Gets or sets the Title or the object.
        /// </summary>
        /// <value>The title.</value>
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a category based on the specified id.
        /// </summary>
        /// <param name="id">
        /// The category id.
        /// </param>
        /// <returns>
        /// The category.
        /// </returns>
        public static Category GetCategory(Guid id)
        {
            return GetCategory(id, false);
        }

        /// <summary>
        /// Returns a category based on the specified id.
        /// </summary>
        /// <param name="id">
        /// The category id.
        /// </param>
        /// <param name="acrossAllBlogInstances">
        /// Whether to search across the categories of all blog instances.
        /// </param>
        /// <returns>
        /// The category.
        /// </returns>
        public static Category GetCategory(Guid id, bool acrossAllBlogInstances)
        {
            return (acrossAllBlogInstances ? AllBlogCategories : Categories).FirstOrDefault(category => category.Id == id);
        }

        /// <summary>
        /// Gets the full title with Parent names included
        /// </summary>
        /// <returns>
        /// The complete title.
        /// </returns>
        public string CompleteTitle()
        {
            if (parent == null)
                return title;

            var cat = GetCategory((Guid)parent, Blog.CurrentInstance.IsSiteAggregation);

            return cat == null ? title : $"{cat.CompleteTitle()} - {title}";
        }


        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.CompleteTitle();
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable<Category>

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
        public int CompareTo(Category other)
        {
            return this.CompleteTitle().CompareTo(other.CompleteTitle());
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Deletes the object from the data store.
        /// </summary>
        protected override void DataDelete()
        {
            if (this.Deleted)
            {
                foreach (var c in
                    Categories.ToArray().Where(
                        c => !c.Id.Equals(this.Id) && c.Parent.HasValue && c.Parent.Value.Equals(this.Id)))
                {
                    c.Parent = null;
                    c.Save();
                }

                BlogService.DeleteCategory(this);
            }

            if (Categories.Contains(this))
            {
                Categories.Remove(this);
            }

            this.Dispose();
        }

        /// <summary>
        /// Inserts a new object to the data store.
        /// </summary>
        protected override void DataInsert()
        {
            if (this.New)
            {
                BlogService.InsertCategory(this);
            }
        }

        /// <summary>
        /// Retrieves the object from the data store and populates it.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the object.
        /// </param>
        /// <returns>
        /// True if the object exists and is being populated successfully
        /// </returns>
        protected override Category DataSelect(Guid id)
        {
            return BlogService.SelectCategory(id);
        }

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected override void DataUpdate()
        {
            if (this.IsChanged)
            {
                BlogService.UpdateCategory(this);
            }
        }

        /// <summary>
        /// Reinforces the business rules by adding additional rules to the
        ///     broken rules collection.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule("Title", "Title must be set", string.IsNullOrEmpty(this.Title));
        }

        #endregion
    }
}