using System;

namespace BlogEngine.Core
{
    /// <summary>
    /// Pager for data grids
    /// </summary>
    public static class Pager
    {
        #region Properties

        /// <summary>
        /// First page
        /// </summary>
        static int First { get; set; }
        /// <summary>
        /// Previous page
        /// </summary>
        static int Previous { get; set; }
        /// <summary>
        /// Show items on the page from
        /// </summary>
        static int From { get; set; }
        /// <summary>
        /// Show items from the page up to
        /// </summary>
        static int To { get; set; }
        /// <summary>
        /// Next page in the list
        /// </summary>
        static int Next { get; set; }
        /// <summary>
        /// Last page in the list
        /// </summary>
        static int Last { get; set; }
        /// <summary>
        /// Current page
        /// </summary>
        static int CurrentPage { get; set; }
        /// <summary>
        /// Records total
        /// </summary>
        static int Total { get; set; }

        #endregion

        /// <summary>
        /// Pager constructor
        /// </summary>
        /// <param name="page">Page #</param>
        /// <param name="pageSize">Page size (number of items per page)</param>
        /// <param name="listCount">Number of items in the list</param>
        public static void  Reset(int page, int pageSize, int listCount)
        {
            if (page < 1) page = 1;
            Total = listCount;

            var pgs = Convert.ToDecimal(listCount) / Convert.ToDecimal(pageSize);
            var p = pgs - (int)pgs;
            Last = p > 0 ? (int)pgs + 1 : (int)pgs;

            if (page > Last) page = 1;
            CurrentPage = page;

            From = ((page * pageSize) - (pageSize - 1));
            To = (page * pageSize);

            // adjust for the Last (or single) page
            if (listCount < To) To = listCount;

            // when Last item on the Last page deleted
            // this will reset "from" counter
            if (From > To) From = From - pageSize;

            if (page > 1)
            {
                Previous = page - 1;
                First = 1;
            }

            if (page < Last) Next = page + 1;
            if (page == Last) Last = 0;
        }

        /// <summary>
        /// Renders pager HMTML
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="callback">Callback for JavaScript</param>
        /// <returns>HTML markup</returns>
        public static string Render(string callback = "false")
        {
            var prvLnk = string.Empty;
            var nxtLnk = string.Empty;
            var firstLnk = string.Empty;
            var lastLnk = string.Empty;

            if (string.IsNullOrEmpty(callback))
                callback = "false";

            var linkFormat = "<a href=\"#\" id=\"{0}\" onclick=\"return " + callback + ";\" class=\"{0}\">{1}</a>";

            var pageLink = string.Format("<span>Showing {0} - {1} of {2}</span>", From, To, Total);

            if (CurrentPage > 1)
            {
                prvLnk = string.Format(linkFormat, "prevLink", Previous);
                firstLnk = string.Format(linkFormat, "firstLink", First);
            }

            if (CurrentPage < Last)
            {
                nxtLnk = string.Format(linkFormat, "nextLink", Next);
                lastLnk = string.Format(linkFormat, "lastLink", Last);
            }

            var currpage = "<span id=\"current-page\" style=\"display:none\">" + CurrentPage.ToString() + "</span>";
            return firstLnk + prvLnk + pageLink + nxtLnk + lastLnk + currpage;
        }
    }
}
