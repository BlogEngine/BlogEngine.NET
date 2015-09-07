using App_Code;
using BlogEngine.Core;
using BlogEngine.Core.API.BlogML;
using BlogEngine.Core.Providers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

public class UploadController : ApiController
{
    public HttpResponseMessage Post(string action, string dirPath = "")
    {
        WebUtils.CheckRightsForAdminPostPages(false);

        HttpPostedFile file = HttpContext.Current.Request.Files[0];
        action = action.ToLowerInvariant();

        if (file != null && file.ContentLength > 0)
        {
            var dirName = string.Format("/{0}/{1}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            var fileName = new FileInfo(file.FileName).Name; // to work in IE and others

            // iOS sends all images as "image.jpg" or "image.png"
            fileName = fileName.Replace("image.jpg", DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
            fileName = fileName.Replace("image.png", DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");

            if (!string.IsNullOrEmpty(dirPath))
                dirName = dirPath;

            if (action == "filemgr" || action == "file")
            {
                string[] ImageExtensnios = { ".jpg", ".png", ".jpeg", ".tiff", ".gif", ".bmp" };

                if (ImageExtensnios.Any(x => fileName.ToLower().Contains(x.ToLower())))
                    action = "image";
                else
                    action = "file";
            }

            var dir = new BlogEngine.Core.FileSystem.Directory();
            var retUrl = "";

            if (action == "import")
            {
                if (Security.IsAdministrator)
                {
                    return ImportBlogML();
                }
            } 
            if (action == "profile")
            {
                if (Security.IsAuthorizedTo(Rights.EditOwnUser))
                {
                    // upload profile image
                    dir = BlogService.GetDirectory("/avatars");
                    var dot = fileName.LastIndexOf(".");
                    var ext = dot > 0 ? fileName.Substring(dot) : "";
                    var profileFileName = User.Identity.Name + ext;

                    var imgPath = HttpContext.Current.Server.MapPath(dir.FullPath + "/" + profileFileName);
                    var image = Image.FromStream(file.InputStream);
                    Image thumb = image.GetThumbnailImage(80, 80, () => false, IntPtr.Zero);
                    thumb.Save(imgPath);

                    return Request.CreateResponse(HttpStatusCode.Created, profileFileName);
                }
            }
            if (action == "image")
            {
                if (Security.IsAuthorizedTo(Rights.EditOwnPosts))
                {
                    dir = BlogService.GetDirectory(dirName);
                    var uploaded = BlogService.UploadFile(file.InputStream, fileName, dir, true);
                    return Request.CreateResponse(HttpStatusCode.Created, uploaded.AsImage.ImageUrl);
                }
            }
            if (action == "file")
            {
                if (Security.IsAuthorizedTo(Rights.EditOwnPosts)) 
                {
                    dir = BlogService.GetDirectory(dirName);
                    var uploaded = BlogService.UploadFile(file.InputStream, fileName, dir, true);
                    retUrl = uploaded.FileDownloadPath + "|" + fileName + " (" + BytesToString(uploaded.FileSize) + ")";
                    return Request.CreateResponse(HttpStatusCode.Created, retUrl);
                }
            }
            if (action == "video")
            {
                if (Security.IsAuthorizedTo(Rights.EditOwnPosts))
                {
                    // default media folder
                    var mediaFolder = "Custom/Media";

                    // get the mediaplayer extension and use it's folder
                    var mediaPlayerExtension = BlogEngine.Core.Web.Extensions.ExtensionManager.GetExtension("MediaElementPlayer");
                    mediaFolder = mediaPlayerExtension.Settings[0].GetSingleValue("folder");

                    var folder = Utils.ApplicationRelativeWebRoot + mediaFolder + "/";
                    //var fileName = file.FileName;

                    UploadVideo(folder, file, fileName);

                    return Request.CreateResponse(HttpStatusCode.Created, fileName);
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.BadRequest);
    }

    #region Private methods

    HttpResponseMessage ImportBlogML()
    {
        HttpPostedFile file = HttpContext.Current.Request.Files[0];
        if (file != null && file.ContentLength > 0)
        {
            var reader = new BlogReader();
            var rdr = new StreamReader(file.InputStream);
            reader.XmlData = rdr.ReadToEnd();

            if (reader.Import())
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }
        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    }

    static String BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    private void UploadVideo(string virtualFolder, HttpPostedFile file, string fileName)
    {
        var folder = HttpContext.Current.Server.MapPath(virtualFolder);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        file.SaveAs(folder + fileName);
    }

    #endregion
}