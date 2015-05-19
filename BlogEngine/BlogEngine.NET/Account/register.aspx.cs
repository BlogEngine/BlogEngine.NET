namespace Account
{
    using System;
    using System.Web.Security;
    using System.Web.UI.WebControls;
    using System.Linq;
    using BlogEngine.Core;
    using Resources;

    using Page = System.Web.UI.Page;
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// The account register.
    /// </summary>
    public partial class Register : Page
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BlogSettings.Instance.CreateBlogOnSelfRegistration && Blog.CurrentInstance.IsPrimary)
            {
                Response.Redirect("create-blog.aspx");
            }
            HtmlAnchor HeadLoginStatus = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("HeadLoginStatus") as HtmlAnchor;
            if (HeadLoginStatus != null)
            {
                HeadLoginStatus.HRef = Utils.RelativeWebRoot + "Account/login.aspx";
            }

            this.RegisterUser.ContinueDestinationPageUrl = this.Request.QueryString["ReturnUrl"];
            this.hdnPassLength.Value = Membership.MinRequiredPasswordLength.ToString();

            // if self registration not allowed and user is trying to directly
            // navigate to register page, redirect to login
            if (!BlogSettings.Instance.EnableSelfRegistration)
            {
                Response.Redirect(Utils.RelativeWebRoot + "Account/login.aspx");
            }
        }

        /// <summary>
        /// Handles the CreatedUser event of the RegisterUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(BlogSettings.Instance.SelfRegistrationInitialRole))
            {
                string role = Roles.GetAllRoles().FirstOrDefault(r => r.Equals(BlogSettings.Instance.SelfRegistrationInitialRole, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(role))
                {
                    Roles.AddUsersToRoles(new string[] { this.RegisterUser.UserName }, new string[] { role });
                }
            }

            Security.AuthenticateUser(this.RegisterUser.UserName, this.RegisterUser.Password, false);

            FormsAuthentication.SetAuthCookie(this.RegisterUser.UserName, false /* createPersistentCookie */);

            var continueUrl = this.RegisterUser.ContinueDestinationPageUrl;
            if (String.IsNullOrEmpty(continueUrl))
            {
                continueUrl = Utils.RelativeWebRoot;
            }

            this.Response.Redirect(continueUrl);
        }

        /// <summary>
        /// Handles the CreatingUser event of the RegisterUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.LoginCancelEventArgs"/> instance containing the event data.</param>
        protected void RegisterUser_CreatingUser(object sender, LoginCancelEventArgs e)
        {
            App_Code.Controls.RecaptchaControl captcha =
                (App_Code.Controls.RecaptchaControl)RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("recaptcha") as App_Code.Controls.RecaptchaControl;

            if (Membership.GetUser(this.RegisterUser.UserName) != null)
            {
                e.Cancel = true;
                this.Master.SetStatus("warning", Resources.labels.anotherUserName);
            }
            else if (Membership.GetUserNameByEmail(this.RegisterUser.Email) != null)
            {
                e.Cancel = true;
                this.Master.SetStatus("warning", Resources.labels.anotherEmail);
            }
            else if (captcha != null)
            {
                captcha.Validate();

                if (!captcha.IsValid)
                {
                    e.Cancel = true;
                    this.Master.SetStatus("warning", "Captcha invalid.");
                }
            }
        }

        #endregion

    }
}