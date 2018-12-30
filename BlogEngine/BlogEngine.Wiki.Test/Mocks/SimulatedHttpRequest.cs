using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.IO;

namespace BlogEngine.Wiki.Test.Mocks
{

    /// <summary>
    /// http://haacked.com/archive/2005/06/11/Simulating_HttpContext.aspx
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        string _host;

        /// <summary>
        /// Creates a new <see cref="SimulatedHttpRequest"/> instance.
        /// </summary>
        /// <param name="appVirtualDir">App virtual dir.</param>
        /// <param name="appPhysicalDir">App physical dir.</param>
        /// <param name="page">Page.</param>
        /// <param name="query">Query.</param>
        /// <param name="output">Output.</param>
        /// <param name="host">Host.</param>
        public SimulatedHttpRequest(string appVirtualDir, string appPhysicalDir, string page, string query, TextWriter output, string host)
            : base(appVirtualDir, appPhysicalDir, page, query, output)
        {
            if (host == null || host.Length == 0)
                throw new ArgumentNullException("host", "Host cannot be null nor empty.");
            _host = host;
        }



        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <returns></returns>
        public override string GetServerName()
        {
            return _host;
        }



        /// <summary>
        /// Maps the path to a filesystem path.
        /// </summary>
        /// <param name="virtualPath">Virtual path.</param>
        /// <returns></returns>
        public override string MapPath(string virtualPath)
        {
            return Path.Combine(this.GetAppPath(), virtualPath);
        }
    }
 
}
