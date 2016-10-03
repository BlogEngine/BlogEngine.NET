namespace BlogEngine.Core.Web.HttpHandlers
{
    using System.Text;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// RSD (Really Simple Discoverability) Handler
    ///     http://cyber.law.harvard.edu/blogs/gems/tech/rsd.html
    /// </summary>
    public class RsdHandler : IHttpHandler
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
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
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            using (var rsd = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                rsd.Formatting = Formatting.Indented;
                rsd.WriteStartDocument();

                // Rsd tag
                rsd.WriteStartElement("rsd");
                rsd.WriteAttributeString("version", "1.0");

                // Service 
                rsd.WriteStartElement("service");
                rsd.WriteElementString("engineName", $"BlogEngine.NET {BlogSettings.Instance.Version()}");
                rsd.WriteElementString("engineLink", "http://dotnetblogengine.com");
                rsd.WriteElementString("homePageLink", Utils.AbsoluteWebRoot.ToString());

                // APIs
                rsd.WriteStartElement("apis");

                // MetaWeblog
                rsd.WriteStartElement("api");
                rsd.WriteAttributeString("name", "MetaWeblog");
                rsd.WriteAttributeString("preferred", "true");
                var prefix = BlogSettings.Instance.RequireSslMetaWeblogApi ? "https://" : "http://";
                rsd.WriteAttributeString(
                    "apiLink", $"{prefix}{context.Request.Url.Authority}{Utils.RelativeWebRoot}metaweblog.axd");
                rsd.WriteAttributeString("blogID", Utils.AbsoluteWebRoot.ToString());
                rsd.WriteEndElement();

                // BlogML
                rsd.WriteStartElement("api");
                rsd.WriteAttributeString("name", "BlogML");
                rsd.WriteAttributeString("preferred", "false");
                rsd.WriteAttributeString("apiLink", $"{Utils.AbsoluteWebRoot}api/BlogImporter.asmx");
                rsd.WriteAttributeString("blogID", Utils.AbsoluteWebRoot.ToString());
                rsd.WriteEndElement();

                // End APIs
                rsd.WriteEndElement();

                // End Service
                rsd.WriteEndElement();

                // End Rsd
                rsd.WriteEndElement();

                rsd.WriteEndDocument();
            }
        }

        #endregion

        #endregion
    }
}