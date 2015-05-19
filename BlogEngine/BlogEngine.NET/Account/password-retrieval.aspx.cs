namespace Account
{
    using System;
    using System.Net.Mail;
    using System.Text;
    using System.Web.Security;
    using Resources;
    using BlogEngine.Core;

    using Page = System.Web.UI.Page;

    /// <summary>
    /// The password retrieval.
    /// </summary>
    public partial class PasswordRetrieval : Page
    {
        #region Methods

        static string _confirmationCode = "";
        static string _userName = "";
        static string _email = "";
        
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!BlogSettings.Instance.EnablePasswordReset)
            {
                Response.Redirect(Utils.RelativeWebRoot + "Account/login.aspx");
            }
        }

        /// <summary>
        /// Handles the Click event of the LoginButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_userName))
            {
                var user = Membership.Provider.GetUser(this.txtUser.Text.Trim(), false);
                if (user == null)
                {
                    this.Master.SetStatus("warning", "User not found");
                    return;
                }

                _userName = txtUser.Text;
                _email = user.Email;

                if (string.IsNullOrEmpty(_email))
                {
                    this.Master.SetStatus("warning", Resources.labels.emailIsRequired);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(_confirmationCode))
            {
                if (txtUser.Text == _confirmationCode)
                {
                    this.SendMail();
                }
                else
                {
                    this.Master.SetStatus("warning", "Sorry, code does not match. Please try again.");
                }
            }
            else
            {
                SendCode();
            }
        }

        private void SendCode()
        {
            _confirmationCode = new Random().Next(1000, 9999).ToString();

            var mail = new MailMessage
            {
                From = new MailAddress(BlogSettings.Instance.Email),
                Subject = "Your code for password reset is: " + _confirmationCode
            };

            mail.To.Add(_email);

            var sb = new StringBuilder();
            sb.Append("<div style=\"font: 11px verdana, arial\">");
            sb.AppendFormat("Dear {0}:", _userName);
            sb.AppendFormat("<br/><br/>Your password reset code at \"{0}\" is: {1}", BlogSettings.Instance.Name, _confirmationCode);
            sb.Append("<br/></br>Please enter this code in the form you used to send this email and we will reset password and send it to you.");
            sb.Append(
                "<br/><br/>If it wasn't you who initiated the reset, please let us know immediately (use contact form on our site)");
            sb.AppendFormat("<br/><br/>Sincerely,<br/><br/><a href=\"{0}\">{1}</a> team.", Utils.AbsoluteWebRoot, BlogSettings.Instance.Name);
            sb.Append("</div>");

            mail.Body = sb.ToString();

            var msg = Utils.SendMailMessage(mail);

            if (string.IsNullOrEmpty(msg))
            {
                this.Master.SetStatus("success", "Confirmation code was sent, please check your email.");
            }
            else
            {
                this.Master.SetStatus("warning", msg);
                ClearCode();
            }           
        }

        private void SendMail()
        {
            var mail = new MailMessage
                {
                    From = new MailAddress(BlogSettings.Instance.Email),
                    Subject = "Your password has been reset"
                };

            mail.To.Add(_email);

            var pwd = Membership.Provider.ResetPassword(_userName, string.Empty);
            var sb = new StringBuilder();
            sb.Append("<div style=\"font: 11px verdana, arial\">");
            sb.AppendFormat("Dear {0}:", _userName);
            sb.AppendFormat("<br/><br/>Your password at \"{0}\" has been reset to: {1}", BlogSettings.Instance.Name, pwd);
            sb.Append(
                "<br/><br/>If it wasn't you who initiated the reset, please let us know immediately (use contact form on our site)");
            sb.AppendFormat("<br/><br/>Sincerely,<br/><br/><a href=\"{0}\">{1}</a> team.", Utils.AbsoluteWebRoot, BlogSettings.Instance.Name);
            sb.Append("</div>");

            mail.Body = sb.ToString();

            Utils.SendMailMessageAsync(mail);
            ClearCode();

            //this.Master.SetStatus("success", Resources.labels.passwordSent);
            Response.Redirect(Utils.RelativeWebRoot + "Account/login.aspx");
        }

        void ClearCode()
        {
            _confirmationCode = "";
            _userName = "";
            _email = "";
        }

        #endregion
    }
}