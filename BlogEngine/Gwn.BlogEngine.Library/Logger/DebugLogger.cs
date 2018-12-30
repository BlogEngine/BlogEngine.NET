using System;
using System.Diagnostics;
using Gwn.BlogEngine.Library.Interfaces;

namespace Gwn.BlogEngine.Library.Logger
{
    /// <summary>
    /// Sends logging information to debug output
    /// </summary>
    public class DebugLogger : IBeLogger
    {

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public void Log(string message, params string[] parameters)
        {
            var messageData = string.Format(message, parameters);

            Debug.WriteLine(string.Format("GWN:{0}: {1}",
                DateTime.Now.ToLongTimeString(),
                messageData ));
        }
    }
}
