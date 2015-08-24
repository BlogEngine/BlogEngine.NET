namespace BlogEngine.Core.Ping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Manages to send out trackbacks and then pingbacks if trackbacks aren't supported by the linked site.
    /// </summary>
    public static class Manager
    {
        #region Constants and Fields

        /// <summary>
        ///     Regex used to find the trackback link on a remote web page.
        /// </summary>
        private static readonly Regex TrackbackLinkRegex = new Regex(
            "trackback:ping=\"([^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // private static readonly Regex urlsRegex = new Regex(@"\<a\s+href=""(http://.*?)"".*\>.+\<\/a\>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // private static readonly Regex urlsRegex = new Regex(@"<a[^(href)]?href=""([^""]+)""[^>]?>([^<]+)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        ///     Regex used to find all hyperlinks.
        /// </summary>
        private static readonly Regex UrlsRegex = new Regex(
            @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the trackback or pingback message.
        ///     <remarks>
        /// It will try to send a trackback message first, and if the refered web page
        ///         doesn't support trackbacks, a pingback is sent.
        ///     </remarks>
        /// </summary>
        /// <param name="item">
        /// The publishable item.
        /// </param>
        /// <param name="itemUrl">
        /// The item Url.
        /// </param>
        public static void Send(IPublishable item, Uri itemUrl)
        {
            foreach (var url in GetUrlsFromContent(item.Content))
            {
                var trackbackSent = false;

                if (BlogSettings.Instance.EnableTrackBackSend)
                {
                    // ignoreRemoteDownloadSettings should be set to true
                    // for backwards compatibilty with Utils.DownloadWebPage.
                    var remoteFile = new RemoteFile(url, true);
                    var pageContent = remoteFile.GetFileAsString(); // ReadFromWeb(url);
                    var trackbackUrl = GetTrackBackUrlFromPage(pageContent);

                    if (trackbackUrl != null)
                    {
                        var message = new TrackbackMessage(item, trackbackUrl, itemUrl);
                        trackbackSent = Trackback.Send(message);
                    }
                }

                if (!trackbackSent && BlogSettings.Instance.EnablePingBackSend)
                {
                    Pingback.Send(itemUrl, url);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Examines the web page source code to retrieve the trackback link from the RDF.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The trackback Uri</returns>
        private static Uri GetTrackBackUrlFromPage(string input)
        {
            var url = TrackbackLinkRegex.Match(input).Groups[1].ToString().Trim();
            Uri uri;

            return Uri.TryCreate(url, UriKind.Absolute, out uri) ? uri : null;
        }

        /// <summary>
        /// Gets all the URLs from the specified string.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>A list of Uri</returns>
        private static IEnumerable<Uri> GetUrlsFromContent(string content)
        {
            var urlsList = new List<Uri>();
            foreach (var url in
                UrlsRegex.Matches(content).Cast<Match>().Select(myMatch => myMatch.Groups["url"].ToString().Trim()))
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    urlsList.Add(uri);
                }
            }

            return urlsList;
        }

        #endregion
    }
}