namespace BlogEngine.Core.Web.HttpModules
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Net;
    using System.Net.Sockets;
    using Microsoft.Ajax.Utilities;
    using BlogEngine.Core.Web.Scripting;

    /// <summary>
    /// Compresses the output using standard gzip/deflate.
    /// </summary>
    public sealed class CompressionModule : IHttpModule
    {
        #region Constants and Fields

        /// <summary>
        /// The deflate string.
        /// </summary>
        private const string Deflate = "deflate";

        /// <summary>
        /// The gzip string.
        /// </summary>
        private const string Gzip = "gzip";

        #endregion

        #region Public Methods

        /// <summary>
        /// Compresses the response stream using either deflate or gzip depending on the client.
        /// </summary>
        /// <param name="context">
        /// The HTTP context to compress.
        /// </param>
        public static void CompressResponse(HttpContext context)
        {
            if (!BlogSettings.Instance.EnableHttpCompression)
            {
                WillCompressResponse = false;
                return;
            }

            if (IsEncodingAccepted(Deflate))
            {
                context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
                WillCompressResponse = true;
                SetEncoding(Deflate);
            }
            else if (IsEncodingAccepted(Gzip))
            {
                context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                WillCompressResponse = true;
                SetEncoding(Gzip);
            }
        }

        #endregion

        #region Private Methods

        private static bool WillCompressResponse
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context == null) { return false; }
                return context.Items["will-compress-resource"] != null && (bool)context.Items["will-compress-resource"];
            }
            set
            {
                HttpContext context = HttpContext.Current;
                if (context == null) { return; }
                context.Items["will-compress-resource"] = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpModule

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module 
        ///     that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        void IHttpModule.Dispose()
        {
            // Nothing to dispose; 
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpApplication"></see> 
        ///     that provides access to the methods, properties, and events common to 
        ///     all application objects within an ASP.NET application.
        /// </param>
        void IHttpModule.Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += ContextPostReleaseRequestState;
            context.Error += new EventHandler(context_Error);
        }

        void context_Error(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            Exception ex = context.Server.GetLastError();

            // If this CompressionModule will be compressing the response and an unhandled exception
            // has occurred, remove the WebResourceFilter as that will cause garbage characters to
            // be sent to the browser instead of a yellow screen of death.
            if (WillCompressResponse)
            {
                context.Response.Filter = null;
                WillCompressResponse = false;
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks the request headers to see if the specified
        ///     encoding is accepted by the client.
        /// </summary>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The is encoding accepted.
        /// </returns>
        private static bool IsEncodingAccepted(string encoding)
        {
            var context = HttpContext.Current;
            return context.Request.Headers["Accept-encoding"] != null &&
                   context.Request.Headers["Accept-encoding"].Contains(encoding);
        }

        /// <summary>
        /// Adds the specified encoding to the response headers.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        private static void SetEncoding(string encoding)
        {
            HttpContext.Current.Response.AppendHeader("Content-encoding", encoding);
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void ContextPostReleaseRequestState(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            //System.Diagnostics.Debug.WriteLine("precessing request --->" + context.Request.Path);

            // bundled javacripts and styles
            if (context.Request.Path.Contains("/Scripts/") || context.Request.Path.Contains("/Styles/") || context.Request.Path.EndsWith(".js.axd"))
            {
                if (!context.Request.Path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.EndsWith(".swf", StringComparison.OrdinalIgnoreCase))
                {
                    SetHeaders(context);
                    CompressResponse(context);
                }
            }

            // only when page is requested
            if (context.CurrentHandler is Page && context.Request["HTTP_X_MICROSOFTAJAX"] == null && context.Request.HttpMethod == "GET")
            {
                CompressResponse(context);

                if (!context.Request.Path.Contains("/admin/", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Filter = new WebResourceFilter(context.Response.Filter);
                    WillCompressResponse = true;
                }
            }
        }

        private static void SetHeaders(HttpContext context)
        {
            var response = context.Response;
            var cache = response.Cache;

            cache.VaryByHeaders["Accept-Encoding"] = true;
            cache.SetExpires(DateTime.UtcNow.AddDays(30));
            cache.SetMaxAge(new TimeSpan(365, 0, 0, 0));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            var etag = string.Format("\"{0}\"", context.Request.Path.GetHashCode());
            var incomingEtag = context.Request.Headers["If-None-Match"];

            cache.SetETag(etag);
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