namespace BlogEngine.Core
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml;

    /// <summary>
    /// Derived XmlUrlResolver class designed to prevent security problems with
    ///     dangerous XML input, by limiting the amount of data that can be retrieved.
    /// </summary>
    public class XmlSafeResolver : XmlUrlResolver
    {
        #region Constants and Fields

        /// <summary>
        /// The buffer size. 1 KB.
        /// </summary>
        private const int BufferSize = 1024;

        /// <summary>
        /// The max response size. 1 MB.
        /// </summary>
        private const int MaxResponseSize = 1024 * 1024;

        /// <summary>
        /// The timeout. 10 seconds.
        /// </summary>
        private const int Timeout = 10000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Maps a URI to an object containing the actual resource.
        /// </summary>
        /// <param name="absoluteUri">
        /// The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)"/>
        /// </param>
        /// <param name="role">
        /// The current implementation does not use this parameter when resolving URIs. This is provided for future extensibility purposes. For example, this can be mapped to the xlink:role and used as an implementation specific argument in other scenarios.
        /// </param>
        /// <param name="typeOfObjectToReturn">
        /// The type of object to return. The current implementation only returns System.IO.Stream objects.
        /// </param>
        /// <returns>
        /// A System.IO.Stream object or null if a type other than stream is specified.
        /// </returns>
        /// <exception cref="T:System.Xml.XmlException">
        /// <paramref name="typeOfObjectToReturn"/> is neither null nor a Stream type.
        /// </exception>
        /// <exception cref="T:System.UriFormatException">
        /// The specified URI is not an absolute URI.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="absoluteUri"/> is null.
        /// </exception>
        /// <exception cref="T:System.Exception">
        /// There is a runtime error (for example, an interrupted server connection).
        /// </exception>
        public override object GetEntity(Uri absoluteUri, string role, Type typeOfObjectToReturn)
        {
            if (absoluteUri.IsLoopback)
            {
                return null;
            }

            var request = WebRequest.Create(absoluteUri);
            request.Timeout = Timeout;

            WebResponse response;
            try
            {
                response = request.GetResponse();
                if (response == null)
                {
                    Utils.Log(string.Format("Could not resolve external entity ({0})", absoluteUri));
                    return null;
                }
            }
            catch (Exception)
            {
                Utils.Log(string.Format("Could not resolve external entity ({0})", absoluteUri));
                return null;
            }
            

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new XmlException("Could not resolve external entity");
            }

            responseStream.ReadTimeout = Timeout;

            var copyStream = new MemoryStream();
            var buffer = new byte[BufferSize];
            int bytesRead;
            var totalBytesRead = 0;
            do
            {
                bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;
                if (totalBytesRead > MaxResponseSize)
                {
                    throw new XmlException("Could not resolve external entity");
                }

                copyStream.Write(buffer, 0, bytesRead);
            }
            while (bytesRead > 0);

            copyStream.Seek(0, SeekOrigin.Begin);
            return copyStream;
        }

        #endregion
    }
}