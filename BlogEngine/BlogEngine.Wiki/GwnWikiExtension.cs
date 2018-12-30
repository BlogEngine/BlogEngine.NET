using BlogEngine.Core.Web.Controls;
using Gwn.BlogEngine.Library.Base;

namespace BlogEngine.Wiki
{
    /// <summary>
    /// Wiki Extension
    /// </summary>
    [Extension("<a target='_blank' href='http://www.Global-webnet.com/Blog'>Global Webnet Wiki</a>",
        "1.0", "<a target='_blank' href='http://www.Global-webnet.com/Blog'>BillKrat</a>")]
    public class GwnWikiExtension : BeExtensionBase
    {
        static GwnWikiExtension()
        {
            // Baseclass handles wire-up and the 
            // GwnWikiEngine will handle processing

            BeEngine = new GwnWikiEngine();
        }

    }
}
