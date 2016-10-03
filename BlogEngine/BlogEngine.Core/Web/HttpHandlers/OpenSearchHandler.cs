namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Text;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Displays the open search XML provider as
    ///     descriped at http://opensearch.a9.com/
    /// </summary>
    /// <remarks>
    /// The OpenSearch document needs to be linked to from the 
    ///     HTML head tag. This link will be added automatically.
    /// </remarks>
    public class OpenSearchHandler : IHttpHandler
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
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            var settings = new XmlWriterSettings { Indent = true };

            using (var writer = XmlWriter.Create(context.Response.OutputStream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("OpenSearchDescription", "http://a9.com/-/spec/opensearch/1.1/");
                writer.WriteAttributeString("xmlns", "http://a9.com/-/spec/opensearch/1.1/");

                writer.WriteElementString("ShortName", BlogSettings.Instance.Name);
                writer.WriteElementString("Description", BlogSettings.Instance.Description);

                // writer.WriteRaw("<Image height=\"16\" width=\"16\" type=\"image/vnd.microsoft.icon\">" + Utils.AbsoluteWebRoot.ToString() + "Content/images/blog/favicon.ico</Image>");
                writer.WriteStartElement("Image");
                writer.WriteAttributeString("height", "16");
                writer.WriteAttributeString("width", "16");
                writer.WriteAttributeString("type", "image/vnd.microsoft.icon");
                writer.WriteValue($"{Utils.AbsoluteWebRoot}Content/images/blog/favicon.ico");
                writer.WriteEndElement();

                writer.WriteStartElement("Url");
                writer.WriteAttributeString("type", "text/html");
                writer.WriteAttributeString("template", $"{Utils.AbsoluteWebRoot}search{BlogConfig.FileExtension}?q={{searchTerms}}");
                writer.WriteEndElement();

                writer.WriteStartElement("Url");
                writer.WriteAttributeString("type", "application/rss+xml");
                writer.WriteAttributeString("template", $"{Utils.AbsoluteWebRoot}syndication.axd?q={{searchTerms}}");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "text/xml";
            context.Response.Charset = "UTF-8";
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(30));
            context.Response.Cache.SetETag(Guid.NewGuid().ToString());
        }

        #endregion

        #endregion
    }
}