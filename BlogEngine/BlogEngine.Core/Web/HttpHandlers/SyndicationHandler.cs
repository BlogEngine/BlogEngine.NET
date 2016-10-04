namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Xml;

    using BlogEngine.Core.Web.HttpModules;

    /// <summary>
    /// Implements a custom handler to synchronously process HTTP Web requests for a syndication feed.
    /// </summary>
    /// <remarks>
    /// This handler can generate syndication feeds in a variety of formats and filtering 
    ///     options based on the query string parmaeters provided.
    /// </remarks>
    /// <seealso cref="IHttpHandler"/>
    /// <seealso cref="SyndicationGenerator"/>
    public class SyndicationHandler : IHttpHandler
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements 
        ///     the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object that provides references 
        ///     to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var title = RetrieveTitle(context);
            var format = RetrieveFormat(context);
            var list = GenerateItemList(context);
            list = CleanList(list);

            if (string.IsNullOrEmpty(context.Request.QueryString["post"]))
            {
                // Shorten the list to the number of posts stated in the settings, except for the comment feed.
                var max = BlogSettings.Instance.PostsPerFeed;
                
                // usually we want to restrict number of posts for subscribers to latest
                // but it can be overriden in query string to bring any number of items
                if (!string.IsNullOrEmpty(context.Request.QueryString["maxitems"]))
                {
                    int maxItems;
                    if (int.TryParse(context.Request.QueryString["maxitems"], out maxItems))
                        max = maxItems;
                }

                list = list.FindAll(item => item.IsVisible).Take(Math.Min(max, list.Count)).ToList();
            }

            SetHeaderInformation(context, list, format);

            if (BlogSettings.Instance.EnableHttpCompression)
            {
                CompressionModule.CompressResponse(context);
            }

            var generator = new SyndicationGenerator(BlogSettings.Instance, Category.Categories);
            generator.WriteFeed(format, context.Response.OutputStream, list, title);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Cleans the list.
        /// </summary>
        /// <param name="list">The list of IPublishable.</param>
        /// <returns>The cleaned list of IPublishable.</returns>
        private static List<IPublishable> CleanList(List<IPublishable> list)
        {
            return list.FindAll(item => item.IsVisible);
        }

        /// <summary>
        /// A converter delegate used for converting Results to Posts.
        /// </summary>
        /// <param name="item">
        /// The publishable item.
        /// </param>
        /// <returns>
        /// Converts to publishable interface.
        /// </returns>
        private static IPublishable ConvertToIPublishable(IPublishable item)
        {
            return item;
        }

        /// <summary>
        /// Generates the list of feed items based on the URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// A list of IPublishable.
        /// </returns>
        private static List<IPublishable> GenerateItemList(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString["category"]))
            {
                // All posts in the specified category
                if (context.Request.QueryString["category"].Length < 36)
                    return null;
                // for aggregated blog there can be multiple categories with same title
                // get the first one, then we match all posts based on category title
                var categoryId = new Guid(context.Request.QueryString["category"].Substring(0, 36));
                return Post.GetPostsByCategory(categoryId).ConvertAll(ConvertToIPublishable);
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["tag"]))
            {
                // All posts with the specified tag
                return Post.GetPostsByTag(context.Request.QueryString["tag"].Trim()).ConvertAll(ConvertToIPublishable);
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["author"]))
            {
                // All posts by the specified author
                var author = context.Request.QueryString["author"];
                return Post.GetPostsByAuthor(author).ConvertAll(ConvertToIPublishable);
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["post"]))
            {
                // All comments of the specified post
                var post = Post.GetPost(new Guid(context.Request.QueryString["post"]));
                return post.Comments.ConvertAll(ConvertToIPublishable);
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["comments"]))
            {
                // The recent comments added to any post.
                return RecentComments();
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["q"]))
            {
                // Searches posts and pages
                return Search.Hits(context.Request.QueryString["q"], false);
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["apml"]))
            {
                // Finds matches to  an APML file in both posts and pages
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Credentials = CredentialCache.DefaultNetworkCredentials;
                        client.Encoding = Encoding.Default;
                        using (var stream = client.OpenRead(context.Request.QueryString["apml"]))
                        {
                            var doc = new XmlDocument();
                            if (stream != null)
                            {
                                doc.Load(stream);
                            }

                            var list = Search.ApmlMatches(doc, 30);
                            list.Sort((i1, i2) => i2.DateCreated.CompareTo(i1.DateCreated));
                            return list;
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Response.Clear();
                    context.Response.Write(ex.Message);
                    context.Response.ContentType = "text/plain";
                    context.Response.AppendHeader("Content-Disposition", "inline; filename=\"error.txt\"");
                    context.Response.End();
                }
            }

            // The latest posts
            return Post.ApplicablePosts.ConvertAll(ConvertToIPublishable);
        }

        /// <summary>
        /// Creates a list of the most recent comments
        /// </summary>
        /// <returns>
        /// A list of IPublishable.
        /// </returns>
        private static List<IPublishable> RecentComments()
        {
            var temp = Post.ApplicablePosts.SelectMany(post => post.ApprovedComments).ToList();

            temp.Sort();
            temp.Reverse();
            var list = temp.ToList();

            return list.ConvertAll(ConvertToIPublishable);
        }

        /// <summary>
        /// Retrieves the syndication format from the urL parameters.
        /// </summary>
        /// <param name="context">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// The syndication format.
        /// </returns>
        private static SyndicationFormat RetrieveFormat(HttpContext context)
        {
            var query = context.Request.QueryString["format"];
            var format = BlogSettings.Instance.SyndicationFormat;
            if (!string.IsNullOrEmpty(query))
            {
                format = context.Request.QueryString["format"];
            }

            try
            {
                return (SyndicationFormat)Enum.Parse(typeof(SyndicationFormat), format, true);
            }
            catch (ArgumentException)
            {
                return SyndicationFormat.None;
            }
        }

        /// <summary>
        /// Retrieves the title.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The title string.</returns>
        private static string RetrieveTitle(HttpContext context)
        {
            var title = BlogSettings.Instance.Name;
            string subTitle = null;

            if (!string.IsNullOrEmpty(context.Request.QueryString["category"]))
            {
                if (context.Request.QueryString["category"].Length < 36)
                {
                    StopServing(context);
                }

                var firstCat = context.Request.QueryString["category"].Substring(0, 36);
                var categoryId = new Guid(firstCat);
                var currentCategory = Category.GetCategory(categoryId, Blog.CurrentInstance.IsSiteAggregation);
                if (currentCategory == null)
                {
                    StopServing(context);
                }

                if (currentCategory != null)
                {
                    subTitle = currentCategory.Title;
                }
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["author"]))
            {
                subTitle = context.Request.QueryString["author"];
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["post"]))
            {
                if (context.Request.QueryString["post"].Length != 36)
                {
                    StopServing(context);
                }

                var post = Post.GetPost(new Guid(context.Request.QueryString["post"]));



                if (post == null)
                {
                    StopServing(context);
                }

                if (post != null)
                {
                    var arg = new ServingEventArgs(post.Content, ServingLocation.Feed);
                    post.OnServing(arg);
                    if (arg.Cancel)
                    {
                        StopServing(context);
                    }
                    else
                    {
                        subTitle = post.Title;
                    }
                }
            }

            if (!string.IsNullOrEmpty(context.Request.QueryString["comments"]))
            {
                subTitle = "Comments";
            }

            if (subTitle != null)
            {
                return $"{title} - {subTitle}";
            }

            return title;
        }

        /// <summary>
        /// Sets the response header information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="HttpContext"/> object that provides references to the intrinsic server objects (for example, <b>Request</b>, <b>Response</b>, <b>Session</b>, and <b>Server</b>) used to service HTTP requests.
        /// </param>
        /// <param name="items">
        /// The collection of <see cref="IPublishable"/> instances used when setting the response header details.
        /// </param>
        /// <param name="format">
        /// The format of the syndication feed being generated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="context"/> is a null reference (Nothing in Visual Basic) -or- the <paramref name="items"/> is a null reference (Nothing in Visual Basic).
        /// </exception>
        private static void SetHeaderInformation(
            HttpContext context, IEnumerable<IPublishable> items, SyndicationFormat format)
        {
            var lastModified = new DateTime(1900, 1, 3); // don't use DateTime.MinValue here, as Mono doesn't like it
            foreach (var item in items)
            {
                if (BlogSettings.Instance.FromUtc(item.DateModified) > lastModified)
                {
                    lastModified = BlogSettings.Instance.FromUtc(item.DateModified);
                }
            }

            switch (format)
            {
                case SyndicationFormat.Atom:
                    context.Response.ContentType = "application/atom+xml";
                    context.Response.AppendHeader("Content-Disposition", "inline; filename=atom.xml");
                    break;

                case SyndicationFormat.Rss:
                    context.Response.ContentType = "application/rss+xml";
                    context.Response.AppendHeader("Content-Disposition", "inline; filename=rss.xml");
                    break;
            }

            if (Utils.SetConditionalGetHeaders(lastModified))
            {
                context.Response.End();
            }
        }

        /// <summary>
        /// The stop serving.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void StopServing(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = 404;
            context.Response.End();
        }

        #endregion
    }
}