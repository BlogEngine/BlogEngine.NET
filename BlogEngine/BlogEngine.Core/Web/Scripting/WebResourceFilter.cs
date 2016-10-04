using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Net.Sockets;
using System.Collections.Generic;
using BlogEngine.Core.Data.Services;

namespace BlogEngine.Core.Web.Scripting
{
    /// <summary>
    /// The web resource filter.
    /// </summary>
    public class WebResourceFilter : Stream
    {
        #region Constants and Fields

        /// <summary>
        /// The _sink.
        /// </summary>
        private readonly Stream sink;
        string HtmlOut;

        /// <summary>
        /// Regex for parsing webresource.axd
        /// </summary>
        private static readonly Regex WebResourceRegex =
            new Regex(
                "<script\\s*src=\"((?=[^\"]*webresource.axd)[^\"]*)\"\\s*type=\"text/javascript\"[^>]*>[^<]*(?:</script>)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResourceFilter"/> class.
        /// </summary>
        /// <param name="sink">
        /// The sink stream.
        /// </param>
        public WebResourceFilter(Stream sink)
        {
            this.sink = sink;
            HtmlOut = "";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether CanRead.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanSeek.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanWrite.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets Length.
        /// </summary>
        public override long Length
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public override long Position { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Write stream to the client before closing.
        /// </summary>
        public override void Close()
        {
            if (HtmlOut.Contains("webresource.axd", StringComparison.OrdinalIgnoreCase) && BlogSettings.Instance.CompressWebResource)
            {
                Regex reg = new Regex("<script\\s*src=\"((?=[^\"]*webresource.axd)[^\"]*)\"\\s*type=\"text/javascript\"[^>]*>[^<]*(?:</script>)?", RegexOptions.IgnoreCase);
                bool found = false;
                int idx = 0;
                var resKey = "";
                var resources = new List<string>();

                foreach (Match m in reg.Matches(HtmlOut))
                {
                    if (!found) // save where we found first resource file
                        idx = HtmlOut.IndexOf(m.Value);

                    var resUrl = GetScriptPathFromJsTag(m.Value);
                    resKey += resUrl;
                    resources.Add(resUrl);
                    HtmlOut = HtmlOut.Replace(m.Value, "");
                    found = true;
                }

                if (found)
                {
                    var hashKey = resKey.GetHashCode().ToString();
                    if (Blog.CurrentInstance.Cache[hashKey] == null)
                    {
                        // add resource file to the cache
                        var resValue = "";
                        foreach (var r in resources)
                        {
                            resValue += RetrieveRemoteFile(r);
                        }
                        Blog.CurrentInstance.Cache.Insert(hashKey, resValue);
                    }

                    if (HtmlOut.Contains("</head>", StringComparison.OrdinalIgnoreCase))
                    {
                        HtmlOut = HtmlOut.Insert(idx,
                            $"\n<script src=\"{Utils.RelativeWebRoot}res-{resKey.GetHashCode()}.js.axd\" type=\"text/javascript\"></script>");
                    }
                }
            }

            // parse custom fields
            HtmlOut = CustomFieldsParser.GetPageHtml(HtmlOut);

            var outdata = Encoding.UTF8.GetBytes(HtmlOut);

            sink.Write(outdata, 0, outdata.GetLength(0));
            sink.Close();
        }

        /// <summary>
        /// Evaluates the replacement for each link match.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <returns>
        /// The evaluator.
        /// </returns>
        public string Evaluator(Match match)
        {
            var relative = match.Groups[1].Value;
            var absolute = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            return match.Value.Replace(
                relative, $"{Utils.ApplicationRelativeWebRoot}js.axd?path={HttpUtility.UrlEncode(absolute + relative)}");
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void Flush()
        {
            this.sink.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.sink.Read(buffer, offset, count);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.sink.Seek(offset, origin);
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            this.sink.SetLength(value);
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // collect all HTML in local variable
            var html = Encoding.UTF8.GetString(buffer, offset, count);
            HtmlOut += html;
        }

        static internal string GetScriptPathFromJsTag(string tag)
        {
            int start = tag.IndexOf("src=") + 5;
            int end = tag.IndexOf("\"", start);

            if (tag.Contains("js.axd?path="))
            {
                start = tag.IndexOf("path=") + 5;
            }
            else
            {
                if (tag.Contains(".js", StringComparison.OrdinalIgnoreCase))
                    end = tag.IndexOf(".js") + 3;
                else
                    end = tag.IndexOf("\"", start);
            }
            if (end < start || (end - start < 0))
                return string.Empty;

            var s = tag.Substring(start, end - start);

            return s;
        }

        private static string RetrieveRemoteFile(string file)
        {
            file = Utils.AbsoluteWebRoot.ToString() + file.Substring(1); 
            Uri url;
            if (Uri.TryCreate(file, UriKind.Absolute, out url))
            {
                try
                {
                    var script = new RemoteFile(url, false).GetFileAsString();
                    
                    if (BlogSettings.Instance.CompressWebResource)
                    {
                        var min = new JavascriptMinifier();
                        return min.Minify(script);
                    }
                    
                    return script;
                }
                catch (SocketException)
                {
                    // The remote site is currently down. Try again next time.
                }
            }
            return string.Empty;
        }

        #endregion
    }
}
