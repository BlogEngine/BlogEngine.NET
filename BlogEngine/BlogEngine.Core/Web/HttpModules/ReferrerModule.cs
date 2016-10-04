namespace BlogEngine.Core.Web.HttpModules
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Referrer Module
    /// </summary>
    public class ReferrerModule : IHttpModule
    {
        #region Constants and Fields

        // private static string _Folder = HttpContext.Current.Server.MapPath("~/App_Data/log/");

/*
        /// <summary>
        ///     The relative path of the XML file.
        /// </summary>
        private static string folder =
            HttpContext.Current.Server.MapPath(string.Format("{0}log/", BlogSettings.Instance.StorageLocation));
*/

/*
        /// <summary>
        ///     Used to thread safe the file operations
        /// </summary>
        private static object syncRoot = new object();
*/

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when a visitor enters the website and the referrer is logged.
        /// </summary>
        public static event EventHandler<EventArgs> ReferrerRegistered;

        #endregion

        #region Implemented Interfaces

        #region IHttpModule

        /// <summary>
        /// Disposes of the resources (other than memory) used by the 
        ///     module that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose.
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpApplication"></see> that 
        ///     provides access to the methods, properties, and events common to all application 
        ///     objects within an ASP.NET application
        /// </param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += ContextBeginRequest;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The begin register click.
        /// </summary>
        /// <param name="entry">
        /// Contains data to register.
        /// </param>
        private static void BeginRegisterClick(DictionaryEntry entry)
        {
            try
            {
                var referrer = (Uri)entry.Key;
                var url = (Uri)entry.Value;

                RegisterClick(url, referrer);
                OnReferrerRegistered(referrer);
            }
            catch (Exception)
            {
                // Could write to the file.
            }
        }

        /// <summary>
        /// Determines whether [is search engine] [the specified referrer].
        /// </summary>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        ///     <c>true</c> if [is search engine] [the specified referrer]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSearchEngine(string referrer)
        {
            var lower = referrer.ToUpperInvariant();
            return (lower.Contains("YAHOO") && lower.Contains("P=")) || (lower.Contains("?Q=") || lower.Contains("&Q="));
        }

        /// <summary>
        /// Determines whether the specified referrer is spam.
        /// </summary>
        /// <param name="referrer">
        /// The referrer Uri.
        /// </param>
        /// <param name="url">
        /// The URL Uri.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified referrer is spam; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSpam(Uri referrer, Uri url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var html = client.DownloadString(referrer).ToUpperInvariant();
                    var subdomain = Utils.GetSubDomain(url);
                    var host = url.Host.ToUpperInvariant();

                    if (subdomain != null)
                    {
                        host = host.Replace($"{subdomain.ToUpperInvariant()}.", string.Empty);
                    }

                    return !html.Contains(host);
                }
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Raises the event in a safe way
        /// </summary>
        /// <param name="referrer">
        /// The referrer.
        /// </param>
        private static void OnReferrerRegistered(Uri referrer)
        {
            if (ReferrerRegistered != null)
            {
                ReferrerRegistered(referrer, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Registers click.
        /// </summary>
        /// <param name="url">
        /// The url Uri.
        /// </param>
        /// <param name="referrer">
        /// The referrer Uri.
        /// </param>
        private static void RegisterClick(Uri url, Uri referrer)
        {
            Referrer refer = null;
            if (Referrer.Referrers != null && Referrer.Referrers.Count > 0)
            {
                refer =
                    Referrer.Referrers.Find(
                        r => r.ReferrerUrl.Equals(referrer) && r.Url.Equals(url) && r.Day == DateTime.Today);
            }

            if (refer == null)
            {
                refer = new Referrer
                    {
                        Day = DateTime.Today,
                        ReferrerUrl = referrer,
                        Url = url,
                        PossibleSpam = IsSpam(referrer, url)
                    };
            }

            refer.Count += 1;

            refer.Save();
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
        private static void ContextBeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            if (!BlogSettings.Instance.EnableReferrerTracking) { return; }

            if (!context.Request.Path.ToUpperInvariant().Contains(".ASPX"))
            {
                return;
            }

            if (context.Request.UrlReferrer == null)
            {
                return;
            }

            var referrer = context.Request.UrlReferrer;
            if (!referrer.Host.Equals(Utils.AbsoluteWebRoot.Host, StringComparison.OrdinalIgnoreCase) &&
                !IsSearchEngine(referrer.ToString()))
            {
                Guid blogId = Blog.CurrentInstance.Id;

                ThreadPool.QueueUserWorkItem(state =>
                    {
                        // because HttpContext is not available within this BG thread
                        // needed to determine the current blog instance,
                        // set override value here.
                        Blog.InstanceIdOverride = blogId;

                        BeginRegisterClick(new DictionaryEntry(referrer, context.Request.Url));
                    });
            }
        }

        #endregion
    }
}