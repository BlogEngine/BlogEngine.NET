using BlogEngine.Core.Web;

namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using HttpModules;

    /// <summary>
    /// Removes whitespace in all stylesheets added to the 
    ///     header of the HTML document in site.master.
    /// </summary>
    public class ResourceHandler : IHttpHandler
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom 
        ///     HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object that provides 
        ///     references to the intrinsic server objects 
        ///     (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var lang = request.FilePath;

            lang = lang.Replace(".res.axd", "");

            if (lang.IndexOf("/") >= 0)
                lang = lang.Substring(lang.LastIndexOf("/") + 1);

            if (string.IsNullOrEmpty(lang))
            {
                // Use the current Language if the lang query isn't set.
                lang = BlogSettings.Instance.Language;
            }

            lang = lang.ToLowerInvariant();
            var sb = new StringBuilder();
            string cacheKey;
            string script;

            if (request.FilePath.Contains("admin.res.axd"))
            {
                lang = BlogSettings.Instance.Culture;

                cacheKey = "admin.resource.axd - " + lang;
                script = (string)Blog.CurrentInstance.Cache[cacheKey];

                if (String.IsNullOrEmpty(script))
                {
                    System.Globalization.CultureInfo culture;
                    try
                    {
                        culture = new System.Globalization.CultureInfo(lang);
                    }
                    catch (Exception)
                    {
                        culture = Utils.GetDefaultCulture();
                    }

                    var jc = new BlogCulture(culture, BlogCulture.ResourceType.Admin);

                    // add SiteVars used to pass server-side values to JavaScript in admin UI
                    var sbSiteVars = new StringBuilder();

                    sbSiteVars.Append("ApplicationRelativeWebRoot: '" + Utils.ApplicationRelativeWebRoot + "',");
                    sbSiteVars.Append("RelativeWebRoot: '" + Utils.RelativeWebRoot + "',");
                    sbSiteVars.Append("AbsoluteWebRoot:  '" + Utils.AbsoluteWebRoot + "',");

                    sbSiteVars.Append("IsPrimary: '" + Blog.CurrentInstance.IsPrimary + "',");
                    sbSiteVars.Append("BlogInstanceId: '" + Blog.CurrentInstance.Id + "',");
                    sbSiteVars.Append("BlogStorageLocation: '" + Blog.CurrentInstance.StorageLocation + "',");
                    sbSiteVars.Append("BlogFilesFolder: '" + Utils.FilesFolder + "',");

                    sbSiteVars.Append("GenericPageSize:  '" + BlogConfig.GenericPageSize.ToString() + "',");
                    sbSiteVars.Append("GalleryFeedUrl:  '" + BlogConfig.GalleryFeedUrl + "',");                 
                    sbSiteVars.Append("Version: 'BlogEngine.NET " + BlogSettings.Instance.Version() + "'");

                    sb.Append("SiteVars = {" + sbSiteVars.ToString() + "}; BlogAdmin = { i18n: " + jc.ToJsonString() + "};");
                    script = sb.ToString();
                }
            }
            else
            {
                cacheKey = "resource.axd - " + lang;
                script = (string)Blog.CurrentInstance.Cache[cacheKey];
                
                if (String.IsNullOrEmpty(script))
                {
                    System.Globalization.CultureInfo culture;
                    try
                    {
                        culture = new System.Globalization.CultureInfo(lang);
                    }
                    catch (Exception)
                    {
                        culture = Utils.GetDefaultCulture();
                    }

                    var jc = new BlogCulture(culture, BlogCulture.ResourceType.Blog);

                    // Although this handler is intended to output resource strings,
                    // also outputting other non-resource variables.
                    sb.AppendFormat("webRoot: '{0}',", Utils.RelativeWebRoot);
                    sb.AppendFormat("applicationWebRoot: '{0}',", Utils.ApplicationRelativeWebRoot);
                    sb.AppendFormat("blogInstanceId: '{0}',", Blog.CurrentInstance.Id);
                    sb.AppendFormat("fileExtension: '{0}',", BlogConfig.FileExtension);
                    sb.AppendFormat("i18n: {0}", jc.ToJsonString());
                    script = "BlogEngineRes = {" + sb + "};";
                }
            }

            Blog.CurrentInstance.Cache.Insert(cacheKey, script, null, Cache.NoAbsoluteExpiration, new TimeSpan(3, 0, 0, 0));

            SetHeaders(script.GetHashCode(), context);
            context.Response.Write(script);

            if (BlogSettings.Instance.EnableHttpCompression)
            {
                CompressionModule.CompressResponse(context); // Compress(context);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the script path used to load resources on a page. 
        /// Resource script needs to use CURRENT INSTANCE relative root
        /// to support child-blog widgets, unlike other site-wide scripts
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string GetScriptPath(System.Globalization.CultureInfo cultureInfo)
        {
            return $"{Utils.RelativeWebRoot}{cultureInfo.Name.ToLowerInvariant()}.res.axd";
        }

        /// <summary>
        /// This will make the browser and server keep the output
        ///     in its cache and thereby improve performance.
        /// </summary>
        /// <param name="hash">
        /// The hash number.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void SetHeaders(int hash, HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "text/javascript";
            var cache = response.Cache;

            cache.VaryByHeaders["Accept-Encoding"] = true;
            cache.SetExpires(DateTime.UtcNow.AddDays(30));
            cache.SetMaxAge(new TimeSpan(30, 0, 0, 0));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            var etag = $"\"{hash}\"";
            var incomingEtag = context.Request.Headers["If-None-Match"];

            cache.SetETag(etag);
            cache.SetCacheability(HttpCacheability.Public);

            if (String.Compare(incomingEtag, etag) != 0)
                return;

            response.Clear();
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.SuppressContent = true;
        }

        #endregion
    }
}