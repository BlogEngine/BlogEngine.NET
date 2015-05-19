namespace BlogEngine.Core.API.MetaWeblog
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Object is the outgoing XML-RPC response.  This objects properties are set
    ///     and the Response method is called sending the response via the HttpContext Response.
    /// </summary>
    internal class XMLRPCResponse
    {
        #region Constants and Fields

        /// <summary>
        /// The method name.
        /// </summary>
        private readonly string methodName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XMLRPCResponse"/> class. 
        /// Constructor sets default value
        /// </summary>
        /// <param name="methodName">
        /// MethodName of called XML-RPC method
        /// </param>
        public XMLRPCResponse(string methodName)
        {
            this.methodName = methodName;
            this.Blogs = new List<MWABlogInfo>();
            this.Categories = new List<MWACategory>();
            this.Keywords = new List<string>();
            this.Posts = new List<MWAPost>();
            this.Pages = new List<MWAPage>();
            this.Authors = new List<MWAAuthor>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets List if author structs.  Used by wp.getAuthors.
        /// </summary>
        public List<MWAAuthor> Authors { get; set; }

        /// <summary>
        ///     Gets or sets List of blog structs.  Used by blogger.getUsersBlogs.
        /// </summary>
        public List<MWABlogInfo> Blogs { get; set; }

        /// <summary>
        ///     Gets or sets List of category structs. Used by metaWeblog.getCategories.
        /// </summary>
        public List<MWACategory> Categories { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether function call was completed and successful.  
        ///     Used by metaWeblog.editPost and blogger.deletePost.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        ///     Gets or sets Fault Struct. Used by API to return error information
        /// </summary>
        public MWAFault Fault { get; set; }

        /// <summary>
        ///     Gets or sets List of Tags.  Used by wp.getTags.
        /// </summary>
        public List<string> Keywords { get; set; }

        /// <summary>
        ///     Gets or sets MediaInfo Struct
        /// </summary>
        public MWAMediaInfo MediaInfo { get; set; }

        /// <summary>
        ///     Gets or sets MWAPage struct
        /// </summary>
        public MWAPage Page { get; set; }

        /// <summary>
        ///     Gets or sets Id of page that was just added.
        /// </summary>
        public string PageID { get; set; }

        /// <summary>
        ///     Gets or sets List of Page Structs
        /// </summary>
        public List<MWAPage> Pages { get; set; }

        /// <summary>
        ///     Gets or sets Metaweblog Post Struct. Used by metaWeblog.getPost
        /// </summary>
        public MWAPost Post { get; set; }

        /// <summary>
        ///     Gets or sets Id of post that was just added.  Used by metaWeblog.newPost
        /// </summary>
        public string PostID { get; set; }

        /// <summary>
        ///     Gets or sets List of Metaweblog Post Structs.  Used by metaWeblog.getRecentPosts
        /// </summary>
        public List<MWAPost> Posts { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Response generates the XML-RPC response and returns it to the caller.
        /// </summary>
        /// <param name="context">
        /// httpContext.Response.OutputStream is used from the context
        /// </param>
        public void Response(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            using (var data = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                data.Formatting = Formatting.Indented;
                data.WriteStartDocument();
                data.WriteStartElement("methodResponse");
                data.WriteStartElement(this.methodName == "fault" ? "fault" : "params");

                switch (this.methodName)
                {
                    case "metaWeblog.newPost":
                        this.WriteNewPost(data);
                        break;
                    case "metaWeblog.getPost":
                        this.WritePost(data);
                        break;
                    case "metaWeblog.newMediaObject":
                        this.WriteMediaInfo(data);
                        break;
                    case "metaWeblog.getCategories":
                        this.WriteGetCategories(data);
                        break;
                    case "metaWeblog.getRecentPosts":
                        this.WritePosts(data);
                        break;
                    case "blogger.getUsersBlogs":
                    case "metaWeblog.getUsersBlogs":
                        this.WriteGetUsersBlogs(data);
                        break;
                    case "metaWeblog.editPost":
                    case "blogger.deletePost":
                    case "wp.editPage":
                    case "wp.deletePage":
                        this.WriteBool(data);
                        break;
                    case "wp.newPage":
                        this.WriteNewPage(data);
                        break;
                    case "wp.getPage":
                        this.WritePage(data);
                        break;
                    case "wp.getPageList":
                        this.WriteShortPages(data);
                        break;
                    case "wp.getPages":
                        this.WritePages(data);
                        break;
                    case "wp.getAuthors":
                        this.WriteAuthors(data);
                        break;
                    case "wp.getTags":
                        this.WriteKeywords(data);
                        break;
                    case "fault":
                        this.WriteFault(data);
                        break;
                }

                data.WriteEndElement();
                data.WriteEndElement();
                data.WriteEndDocument();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert Date to format expected by MetaWeblog Response.
        /// </summary>
        /// <param name="date">
        /// DateTime to convert
        /// </param>
        /// <returns>
        /// ISO8601 date string
        /// </returns>
        private static string ConvertDatetoISO8601(DateTime date)
        {
            var temp = string.Format(
                "{0}{1}{2}T{3}:{4}:{5}",
                date.Year,
                date.Month.ToString().PadLeft(2, '0'),
                date.Day.ToString().PadLeft(2, '0'),
                date.Hour.ToString().PadLeft(2, '0'),
                date.Minute.ToString().PadLeft(2, '0'),
                date.Second.ToString().PadLeft(2, '0'));
            return temp;
        }

        /// <summary>
        /// Writes the Array of Category structs parameters of Response
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteAuthors(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var author in this.Authors)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // user id
                data.WriteStartElement("member");
                data.WriteElementString("name", "user_id");
                data.WriteElementString("value", author.user_id);
                data.WriteEndElement();

                // login
                data.WriteStartElement("member");
                data.WriteElementString("name", "user_login");
                data.WriteElementString("value", author.user_login);
                data.WriteEndElement();

                // display name 
                data.WriteStartElement("member");
                data.WriteElementString("name", "display_name");
                data.WriteElementString("value", author.display_name);
                data.WriteEndElement();

                // user email
                data.WriteStartElement("member");
                data.WriteElementString("name", "user_email");
                data.WriteElementString("value", author.user_email);
                data.WriteEndElement();

                // meta value
                data.WriteStartElement("member");
                data.WriteElementString("name", "meta_value");
                data.WriteElementString("value", author.meta_value);
                data.WriteEndElement();

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes Boolean parameter of Response
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteBool(XmlWriter data)
        {
            var postValue = "0";
            if (this.Completed)
            {
                postValue = "1";
            }

            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteElementString("boolean", postValue);
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes Fault Parameters of Response.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteFault(XmlWriter data)
        {
            data.WriteStartElement("value");
            data.WriteStartElement("struct");

            // faultCode
            data.WriteStartElement("member");
            data.WriteElementString("name", "faultCode");
            data.WriteElementString("value", this.Fault.faultCode);
            data.WriteEndElement();

            // faultString
            data.WriteStartElement("member");
            data.WriteElementString("name", "faultString");
            data.WriteElementString("value", this.Fault.faultString);
            data.WriteEndElement();

            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the Array of Category structs parameters of Response
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteGetCategories(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var category in this.Categories)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // description
                data.WriteStartElement("member");
                data.WriteElementString("name", "description");
                data.WriteElementString("value", category.description);
                data.WriteEndElement();

                // categoryid
                data.WriteStartElement("member");
                data.WriteElementString("name", "categoryid");
                data.WriteElementString("value", category.id);
                data.WriteEndElement();

                // title
                data.WriteStartElement("member");
                data.WriteElementString("name", "title");
                data.WriteElementString("value", category.title);
                data.WriteEndElement();

                // htmlUrl 
                data.WriteStartElement("member");
                data.WriteElementString("name", "htmlUrl");
                data.WriteElementString("value", category.htmlUrl);
                data.WriteEndElement();

                // rssUrl
                data.WriteStartElement("member");
                data.WriteElementString("name", "rssUrl");
                data.WriteElementString("value", category.rssUrl);
                data.WriteEndElement();

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes array of BlogInfo structs of Response
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteGetUsersBlogs(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var blog in this.Blogs)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // url
                data.WriteStartElement("member");
                data.WriteElementString("name", "url");
                data.WriteElementString("value", blog.url);
                data.WriteEndElement();

                // blogid
                data.WriteStartElement("member");
                data.WriteElementString("name", "blogid");
                data.WriteElementString("value", blog.blogID);
                data.WriteEndElement();

                // blogName
                data.WriteStartElement("member");
                data.WriteElementString("name", "blogName");
                data.WriteElementString("value", blog.blogName);
                data.WriteEndElement();

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the Array of Keyword structs parameters of Response
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteKeywords(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var keyword in this.Keywords)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // keywordName
                data.WriteStartElement("member");
                data.WriteElementString("name", "name");
                data.WriteElementString("value", keyword);
                data.WriteEndElement();

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the MediaInfo Struct of Response
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteMediaInfo(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("struct");

            // url
            data.WriteStartElement("member");
            data.WriteElementString("name", "url");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.MediaInfo.url);
            data.WriteEndElement();
            data.WriteEndElement();

            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the PageID string of Response.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteNewPage(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.PageID);
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the PostID string of Response.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteNewPost(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.PostID);
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the Metaweblog Post Struct of Response.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WritePage(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("struct");

            // pageid
            data.WriteStartElement("member");
            data.WriteElementString("name", "page_id");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Page.pageID);
            data.WriteEndElement();
            data.WriteEndElement();

            // title
            data.WriteStartElement("member");
            data.WriteElementString("name", "title");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Page.title);
            data.WriteEndElement();
            data.WriteEndElement();

            // description
            data.WriteStartElement("member");
            data.WriteElementString("name", "description");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Page.description);
            data.WriteEndElement();
            data.WriteEndElement();

            // link
            data.WriteStartElement("member");
            data.WriteElementString("name", "link");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Page.link);
            data.WriteEndElement();
            data.WriteEndElement();

            // mt_convert_breaks
            data.WriteStartElement("member");
            data.WriteElementString("name", "mt_convert_breaks");
            data.WriteStartElement("value");
            data.WriteElementString("string", "__default__");
            data.WriteEndElement();
            data.WriteEndElement();

            // dateCreated
            data.WriteStartElement("member");
            data.WriteElementString("name", "dateCreated");
            data.WriteStartElement("value");
            data.WriteElementString("dateTime.iso8601", ConvertDatetoISO8601(this.Page.pageDate));
            data.WriteEndElement();
            data.WriteEndElement();

            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// The write pages.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WritePages(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var page in this.Pages)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // pageid
                data.WriteStartElement("member");
                data.WriteElementString("name", "page_id");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.pageID);
                data.WriteEndElement();
                data.WriteEndElement();

                // title
                data.WriteStartElement("member");
                data.WriteElementString("name", "title");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.title);
                data.WriteEndElement();
                data.WriteEndElement();

                // description
                data.WriteStartElement("member");
                data.WriteElementString("name", "description");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.description);
                data.WriteEndElement();
                data.WriteEndElement();

                // link
                data.WriteStartElement("member");
                data.WriteElementString("name", "link");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.link);
                data.WriteEndElement();
                data.WriteEndElement();

                // mt_convert_breaks
                data.WriteStartElement("member");
                data.WriteElementString("name", "mt_convert_breaks");
                data.WriteStartElement("value");
                data.WriteElementString("string", "__default__");
                data.WriteEndElement();
                data.WriteEndElement();

                // dateCreated
                data.WriteStartElement("member");
                data.WriteElementString("name", "dateCreated");
                data.WriteStartElement("value");
                data.WriteElementString("dateTime.iso8601", ConvertDatetoISO8601(page.pageDate));
                data.WriteEndElement();
                data.WriteEndElement();

                // page_parent_id
                if (!string.IsNullOrEmpty(page.pageParentID))
                {
                    data.WriteStartElement("member");
                    data.WriteElementString("name", "page_parent_id");
                    data.WriteStartElement("value");
                    data.WriteElementString("string", page.pageParentID);
                    data.WriteEndElement();
                    data.WriteEndElement();
                }

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the Metaweblog Post Struct of Response.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WritePost(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("struct");

            // postid
            data.WriteStartElement("member");
            data.WriteElementString("name", "postid");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Post.postID);
            data.WriteEndElement();
            data.WriteEndElement();

            // title
            data.WriteStartElement("member");
            data.WriteElementString("name", "title");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Post.title);
            data.WriteEndElement();
            data.WriteEndElement();

            // description
            data.WriteStartElement("member");
            data.WriteElementString("name", "description");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Post.description);
            data.WriteEndElement();
            data.WriteEndElement();

            // link
            data.WriteStartElement("member");
            data.WriteElementString("name", "link");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Post.link);
            data.WriteEndElement();
            data.WriteEndElement();

            // slug
            data.WriteStartElement("member");
            data.WriteElementString("name", "wp_slug");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Post.slug);
            data.WriteEndElement();
            data.WriteEndElement();

            // excerpt
            data.WriteStartElement("member");
            data.WriteElementString("name", "mt_excerpt");
            data.WriteStartElement("value");
            data.WriteElementString("string", this.Post.excerpt);
            data.WriteEndElement();
            data.WriteEndElement();

            // comment policy
            data.WriteStartElement("member");
            data.WriteElementString("name", "mt_allow_comments");
            data.WriteStartElement("value");
            data.WriteElementString("int", this.Post.commentPolicy);
            data.WriteEndElement();
            data.WriteEndElement();

            // dateCreated
            data.WriteStartElement("member");
            data.WriteElementString("name", "dateCreated");
            data.WriteStartElement("value");
            data.WriteElementString("dateTime.iso8601", ConvertDatetoISO8601(this.Post.postDate));
            data.WriteEndElement();
            data.WriteEndElement();

            // publish
            data.WriteStartElement("member");
            data.WriteElementString("name", "publish");
            data.WriteStartElement("value");
            data.WriteElementString("boolean", this.Post.publish ? "1" : "0");

            data.WriteEndElement();
            data.WriteEndElement();

            // tags (mt_keywords)
            data.WriteStartElement("member");
            data.WriteElementString("name", "mt_keywords");
            data.WriteStartElement("value");
            var tags = new string[this.Post.tags.Count];
            for (var i = 0; i < this.Post.tags.Count; i++)
            {
                tags[i] = this.Post.tags[i];
            }

            var tagList = string.Join(",", tags);
            data.WriteElementString("string", tagList);
            data.WriteEndElement();
            data.WriteEndElement();

            // categories
            if (this.Post.categories.Count > 0)
            {
                data.WriteStartElement("member");
                data.WriteElementString("name", "categories");
                data.WriteStartElement("value");
                data.WriteStartElement("array");
                data.WriteStartElement("data");
                foreach (var cat in this.Post.categories)
                {
                    data.WriteStartElement("value");
                    data.WriteElementString("string", cat);
                    data.WriteEndElement();
                }

                data.WriteEndElement();
                data.WriteEndElement();
                data.WriteEndElement();
                data.WriteEndElement();
            }

            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// Writes the array of Metaweblog Post Structs of Response.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WritePosts(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var post in this.Posts)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // postid
                data.WriteStartElement("member");
                data.WriteElementString("name", "postid");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.postID);
                data.WriteEndElement();
                data.WriteEndElement();

                // dateCreated
                data.WriteStartElement("member");
                data.WriteElementString("name", "dateCreated");
                data.WriteStartElement("value");
                data.WriteElementString("dateTime.iso8601", ConvertDatetoISO8601(post.postDate));
                data.WriteEndElement();
                data.WriteEndElement();

                // title
                data.WriteStartElement("member");
                data.WriteElementString("name", "title");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.title);
                data.WriteEndElement();
                data.WriteEndElement();

                // description
                data.WriteStartElement("member");
                data.WriteElementString("name", "description");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.description);
                data.WriteEndElement();
                data.WriteEndElement();

                // link
                data.WriteStartElement("member");
                data.WriteElementString("name", "link");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.link);
                data.WriteEndElement();
                data.WriteEndElement();

                // slug
                data.WriteStartElement("member");
                data.WriteElementString("name", "wp_slug");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.slug);
                data.WriteEndElement();
                data.WriteEndElement();

                // excerpt
                data.WriteStartElement("member");
                data.WriteElementString("name", "mt_excerpt");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.excerpt);
                data.WriteEndElement();
                data.WriteEndElement();

                // comment policy
                data.WriteStartElement("member");
                data.WriteElementString("name", "mt_allow_comments");
                data.WriteStartElement("value");
                data.WriteElementString("string", post.commentPolicy);
                data.WriteEndElement();
                data.WriteEndElement();

                // tags (mt_keywords)
                data.WriteStartElement("member");
                data.WriteElementString("name", "mt_keywords");
                data.WriteStartElement("value");
                var tags = new string[post.tags.Count];
                for (var i = 0; i < post.tags.Count; i++)
                {
                    tags[i] = post.tags[i];
                }

                var tagList = string.Join(",", tags);
                data.WriteElementString("string", tagList);
                data.WriteEndElement();
                data.WriteEndElement();

                // publish
                data.WriteStartElement("member");
                data.WriteElementString("name", "publish");
                data.WriteStartElement("value");
                data.WriteElementString("boolean", post.publish ? "1" : "0");

                data.WriteEndElement();
                data.WriteEndElement();

                // categories
                if (post.categories.Count > 0)
                {
                    data.WriteStartElement("member");
                    data.WriteElementString("name", "categories");
                    data.WriteStartElement("value");
                    data.WriteStartElement("array");
                    data.WriteStartElement("data");
                    foreach (var cat in post.categories)
                    {
                        data.WriteStartElement("value");
                        data.WriteElementString("string", cat);
                        data.WriteEndElement();
                    }

                    data.WriteEndElement();
                    data.WriteEndElement();
                    data.WriteEndElement();
                    data.WriteEndElement();
                }

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        /// <summary>
        /// The write short pages.
        /// </summary>
        /// <param name="data">
        /// xml response
        /// </param>
        private void WriteShortPages(XmlWriter data)
        {
            data.WriteStartElement("param");
            data.WriteStartElement("value");
            data.WriteStartElement("array");
            data.WriteStartElement("data");

            foreach (var page in this.Pages)
            {
                data.WriteStartElement("value");
                data.WriteStartElement("struct");

                // pageid
                data.WriteStartElement("member");
                data.WriteElementString("name", "page_id");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.pageID);
                data.WriteEndElement();
                data.WriteEndElement();

                // title
                data.WriteStartElement("member");
                data.WriteElementString("name", "page_title");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.title);
                data.WriteEndElement();
                data.WriteEndElement();

                // page_parent_id
                data.WriteStartElement("member");
                data.WriteElementString("name", "page_parent_id");
                data.WriteStartElement("value");
                data.WriteElementString("string", page.pageParentID);
                data.WriteEndElement();
                data.WriteEndElement();

                // dateCreated
                data.WriteStartElement("member");
                data.WriteElementString("name", "dateCreated");
                data.WriteStartElement("value");
                data.WriteElementString("dateTime.iso8601", ConvertDatetoISO8601(page.pageDate));
                data.WriteEndElement();
                data.WriteEndElement();

                // dateCreated gmt
                data.WriteStartElement("member");
                data.WriteElementString("name", "date_created_gmt");
                data.WriteStartElement("value");
                data.WriteElementString("dateTime.iso8601", ConvertDatetoISO8601(page.pageDate));
                data.WriteEndElement();
                data.WriteEndElement();

                data.WriteEndElement();
                data.WriteEndElement();
            }

            // Close tags
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
            data.WriteEndElement();
        }

        #endregion
    }
}