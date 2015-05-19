namespace Account
{
    using System;
    using System.Web.Security;
    using System.Web.UI;
    using BlogEngine.Core;
    using Resources;

    /// <summary>
    /// The account change password.
    /// </summary>
    public partial class ChangePassword : System.Web.UI.Page
    {
        #region Methods

        /// <summary>
        /// Handles the Click event of the ChangePasswordPushButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ChangePasswordPushButton_Click(object sender, EventArgs e)
        {
            Master.SetStatus("warning", Resources.labels.passwordNotChanged);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnPassLength.Value = Membership.MinRequiredPasswordLength.ToString();
            ChangeUserPassword.SuccessPageUrl = Utils.RelativeWebRoot + "Account/change-password-success.aspx";
        }

        #endregion
    }
}