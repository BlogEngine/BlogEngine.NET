using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Transactions;

using BlogEngine.Core.DataStore;
using BlogEngine.Core.Packaging;
using BlogEngine.Core.Notes;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Providers
{
    /// <summary>
    /// Generic Database BlogProvider
    /// </summary>
    public partial class DbBlogProvider : BlogProvider
    {
        #region Constants and Fields

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes a BlogRoll from the database
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void DeleteBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = BlogRollItem.BlogRolls;
            blogRolls.Remove(blogRollItem);
            blogRolls.Add(blogRollItem);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}BlogRollItems WHERE BlogId = {1}BlogId AND BlogRollId = {1}BlogRollId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogRollId"), blogRollItem.Id.ToString()));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the blog's storage container.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public override bool DeleteBlogStorageContainer(Blog blog)
        {
            // First delete the blog folder.  Even the DB provider uses
            // the folder.  This is rare and is usually by widgets/extensions.
            if (!blog.DeleteBlogFolder())
            {
                return false;
            }

            // Delete data from all tables except for be_Blogs.  The blog
            // data from be_Blogs will be deleted in DeleteBlog().

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    // Note, the order here is important, especially for the DBs where
                    // foreign key constraints are setup (SQL Server is one).  The data
                    // in the referencing tables needs to be deleted first.

                    var sqlQuery = string.Format("DELETE FROM {0}PostTag WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostNotify WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostComment WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Posts WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}RightRoles WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Profiles WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}UserRoles WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Roles WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Rights WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Users WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Pages WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}StopWords WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Settings WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Referrers WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PingService WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}DataStoreSettings WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}BlogRollItems WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Categories WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes a Blog from the database
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public override void DeleteBlog(Blog blog)
        {
            // Only deleting data from be_Blogs.  Data from the other tables
            // will be deleted in DeleteBlogStorageContainer().
            using (TransactionScope ts = new TransactionScope())
            {
                using (var conn = this.CreateConnection())
                {
                    if (conn.HasConnection)
                    {
                        var sqlQuery = string.Format("DELETE FROM {0}Blogs WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);
                        using (var cmd = conn.CreateTextCommand(sqlQuery))
                        {
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// Deletes a category from the database
        /// </summary>
        /// <param name="category">
        /// category to be removed
        /// </param>
        public override void DeleteCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Remove(category);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE BlogID = {1}blogid AND CategoryID = {1}catid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Categories WHERE BlogID = {1}blogid AND CategoryID = {1}catid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a page from the database
        /// </summary>
        /// <param name="page">
        /// page to be deleted
        /// </param>
        public override void DeletePage(Page page)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Pages WHERE BlogID = {1}blogid AND PageID = {1}id", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), page.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a post in the database
        /// </summary>
        /// <param name="post">
        /// post to delete
        /// </param>
        public override void DeletePost(Post post)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}PostTag WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostNotify WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostComment WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Posts WHERE BlogID = @blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        /// <summary>
        /// Gets all BlogRolls in database
        /// </summary>
        /// <returns>
        /// List of BlogRolls
        /// </returns>
        public override List<BlogRollItem> FillBlogRoll()
        {
            var blogRoll = new List<BlogRollItem>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT BlogRollId, Title, Description, BlogUrl, FeedUrl, Xfn, SortIndex FROM {0}BlogRollItems WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var br = new BlogRollItem
                                    {
                                        Id = rdr.GetGuid(0),
                                        Title = rdr.GetString(1),
                                        Description = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
                                        BlogUrl = rdr.IsDBNull(3) ? null : new Uri(rdr.GetString(3)),
                                        FeedUrl = rdr.IsDBNull(4) ? null : new Uri(rdr.GetString(4)),
                                        Xfn = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5),
                                        SortIndex = rdr.GetInt32(6)
                                    };

                                blogRoll.Add(br);
                                br.MarkOld();
                            }
                        }
                    }
                }
            }

            return blogRoll;
        }

        /// <summary>
        /// Gets all Blogs in database
        /// </summary>
        /// <returns>
        /// List of Blogs
        /// </returns>
        public override List<Blog> FillBlogs()
        {
            var blogs = new List<Blog>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT BlogId, BlogName, Hostname, IsAnyTextBeforeHostnameAccepted, StorageContainerName, VirtualPath, IsPrimary, IsActive, IsSiteAggregation FROM {0}Blogs ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var b = new Blog
                                {
                                    Id = rdr.GetGuid(0),
                                    Name = rdr.GetString(1),
                                    Hostname = rdr.GetString(2),
                                    IsAnyTextBeforeHostnameAccepted = rdr.GetBoolean(3),
                                    StorageContainerName = rdr.GetString(4),
                                    VirtualPath = rdr.GetString(5),
                                    IsPrimary = rdr.GetBoolean(6),
                                    IsActive = rdr.GetBoolean(7),
                                    IsSiteAggregation = rdr.GetBoolean(8)
                                };

                                blogs.Add(b);
                                b.MarkOld();
                            }
                        }
                    }
                }
            }

            return blogs;
        }

        /// <summary>
        /// Gets all categories in database
        /// </summary>
        /// <returns>
        /// List of categories
        /// </returns>
        public override List<Category> FillCategories(Blog blog)
        {
            var categories = new List<Category>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT CategoryID, CategoryName, description, ParentID FROM {0}Categories WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), blog.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var cat = new Category
                                    {
                                        Title = rdr.GetString(1),
                                        Description = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
                                        Parent = rdr.IsDBNull(3) ? (Guid?)null : new Guid(rdr.GetGuid(3).ToString()),
                                        Id = new Guid(rdr.GetGuid(0).ToString())
                                    };

                                categories.Add(cat);
                                cat.MarkOld();
                            }
                        }
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// Gets all pages in database
        /// </summary>
        /// <returns>
        /// List of pages
        /// </returns>
        public override List<Page> FillPages()
        {
            var pageIDs = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT PageID FROM {0}Pages WHERE BlogID = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                pageIDs.Add(rdr.GetGuid(0).ToString());
                            }
                        }
                    }
                }
            }

            return pageIDs.Select(id => Page.Load(new Guid(id))).ToList();
        }

        /// <summary>
        /// Gets all post from the database
        /// </summary>
        /// <returns>
        /// List of posts
        /// </returns>
        public override List<Post> FillPosts()
        {
            var postIDs = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT PostID FROM {0}Posts WHERE BlogID = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                postIDs.Add(rdr.GetGuid(0).ToString());
                            }
                        }
                    }
                }
            }

            var posts = postIDs.Select(id => Post.Load(new Guid(id))).ToList();

            posts.Sort();
            return posts;
        }

        /// <summary>
        /// Gets all Referrers from the database.
        /// </summary>
        /// <returns>
        /// List of Referrers.
        /// </returns>
        public override List<Referrer> FillReferrers()
        {
            this.DeleteOldReferrers();

            var referrers = new List<Referrer>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT ReferrerId, ReferralDay, ReferrerUrl, ReferralCount, Url, IsSpam FROM {0}Referrers WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var refer = new Referrer
                                    {
                                        Id = rdr.GetGuid(0),
                                        Day = rdr.GetDateTime(1),
                                        ReferrerUrl = new Uri(rdr.GetString(2)),
                                        Count = rdr.GetInt32(3),
                                        Url = rdr.IsDBNull(4) ? null : new Uri(rdr.GetString(4)),
                                        PossibleSpam = rdr.IsDBNull(5) ? false : rdr.GetBoolean(5)
                                    };

                                referrers.Add(refer);
                                refer.MarkOld();
                            }
                        }
                    }
                }
            }

            return referrers;
        }

        /// <summary>
        /// Returns a dictionary of right names and the roles associated with them.
        /// </summary>
        /// <returns></returns>
        public override IDictionary<string, IEnumerable<string>> FillRights()
        {
            var rightsWithRoles = new Dictionary<string, IEnumerable<string>>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT RightName FROM {0}Rights WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                rightsWithRoles.Add(rdr.GetString(0), new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                            }
                        }

                        // Get Right Roles.
                        cmd.CommandText = string.Format("SELECT RightName, Role FROM {0}RightRoles WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix);
                        // don't need to add "blogid" parameter again since the same cmd is being used.

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string rightName = rdr.GetString(0);
                                string roleName = rdr.GetString(1);

                                if (rightsWithRoles.ContainsKey(rightName))
                                {
                                    var roles = (HashSet<string>)rightsWithRoles[rightName];
                                    if (!roles.Contains(roleName))
                                    {
                                        roles.Add(roleName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return rightsWithRoles;
        }

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="name">
        /// Configuration name
        /// </param>
        /// <param name="config">
        /// Configuration settings
        /// </param>
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

            if (config["connectionStringName"] == null)
            {
                // default to BlogEngine
                config["connectionStringName"] = "BlogEngine";
            }

            this.connStringName = config["connectionStringName"];
            config.Remove("connectionStringName");

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

        /// <summary>
        /// Adds a new BlogRoll to the database.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void InsertBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = BlogRollItem.BlogRolls;
            blogRolls.Add(blogRollItem);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {

                    var sqlQuery = string.Format("INSERT INTO {0}BlogRollItems (BlogId, BlogRollId, Title, Description, BlogUrl, FeedUrl, Xfn, SortIndex) VALUES ({1}BlogId, {1}BlogRollId, {1}Title, {1}Description, {1}BlogUrl, {1}FeedUrl, {1}Xfn, {1}SortIndex)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddBlogRollParametersToCommand(blogRollItem, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new blog to the database.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        public override void InsertBlog(Blog blog)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Blogs (BlogId, BlogName, Hostname, IsAnyTextBeforeHostnameAccepted, StorageContainerName, VirtualPath, IsPrimary, IsActive, IsSiteAggregation) VALUES ({1}BlogId, {1}BlogName, {1}Hostname, {1}IsAnyTextBeforeHostnameAccepted, {1}StorageContainerName, {1}VirtualPath, {1}IsPrimary, {1}IsActive, {1}IsSiteAggregation)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddBlogParametersToCommand(blog, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new category to the database
        /// </summary>
        /// <param name="category">
        /// category to add
        /// </param>
        public override void InsertCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Add(category);
            categories.Sort();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Categories (BlogID, CategoryID, CategoryName, description, ParentID) VALUES ({1}blogid, {1}catid, {1}catname, {1}description, {1}parentid)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("catname"), category.Title));
                        parms.Add(conn.CreateParameter(FormatParamName("description"), category.Description));
                        parms.Add(conn.CreateParameter(FormatParamName("parentid"), (category.Parent == null ? (object)DBNull.Value : category.Parent.ToString())));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a page to the database
        /// </summary>
        /// <param name="page">
        /// page to be added
        /// </param>
        public override void InsertPage(Page page)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Pages (BlogID, PageID, Title, Description, PageContent, DateCreated, DateModified, Keywords, IsPublished, IsFrontPage, Parent, ShowInList, Slug, IsDeleted, SortOrder) VALUES ({1}blogid, {1}id, {1}title, {1}desc, {1}content, {1}created, {1}modified, {1}keywords, {1}ispublished, {1}isfrontpage, {1}parent, {1}showinlist, {1}slug, {1}isdeleted, {1}sortorder)", this.tablePrefix, this.parmPrefix);
                   
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("id"), page.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("title"), page.Title));
                        parms.Add(conn.CreateParameter(FormatParamName("desc"), page.Description));
                        parms.Add(conn.CreateParameter(FormatParamName("content"), page.Content));
                        parms.Add(conn.CreateParameter(FormatParamName("created"), page.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                        parms.Add(conn.CreateParameter(FormatParamName("modified"),
                                                                    (page.DateModified == new DateTime() ? DateTime.Now : page.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                        parms.Add(conn.CreateParameter(FormatParamName("keywords"), page.Keywords));
                        parms.Add(conn.CreateParameter(FormatParamName("ispublished"), page.IsPublished));
                        parms.Add(conn.CreateParameter(FormatParamName("isfrontpage"), page.IsFrontPage));
                        parms.Add(conn.CreateParameter(FormatParamName("parent"), page.Parent.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("showinlist"), page.ShowInList));
                        parms.Add(conn.CreateParameter(FormatParamName("slug"), page.Slug));
                        parms.Add(conn.CreateParameter(FormatParamName("isdeleted"), page.IsDeleted));
                        parms.Add(conn.CreateParameter(FormatParamName("sortorder"), page.SortOrder));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new post to database
        /// </summary>
        /// <param name="post">
        /// The new post.
        /// </param>
        public override void InsertPost(Post post)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                using (var conn = this.CreateConnection())
                {
                    if (conn.HasConnection)
                    {
                        var sqlQuery = string.Format("INSERT INTO {0}Posts (BlogID, PostID, Title, Description, PostContent, DateCreated, DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug, IsDeleted) VALUES ({1}blogid, {1}id, {1}title, {1}desc, {1}content, {1}created, {1}modified, {1}author, {1}published, {1}commentEnabled, {1}raters, {1}rating, {1}slug, {1}isdeleted)", this.tablePrefix, this.parmPrefix);
                        var blogId = post.BlogId == Guid.Empty ? Blog.CurrentInstance.Id : post.BlogId;

                        using (var cmd = conn.CreateTextCommand(sqlQuery))
                        {

                            var parms = cmd.Parameters;
                            parms.Add(conn.CreateParameter(FormatParamName("blogid"), blogId.ToString()));
                            parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                            parms.Add(conn.CreateParameter(FormatParamName("title"), post.Title));
                            parms.Add(conn.CreateParameter(FormatParamName("desc"), (post.Description ?? string.Empty)));
                            parms.Add(conn.CreateParameter(FormatParamName("content"), post.Content));
                            parms.Add(conn.CreateParameter(FormatParamName("created"), post.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                            parms.Add(conn.CreateParameter(FormatParamName("modified"), (post.DateModified == new DateTime() ? DateTime.Now : post.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                            parms.Add(conn.CreateParameter(FormatParamName("author"), (post.Author ?? string.Empty)));
                            parms.Add(conn.CreateParameter(FormatParamName("published"), post.IsPublished));
                            parms.Add(conn.CreateParameter(FormatParamName("commentEnabled"), post.HasCommentsEnabled));
                            parms.Add(conn.CreateParameter(FormatParamName("raters"), post.Raters));
                            parms.Add(conn.CreateParameter(FormatParamName("rating"), post.Rating));
                            parms.Add(conn.CreateParameter(FormatParamName("slug"), (post.Slug ?? string.Empty)));
                            parms.Add(conn.CreateParameter(FormatParamName("isdeleted"), post.IsDeleted));

                            cmd.ExecuteNonQuery();
                        }

                        // Tags
                        this.UpdateTags(post, conn);

                        // Categories
                        this.UpdateCategories(post, conn);

                        // Comments
                        this.UpdateComments(post, conn);

                        // Email Notification
                        this.UpdateNotify(post, conn);
                    }
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// Adds a new Referrer to the database.
        /// </summary>
        /// <param name="referrer">
        /// Referrer to add.
        /// </param>
        public override void InsertReferrer(Referrer referrer)
        {
            var referrers = Referrer.Referrers;
            referrers.Add(referrer);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Referrers (BlogId, ReferrerId, ReferralDay, ReferrerUrl, ReferralCount, Url, IsSpam) VALUES ({1}BlogId, {1}ReferrerId, {1}ReferralDay, {1}ReferrerUrl, {1}ReferralCount, {1}Url, {1}IsSpam)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddReferrersParametersToCommand(referrer, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Load user data from DataStore
        /// </summary>
        /// <param name="extensionType">
        /// type of info
        /// </param>
        /// <param name="extensionId">
        /// id of info
        /// </param>
        /// <returns>
        /// stream of detail data
        /// </returns>
        public override object LoadFromDataStore(ExtensionType extensionType, string extensionId)
        {
            // MemoryStream stream;
            object o = null;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT Settings FROM {0}DataStoreSettings WHERE BlogId = {1}blogid AND ExtensionType = {1}etype AND ExtensionId = {1}eid", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("etype"), extensionType.GetHashCode()));
                        parms.Add(conn.CreateParameter(FormatParamName("eid"), extensionId));

                        o = cmd.ExecuteScalar();
                    }
                }
            }

            return o;
        }

        /// <summary>
        /// Gets the PingServices from the database
        /// </summary>
        /// <returns>
        /// collection of PingServices
        /// </returns>
        public override StringCollection LoadPingServices()
        {
            var col = new StringCollection();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT Link FROM {0}PingService WHERE BlogID = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!col.Contains(rdr.GetString(0)))
                                {
                                    col.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return col;
        }

        /// <summary>
        /// Gets the settings from the database
        /// </summary>
        /// <returns>
        /// dictionary of settings
        /// </returns>
        public override StringDictionary LoadSettings(Blog blog)
        {
            var dic = new StringDictionary();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT SettingName, SettingValue FROM {0}Settings WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), blog.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var name = rdr.GetString(0);
                                var value = rdr.GetString(1);

                                dic.Add(name, value);
                            }
                        }
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// Get stopwords from the database
        /// </summary>
        /// <returns>
        /// collection of stopwords
        /// </returns>
        public override StringCollection LoadStopWords()
        {
            var col = new StringCollection();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT StopWord FROM {0}StopWords WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var value = rdr.GetString(0);
                                if (!col.Contains(value))
                                {
                                    col.Add(value);
                                }
                            }
                        }
                    }
                }
            }

            return col;
        }

        /// <summary>
        /// Deletes an item from the dataStore
        /// </summary>
        /// <param name="extensionType">
        /// type of item
        /// </param>
        /// <param name="extensionId">
        /// id of item
        /// </param>
        public override void RemoveFromDataStore(ExtensionType extensionType, string extensionId)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}DataStoreSettings WHERE BlogId = {1}blogId AND ExtensionType = {1}type AND ExtensionId = {1}id", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("type"), extensionType));
                        p.Add(conn.CreateParameter(FormatParamName("id"), extensionId));

                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        /// <summary>
        /// Saves the PingServices to the database
        /// </summary>
        /// <param name="services">
        /// collection of PingServices
        /// </param>
        public override void SavePingServices(StringCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {

                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}PingService WHERE BlogID = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.ExecuteNonQuery();

                        foreach (var service in services)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}PingService (BlogID, Link) VALUES ({1}blogid, {1}link)", this.tablePrefix, this.parmPrefix);
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("link"), service));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current BlogEngine rights.
        /// </summary>
        /// <param name="rights"></param>
        public override void SaveRights(IEnumerable<Right> rights)
        {
            if (rights == null)
            {
                throw new ArgumentNullException("rights");
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Rights WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("DELETE FROM {0}RightRoles WHERE BlogId = {1}blogid ", this.tablePrefix, this.parmPrefix);
                        cmd.ExecuteNonQuery();

                        foreach (var right in rights)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}Rights (BlogId, RightName) VALUES ({1}BlogId, {1}RightName)", this.tablePrefix, this.parmPrefix);

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), Blog.CurrentInstance.Id.ToString()));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("RightName"), right.Name));

                            cmd.ExecuteNonQuery();

                            foreach (var role in right.Roles)
                            {
                                cmd.CommandText = string.Format("INSERT INTO {0}RightRoles (BlogId, RightName, Role) VALUES ({1}BlogId, {1}RightName, {1}Role)", this.tablePrefix, this.parmPrefix);

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), Blog.CurrentInstance.Id.ToString()));
                                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("RightName"), right.Name));
                                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("Role"), role));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Saves the settings to the database
        /// </summary>
        /// <param name="settings">
        /// dictionary of settings
        /// </param>
        public override void SaveSettings(StringDictionary settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Settings WHERE BlogId = {1}blogid", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.ExecuteNonQuery();

                        foreach (string key in settings.Keys)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}Settings (BlogId, SettingName, SettingValue) VALUES ({1}blogid, {1}name, {1}value)", this.tablePrefix, this.parmPrefix);
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), key));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("value"), settings[key]));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save to DataStore
        /// </summary>
        /// <param name="extensionType">
        /// type of info
        /// </param>
        /// <param name="extensionId">
        /// id of info
        /// </param>
        /// <param name="settings">
        /// data of info
        /// </param>
        public override void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Save

            var xs = new XmlSerializer(settings.GetType());
            string objectXml;
            using (var sw = new StringWriter())
            {
                xs.Serialize(sw, settings);
                objectXml = sw.ToString();
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}DataStoreSettings WHERE BlogId = {1}blogid AND ExtensionType = {1}type AND ExtensionId = {1}id; ", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("type"), extensionType.GetHashCode()));
                        p.Add(conn.CreateParameter(FormatParamName("id"), extensionId));

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("INSERT INTO {0}DataStoreSettings (BlogId, ExtensionType, ExtensionId, Settings) VALUES ({1}blogid, {1}type, {1}id, {1}file)", this.tablePrefix, this.parmPrefix);

                        p.Add(conn.CreateParameter(FormatParamName("file"), objectXml));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a BlogRoll based on a Guid.
        /// </summary>
        /// <param name="id">
        /// The BlogRoll's Guid.
        /// </param>
        /// <returns>
        /// A matching BlogRoll
        /// </returns>
        public override BlogRollItem SelectBlogRollItem(Guid id)
        {
            var blogRoll = BlogRollItem.BlogRolls.Find(br => br.Id == id) ?? new BlogRollItem();

            blogRoll.MarkOld();
            return blogRoll;
        }

        /// <summary>
        /// Gets a Blog based on a Guid.
        /// </summary>
        /// <param name="id">
        /// The Blog's Guid.
        /// </param>
        /// <returns>
        /// A matching Blog
        /// </returns>
        public override Blog SelectBlog(Guid id)
        {
            var blog = Blog.Blogs.Find(b => b.Id == id) ?? new Blog();

            blog.MarkOld();
            return blog;
        }

        /// <summary>
        /// Returns a category
        /// </summary>
        /// <param name="id">Id of category to return</param>
        /// <returns>A category.</returns>
        public override Category SelectCategory(Guid id)
        {
            var categories = Category.Categories;

            var category = new Category();

            foreach (var cat in categories.Where(cat => cat.Id == id))
            {
                category = cat;
            }

            category.MarkOld();
            return category;
        }

        /// <summary>
        /// Returns a page for given ID
        /// </summary>
        /// <param name="id">
        /// ID of page to return
        /// </param>
        /// <returns>
        /// selected page
        /// </returns>
        public override Page SelectPage(Guid id)
        {
            var page = new Page();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT PageID, Title, Description, PageContent, DateCreated, DateModified, Keywords, IsPublished, IsFrontPage, Parent, ShowInList, Slug, IsDeleted, SortOrder FROM {0}Pages WHERE BlogID = {1}blogid AND PageID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                page.Id = rdr.GetGuid(0);
                                page.Title = rdr.IsDBNull(1) ? String.Empty : rdr.GetString(1);
                                page.Content = rdr.IsDBNull(3) ? String.Empty : rdr.GetString(3);
                                page.Description = rdr.IsDBNull(2) ? String.Empty : rdr.GetString(2);
                                if (!rdr.IsDBNull(4))
                                {
                                    page.DateCreated = rdr.GetDateTime(4);
                                }

                                if (!rdr.IsDBNull(5))
                                {
                                    page.DateModified = rdr.GetDateTime(5);
                                }

                                if (!rdr.IsDBNull(6))
                                {
                                    page.Keywords = rdr.GetString(6);
                                }

                                if (!rdr.IsDBNull(7))
                                {
                                    page.IsPublished = rdr.GetBoolean(7);
                                }

                                if (!rdr.IsDBNull(8))
                                {
                                    page.IsFrontPage = rdr.GetBoolean(8);
                                }

                                if (!rdr.IsDBNull(9))
                                {
                                    page.Parent = rdr.GetGuid(9);
                                }

                                if (!rdr.IsDBNull(10))
                                {
                                    page.ShowInList = rdr.GetBoolean(10);
                                }

                                if (!rdr.IsDBNull(11))
                                {
                                    page.Slug = rdr.GetString(11);
                                }

                                if (!rdr.IsDBNull(12))
                                {
                                    page.IsDeleted = rdr.GetBoolean(12);
                                }

                                if (!rdr.IsDBNull(13))
                                {
                                    page.SortOrder = rdr.GetInt32(13);
                                }
                            }
                        }
                    }
                }
            }

            return page;
        }

        /// <summary>
        /// Returns a Post based on Id.
        /// </summary>
        /// <param name="id">
        /// The Post ID.
        /// </param>
        /// <returns>
        /// The Post..
        /// </returns>
        public override Post SelectPost(Guid id)
        {
            var post = new Post();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT PostID, Title, Description, PostContent, DateCreated, DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug, IsDeleted FROM {0}Posts WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                post.Id = rdr.GetGuid(0);
                                post.Title = rdr.GetString(1);
                                post.Content = rdr.GetString(3);
                                post.Description = rdr.IsDBNull(2) ? String.Empty : rdr.GetString(2);
                                if (!rdr.IsDBNull(4))
                                {
                                    post.DateCreated = rdr.GetDateTime(4);
                                }

                                if (!rdr.IsDBNull(5))
                                {
                                    post.DateModified = rdr.GetDateTime(5);
                                }

                                if (!rdr.IsDBNull(6))
                                {
                                    post.Author = rdr.GetString(6);
                                }

                                if (!rdr.IsDBNull(7))
                                {
                                    post.IsPublished = rdr.GetBoolean(7);
                                }

                                if (!rdr.IsDBNull(8))
                                {
                                    post.HasCommentsEnabled = rdr.GetBoolean(8);
                                }

                                if (!rdr.IsDBNull(9))
                                {
                                    post.Raters = rdr.GetInt32(9);
                                }

                                if (!rdr.IsDBNull(10))
                                {
                                    post.Rating = rdr.GetFloat(10);
                                }

                                post.Slug = !rdr.IsDBNull(11) ? rdr.GetString(11) : string.Empty;

                                if (!rdr.IsDBNull(12))
                                {
                                    post.IsDeleted = rdr.GetBoolean(12);
                                }
                            }
                        }

                        // Tags
                        cmd.CommandText = string.Format("SELECT Tag FROM {0}PostTag WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    post.Tags.Add(rdr.GetString(0));
                                }
                            }
                        }

                        post.Tags.MarkOld();

                        // Categories
                        cmd.CommandText = string.Format("SELECT CategoryID FROM {0}PostCategory WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var key = rdr.GetGuid(0);
                                if (Category.GetCategory(key) != null)
                                {
                                    post.Categories.Add(Category.GetCategory(key));
                                }
                            }
                        }

                        // Comments
                        cmd.CommandText = string.Format("SELECT PostCommentID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ParentCommentID, ModeratedBy, Avatar, IsSpam, IsDeleted FROM {0}PostComment WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var comment = new Comment
                                    {
                                        Id = rdr.GetGuid(0),
                                        IsApproved = true,
                                        Author = rdr.GetString(2)
                                    };
                                if (!rdr.IsDBNull(4))
                                {
                                    Uri website;
                                    if (Uri.TryCreate(rdr.GetString(4), UriKind.Absolute, out website))
                                    {
                                        comment.Website = website;
                                    }
                                }

                                comment.Email = rdr.GetString(3);
                                comment.Content = rdr.GetString(5);
                                comment.DateCreated = rdr.GetDateTime(1);
                                comment.Parent = post;

                                if (!rdr.IsDBNull(6))
                                {
                                    comment.Country = rdr.GetString(6);
                                }

                                if (!rdr.IsDBNull(7))
                                {
                                    comment.IP = rdr.GetString(7);
                                }

                                comment.IsApproved = rdr.IsDBNull(8) || rdr.GetBoolean(8);

                                comment.ParentId = rdr.GetGuid(9);

                                if (!rdr.IsDBNull(10))
                                {
                                    comment.ModeratedBy = rdr.GetString(10);
                                }

                                if (!rdr.IsDBNull(11))
                                {
                                    comment.Avatar = rdr.GetString(11);
                                }

                                if (!rdr.IsDBNull(12))
                                {
                                    comment.IsSpam = rdr.GetBoolean(12);
                                }

                                if (!rdr.IsDBNull(13))
                                {
                                    comment.IsDeleted = rdr.GetBoolean(13);
                                }

                                post.AllComments.Add(comment);
                            }
                        }

                        post.AllComments.Sort();

                        // Email Notification
                        cmd.CommandText = string.Format("SELECT NotifyAddress FROM {0}PostNotify WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    var email = rdr.GetString(0);
                                    if (!post.NotificationEmails.Contains(email))
                                    {
                                        post.NotificationEmails.Add(email);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return post;
        }

        /// <summary>
        /// Gets a Referrer based on an Id.
        /// </summary>
        /// <param name="id">
        /// The Referrer Id.
        /// </param>
        /// <returns>
        /// A matching Referrer
        /// </returns>
        public override Referrer SelectReferrer(Guid id)
        {
            var refer = Referrer.Referrers.Find(r => r.Id.Equals(id)) ?? new Referrer();

            refer.MarkOld();
            return refer;
        }

        /// <summary>
        /// Sets up the required storage files/tables for a new Blog instance, from an existing blog instance.
        /// </summary>
        /// <param name="existingBlog">
        /// The existing blog to copy from.
        /// </param>
        /// <param name="newBlog">
        /// The new blog to copy to.
        /// </param>
        public override bool SetupBlogFromExistingBlog(Blog existingBlog, Blog newBlog)
        {
            // Even for the DbBlogProvider, we call newBlog.CopyExistingBlogFolderToNewBlogFolder().
            // The reasons are that a small number of extensions/widgets use App_Data even if
            // the DbBlogProvider is being used (Newsletter widget, Logger extension, and any
            // other custom components written by other people).  Also, even if the
            // DbBlogProvider is being used, the XmlMembershipProvider and XmlRoleProvider could
            // also be used, which stores data in App_Data.
            // So as a rule of thumb, whenever a new blog instance is created, we will create
            // a new folder in App_Data for that new instance, and copy all the files/folders in.

            bool copyResult = newBlog.CopyExistingBlogFolderToNewBlogFolder(existingBlog);

            if (!copyResult)
            {
                Utils.Log("DbBlogProvider.SetupBlogFromExistingBlog", new Exception("Unsuccessful result from newBlog.CopyExistingBlogFolderToNewBlogFolder."));
                return false;
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    //
                    // For SQL CE compatibility, all the "newblogid" parameters below need to have their DBType set to DBType.String.
                    // This is done with the CreateParameter() overload that accepts a DBType.
                    //

                    // be_BlogRollItems
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}BlogRollItems ( BlogId, BlogRollId, Title, Description, BlogUrl, FeedUrl, Xfn, SortIndex ) " +
                        " SELECT {1}newblogid, BlogRollId, Title, Description, BlogUrl, FeedUrl, Xfn, SortIndex " +
                        " FROM {0}BlogRollItems " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Categories
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Categories ( BlogID, CategoryID, CategoryName, Description, ParentID ) " +
                        " SELECT {1}newblogid, CategoryID, CategoryName, Description, ParentID " +
                        " FROM {0}Categories " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_DataStoreSettings
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}DataStoreSettings ( BlogId, ExtensionType, ExtensionId, Settings ) " +
                        " SELECT {1}newblogid, ExtensionType, ExtensionId, Settings " +
                        " FROM {0}DataStoreSettings " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Pages
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Pages ( BlogID, PageID, Title, Description, PageContent, Keywords, DateCreated, DateModified, IsPublished, IsFrontPage, Parent, ShowInList, Slug, IsDeleted, SortOrder) " +
                        " SELECT {1}newblogid, PageID, Title, Description, PageContent, Keywords, DateCreated, DateModified, IsPublished, IsFrontPage, Parent, ShowInList, Slug, IsDeleted, SortOrder " +
                        " FROM {0}Pages " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_PingService
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}PingService ( BlogID, Link ) " +
                        " SELECT {1}newblogid, Link " +
                        " FROM {0}PingService " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Profiles
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Profiles ( BlogID, UserName, SettingName, SettingValue ) " +
                        " SELECT {1}newblogid, UserName, SettingName, SettingValue " +
                        " FROM {0}Profiles " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Referrers
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Referrers ( BlogId, ReferrerId, ReferralDay, ReferrerUrl, ReferralCount, Url, IsSpam ) " +
                        " SELECT {1}newblogid, ReferrerId, ReferralDay, ReferrerUrl, ReferralCount, Url, IsSpam " +
                        " FROM {0}Referrers " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Rights
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Rights ( BlogId, RightName ) " +
                        " SELECT {1}newblogid, RightName " +
                        " FROM {0}Rights " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_RightRoles
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}RightRoles ( BlogId, RightName, Role ) " +
                        " SELECT {1}newblogid, RightName, Role " +
                        " FROM {0}RightRoles " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Settings
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Settings ( BlogId, SettingName, SettingValue ) " +
                        " SELECT {1}newblogid, SettingName, SettingValue " +
                        " FROM {0}Settings " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_StopWords
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}StopWords ( BlogId, StopWord ) " +
                        " SELECT {1}newblogid, StopWord " +
                        " FROM {0}StopWords " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Posts
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Posts ( BlogId, PostID, Title, Description, PostContent, DateCreated, DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug, IsDeleted ) " +
                        " SELECT {1}newblogid, PostID, Title, Description, PostContent, DateCreated, DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug, IsDeleted " +
                        " FROM {0}Posts " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_PostCategory
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}PostCategory ( BlogId, PostID, CategoryID ) " +
                        " SELECT {1}newblogid, PostID, CategoryID " +
                        " FROM {0}PostCategory " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_PostComment
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}PostComment ( BlogId, PostCommentID, PostID, ParentCommentID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ModeratedBy, Avatar, IsSpam, IsDeleted ) " +
                        " SELECT {1}newblogid, PostCommentID, PostID, ParentCommentID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ModeratedBy, Avatar, IsSpam, IsDeleted " +
                        " FROM {0}PostComment " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_PostNotify
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}PostNotify ( BlogId, PostID, NotifyAddress ) " +
                        " SELECT {1}newblogid, PostID, NotifyAddress " +
                        " FROM {0}PostNotify " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_PostTag
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}PostTag ( BlogId, PostID, Tag ) " +
                        " SELECT {1}newblogid, PostID, Tag " +
                        " FROM {0}PostTag " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    //////////////////////////////////////
                    // The DbMembershipProvider and DbRoleProvider may or may not be in use.
                    // Even if it's not in use, copy the rows for the Users and Roles tables.
                    //

                    // be_Users
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Users ( BlogId, UserName, Password, LastLoginTime, EmailAddress ) " +
                        " SELECT {1}newblogid, UserName, Password, LastLoginTime, EmailAddress " +
                        " FROM {0}Users " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Roles
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Roles ( BlogId, Role ) " +
                        " SELECT {1}newblogid, Role " +
                        " FROM {0}Roles " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_UserRoles
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}UserRoles ( BlogId, UserName, Role ) " +
                        " SELECT {1}newblogid, UserName, Role " +
                        " FROM {0}UserRoles " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Setup new blog from scratch
        /// </summary>
        /// <param name="newBlog">New blog</param>
        /// <param name="userName">User name</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns>True if successful</returns>
        public override bool SetupNewBlog(Blog newBlog, string userName, string email, string password)
        {
            bool copyResult = BlogGenerator.CopyTemplateBlogFolder(newBlog.Name, userName, email, password);

            if (!copyResult)
            {
                Utils.Log("DbBlogProvider.SetupNewBlog", new Exception("Unsuccessful result from BlogGenerator.CopyTemplateBlogFolder."));
                return false;
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    Blog existingBlog = Blog.Blogs.Where(b => b.Name == "Template").FirstOrDefault();

                    if (existingBlog == null)
                        existingBlog = Blog.Blogs[0];

                    // be_PingService
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}PingService ( BlogID, Link ) " +
                        " SELECT {1}newblogid, Link " +
                        " FROM {0}PingService " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Rights
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}Rights ( BlogId, RightName ) " +
                        " SELECT {1}newblogid, RightName " +
                        " FROM {0}Rights " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_RightRoles
                    using (var cmd = conn.CreateTextCommand(string.Format(
                        " INSERT INTO {0}RightRoles ( BlogId, RightName, Role ) " +
                        " SELECT {1}newblogid, RightName, Role " +
                        " FROM {0}RightRoles " +
                        " WHERE BlogID = {1}existingblogid ", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("newblogid"), newBlog.Id.ToString(), System.Data.DbType.String));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("existingblogid"), existingBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Settings
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Settings WHERE BlogId = {1}blogid", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        cmd.ExecuteNonQuery();
                        var settings = BlogGeneratorConfig.NewBlogSettings;
                        var setValue = "";

                        foreach (string key in settings.Keys)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}Settings (BlogId, SettingName, SettingValue) VALUES ({1}blogid, {1}name, {1}value)", this.tablePrefix, this.parmPrefix);
                            cmd.Parameters.Clear();

                            setValue = settings[key];
                            if (setValue == "[BlogName]") setValue = newBlog.Name;
                            if (setValue == "[Description]") setValue = BlogGeneratorConfig.BlogDescription;

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), key));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("value"), setValue));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    var catId = Guid.NewGuid().ToString();

                    // be_Categories
                    var sqlQuery = string.Format("INSERT INTO {0}Categories (BlogID, CategoryID, CategoryName, description, ParentID) VALUES ({1}blogid, {1}catid, {1}catname, {1}description, {1}parentid)", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("catid"), catId));
                        parms.Add(conn.CreateParameter(FormatParamName("catname"), "General"));
                        parms.Add(conn.CreateParameter(FormatParamName("description"), "General topics"));
                        parms.Add(conn.CreateParameter(FormatParamName("parentid"), (object)DBNull.Value));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Posts
                    Post post = new Post();
                    post.BlogId = newBlog.Id;
                    post.Title = BlogGeneratorConfig.PostTitle.Contains("{0}") ?
                        string.Format(BlogGeneratorConfig.PostTitle, newBlog.Name) :
                        BlogGeneratorConfig.PostTitle;
                    post.Slug = Utils.RemoveIllegalCharacters(post.Title);
                    post.Description = "The description is used as the meta description as well as shown in the related posts. It is recommended that you write a description, but not mandatory";
                    post.Author = userName;
                    string content = BlogGeneratorConfig.PostContent.Replace("&lt;", "<").Replace("&gt;", ">");
                    post.Content = content.Contains("{1}") ?
                        string.Format(content, userName, Utils.RelativeWebRoot + newBlog.Name + "/Account/login.aspx") : content;       
                    InsertPost(post);

                    // be_posttags
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}PostTag (BlogID, PostID, Tag) VALUES ({1}blogid, {1}id, {1}tag)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("tag"), "welcome"));
                        cmd.ExecuteNonQuery();
                    }

                    // be_postcategories
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}PostCategory (BlogID, PostID, CategoryID) VALUES ({1}blogid, {1}id, {1}cat)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("cat"), catId));
                        cmd.ExecuteNonQuery();
                    }

                    // be_DataStoreSettings
                    string widgetZone = BlogGeneratorConfig.WidgetZone;
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}DataStoreSettings (BlogId, ExtensionType, ExtensionId, Settings) VALUES ({1}blogid, {1}type, {1}id, {1}file)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("type"), 1));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), "be_WIDGET_ZONE"));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("file"), widgetZone));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Users
                    sqlQuery = string.Format("INSERT INTO {0}Users (blogId, userName, password, emailAddress, lastLoginTime) VALUES ({1}blogid, {1}name, {1}pwd, {1}email, {1}login)", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("name"), userName));
                        parms.Add(conn.CreateParameter(FormatParamName("pwd"), Utils.HashPassword(password)));
                        parms.Add(conn.CreateParameter(FormatParamName("email"), email));
                        parms.Add(conn.CreateParameter(FormatParamName("login"), DateTime.Now));
                        cmd.ExecuteNonQuery();
                    }

                    // be_Roles
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}Roles (BlogID, role) VALUES ({1}blogid, {1}role)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), "Administrators"));
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}Roles (BlogID, role) VALUES ({1}blogid, {1}role)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), newBlog.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), "Anonymous"));
                        cmd.ExecuteNonQuery();
                    }

                    // be_UserRoles
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}UserRoles (BlogID, UserName, Role) VALUES ({1}blogID, {1}username, {1}role)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogID"), newBlog.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("username"), userName.Trim()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), "Administrators"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Saves an existing BlogRoll to the database
        /// </summary>
        /// <param name="blogRollItem">
        /// BlogRoll to be saved
        /// </param>
        public override void UpdateBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = BlogRollItem.BlogRolls;
            blogRolls.Remove(blogRollItem);
            blogRolls.Add(blogRollItem);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}BlogRollItems SET Title = {1}Title, Description = {1}Description, BlogUrl = {1}BlogUrl, FeedUrl = {1}FeedUrl, Xfn = {1}Xfn, SortIndex = {1}SortIndex WHERE BlogId = {1}BlogId AND BlogRollId = {1}BlogRollId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddBlogRollParametersToCommand(blogRollItem, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves an existing Blog to the database
        /// </summary>
        /// <param name="blog">
        /// Blog to be saved
        /// </param>
        public override void UpdateBlog(Blog blog)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Blogs SET BlogName = {1}BlogName, Hostname = {1}Hostname, IsAnyTextBeforeHostnameAccepted = {1}IsAnyTextBeforeHostnameAccepted, StorageContainerName = {1}StorageContainerName, VirtualPath = {1}VirtualPath, IsPrimary = {1}IsPrimary, IsActive = {1}IsActive, IsSiteAggregation = {1}IsSiteAggregation WHERE BlogId = {1}BlogId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddBlogParametersToCommand(blog, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves an existing category to the database
        /// </summary>
        /// <param name="category">
        /// category to be saved
        /// </param>
        public override void UpdateCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Remove(category);
            categories.Add(category);
            categories.Sort();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Categories SET CategoryName = {1}catname, Description = {1}description, ParentID = {1}parentid WHERE BlogID = {1}blogid AND CategoryID = {1}catid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("catname"), category.Title));
                        p.Add(conn.CreateParameter(FormatParamName("description"), category.Description));
                        p.Add(conn.CreateParameter(FormatParamName("parentid"), (category.Parent == null ? (object)DBNull.Value : category.Parent.ToString())));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves an existing page in the database
        /// </summary>
        /// <param name="page">
        /// page to be saved
        /// </param>
        public override void UpdatePage(Page page)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Pages SET Title = {1}title, Description = {1}desc, PageContent = {1}content, DateCreated = {1}created, DateModified = {1}modified, Keywords = {1}keywords, IsPublished = {1}ispublished, IsFrontPage = {1}isfrontpage, Parent = {1}parent, ShowInList = {1}showinlist, Slug = {1}slug, IsDeleted = {1}isdeleted, SortOrder = {1}sortorder WHERE BlogID = {1}blogid AND PageID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("id"), page.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("title"), page.Title));
                        p.Add(conn.CreateParameter(FormatParamName("desc"), page.Description));
                        p.Add(conn.CreateParameter(FormatParamName("content"), page.Content));
                        p.Add(conn.CreateParameter(FormatParamName("created"), page.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                        p.Add(conn.CreateParameter(FormatParamName("modified"), (page.DateModified == new DateTime() ? DateTime.Now : page.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                        p.Add(conn.CreateParameter(FormatParamName("keywords"), page.Keywords));
                        p.Add(conn.CreateParameter(FormatParamName("ispublished"), page.IsPublished));
                        p.Add(conn.CreateParameter(FormatParamName("isfrontpage"), page.IsFrontPage));
                        p.Add(conn.CreateParameter(FormatParamName("parent"), page.Parent.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("showinlist"), page.ShowInList));
                        p.Add(conn.CreateParameter(FormatParamName("slug"), page.Slug));
                        p.Add(conn.CreateParameter(FormatParamName("isdeleted"), page.IsDeleted));
                        p.Add(conn.CreateParameter(FormatParamName("sortorder"), page.SortOrder));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves and existing post in the database
        /// </summary>
        /// <param name="post">
        /// post to be saved
        /// </param>
        public override void UpdatePost(Post post)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                using (var conn = this.CreateConnection())
                {
                    if (conn.HasConnection)
                    {
                        var sqlQuery = string.Format("UPDATE {0}Posts SET Title = {1}title, Description = {1}desc, PostContent = {1}content, DateCreated = {1}created, DateModified = {1}modified, Author = {1}Author, IsPublished = {1}published, IsCommentEnabled = {1}commentEnabled, Raters = {1}raters, Rating = {1}rating, Slug = {1}slug, IsDeleted = {1}isdeleted WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);

                        using (var cmd = conn.CreateTextCommand(sqlQuery))
                        {

                            var p = cmd.Parameters;

                            p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                            p.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                            p.Add(conn.CreateParameter(FormatParamName("title"), post.Title));
                            p.Add(conn.CreateParameter(FormatParamName("desc"), (post.Description ?? string.Empty)));
                            p.Add(conn.CreateParameter(FormatParamName("content"), post.Content));
                            p.Add(conn.CreateParameter(FormatParamName("created"), post.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                            p.Add(conn.CreateParameter(FormatParamName("modified"), (post.DateModified == new DateTime() ? DateTime.Now : post.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                            p.Add(conn.CreateParameter(FormatParamName("author"), (post.Author ?? string.Empty)));
                            p.Add(conn.CreateParameter(FormatParamName("published"), post.IsPublished));
                            p.Add(conn.CreateParameter(FormatParamName("commentEnabled"), post.HasCommentsEnabled));
                            p.Add(conn.CreateParameter(FormatParamName("raters"), post.Raters));
                            p.Add(conn.CreateParameter(FormatParamName("rating"), post.Rating));
                            p.Add(conn.CreateParameter(FormatParamName("slug"), (post.Slug ?? string.Empty)));
                            p.Add(conn.CreateParameter(FormatParamName("isdeleted"), post.IsDeleted));

                            cmd.ExecuteNonQuery();
                        }

                        // Tags
                        this.UpdateTags(post, conn);

                        // Categories
                        this.UpdateCategories(post, conn);

                        // Comments
                        this.UpdateComments(post, conn);

                        // Email Notification
                        this.UpdateNotify(post, conn);
                    }
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// Saves an existing Referrer to the database.
        /// </summary>
        /// <param name="referrer">
        /// Referrer to be saved.
        /// </param>
        public override void UpdateReferrer(Referrer referrer)
        {
            var referrers = Referrer.Referrers;
            referrers.Remove(referrer);
            referrers.Add(referrer);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Referrers SET ReferralDay = {1}ReferralDay, ReferrerUrl = {1}ReferrerUrl, ReferralCount = {1}ReferralCount, Url = {1}Url, IsSpam = {1}IsSpam WHERE BlogId = {1}BlogId AND ReferrerId = {1}ReferrerId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddReferrersParametersToCommand(referrer, conn, cmd);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #region Profiles

        /// <summary>
        /// Return collection for AuthorProfiles from database
        /// </summary>
        /// <returns>
        /// List of AuthorProfile
        /// </returns>
        public override List<AuthorProfile> FillProfiles()
        {
            var profileNames = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    if (Blog.CurrentInstance.IsSiteAggregation)
                    {
                        using (var cmd = conn.CreateTextCommand(string.Format("SELECT UserName FROM {0}Profiles GROUP BY UserName", this.tablePrefix, this.parmPrefix)))
                        {
                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    profileNames.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var cmd = conn.CreateTextCommand(string.Format("SELECT UserName FROM {0}Profiles WHERE BlogID = {1}blogid GROUP BY UserName", this.tablePrefix, this.parmPrefix)))
                        {
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    profileNames.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return profileNames.Select(BusinessBase<AuthorProfile, string>.Load).ToList();
        }

        /// <summary>
        /// Loads AuthorProfile from database
        /// </summary>
        /// <param name="id">The user name.</param>
        /// <returns>An AuthorProfile.</returns>
        public override AuthorProfile SelectProfile(string id)
        {
            var dic = new StringDictionary();
            var profile = new AuthorProfile(id);

            // Retrieve Profile data from Db

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    if (Blog.CurrentInstance.IsSiteAggregation)
                    {
                        using (var cmd = conn.CreateTextCommand(string.Format("SELECT SettingName, SettingValue FROM {0}Profiles WHERE UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                        {
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), id));

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    dic.Add(rdr.GetString(0), rdr.GetString(1));
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var cmd = conn.CreateTextCommand(string.Format("SELECT SettingName, SettingValue FROM {0}Profiles WHERE BlogID = {1}blogid AND UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                        {
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), id));

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    dic.Add(rdr.GetString(0), rdr.GetString(1));
                                }
                            }
                        }
                    }
                }
            }

            // Load profile with data from dictionary
            if (dic.ContainsKey("DisplayName"))
            {
                profile.DisplayName = dic["DisplayName"];
            }

            if (dic.ContainsKey("FirstName"))
            {
                profile.FirstName = dic["FirstName"];
            }

            if (dic.ContainsKey("MiddleName"))
            {
                profile.MiddleName = dic["MiddleName"];
            }

            if (dic.ContainsKey("LastName"))
            {
                profile.LastName = dic["LastName"];
            }

            if (dic.ContainsKey("CityTown"))
            {
                profile.CityTown = dic["CityTown"];
            }

            if (dic.ContainsKey("RegionState"))
            {
                profile.RegionState = dic["RegionState"];
            }

            if (dic.ContainsKey("Country"))
            {
                profile.Country = dic["Country"];
            }

            if (dic.ContainsKey("Birthday"))
            {
                DateTime date;
                if (DateTime.TryParse(dic["Birthday"], out date))
                {
                    profile.Birthday = date;
                }
            }

            if (dic.ContainsKey("AboutMe"))
            {
                profile.AboutMe = dic["AboutMe"];
            }

            if (dic.ContainsKey("PhotoURL"))
            {
                profile.PhotoUrl = dic["PhotoURL"];
            }

            if (dic.ContainsKey("Company"))
            {
                profile.Company = dic["Company"];
            }

            if (dic.ContainsKey("EmailAddress"))
            {
                profile.EmailAddress = dic["EmailAddress"];
            }

            if (dic.ContainsKey("PhoneMain"))
            {
                profile.PhoneMain = dic["PhoneMain"];
            }

            if (dic.ContainsKey("PhoneMobile"))
            {
                profile.PhoneMobile = dic["PhoneMobile"];
            }

            if (dic.ContainsKey("PhoneFax"))
            {
                profile.PhoneFax = dic["PhoneFax"];
            }

            if (dic.ContainsKey("IsPrivate"))
            {
                profile.Private = dic["IsPrivate"] == "true";
            }

            return profile;
        }

        /// <summary>
        /// Adds AuthorProfile to database
        /// </summary>
        /// <param name="profile">An AuthorProfile.</param>
        public override void InsertProfile(AuthorProfile profile)
        {
            this.UpdateProfile(profile);
        }

        /// <summary>
        /// Updates AuthorProfile to database
        /// </summary>
        /// <param name="profile">
        /// An AuthorProfile.
        /// </param>
        public override void UpdateProfile(AuthorProfile profile)
        {
            // Remove Profile
            this.DeleteProfile(profile);

            // Create Profile Dictionary
            var dic = new StringDictionary();

            if (!String.IsNullOrEmpty(profile.DisplayName))
            {
                dic.Add("DisplayName", profile.DisplayName);
            }

            if (!String.IsNullOrEmpty(profile.FirstName))
            {
                dic.Add("FirstName", profile.FirstName);
            }

            if (!String.IsNullOrEmpty(profile.MiddleName))
            {
                dic.Add("MiddleName", profile.MiddleName);
            }

            if (!String.IsNullOrEmpty(profile.LastName))
            {
                dic.Add("LastName", profile.LastName);
            }

            if (!String.IsNullOrEmpty(profile.CityTown))
            {
                dic.Add("CityTown", profile.CityTown);
            }

            if (!String.IsNullOrEmpty(profile.RegionState))
            {
                dic.Add("RegionState", profile.RegionState);
            }

            if (!String.IsNullOrEmpty(profile.Country))
            {
                dic.Add("Country", profile.Country);
            }

            if (!String.IsNullOrEmpty(profile.AboutMe))
            {
                dic.Add("AboutMe", profile.AboutMe);
            }

            if (!String.IsNullOrEmpty(profile.PhotoUrl))
            {
                dic.Add("PhotoURL", profile.PhotoUrl);
            }

            if (!String.IsNullOrEmpty(profile.Company))
            {
                dic.Add("Company", profile.Company);
            }

            if (!String.IsNullOrEmpty(profile.EmailAddress))
            {
                dic.Add("EmailAddress", profile.EmailAddress);
            }

            if (!String.IsNullOrEmpty(profile.PhoneMain))
            {
                dic.Add("PhoneMain", profile.PhoneMain);
            }

            if (!String.IsNullOrEmpty(profile.PhoneMobile))
            {
                dic.Add("PhoneMobile", profile.PhoneMobile);
            }

            if (!String.IsNullOrEmpty(profile.PhoneFax))
            {
                dic.Add("PhoneFax", profile.PhoneFax);
            }

            if (profile.Birthday != DateTime.MinValue)
            {
                dic.Add("Birthday", profile.Birthday.ToString("yyyy-MM-dd"));
            }

            dic.Add("IsPrivate", profile.Private.ToString());

            // Save Profile Dictionary

            using (var conn = this.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    foreach (string key in dic.Keys)
                    {
                        var sqlQuery = string.Format("INSERT INTO {0}Profiles (BlogID, UserName, SettingName, SettingValue) VALUES ({1}blogid, {1}user, {1}name, {1}value)", this.tablePrefix, this.parmPrefix);

                        cmd.CommandText = sqlQuery;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("user"), profile.Id));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), key));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("value"), dic[key]));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Remove AuthorProfile from database
        /// </summary>
        /// <param name="profile">An AuthorProfile.</param>
        public override void DeleteProfile(AuthorProfile profile)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Profiles WHERE BlogID = {1}blogid AND UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), profile.Id));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        #region Packaging

        /// <summary>
        /// Log of all installed packages
        /// </summary>
        /// <param name="package">Intalled package</param>
        public override void SavePackage(InstalledPackage package)
        {
            using (var conn = CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Packages (PackageId, Version) VALUES ({1}PackageId, {1}Version)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("PackageId"), package.PackageId));
                        parms.Add(conn.CreateParameter(FormatParamName("Version"), package.Version));
 
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Log of all files for installed package
        /// </summary>
        /// <param name="packageFiles">List of intalled package files</param>
        public override void SavePackageFiles(List<PackageFile> packageFiles)
        {
            using (var conn = CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}PackageFiles (PackageId, FileOrder, FilePath, IsDirectory) VALUES ({1}PackageId, {1}FileOrder, {1}FilePath, {1}IsDirectory)", this.tablePrefix, this.parmPrefix);

                    foreach (var file in packageFiles)
                    {
                        using (var cmd = conn.CreateTextCommand(sqlQuery))
                        {
                            var parms = cmd.Parameters;
                            parms.Add(conn.CreateParameter(FormatParamName("PackageId"), file.PackageId));
                            parms.Add(conn.CreateParameter(FormatParamName("FileOrder"), file.FileOrder));
                            parms.Add(conn.CreateParameter(FormatParamName("FilePath"), file.FilePath));
                            parms.Add(conn.CreateParameter(FormatParamName("IsDirectory"), file.IsDirectory));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets list of files for installed package
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <returns>List of files for installed package</returns>
        public override List<PackageFile> FillPackageFiles(string packageId)
        {
            var files = new List<PackageFile>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT PackageId, FileOrder, FilePath, IsDirectory FROM {0}PackageFiles ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var f = new PackageFile()
                                {
                                    PackageId = rdr.GetString(0),
                                    FileOrder = rdr.GetInt32(1),
                                    FilePath = rdr.GetString(2),
                                    IsDirectory = rdr.GetBoolean(3)
                                };

                                files.Add(f);
                            }
                        }
                    }
                }
            }

            return files;
        }
        /// <summary>
        /// Gets all installed from gallery packages
        /// </summary>
        /// <returns>List of installed packages</returns>
        public override List<InstalledPackage> FillPackages()
        {
            var packages = new List<InstalledPackage>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT PackageId, Version FROM {0}Packages ", tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var p = new InstalledPackage()
                                {
                                    PackageId = rdr.GetString(0),
                                    Version = rdr.GetString(1)
                                };

                                packages.Add(p);
                            }
                        }
                    }
                }
            }

            return packages;
        }
        /// <summary>
        /// Should delete package and remove all package files
        /// </summary>
        /// <param name="packageId">Package ID</param>
        public override void DeletePackage(string packageId)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}PackageFiles WHERE PackageId = {1}PackageId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("PackageId"), packageId));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Packages WHERE PackageId = {1}PackageId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("PackageId"), packageId));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        #region QuickNotes
        /// <summary>
        /// Save quick note
        /// </summary>
        /// <param name="note">Quick note</param>
        public override void SaveQuickNote(QuickNote note)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("select count(*) from {0}QuickNotes where NoteId = {1}noteid", this.tablePrefix, this.parmPrefix);
                    object cnt;

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("noteid"), note.Id));
                        cnt = cmd.ExecuteScalar();
                    }

                    if (int.Parse(cnt.ToString()) > 0)
                        sqlQuery = string.Format("update {0}QuickNotes set Note = {1}note, updated = {1}updated where NoteId = {1}noteid", this.tablePrefix, this.parmPrefix);
                    else
                        sqlQuery = string.Format("insert into {0}QuickNotes (NoteId, BlogId, UserName, Note, Updated) values ({1}noteid, {1}blogid, {1}username, {1}note, {1}updated)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("noteid"), note.Id));
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("username"), note.Author));
                        p.Add(conn.CreateParameter(FormatParamName("note"), note.Note));
                        p.Add(conn.CreateParameter(FormatParamName("updated"), DateTime.Now.AddHours(-BlogSettings.Instance.Timezone)));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Save quick setting
        /// </summary>
        /// <param name="setting">Quick setting</param>
        public override void SaveQuickSetting(QuickSetting setting)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("select count(*) from {0}QuickSettings where BlogId = {1}blogid and UserName = {1}username and SettingName = {1}settingname", this.tablePrefix, this.parmPrefix);
                    object cnt;

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("settingname"), setting.SettingName));
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("username"), setting.Author));
                        cnt = cmd.ExecuteScalar();
                    }

                    if (int.Parse(cnt.ToString()) > 0)
                        sqlQuery = string.Format("update {0}QuickSettings set SettingValue = {1}settingvalue where BlogId = {1}blogid and UserName = {1}username and SettingName = {1}settingname", this.tablePrefix, this.parmPrefix);
                    else
                        sqlQuery = string.Format("insert into {0}QuickSettings (BlogId, UserName, SettingName, SettingValue) values ({1}blogid, {1}username, {1}settingname, {1}settingvalue)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("username"), setting.Author));
                        p.Add(conn.CreateParameter(FormatParamName("settingname"), setting.SettingName));
                        p.Add(conn.CreateParameter(FormatParamName("settingvalue"), setting.SettingValue));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Fill quick notes
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user notes</returns>
        public override List<QuickNote> FillQuickNotes(string userId)
        {
            var notes = new List<QuickNote>();
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT NoteId, Note, Updated FROM {0}QuickNotes where UserName = '{1}' and BlogId = '{2}' order by Updated desc", tablePrefix, userId, Blog.CurrentInstance.Id.ToString())))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var n = new QuickNote()
                                {
                                    Id = rdr.GetGuid(0),
                                    Author = userId,
                                    Note = rdr.GetString(1),
                                    Updated = rdr.GetDateTime(2)
                                };
                                notes.Add(n);
                            }
                        }
                    }
                }
            }
            return notes;
        }
        /// <summary>
        /// Fill quick settings
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user settings</returns>
        public override List<QuickSetting> FillQuickSettings(string userId)
        {
            var settings = new List<QuickSetting>();
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT SettingName, SettingValue FROM {0}QuickSettings where UserName = '{1}' and BlogId = '{2}'", tablePrefix, userId, Blog.CurrentInstance.Id.ToString())))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var s = new QuickSetting()
                                {
                                    SettingName = rdr.GetString(0),
                                    SettingValue = rdr.GetString(1)
                                };
                                settings.Add(s);
                            }
                        }
                    }
                }
            }
            return settings;
        }
        /// <summary>
        /// Delete quick note
        /// </summary>
        /// <param name="noteId">Note ID</param>
        public override void DeleteQuickNote(Guid noteId)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("delete from {0}QuickNotes where NoteId = {1}noteid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("noteid"), noteId.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion

        #region Custom fields

        /// <summary>
        /// Saves custom field
        /// </summary>
        /// <param name="field">Object custom field</param>
        public override void SaveCustomField(CustomField field)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    string conPrv = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BlogEngine"].ProviderName;
                    var q1 = "select count(*) from {0}CustomFields where CustomType = {1}customtype and BlogId = {1}blogid and ObjectId = {1}objectid and [Key] = {1}key";
                    var q2 = "insert into {0}CustomFields (CustomType, BlogId, ObjectId, [Key], [Value], [Attribute]) values ({1}customtype, {1}blogid, {1}objectid, {1}key, {1}value, {1}attribute)";
                    var q3 = "update {0}CustomFields set Value = {1}value, Attribute = {1}attribute where CustomType = {1}customtype and BlogId = {1}blogid and ObjectId = {1}objectid and [Key] = {1}key";
                    if (conPrv == "MySql.Data.MySqlClient")
                    {
                        q1 = "select count(*) from {0}CustomFields where CustomType = {1}customtype and BlogId = {1}blogid and ObjectId = {1}objectid and `Key` = {1}key";
                        q2 = "insert into {0}CustomFields (CustomType, BlogId, ObjectId, `Key`, `Value`, `Attribute`) values ({1}customtype, {1}blogid, {1}objectid, {1}key, {1}value, {1}attribute)";
                        q3 = "update {0}CustomFields set Value = {1}value, Attribute = {1}attribute where CustomType = {1}customtype and BlogId = {1}blogid and ObjectId = {1}objectid and `Key` = {1}key";
                    }

                    var sqlQuery = string.Format(q1, this.tablePrefix, this.parmPrefix);
                    object cnt;

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("customtype"), field.CustomType));
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), field.BlogId));
                        p.Add(conn.CreateParameter(FormatParamName("objectid"), field.ObjectId));
                        p.Add(conn.CreateParameter(FormatParamName("key"), field.Key));
                        cnt = cmd.ExecuteScalar();
                    }

                    if (int.Parse(cnt.ToString()) > 0)
                        sqlQuery = string.Format(q3, this.tablePrefix, this.parmPrefix);
                    else
                        sqlQuery = string.Format(q2, this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("customtype"), field.CustomType));
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("objectid"), field.ObjectId));
                        p.Add(conn.CreateParameter(FormatParamName("key"), field.Key));
                        p.Add(conn.CreateParameter(FormatParamName("value"), field.Value));
                        p.Add(conn.CreateParameter(FormatParamName("attribute"), (field.Attribute != null ? field.Attribute : string.Empty)));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Fills list of custom fields for a blog
        /// </summary>
        /// <returns>List of custom fields</returns>
        public override List<CustomField> FillCustomFields()
        {
            var items = new List<CustomField>();
            string conPrv = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BlogEngine"].ProviderName;
            var q = "SELECT CustomType, BlogId, ObjectId, [Key], [Value], [Attribute] FROM {0}CustomFields where BlogId = '{1}'";
            if (conPrv == "MySql.Data.MySqlClient")
            {
                q = "SELECT CustomType, BlogId, ObjectId, `Key`, `Value`, `Attribute` FROM {0}CustomFields where BlogId = '{1}'";
            }
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format(q, tablePrefix, Blog.CurrentInstance.Id.ToString())))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var n = new CustomField()
                                {
                                    CustomType = rdr.GetString(0),
                                    BlogId = rdr.GetGuid(1),
                                    ObjectId = rdr.GetString(2),
                                    Key = rdr.GetString(3),
                                    Value = rdr.GetString(4),
                                    Attribute = rdr.GetString(5)
                                };
                                items.Add(n);
                            }
                        }
                    }
                }
            }
            return items;
        }
        /// <summary>
        /// Deletes custom field
        /// </summary>
        /// <param name="field">Object field</param>
        public override void DeleteCustomField(CustomField field)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    string conPrv = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BlogEngine"].ProviderName;
                    var sqlQuery = "delete from {0}CustomFields where CustomType = {1}customtype and BlogId = {1}blogid and ObjectId = {1}objectid and [Key] = {1}key";
                    if (conPrv == "MySql.Data.MySqlClient")
                    {
                        sqlQuery = "delete from {0}CustomFields where CustomType = {1}customtype and BlogId = {1}blogid and ObjectId = {1}objectid and `Key` = {1}key";
                    }
                    using (var cmd = conn.CreateTextCommand(string.Format(sqlQuery, this.tablePrefix, this.parmPrefix)))
                    {
                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("customtype"), field.CustomType));
                        p.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("objectid"), field.ObjectId));
                        p.Add(conn.CreateParameter(FormatParamName("key"), field.Key));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns a formatted parameter name to include this DbBlogProvider instance's paramPrefix.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private string FormatParamName(string parameterName)
        {
            return String.Format("{0}{1}", this.parmPrefix, parameterName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The update categories.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        private void UpdateCategories(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {
                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));

                cmd.ExecuteNonQuery();

                foreach (var cat in post.Categories)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0}PostCategory (BlogID, PostID, CategoryID) VALUES ({1}blogid, {1}id, {1}cat)", this.tablePrefix, this.parmPrefix);
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("cat"), cat.Id.ToString()));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The update comments.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        private void UpdateComments(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostComment WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {

                var parms = cmd.Parameters;

                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));

                cmd.ExecuteNonQuery();

                foreach (var comment in post.AllComments)
                {
                    sqlQuery = string.Format("INSERT INTO {0}PostComment (BlogID, PostCommentID, ParentCommentID, PostID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ModeratedBy, Avatar, IsSpam, IsDeleted) VALUES ({1}blogid, {1}postcommentid, {1}parentid, {1}id, {1}date, {1}author, {1}email, {1}website, {1}comment, {1}country, {1}ip, {1}isapproved, {1}moderatedby, {1}avatar, {1}isspam, {1}isdeleted)", this.tablePrefix, this.parmPrefix);

                    cmd.CommandText = sqlQuery;
                    parms.Clear();

                    parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("postcommentid"), comment.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("parentid"), comment.ParentId.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("date"), comment.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                    parms.Add(conn.CreateParameter(FormatParamName("author"), comment.Author));
                    parms.Add(conn.CreateParameter(FormatParamName("email"), comment.Email));
                    parms.Add(conn.CreateParameter(FormatParamName("website"), (comment.Website == null ? string.Empty : comment.Website.ToString())));
                    parms.Add(conn.CreateParameter(FormatParamName("comment"), comment.Content));
                    parms.Add(conn.CreateParameter(FormatParamName("country"), (comment.Country ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("ip"), (comment.IP ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("isapproved"), comment.IsApproved));
                    parms.Add(conn.CreateParameter(FormatParamName("moderatedby"), (comment.ModeratedBy ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("avatar"), (comment.Avatar ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("isspam"), comment.IsSpam));
                    parms.Add(conn.CreateParameter(FormatParamName("isdeleted"), comment.IsDeleted));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The update notify.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        private void UpdateNotify(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostNotify WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {
                var parms = cmd.Parameters;

                parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));

                cmd.ExecuteNonQuery();

                foreach (var email in post.NotificationEmails)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0}PostNotify (BlogID, PostID, NotifyAddress) VALUES ({1}blogid, {1}id, {1}notify)", this.tablePrefix, this.parmPrefix);
                    parms.Clear();

                    parms.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("notify"), email));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The update tags.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection
        /// </param>
        private void UpdateTags(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostTag WHERE BlogID = {1}blogid AND PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {
                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                cmd.ExecuteNonQuery();

                foreach (var tag in post.Tags)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0}PostTag (BlogID, PostID, Tag) VALUES ({1}blogid, {1}id, {1}tag)", this.tablePrefix, this.parmPrefix);
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("blogid"), Blog.CurrentInstance.Id.ToString()));
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("tag"), tag));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The add blog roll parameters to command.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog roll item.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        private void AddBlogRollParametersToCommand(
            BlogRollItem blogRollItem, DbConnectionHelper conn, DbCommand cmd)
        {

            var parms = cmd.Parameters;
            parms.Add(conn.CreateParameter(FormatParamName("BlogId"), Blog.CurrentInstance.Id.ToString()));
            parms.Add(conn.CreateParameter(FormatParamName("BlogRollId"), blogRollItem.Id.ToString()));
            parms.Add(conn.CreateParameter(FormatParamName("Title"), blogRollItem.Title));
            parms.Add(conn.CreateParameter(FormatParamName("Description"), blogRollItem.Description));
            parms.Add(conn.CreateParameter(FormatParamName("BlogUrl"), (blogRollItem.BlogUrl != null ? (object)blogRollItem.BlogUrl.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("FeedUrl"), (blogRollItem.FeedUrl != null ? (object)blogRollItem.FeedUrl.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("Xfn"), blogRollItem.Xfn));
            parms.Add(conn.CreateParameter(FormatParamName("SortIndex"), blogRollItem.SortIndex));
        }

        /// <summary>
        /// Adds blog parameters to command.
        /// </summary>
        /// <param name="blog">
        /// The blog.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        private void AddBlogParametersToCommand(
            Blog blog, DbConnectionHelper conn, DbCommand cmd)
        {
            var parms = cmd.Parameters;
            parms.Add(conn.CreateParameter(FormatParamName("BlogId"), blog.Id.ToString()));
            parms.Add(conn.CreateParameter(FormatParamName("BlogName"), blog.Name ?? string.Empty));
            parms.Add(conn.CreateParameter(FormatParamName("Hostname"), blog.Hostname ?? string.Empty));
            parms.Add(conn.CreateParameter(FormatParamName("IsAnyTextBeforeHostnameAccepted"), blog.IsAnyTextBeforeHostnameAccepted));
            parms.Add(conn.CreateParameter(FormatParamName("StorageContainerName"), blog.StorageContainerName));
            parms.Add(conn.CreateParameter(FormatParamName("VirtualPath"), blog.VirtualPath ?? string.Empty));
            parms.Add(conn.CreateParameter(FormatParamName("IsPrimary"), blog.IsPrimary));
            parms.Add(conn.CreateParameter(FormatParamName("IsActive"), blog.IsActive));
            parms.Add(conn.CreateParameter(FormatParamName("IsSiteAggregation"), blog.IsSiteAggregation));
        }

        /// <summary>
        /// The add referrers parameters to command.
        /// </summary>
        /// <param name="referrer">
        /// The referrer.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        private void AddReferrersParametersToCommand(Referrer referrer, DbConnectionHelper conn, DbCommand cmd)
        {
            var parms = cmd.Parameters;

            parms.Add(conn.CreateParameter("BlogId", Blog.CurrentInstance.Id.ToString()));
            parms.Add(conn.CreateParameter("ReferrerId", referrer.Id.ToString()));
            parms.Add(conn.CreateParameter(FormatParamName("ReferralDay"), referrer.Day));
            parms.Add(conn.CreateParameter(FormatParamName("ReferrerUrl"), (referrer.ReferrerUrl != null ? (object)referrer.ReferrerUrl.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("ReferralCount"), referrer.Count));
            parms.Add(conn.CreateParameter(FormatParamName("Url"), (referrer.Url != null ? (object)referrer.Url.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("IsSpam"), referrer.PossibleSpam));
        }

        /// <summary>
        /// The delete old referrers.
        /// </summary>
        private void DeleteOldReferrers()
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Referrers WHERE BlogId = {1}BlogId AND ReferralDay < {1}ReferralDay", this.tablePrefix, this.parmPrefix)))
                    {
                        var cutoff = DateTime.Today.AddDays(-BlogSettings.Instance.NumberOfReferrerDays);

                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogId"), Blog.CurrentInstance.Id.ToString()));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("ReferralDay"), cutoff));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Creates a new DbConnectionHelper for this DbBlogProvider instance.
        /// </summary>
        /// <returns></returns>
        private DbConnectionHelper CreateConnection()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[this.connStringName];
            return new DbConnectionHelper(settings);
        }

    }
}