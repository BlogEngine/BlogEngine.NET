// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The AdminMasterPage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System;
    using System.Threading;
    using System.Web.UI;

    using BlogEngine.Core;

    /// <summary>
    /// The AdminMasterPage.
    /// </summary>
    public partial class AdminMasterPage : MasterPage
    {
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
            this.AdminStatus.Attributes.Clear();
            this.AdminStatus.Attributes.Add("class", status);
            this.AdminStatus.InnerHtml =
                string.Format(
                    "{0}<a href=\"javascript:HideStatus()\" style=\"width:20px;float:right\">X</a>", 
                    this.Server.HtmlEncode(msg));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            if (!Security.IsAuthenticated)
            {
                Security.RedirectForUnauthorizedRequest();
                return;
            }

            Page.Header.DataBind();
            base.OnInit(e);
        }

        #endregion
    }
}