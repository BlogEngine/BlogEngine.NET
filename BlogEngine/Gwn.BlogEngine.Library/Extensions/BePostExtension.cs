using System;
using BlogEngine.Core;

namespace Gwn.BlogEngine.Library.Extensions
{
    /// <summary>
    /// Post extension
    /// </summary>
    public static class BePostExtension
    {
        /// <summary>
        /// Gets the id link.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns></returns>
        public static Tuple<string, string> GetIdLink(this Post post)
        {
            return new Tuple<string, string>(post.Id.ToString(), post.PermaLink.ToString());
        }

        
    }
}
