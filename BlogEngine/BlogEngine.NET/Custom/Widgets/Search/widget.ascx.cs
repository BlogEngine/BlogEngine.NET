// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.Search
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Linq;

    using App_Code.Controls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Properties

        static string _buttonText = "search-button-text";
        static string _fieldText = "search-field-text";

        /// <summary>
        ///     Gets a value indicating if the header is visible. This only takes effect if the widgets isn't editable.
        /// </summary>
        /// <value><c>true</c> if [display header]; otherwise, <c>false</c>.</value>
        public override bool DisplayHeader
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets wether or not the widget can be edited.
        ///     <remarks>
        ///         The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        ///     </remarks>
        /// </summary>
        /// <value></value>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets the name. It must be exactly the same as the folder that contains the widget.
        /// </summary>
        /// <value></value>
        public override string Name
        {
            get
            {
                return "Search";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            var settings = this.GetSettings();

            var buttonText = settings.ContainsKey(_buttonText) ? settings[_buttonText] : Resources.labels.search;
            var searchTxt = settings.ContainsKey(_fieldText) ? settings[_fieldText] : "Enter search term or APML url";

            var searchText = this.Context.Request.QueryString["q"] != null
                ? HttpUtility.HtmlEncode(this.Context.Request.QueryString["q"]) : searchTxt;

            var sb = new StringBuilder();
            sb.AppendLine("<div id=\"searchbox\">");
            sb.Append("<label for=\"searchfield\" style=\"display:none\">Search</label>");
            
            sb.AppendFormat(
                "<input type=\"text\" value=\"{0}\" id=\"searchfield\" onkeypress=\"if(event.keyCode==13) return BlogEngine.search('{1}')\" onfocus=\"BlogEngine.searchClear('{2}')\" onblur=\"BlogEngine.searchClear('{2}')\" />",
                searchText, Utils.RelativeWebRoot, searchText.Replace("'", "\\'"));

            sb.AppendFormat(
                "<input type=\"button\" value=\"{0}\" id=\"searchbutton\" onclick=\"BlogEngine.search('{1}');\" onkeypress=\"BlogEngine.search('{1}');\" />",
                buttonText, Utils.RelativeWebRoot);

            sb.AppendLine("</div>");

            var html = new LiteralControl(sb.ToString());

            phSearch.Controls.Add(html);
        }

        #endregion
    }
}