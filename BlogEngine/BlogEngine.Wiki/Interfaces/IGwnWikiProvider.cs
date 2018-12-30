using System;
using System.Data;
using Gwn.BlogEngine.Library.Interfaces;

namespace BlogEngine.Wiki.Interfaces
{
    public interface IGwnWikiProvider : IBeProvider
    {
        /// <summary>
        /// Gets the display template.
        /// </summary>
        /// <param name="wikiCommand">The wiki command.</param>
        /// <param name="wikiCommandParameter">The wiki command parameter.</param>
        /// <returns></returns>
        Tuple<string, DataRow, DataTable> GetDisplayTemplate(string wikiCommand, string wikiCommandParameter);

    }
}
