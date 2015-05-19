// Inspired by and interface heavily borrowed from Filip Stanek's ( http://www.bloodforge.com ) Recaptcha extension for blogengine.net
// SimpleCaptcha created by Aaron Stannard (http://www.aaronstannard.com )

namespace App_Code.Controls
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;

    /// <summary>
    /// This is the IValidator control that gets embedded on the comment form if the SimpleCaptcha extension is enabled.
    /// </summary>
    public class SimpleCaptchaControl : WebControl, IValidator
    {
        #region Constants and Fields

        /// <summary>
        /// The simple captcha answer field.
        /// </summary>
        private const string SimpleCaptchaAnswerField = "simpleCaptchaValue";

        /// <summary>
        /// The error message.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Whether is valid.
        /// </summary>
        private bool valid;

        /// <summary>
        /// The simple captcha answer.
        /// </summary>
        private string simpleCaptchaAnswer;

        /// <summary>
        /// The simple captcha label.
        /// </summary>
        private string simpleCaptchaLabel;

        /// <summary>
        /// The skip simple captcha.
        /// </summary>
        private bool skipSimpleCaptcha = true;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets ErrorMessage.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage ?? Resources.labels.incorrectSimpleCaptcha;
            }

            set
            {
                this.errorMessage = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.valid;
            }

            set
            {
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the control has been enabled via the Extension Manager
        /// </summary>
        public bool SimpleCaptchaEnabled
        {
            get
            {
                var captchaExtension = ExtensionManager.GetExtension("SimpleCaptcha");
                return captchaExtension.Enabled;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the recaptcha needs to be displayed for the current user
        /// </summary>
        public bool SimpleCaptchaNecessary
        {
            get
            {
                var settings = ExtensionManager.GetSettings("SimpleCaptcha");
                return !Security.IsAuthenticated ||
                       Convert.ToBoolean(settings.GetSingleValue("ShowForAuthenticatedUsers"));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="simpleCaptchaChallenge">
        /// The simple captcha challenge.
        /// </param>
        public void Validate(string simpleCaptchaChallenge)
        {
            this.valid = this.skipSimpleCaptcha || this.simpleCaptchaAnswer.Equals(simpleCaptchaChallenge);
        }

        #endregion

        #region Implemented Interfaces

        #region IValidator

        /// <summary>
        /// The validate.
        /// </summary>
        public void Validate()
        {
            var simpleCaptchaChallenge = this.Context.Request.Form[SimpleCaptchaAnswerField];
            this.Validate(simpleCaptchaChallenge);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var settings = ExtensionManager.GetSettings("SimpleCaptcha");
			if (settings != null)
			{
				this.simpleCaptchaAnswer = settings.GetSingleValue("CaptchaAnswer");
				this.simpleCaptchaLabel = settings.GetSingleValue("CaptchaLabel");

				if (this.SimpleCaptchaEnabled && this.SimpleCaptchaNecessary)
				{
					this.skipSimpleCaptcha = false;
				}

				if (String.IsNullOrEmpty(this.simpleCaptchaAnswer) || String.IsNullOrEmpty(this.simpleCaptchaLabel))
				{
					throw new ApplicationException(
						"SimpleCaptcha needs to be configured with an appropriate captcha label and a captcha value.");
				}
			}
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.skipSimpleCaptcha)
            {
                this.RenderContents(writer);
            }
        }

        /// <summary>
        /// Renders the contents.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            output.RenderBeginTag(HtmlTextWriterTag.P);
            output.AddAttribute(HtmlTextWriterAttribute.For, SimpleCaptchaAnswerField);
            output.AddAttribute(HtmlTextWriterAttribute.Style, "margin-right: 5px;");
            output.RenderBeginTag(HtmlTextWriterTag.Label);
            output.Write(this.simpleCaptchaLabel);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Id, SimpleCaptchaAnswerField);
            output.AddAttribute(HtmlTextWriterAttribute.Name, SimpleCaptchaAnswerField);
            output.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            output.AddAttribute(HtmlTextWriterAttribute.Tabindex, this.TabIndex.ToString());
            output.AddAttribute(HtmlTextWriterAttribute.Maxlength, Convert.ToString(SimpleCaptcha.MaxCaptchaLength));
            output.AddAttribute(HtmlTextWriterAttribute.Value, string.Empty);
            output.RenderBeginTag(HtmlTextWriterTag.Input);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Id, "spnSimpleCaptchaIncorrect");
            output.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            output.AddStyleAttribute(HtmlTextWriterStyle.Color, "Red");
            output.RenderBeginTag(HtmlTextWriterTag.Span);
            output.WriteLine(this.ErrorMessage);
            output.RenderEndTag();
            output.RenderEndTag();
        }

        #endregion
    }
}