namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// A blog sitemap suitable for Google Sitemap as well as
    ///     other big search engines such as MSN/Live, Yahoo and Ask.
    /// </summary>
    public class SiteMap : IHttpHandler
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

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that 
        ///     implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> 
        ///     object that provides references to the intrinsic server objects 
        ///     (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            using (var writer = XmlWriter.Create(context.Response.OutputStream))
            {
                writer.WriteStartElement("urlset", BlogConfig.SiteMapUrlSet);

                // Posts
                foreach (var post in Post.Posts.Where(post => post.IsVisibleToPublic))
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", post.AbsoluteLink.AbsoluteUri.ToString());
                    writer.WriteElementString(
                        "lastmod", post.DateModified.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    writer.WriteElementString("changefreq", "monthly");
                    writer.WriteEndElement();
                }

                // Pages
                foreach (var page in Page.Pages.Where(page => page.IsVisibleToPublic))
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", page.AbsoluteLink.AbsoluteUri);
                    writer.WriteElementString(
                        "lastmod", page.DateModified.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    writer.WriteElementString("changefreq", "monthly");
                    writer.WriteEndElement();
                }

                // Removed for SEO reasons
                //// Archive
                // writer.WriteStartElement("url");
                // writer.WriteElementString("loc", Utils.AbsoluteWebRoot.ToString() + "archive.aspx");
                // writer.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
                // writer.WriteElementString("changefreq", "daily");
                // writer.WriteEndElement();

                // Contact
                writer.WriteStartElement("url");
                writer.WriteElementString("loc", string.Format("{0}contact.aspx", Utils.AbsoluteWebRoot));
                writer.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                writer.WriteElementString("changefreq", "monthly");
                writer.WriteEndElement();

                // Blog
                if (Page.GetFrontPage() != null)
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", string.Format("{0}blog.aspx", Utils.AbsoluteWebRoot));
                    writer.WriteElementString(
                        "lastmod", DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    writer.WriteElementString("changefreq", "daily");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            context.Response.ContentType = "text/xml";
        }

        #endregion

        #endregion
    }
}