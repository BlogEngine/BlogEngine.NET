// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   If the visitor arrives through a search engine, this control
//   will display an in-site search result based on the same search term.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    using BlogEngine.Core;

    /// <summary>
    /// If the visitor arrives through a search engine, this control
    ///     will display an in-site search result based on the same search term.
    /// </summary>
    public class SearchOnSearch : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The rx search term.
        /// </summary>
        private static readonly Regex RxSearchTerm;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="SearchOnSearch"/> class.
        /// </summary>
        static SearchOnSearch()
        {
            // Matches the query string parameter "q" and its value.  Does not match if "q" is blank.
            RxSearchTerm = new Regex(
                "[?&]q=([^&#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the text of the headline.
        /// </summary>
        public string Headline { get; set; }

        /// <summary>
        ///     Gets or sets the maximum numbers of results to display.
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        ///     Gets or sets the text displayed below the headline.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the referrer is from a search engine.
        /// </summary>
        private bool IsSearch
        {
            get
            {
                if (this.Context.Request.UrlReferrer != null)
                {
                    var referrer = this.Context.Request.UrlReferrer.ToString().ToLowerInvariant();
                    return RxSearchTerm.IsMatch(referrer);
                }

                return false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Renders the control as a script tag.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            var html = this.Html();
            if (html != null)
            {
                writer.Write(html);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the search term from the specified referrer string.
        /// </summary>
        /// <param name="referrer">
        /// The referrer.
        /// </param>
        /// <returns>
        /// The get search term.
        /// </returns>
        private static string GetSearchTerm(string referrer)
        {
            var term = string.Empty;
            var match = RxSearchTerm.Match(referrer);

            if (match.Success)
            {
                term = match.Groups[1].Value;
            }

            return term.Replace("+", " ");
        }

        /// <summary>
        /// Checks the referrer to see if it qualifies as a search.
        /// </summary>
        /// <returns>
        /// The html string.
        /// </returns>
        private string Html()
        {
            if (this.Context.Request.UrlReferrer != null &&
                !this.Context.Request.UrlReferrer.ToString().Contains(Utils.AbsoluteWebRoot.ToString()) && this.IsSearch)
            {
                var referrer = this.Context.Request.UrlReferrer.ToString().ToLowerInvariant();
                var searchTerm = GetSearchTerm(referrer);
                var items = Search.Hits(searchTerm, false);
                return items.Count == 0 ? null : this.WriteHtml(items, searchTerm);
            }

            return null;
        }

        /// <summary>
        /// Writes the search results as HTML.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
        /// </param>
        /// <returns>
        /// The write html.
        /// </returns>
        private string WriteHtml(IList<IPublishable> items, string searchTerm)
        {
            var results = this.MaxResults < items.Count ? this.MaxResults : items.Count;
            var sb = new StringBuilder();
            sb.Append("<div id=\"searchonsearch\">");
            sb.AppendFormat(
                "<h3>{0} '{1}'</h3>", this.Headline, HttpUtility.HtmlEncode(HttpUtility.UrlDecode(searchTerm)));
            sb.AppendFormat("<p>{0}</p>", this.Text);
            sb.Append("<ol>");

            for (var i = 0; i < results; i++)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", items[i].RelativeLink, items[i].Title);
            }

            sb.Append("</ol>");
            sb.Append("</div>");

            return sb.ToString();
        }

        #endregion
    }
}