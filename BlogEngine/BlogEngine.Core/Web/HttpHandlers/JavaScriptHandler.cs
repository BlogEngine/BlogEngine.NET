namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Net;
    using System.Web;

    /// <summary>
    /// Removes whitespace in all stylesheets added to the 
    ///     header of the HTML document in site.master.
    /// </summary>
    /// <remarks>
    /// 
    /// This handler uses an external library to perform minification of scripts. 
    /// See the BlogEngine.Core.JavascriptMinifier class for more details.
    /// 
    /// </remarks>
    public class JavaScriptHandler : IHttpHandler
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
            string fileName = request.FilePath;
            fileName = fileName.Substring(Utils.RelativeWebRoot.Length);

            string resKey = "";
            string script = "";

            if (fileName.StartsWith("res-"))
            {
                resKey = fileName.Substring(4);
                resKey = resKey.Substring(0, resKey.IndexOf("."));

                if (Blog.CurrentInstance.Cache[resKey] != null)
                {
                    script = (string)Blog.CurrentInstance.Cache[resKey];
                }
            }

            SetHeaders(script.GetHashCode(), context);
            context.Response.Write(script);
        }

        #endregion

        #region Methods

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
            cache.SetExpires(DateTime.Now.ToUniversalTime().AddDays(7));
            cache.SetMaxAge(new TimeSpan(30, 0, 0, 0));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            var etag = string.Format("\"{0}\"", hash);
            var incomingEtag = context.Request.Headers["If-None-Match"];

            //cache.SetETag(etag);
            cache.SetCacheability(HttpCacheability.Public);

            if (String.Compare(incomingEtag, etag) != 0)
            {
                return;
            }

            response.Clear();
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.SuppressContent = true;
        }

        #endregion
    }
}