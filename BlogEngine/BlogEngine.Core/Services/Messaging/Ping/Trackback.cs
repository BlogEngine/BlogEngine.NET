namespace BlogEngine.Core.Ping
{
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    /// The trackback.
    /// </summary>
    public static class Trackback
    {
        #region Events

        /// <summary>
        ///     Occurs just before a trackback is sent.
        /// </summary>
        public static event EventHandler<EventArgs> Sending;

        /// <summary>
        ///     Occurs when a trackback has been sent
        /// </summary>
        public static event EventHandler<EventArgs> Sent;

        #endregion

        #region Public Methods

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <returns>
        /// The send.
        /// </returns>
        public static bool Send(TrackbackMessage message)
        {
            if (!BlogSettings.Instance.EnableTrackBackSend)
            {
                return false;
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            OnSending(message.UrlToNotifyTrackback);

            // Warning:next line if for local debugging porpuse please donot remove it until you need to
            // tMessage.PostURL = new Uri("http://www.artinsoft.com/webmaster/trackback.html");
            var request = (HttpWebRequest)WebRequest.Create(message.UrlToNotifyTrackback);
                
                // HttpHelper.CreateRequest(trackBackItem);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Method = "POST";
            request.ContentLength = message.ToString().Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = false;
            request.Timeout = 10000;

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(message.ToString());
            }

            bool result;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                OnSent(message.UrlToNotifyTrackback);
                string answer;
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    answer = sr.ReadToEnd();
                }

                // TODO: This could be a strict XML parsing if necesary/maybe logging activity here too
                result = response.StatusCode == HttpStatusCode.OK && answer.Contains("<error>0</error>");
            }
            catch
            {
                // (WebException wex)
                result = false;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [sending].
        /// </summary>
        /// <param name="url">The URL.</param>
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
        /// <param name="url">The URL.</param>
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