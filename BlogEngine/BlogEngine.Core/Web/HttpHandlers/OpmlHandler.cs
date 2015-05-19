namespace BlogEngine.Core.Web.HttpHandlers
{
    using System.Text;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// The opml handler.
    /// </summary>
    public class OpmlHandler : IHttpHandler
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
            context.Response.ContentType = "text/xml";

            var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using (var writer = XmlWriter.Create(context.Response.OutputStream, writerSettings))
            {
                // open OPML
                writer.WriteStartElement("opml");

                writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("version", "1.0");

                // open BODY
                writer.WriteStartElement("body");

                foreach (var br in BlogRollItem.BlogRolls)
                {
                    // open OUTLINE
                    writer.WriteStartElement("outline");

                    if (!string.IsNullOrEmpty(br.Xfn))
                    {
                        writer.WriteAttributeString("xfn", br.Xfn);
                    }

                    writer.WriteAttributeString("title", br.Title);
                    writer.WriteAttributeString("description", br.Description);
                    writer.WriteAttributeString("xmlUrl", br.FeedUrl.ToString());
                    writer.WriteAttributeString("htmlUrl", br.BlogUrl.ToString());

                    // close OUTLINE
                    writer.WriteEndElement();
                }

                // close BODY
                writer.WriteEndElement();

                // close OPML
                writer.WriteEndElement();
            }
        }

        #endregion

        #endregion
    }
}