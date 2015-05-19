namespace Account
{
    using System;
    using System.Web.UI;

    using BlogEngine.Core;
    using System.Web.UI.HtmlControls;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The account_ account.
    /// </summary>
    public partial class AccountMasterPage : MasterPage
    {
        /// <summary>
        /// Fetches the localized strings from resource files and generates
        /// the javascript helper string for utilization in .js files.
        /// </summary>
        /// <param name="resourceKeys">Array of resource keys</param>
        public void AddLocalizedStringsToJavaScript(List<string> resourceKeys)
        {
            var script = new HtmlGenericControl("script");
            script.Attributes["type"] = "text/javascript";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("//<![CDATA[");
            builder.AppendLine("var accountResources={ ");
            string lastKey = resourceKeys[resourceKeys.Count - 1];

            foreach (string resourceKey in resourceKeys)
            {
                builder.AppendLine(string.Format("{0}: \"{1}\"{2}", resourceKey, Utils.Translate(resourceKey).Replace("\"", "\\\""), resourceKey == lastKey ? string.Empty : ","));
            }

            builder.AppendLine(" }; ");
            builder.AppendLine("//]]>");

            script.InnerHtml = builder.ToString();

            //add at top in header to prevent any javascript issue.
            // ToDo: Insert this script exactly before account.js
            Page.Header.Controls.AddAt(0, script);
        }

        #region Public Methods

        /// <summary>
        /// Sets the status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void SetStatus(string status, string msg)
        {
            AdminStatus.Attributes.Clear();
            AdminStatus.Attributes.Add("class", status);
            AdminStatus.InnerHtml =
                string.Format(
                    "{0}<a href=\"javascript:HideStatus()\" style=\"width:20px;float:right\">X</a>", 
                    Server.HtmlEncode(msg));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> resources = new List<string>() { 
                "passwordIsRequried",
                "emailIsRequired",
                "emailIsInvalid",
                "userNameIsRequired",
                "newAndConfirmPasswordMismatch",
                "confirmPasswordIsRequired",
                "oldPasswordIsRequired",
                "newPasswordIsRequired",
                "passwordAndConfirmPasswordIsMatch",
                "minPassLengthInChars"
            };

            AddLocalizedStringsToJavaScript(resources);
            //Utils.AddJavaScriptInclude(Page, string.Format("{0}Account/account.js", Utils.ApplicationRelativeWebRoot), false, false);
        }

        #endregion
    }
}