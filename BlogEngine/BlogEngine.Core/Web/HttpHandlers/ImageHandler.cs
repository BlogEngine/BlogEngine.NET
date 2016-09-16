namespace BlogEngine.Core.Web.HttpHandlers
{
    using BlogEngine.Core.Providers;
    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Web;

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

        /// <summary>
        /// md5 hash
        /// </summary>
        static MD5 md5 = MD5.Create();

        /// <summary>
        /// 
        /// </summary>
        static ConcurrentDictionary<string, object> FilesProcessed = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// file sizes
        /// </summary>
        protected enum Sizes : short
        {
            /// <summary>
            /// original (does not scale)
            /// </summary>
            original = 0,
            /// <summary>
            /// thumbnail (width: 50px)
            /// </summary>
            thumbnail = 50,
            /// <summary>
            /// small (width: 200px)
            /// </summary>
            small = 200,
            /// <summary>
            /// medium (width: 600px)
            /// </summary>
            medium = 600,
            /// <summary>
            /// medium (width: 900px)
            /// </summary>
            large = 900,
        }

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
                return true;
            }
        }

        /// <summary>
        /// Directory where scaled images will be saved
        /// </summary>
        public string DirectoryForScalingImages
        {
            get
            {
                return string.Concat(Blog.CurrentInstance.StorageLocation, "files/scaling/");
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
                var size = context.Request.QueryString["size"];

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
                        size = !string.IsNullOrWhiteSpace(size) ? size.ToLower() : string.Empty;

                        var esize = Sizes.original; Enum.TryParse<Sizes>(size, out esize);

                        if (file.IsImage && esize != Sizes.original)
                        {
                            //first: get a md5 from file fullpath
                            var bytes = from c in file.FullPath
                                        select Convert.ToByte(c);

                            var cmp = StringComparison.CurrentCultureIgnoreCase;

                            //hash from fullpath
                            var hash = md5.ComputeHash(bytes.ToArray()).ToHexString();

                            //creates the scaling directory
                            if (BlogService.DirectoryExists(this.DirectoryForScalingImages) == false) BlogService.CreateDirectory(this.DirectoryForScalingImages);

                            /*scale file*/
                            var scaleimage = BlogService
                                .GetDirectory(this.DirectoryForScalingImages)
                                .Files
                                .FirstOrDefault(f => f.Name.Equals(hash + "." + esize + file.Extension, cmp));

                            //scale image does not exist or is out dated
                            if (scaleimage == null || scaleimage.DateCreated <= file.DateCreated || scaleimage.DateModified <= file.DateModified)
                            {
                                var fileexists = ScaleImage(file, hash, esize);

                                scaleimage = fileexists ?
                                    BlogService.GetFile(string.Concat(this.DirectoryForScalingImages, hash, ".", esize, file.Extension)) :
                                    null;
                            }

                            file = scaleimage ?? file;
                        }

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

        /// <summary>
        /// It scales the image to return as thumbnails and so on (depending on the request)
        /// </summary>
        /// <returns></returns>
        protected bool ScaleImage(FileSystem.File file, string hash, Sizes size, HttpContext context = null)
        {
            if (file == null || file.IsImage == false) return false;

            var filename = string.Concat(hash, ".", size.ToString().ToLower(), file.Extension.ToLower());

            if (FilesProcessed.ContainsKey(filename) == false || FilesProcessed[filename].Equals(false))
            {
                lock (FilesProcessed)
                {
                    if (FilesProcessed.ContainsKey(filename) == false) FilesProcessed[filename] = false;
                }

                lock (FilesProcessed[filename])
                {
                    if (FilesProcessed[filename].Equals(false))
                    {
                        try
                        {
                            var realpath = (context ?? HttpContext.Current).Server.MapPath(file.FullPath);
                            var noextension = file.Name.Replace(file.Extension, string.Empty);

                            using (var image = Image.FromFile(realpath))
                            {
                                var w = (short)size;
                                var f = 1 - ((float)(image.Size.Width - (short)size) / image.Size.Width);
                                var h = (int)(image.Size.Height * f);

                                var newImage = new Bitmap(w, h);
                                using (newImage)
                                {
                                    using (var g = Graphics.FromImage(newImage))
                                    {
                                        g.DrawImage(image, 0, 0, w, h);
                                    }

                                    //scaling directory
                                    var newpath = (context ?? HttpContext.Current).Server.MapPath(this.DirectoryForScalingImages);
                                    newImage.Save(newpath + @"\" + filename);
                                    FilesProcessed[filename] = true;
                                }
                            }

                            return true;
                        }
                        catch (Exception x)
                        {
                            return false;
                        }
                    }
                }
            }

            return BlogService.FileExists(this.DirectoryForScalingImages + filename);
        }

        #endregion
    }
}