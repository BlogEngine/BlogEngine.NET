namespace App_Code.Extensions
{
    using System;
    using System.IO;
    using System.Net;

    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;

    /// <summary>
    /// StopForumSpam.com custom comment filter
    /// </summary>
    [Extension("Checks comment against stopforumspam.com spammers list.", "1.0", "BlogEngine.NET", 0, false)]
    public class StopForumSpam : ICustomFilter
    {
        #region Constants and Fields

        /// <summary>
        /// The pass through.
        /// </summary>
        private bool passThrough = true;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether comment will be passed to other
        ///     custom filters for validation
        /// </summary>
        public bool FallThrough
        {
            get
            {
                return passThrough;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICustomFilter

        /// <summary>
        /// Check if comment is spam
        /// </summary>
        /// <param name="comment">
        /// The comment
        /// </param>
        /// <returns>
        /// True if comment is spam
        /// </returns>
        public bool Check(Comment comment)
        {
            try
            {
                var url = string.Format("http://www.stopforumspam.com/api?ip={0}", comment.IP);

                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream);

                    var value = reader.ReadToEnd();
                    reader.Close();

                    var spam = value.ToLowerInvariant().Contains("<appears>yes</appears>") ? true : false;

                    // if comment IP appears in the stopforumspam list
                    // it is for sure spam; no need to pass to others.
                    passThrough = spam ? false : true;

                    return spam;
                }
            
                return false;
            }
            catch (Exception e)
            {
                Utils.Log(string.Format("Error checking stopforumspam.com: {0}", e.Message));
                return false;
            }
        }

        /// <summary>
        /// Enables or disables filter
        /// </summary>
        /// <returns>
        /// True of false
        /// </returns>
        public bool Initialize()
        {
            // do not need any initialization
            // simply return true to enable filter
            return true;
        }

        /// <summary>
        /// Report mistakes to service
        /// </summary>
        /// <param name="comment">
        /// A Comment.
        /// </param>
        public void Report(Comment comment)
        {
            // if we needed report mistakes back to
            // service, we would put code here
        }

        #endregion

        #endregion
    }
}