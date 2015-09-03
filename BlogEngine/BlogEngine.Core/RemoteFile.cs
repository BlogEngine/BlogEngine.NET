using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security;
using System.IO;

namespace BlogEngine.Core
{
    /// <summary>
    /// Class used to download files from a website address.
    /// </summary>
    /// <remarks>
    /// 
    /// The purpose of this class is so there's one core way of downloading remote files with urls that are from
    /// outside users. There's various areas in BlogEngine where an attacker could supply an external url to the server
    /// and tie up resources.
    /// 
    /// For example, the JavascriptHandler accepts off-server addresses as a path. An attacker could, for instance, pass the url
    /// to a file that's a few gigs in size, causing the server to get out-of-memory exceptions or some other errors. An attacker
    /// could also use this same method to use one BlogEngine instance to hammer another site by, again, passing an off-server
    /// address of the victims site to the JavascriptHandler. 
    /// 
    /// RemoteFile makes use of two BlogSettings properties: AllowServerToDownloadRemoteFiles and RemoteFileDownloadTimeout.
    /// 
    /// This class will not throw an exception if the Uri supplied points to a resource local to the running BlogEngine instance.
    /// 
    /// There shouldn't be any security issues there, as the internal WebRequest instance is still calling it remotely. 
    /// Any local files that shouldn't be accessed by this won't be allowed by the remote call.
    /// 
    /// </remarks>
    internal sealed class RemoteFile
    {
        /// <summary>
        /// Creates a new RemoteFile instance that can be used to download files from another server.
        /// </summary>
        /// <param name="filePath">The url of the file to be downloaded.</param>
        /// <param name="ignoreRemoteDownloadSettings">Set to true if RemoteFile should ignore the current BlogEngine instance's remote download settings.</param>
        internal RemoteFile(Uri filePath, bool ignoreRemoteDownloadSettings)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            this.url = filePath;
            this._ignoreRemoteDownloadSettings = ignoreRemoteDownloadSettings;
            this._timeoutLength = BlogSettings.Instance.RemoteFileDownloadTimeout;
        }


        #region "Public Methods"

        /// <summary>
        /// Returns the WebResponse used to download this file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// This method is meant for outside users who need specific access to the WebResponse this class
        /// generates. They're responsible for disposing of it.
        /// 
        /// </remarks>
        public WebResponse GetWebResponse()
        {
            WebResponse response;
            try
            {
                response = GetWebRequest().GetResponse();
            }
            catch (Exception)
            {
                return null;
            }

            long contentLength = response.ContentLength;
            
            // WebResponse.ContentLength doesn't always know the value, it returns -1 in this case.
            if (contentLength == -1) {


                // Response headers may still have the Content-Length inside of it.
                string headerContentLength = response.Headers["Content-Length"];

                if (!String.IsNullOrEmpty(headerContentLength))
                {
                    contentLength = long.Parse(headerContentLength);
                }

            }

            // -1 means an unknown ContentLength was found
            // Numbers any lower than that will always indicate that someone tampered with
            // the Content-Length header.
            if (contentLength <= -1)
            {
                response.Close();
                Utils.Log("An attempt to download a remote file has been halted due to unknown content length: {0}", Uri);
                return null;
            }
            else if ((BlogSettings.Instance.RemoteMaxFileSize > 0) && (contentLength > BlogSettings.Instance.RemoteMaxFileSize))
            {
                response.Close();
                Utils.Log("An attempt to download a remote file has been halted because the file is larger than allowed.");
                return null;
            }
           
          
            return response;
        }

        private WebRequest _webRequest;

        /// <summary>
        /// Returns the remote file as a string.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This returns the resulting stream as a string as passed through a StreamReader.
        /// </remarks>
        public string GetFileAsString()
        {
            using (var response = this.GetWebResponse())
            {
                if (response == null)
                    return string.Empty;

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion

        #region "Private Methods"

        private void CheckCanDownload()
        {
            if (!this.IgnoreRemoteDownloadSettings && !BlogSettings.Instance.AllowServerToDownloadRemoteFiles)
            {
                if (!this.UriPointsToLocalResource)
                {
                    throw new SecurityException("BlogEngine is not configured to allow remote file downloads.");
                }
            }
        }

        /// <summary>
        /// Creates the WebRequest object used internally for this RemoteFile instance.
        /// </summary>
        /// <returns>
        /// 
        /// The WebRequest should not be passed outside of this instance, as it will allow tampering. Anyone
        /// that needs more fine control over the downloading process should probably be using the WebRequest
        /// class on its own.
        /// 
        /// </returns>
        private WebRequest GetWebRequest()
        {
            this.CheckCanDownload();

            if (this._webRequest == null)
            {
                var request = (HttpWebRequest)WebRequest.Create(this.Uri);
                request.Headers["Accept-Encoding"] = "gzip";
                request.Headers["Accept-Language"] = "en-us";
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.AutomaticDecompression = DecompressionMethods.GZip;

                if (this.TimeoutLength > 0)
                {
                    request.Timeout = this.TimeoutLength;
                }
                this._webRequest = request;
            }

            return this._webRequest;

        }

      
         #endregion


        #region "Properties"

        #region "IgnoreRemoteDownloadSettings"

        private readonly bool _ignoreRemoteDownloadSettings;

        /// <summary>
        /// Gets whether this RemoteFile instance is ignoring remote download rules set in the current BlogSettings instance.
        /// </summary>
        /// <remarks>
        /// 
        /// This should only be set to true if the supplied url is a verified resource. Use at your own risk.
        /// 
        /// </remarks>
        public bool IgnoreRemoteDownloadSettings
        {
            get
            {
                return this._ignoreRemoteDownloadSettings;
            }
        }


        #endregion

        /// <summary>
        /// Gets whether the Uri property is pointed at a resource local to the running BlogEngine instance.
        /// </summary>
        /// <remarks>
        /// This property is to determine whether the remote path supplied is pointing to a local file instance.
        /// This check is required because when a user has CompressWebResource set to true, it sends js.axd
        /// the full site path as its query parameter.
        /// </remarks>
        public bool UriPointsToLocalResource
        {
            get
            {
                return (this.Uri.AbsoluteUri.StartsWith(Utils.AbsoluteWebRoot.AbsoluteUri));
            }
        }

        #region "Uri"
        private readonly Uri url;

        /// <summary>
        /// Gets the Uri of the remote file being downloaded.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.url;
            }
        }

        #endregion

        #region "TimeoutLength"

        private int _timeoutLength;

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, that a remote file download attempt can last before timing out.
        /// </summary>
        /// <remarks>
        /// This value can only be set if the instance is supposed to ignore the remote download settings set
        /// in the current BlogSettings instance. 
        /// 
        /// Set this value to 0 if there should be no timeout.
        /// 
        /// </remarks>
        public int TimeoutLength
        {
            get
            {
                return (this.IgnoreRemoteDownloadSettings ? this._timeoutLength : BlogSettings.Instance.RemoteFileDownloadTimeout);
            }
            set
            {
                if (!this.IgnoreRemoteDownloadSettings)
                {
                    throw new SecurityException("TimeoutLength can not be adjusted on RemoteFiles that are abiding by remote download rules");
                }
                else
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException("TimeoutLength must be a value greater than or equal to 0 milliseconds");
                    }
                    else
                    {
                        this._timeoutLength = value;
                    }

                }
            }
        }

        #endregion

        #endregion

    }
}
