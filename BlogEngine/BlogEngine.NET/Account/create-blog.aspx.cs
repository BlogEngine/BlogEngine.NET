namespace Account
{
    using BlogEngine.Core;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Page = System.Web.UI.Page;

    public partial class CreateBlog : Page
    {
        static App_Code.Controls.RecaptchaControl captcha;

        protected void Page_Load(object sender, EventArgs e) 
        {
            if (!BlogSettings.Instance.CreateBlogOnSelfRegistration || !Blog.CurrentInstance.IsPrimary)
            {
                Response.Redirect("login.aspx");
            }
        }

        protected void CreateUserButton_Click(object sender, EventArgs e)
        {
            string blogName = BlogName.Text.Trim().ToLower();

            string msg = CreateNewBlog();

            if (string.IsNullOrEmpty(msg))
            {
                this.Response.Redirect(Utils.ApplicationRelativeWebRoot + blogName);
            }
            else
            {
                this.Master.SetStatus("warning", msg);
            }
        }

        string CreateNewBlog()
        {
            string message = string.Empty;
            Blog blog = null;

            if (!BlogGenerator.ValidateProperties(BlogName.Text, UserName.Text, Email.Text, out message))
            {
                if (string.IsNullOrWhiteSpace(message)) { message = "Validation for new blog failed."; }
                return message;
            }

            // recaptcha check
            SetCaptcha(Form.Controls);

            if (captcha != null)
            {
                captcha.Validate();

                if (!captcha.IsValid)
                {
                    message = "Captcha invalid.";
                    return message;
                }
            }

            blog = BlogGenerator.CreateNewBlog(BlogName.Text, UserName.Text, Email.Text, Password.Text, out message);

            if (blog == null || !string.IsNullOrWhiteSpace(message))
            {
                return message ?? "Failed to create the new blog.";
            }

            return message;
        }

        public void SetCaptcha(ControlCollection controls)
        {
            foreach (Control ctl in controls)
            {
                if (ctl is WebControl)
                {
                    if (ctl.ToString().Contains("RecaptchaControl"))
                    {
                        captcha = (App_Code.Controls.RecaptchaControl)ctl;
                        return;
                    } 
                }
                if (ctl.Controls.Count > 0) SetCaptcha(ctl.Controls);
            }
        }
    }
}