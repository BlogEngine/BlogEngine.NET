namespace BlogEngine.Core.Ping
{
    using System;
    using System.Net;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Sends pingbacks to website that the blog links to.
    /// </summary>
    public static class Pingback
    {
        #region Events

        /// <summary>
        ///     Occurs just before a pingback is sent.
        /// </summary>
        public static event EventHandler<EventArgs> Sending;

        /// <summary>
        ///     Occurs when a pingback has been sent
        /// </summary>
        public static event EventHandler<EventArgs> Sent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends pingbacks to the targetUrl.
        /// </summary>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <param name="targetUrl">
        /// The target Url.
        /// </param>
        public static void Send(Uri sourceUrl, Uri targetUrl)
        {
            if (!BlogSettings.Instance.EnablePingBackSend)
            {
                return;
            }

            if (sourceUrl == null || targetUrl == null)
            {
                return;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(targetUrl);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                var response = (HttpWebResponse)request.GetResponse();
                string pingUrl = null;

                var pingUrlKeyIndex = Array.FindIndex(
                    response.Headers.AllKeys, 
                    delegate(string k)
                        {
                            return k.Equals("x-pingback", StringComparison.OrdinalIgnoreCase) ||
                                   k.Equals("pingback", StringComparison.OrdinalIgnoreCase);
                        });

                if (pingUrlKeyIndex != -1)
                {
                    pingUrl = response.Headers[pingUrlKeyIndex];
                }

                Uri url;
                if (!string.IsNullOrEmpty(pingUrl) && Uri.TryCreate(pingUrl, UriKind.Absolute, out url))
                {
                    OnSending(url);
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";

                    // request.Timeout = 10000;
                    request.ContentType = "text/xml";
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.Headers["Accept-Language"] = "en-us";
                    AddXmlToRequest(sourceUrl, targetUrl, request);
                    var response2 = (HttpWebResponse)request.GetResponse();
                    response2.Close();
                    OnSent(url);
                }
            }
            catch (Exception ex)
            {
                ex = new Exception();

                // Stops unhandled exceptions that can cause the app pool to recycle
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the XML to web request. The XML is the standard
        ///     XML used by RPC-XML requests.
        /// </summary>
        /// <param name="sourceUrl">
        /// The source Url.
        /// </param>
        /// <param name="targetUrl">
        /// The target Url.
        /// </param>
        /// <param name="webreqPing">
        /// The webreq Ping.
        /// </param>
        private static void AddXmlToRequest(Uri sourceUrl, Uri targetUrl, HttpWebRequest webreqPing)
        {
            var stream = webreqPing.GetRequestStream();
            using (var writer = new XmlTextWriter(stream, Encoding.ASCII))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("methodCall");
                writer.WriteElementString("methodName", "pingback.ping");
                writer.WriteStartElement("params");

                writer.WriteStartElement("param");
                writer.WriteStartElement("value");
                writer.WriteElementString("string", sourceUrl.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("param");
                writer.WriteStartElement("value");
                writer.WriteElementString("string", targetUrl.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Called when [sending].
        /// </summary>
        /// <param name="url">The URL Uri.</param>
        private static void OnSending(Uri url)
        {
            if (Sending != null)
            {
                Sending(url, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [sent].
        /// </summary>
        /// <param name="url">The URL Uri.</param>
        private static void OnSent(Uri url)
        {
            if (Sent != null)
            {
                Sent(url, EventArgs.Empty);
            }
        }

        #endregion
    }
}