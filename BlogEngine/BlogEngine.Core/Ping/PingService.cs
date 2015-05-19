namespace BlogEngine.Core.Ping
{
    using System;
    using System.Net;
    using System.Text;
    using System.Xml;

    using BlogEngine.Core.Providers;

    /// <summary>
    /// Pings various blog ping services.
    ///     <remarks>
    /// Whenever a post is created or updated, it is important
    ///         to notify the ping services so that they have the latest
    ///         updated posts from the blog.
    ///     </remarks>
    /// </summary>
    public static class PingService
    {
        #region Public Methods

        /// <summary>
        /// Sends a ping to various ping services asynchronously.
        /// </summary>
        /// <param name="url">
        /// The url Uri.
        /// </param>
        public static void Send(Uri url)
        {
            var services = BlogService.LoadPingServices();
            foreach (var service in services)
            {
                Execute(service, url);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the XML to web request. The XML is the standard
        ///     XML used by RPC-XML requests.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="url">
        /// The url Uri.
        /// </param>
        private static void AddXmlToRequest(HttpWebRequest request, Uri url)
        {
            var stream = request.GetRequestStream();
            using (var writer = new XmlTextWriter(stream, Encoding.ASCII))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("methodCall");
                writer.WriteElementString("methodName", "weblogUpdates.ping");
                writer.WriteStartElement("params");
                writer.WriteStartElement("param");
                writer.WriteElementString("value", BlogSettings.Instance.Name);
                writer.WriteEndElement();
                writer.WriteStartElement("param");
                writer.WriteElementString("value", url.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Creates a web request and invokes the response asynchronous.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="url">
        /// The url Uri.
        /// </param>
        private static void Execute(string service, Uri url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(service);
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;

                AddXmlToRequest(request, url);
                request.GetResponse();
            }
            catch (Exception)
            {
                // Log the error.
            }
        }

        #endregion
    }
}