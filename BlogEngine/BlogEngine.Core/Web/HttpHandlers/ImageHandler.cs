namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.IO;
    using System.Web;
    using BlogEngine.Core.Providers;

    /// <summary>
    /// The ImageHanlder serves all images that is uploaded from
    ///     the admin pages.
    /// </summary>
    /// <remarks>
    /// By using a HttpHandler to serve images, it is very easy
    ///     to add the capability to stop bandwidth leeching or
    ///     to create a statistics analysis feature upon it.
    /// </remarks>
    public class ImageHandler : IHttpHandler
    {
        #region Events

        /// <summary>
        ///     Occurs when the requested file does not exist.
        /// </summary>
        public static event EventHandler<EventArgs> BadRequest;

        /// <summary>
        ///     Occurs when a file is served.
        /// </summary>
        public static event EventHandler<EventArgs> Served;

        /// <summary>
        ///     Occurs before the requested image is served.
        /// </summary>
        public static event EventHandler<EventArgs> Serving;

        #endregion

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

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that 
        ///     implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object 
        ///     that provides references to the intrinsic server objects 
        ///     (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString["picture"]))
            {
                var fileName = context.Request.QueryString["picture"];
                OnServing(fileName);
                try
                {
                    fileName = !fileName.StartsWith("/") ? string.Format("/{0}", fileName) : fileName;

                    // prevent directory traversal
                    if (fileName.Contains(".."))
                    {
                        OnBadRequest(fileName);
                        context.Response.Redirect(string.Format("{0}error404.aspx", Utils.AbsoluteWebRoot));
                    }

                    var file = BlogService.GetFile(string.Format("{0}files{1}", Blog.CurrentInstance.StorageLocation, fileName));
                    if (file != null)
                    {
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
                        
                        if (Utils.SetConditionalGetHeaders(file.DateCreated.ToUniversalTime()))
                            return;
                        
                        var index = fileName.LastIndexOf(".") + 1;
                        var extension = fileName.Substring(index).ToUpperInvariant();
                        context.Response.ContentType = string.Compare(extension, "JPG") == 0 ? "image/jpeg" : string.Format("image/{0}", extension);
                        context.Response.BinaryWrite(file.FileContents);

                        OnServed(fileName);
                    }
                    else
                    {
                        OnBadRequest(fileName);
                        context.Response.Redirect(string.Format("{0}error404.aspx", Utils.AbsoluteWebRoot));
                    }
                }
                catch (Exception ex)
                {
                    OnBadRequest(ex.Message);
                    context.Response.Redirect(string.Format("{0}error404.aspx", Utils.AbsoluteWebRoot));
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Called when [bad request].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnBadRequest(string file)
        {
            if (BadRequest != null)
            {
                BadRequest(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [served].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnServed(string file)
        {
            if (Served != null)
            {
                Served(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [serving].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnServing(string file)
        {
            if (Serving != null)
            {
                Serving(file, EventArgs.Empty);
            }
        }

        #endregion
    }
}