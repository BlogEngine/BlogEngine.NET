// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Includes a reference to a JavaScript.
//   <remarks>
//   This control is needed in order to let the src
//   attribute of the script tage be relative to the root.
//   </remarks>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using BlogEngine.Core;

    /// <summary>
    /// Includes a reference to a JavaScript.
    ///     <remarks>
    /// This control is needed in order to let the src
    ///         attribute of the script tage be relative to the root.
    ///     </remarks>
    /// </summary>
    public class SearchBox : Control
    {

        #region Public Methods

        /// <summary>
        /// Renders the control as a script tag.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(this.BuildHtml());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The build html.
        /// </summary>
        private string BuildHtml()
        {
            var text = this.Context.Request.QueryString["q"] != null
                           ? HttpUtility.HtmlEncode(this.Context.Request.QueryString["q"])
                           : BlogSettings.Instance.SearchDefaultText;
            var sb = new StringBuilder();
            sb.AppendLine("<div id=\"searchbox\">");
            sb.Append("<label for=\"searchfield\" style=\"display:none\">Search</label>");
            sb.AppendFormat(
                "<input type=\"text\" value=\"{0}\" id=\"searchfield\" onkeypress=\"if(event.keyCode==13) return BlogEngine.search('{1}')\" onfocus=\"BlogEngine.searchClear('{2}')\" onblur=\"BlogEngine.searchClear('{2}')\" />",
                text,
                Utils.RelativeWebRoot,
                text.Replace("'", "\\'"));
            sb.AppendFormat(
                "<input type=\"button\" value=\"{0}\" id=\"searchbutton\" onclick=\"BlogEngine.search('{1}');\" onkeypress=\"BlogEngine.search('{1}');\" />",
                BlogSettings.Instance.SearchButtonText,
                Utils.RelativeWebRoot);

            if (BlogSettings.Instance.EnableCommentSearch && BlogSettings.Instance.ShowIncludeCommentsOption)
            {
                var check = this.Context.Request.QueryString["comment"] != null ? "checked=\"checked\"" : string.Empty;
                sb.AppendFormat("<div id=\"search-include-comments\"><input type=\"checkbox\" id=\"searchcomments\" {0} />", check);
                if (!string.IsNullOrEmpty(BlogSettings.Instance.SearchCommentLabelText))
                {
                    sb.AppendFormat(
                        "<label for=\"searchcomments\">{0}</label></div>", BlogSettings.Instance.SearchCommentLabelText);
                }
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        #endregion
    }
}