// Copyright (c) 2007 Adrian Godong, Ben Maurer
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Adapted for dotnetblogengine by Filip Stanek ( http://www.bloodforge.com )

namespace App_Code.Controls
{
    using System;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;

    using Recaptcha;

    /// <summary>
    /// The recaptcha control.
    /// </summary>
    public class RecaptchaControl : WebControl, IValidator
    {
        #region Constants and Fields

        /// <summary>
        ///     The recaptcha challenge field.
        /// </summary>
        private const string RecaptchaChallengeField = "recaptcha_challenge_field";

        /*
        /// <summary>
        /// The recaptcha host.
        /// </summary>
        private const string RecaptchaHost = "http://api.recaptcha.net";
        */

        /// <summary>
        ///     The recaptcha response field.
        /// </summary>
        private const string RecaptchaResponseField = "recaptcha_response_field";

        /*
        /// <summary>
        /// The recaptcha secure host.
        /// </summary>
        private const string RecaptchaSecureHost = "https://api-secure.recaptcha.net";
        */

        /// <summary>
        ///     The error message.
        /// </summary>
        private string errorMessage;

        /// <summary>
        ///     The private key.
        /// </summary>
        private string privateKey;

        /// <summary>
        ///     The public key.
        /// </summary>
        private string publicKey;

        /// <summary>
        ///     The recaptcha response.
        /// </summary>
        private RecaptchaResponse recaptchaResponse;

        /// <summary>
        ///     The skip recaptcha.
        /// </summary>
        private bool skipRecaptcha = true;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets ErrorMessage.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage ?? "The verification words are incorrect.";
            }

            set
            {
                this.errorMessage = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether IsValid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.recaptchaResponse != null && this.recaptchaResponse.IsValid;
            }

            set
            {
            }
        }

        /// <summary>
        ///     Gets MaxLogEntries.
        /// </summary>
        public int MaxLogEntries
        {
            get
            {
                var settings = ExtensionManager.GetSettings("Recaptcha");
                return settings != null ? Convert.ToInt32(settings.GetSingleValue("MaxLogEntries")) : 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the control has been enabled via the Extension Manager
        /// </summary>
        public bool RecaptchaEnabled
        {
            get
            {
                var captchaExtension = ExtensionManager.GetExtension("Recaptcha");
                return captchaExtension.Enabled;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether RecaptchaLoggingEnabled.
        /// </summary>
        public bool RecaptchaLoggingEnabled
        {
            get
            {
                ExtensionManager.GetSettings("Recaptcha");
                return this.MaxLogEntries > 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the recaptcha needs to be displayed for the current user
        /// </summary>
        public bool RecaptchaNecessary
        {
            get
            {
                var settings = ExtensionManager.GetSettings("Recaptcha");
                return !Security.IsAuthenticated ||
                       Convert.ToBoolean(settings.GetSingleValue("ShowForAuthenticatedUsers"));
            }
        }

        /// <summary>
        ///     Gets or sets Theme.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        ///     Gets or sets Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Gets or sets UserUniqueIdentifier.
        /// </summary>
        public string UserUniqueIdentifier { get; set; }

        /// <summary>
        ///     Gets or sets PageLoadTime.
        /// </summary>
        internal DateTime PageLoadTime
        {
            get
            {
                return Blog.CurrentInstance.Cache[string.Format("{0}PageLoadTime", this.UserUniqueIdentifier)] != null
                           ? Convert.ToDateTime(
                               Blog.CurrentInstance.Cache[string.Format("{0}PageLoadTime", this.UserUniqueIdentifier)])
                           : DateTime.Now;
            }

            set
            {
                if (Blog.CurrentInstance.Cache[string.Format("{0}PageLoadTime", this.UserUniqueIdentifier)] != null)
                {
                    Blog.CurrentInstance.Cache[string.Format("{0}PageLoadTime", this.UserUniqueIdentifier)] = value;
                }
                else
                {
                    Blog.CurrentInstance.Cache.Add(
                        string.Format("{0}PageLoadTime", this.UserUniqueIdentifier), 
                        value, 
                        null, 
                        Cache.NoAbsoluteExpiration, 
                        new TimeSpan(1, 0, 0), 
                        CacheItemPriority.Low, 
                        null);
                }
            }
        }

        /// <summary>
        ///     Gets or sets RecaptchaAttempts.
        /// </summary>
        internal ushort RecaptchaAttempts
        {
            get
            {
                return Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaAttempts", this.UserUniqueIdentifier)] !=
                       null
                           ? Convert.ToUInt16(
                               Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaAttempts", this.UserUniqueIdentifier)])
                           : (ushort)0;
            }

            set
            {
                if (Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaAttempts", this.UserUniqueIdentifier)] != null)
                {
                    Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaAttempts", this.UserUniqueIdentifier)] = value;
                }
                else
                {
                    Blog.CurrentInstance.Cache.Add(
                        string.Format("{0}RecaptchaAttempts", this.UserUniqueIdentifier), 
                        value, 
                        null, 
                        Cache.NoAbsoluteExpiration, 
                        new TimeSpan(0, 15, 0), 
                        CacheItemPriority.Low, 
                        null);
                }
            }
        }

        /// <summary>
        ///     Gets or sets RecaptchaChallengeValue.
        /// </summary>
        internal string RecaptchaChallengeValue
        {
            get
            {
                return
                    Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaChallengeValue", this.UserUniqueIdentifier)] !=
                    null
                        ? Convert.ToString(
                            Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaChallengeValue", this.UserUniqueIdentifier)])
                        : string.Empty;
            }

            set
            {
                if (Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaChallengeValue", this.UserUniqueIdentifier)] !=
                    null)
                {
                    Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaChallengeValue", this.UserUniqueIdentifier)] =
                        value;
                }
                else
                {
                    Blog.CurrentInstance.Cache.Add(
                        string.Format("{0}RecaptchaChallengeValue", this.UserUniqueIdentifier), 
                        value, 
                        null, 
                        Cache.NoAbsoluteExpiration, 
                        new TimeSpan(0, 1, 0), 
                        CacheItemPriority.NotRemovable, 
                        null);
                }
            }
        }

        /// <summary>
        ///     Gets or sets RecaptchaRenderTime.
        /// </summary>
        internal DateTime RecaptchaRenderTime
        {
            get
            {
                return Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaRenderTime", this.UserUniqueIdentifier)] !=
                       null
                           ? Convert.ToDateTime(
                               Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaRenderTime", this.UserUniqueIdentifier)])
                           : DateTime.Now;
            }

            set
            {
                if (Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaRenderTime", this.UserUniqueIdentifier)] !=
                    null)
                {
                    Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaRenderTime", this.UserUniqueIdentifier)] =
                        value;
                }
                else
                {
                    Blog.CurrentInstance.Cache.Add(
                        string.Format("{0}RecaptchaRenderTime", this.UserUniqueIdentifier), 
                        value, 
                        null, 
                        Cache.NoAbsoluteExpiration, 
                        new TimeSpan(1, 0, 0), 
                        CacheItemPriority.Low, 
                        null);
                }
            }
        }

        /// <summary>
        ///     Gets or sets RecaptchaResponseValue.
        /// </summary>
        internal string RecaptchaResponseValue
        {
            get
            {
                return
                    Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaResponseValue", this.UserUniqueIdentifier)] !=
                    null
                        ? Convert.ToString(
                            Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaResponseValue", this.UserUniqueIdentifier)])
                        : string.Empty;
            }

            set
            {
                if (Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaResponseValue", this.UserUniqueIdentifier)] !=
                    null)
                {
                    Blog.CurrentInstance.Cache[string.Format("{0}RecaptchaResponseValue", this.UserUniqueIdentifier)] =
                        value;
                }
                else
                {
                    Blog.CurrentInstance.Cache.Add(
                        string.Format("{0}RecaptchaResponseValue", this.UserUniqueIdentifier), 
                        value, 
                        null, 
                        Cache.NoAbsoluteExpiration, 
                        new TimeSpan(0, 1, 0), 
                        CacheItemPriority.NotRemovable, 
                        null);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The update log.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        public void UpdateLog(Comment comment)
        {
            if (!this.RecaptchaLoggingEnabled || this.skipRecaptcha)
            {
                return;
            }

            var log = RecaptchaLogger.ReadLogItems();

            var logItem = new RecaptchaLogItem
                {
                    Response = this.RecaptchaResponseValue, 
                    Challenge = this.RecaptchaChallengeValue, 
                    CommentId = comment.Id, 
                    Enabled = this.RecaptchaEnabled, 
                    Necessary = this.RecaptchaNecessary, 
                    NumberOfAttempts = this.RecaptchaAttempts, 
                    TimeToComment = DateTime.Now.Subtract(this.PageLoadTime).TotalSeconds, 
                    TimeToSolveCapcha = DateTime.Now.Subtract(this.RecaptchaRenderTime).TotalSeconds
                };
            log.Add(logItem);

            if (log.Count > this.MaxLogEntries)
            {
                log.RemoveRange(0, log.Count - this.MaxLogEntries);
            }

            RecaptchaLogger.SaveLogItems(log);

            this.RecaptchaAttempts = 0;
            this.PageLoadTime = DateTime.Now;
            Blog.CurrentInstance.Cache.Remove(string.Format("{0}RecaptchaChallengeValue", this.UserUniqueIdentifier));
            Blog.CurrentInstance.Cache.Remove(string.Format("{0}RecaptchaResponseValue", this.UserUniqueIdentifier));
        }

        /// <summary>
        /// Validates the async.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="challenge">
        /// The challenge.
        /// </param>
        /// <returns>
        /// Whether valid.
        /// </returns>
        public bool ValidateAsync(string response, string challenge)
        {
            if (this.RecaptchaLoggingEnabled)
            {
                this.RecaptchaAttempts++;
            }

            this.RecaptchaResponseValue = response;
            this.RecaptchaChallengeValue = challenge;
            this.Validate();
            return this.IsValid;
        }

        #endregion

        #region Implemented Interfaces

        #region IValidator

        /// <summary>
        /// The validate.
        /// </summary>
        public void Validate()
        {
            if (this.skipRecaptcha)
            {
                this.recaptchaResponse = RecaptchaResponse.Valid;
            }
            else
            {
                var validator = new RecaptchaValidator
                    {
                       PrivateKey = this.privateKey, RemoteIP = Utils.GetClientIP() 
                    };
                if (String.IsNullOrEmpty(this.RecaptchaChallengeValue) &&
                    String.IsNullOrEmpty(this.RecaptchaResponseValue))
                {
                    validator.Challenge = this.Context.Request.Form[RecaptchaChallengeField];
                    validator.Response = this.Context.Request.Form[RecaptchaResponseField];
                }
                else
                {
                    validator.Challenge = this.RecaptchaChallengeValue;
                    validator.Response = this.RecaptchaResponseValue;
                }

                this.recaptchaResponse = validator.Validate();
            }
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

            var settings = ExtensionManager.GetSettings("Recaptcha");
            this.publicKey = settings.GetSingleValue("PublicKey");
            this.privateKey = settings.GetSingleValue("PrivateKey");

            if (String.IsNullOrEmpty(this.Theme))
            {
                this.Theme = settings.GetSingleValue("Theme");
            }

            if (String.IsNullOrEmpty(this.Language))
            {
                this.Language = settings.GetSingleValue("Language");
            }

            if (this.RecaptchaEnabled && this.RecaptchaNecessary)
            {
                this.skipRecaptcha = false;
            }

            if (String.IsNullOrEmpty(this.publicKey) || String.IsNullOrEmpty(this.privateKey))
            {
                throw new ApplicationException("reCAPTCHA needs to be configured with a public & private key.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Unload"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains event data.
        /// </param>
        protected override void OnUnload(EventArgs e)
        {
            if (this.RecaptchaLoggingEnabled)
            {
                if (!this.Page.IsCallback)
                {
                    this.PageLoadTime = DateTime.Now;
                    this.RecaptchaAttempts = 0;
                }

                this.RecaptchaRenderTime = DateTime.Now;
            }

            base.OnUnload(e);
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.skipRecaptcha)
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
            output.AddAttribute("type", "text/javascript");
            output.AddAttribute("src", "http://www.google.com/recaptcha/api/js/recaptcha_ajax.js");
            output.RenderBeginTag("script");
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Id, "spnCaptchaIncorrect");
            output.AddAttribute(HtmlTextWriterAttribute.Style, "color:Red;display:none;");
            output.RenderBeginTag("span");
            output.WriteLine("The captcha text was not valid. Please try again.");
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Id, "recaptcha_placeholder");
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.RenderBeginTag(HtmlTextWriterTag.Script);
            output.WriteLine("function showRecaptcha() {");
            output.WriteLine("Recaptcha.create('{0}', 'recaptcha_placeholder', {{", this.publicKey);
            output.WriteLine("theme: '{0}',", this.Theme);
            output.WriteLine("lang: '{0}',", this.Language);
            output.WriteLine("tabindex: {0}", this.TabIndex);
            output.WriteLine("})");
            output.WriteLine("}");

            output.WriteLine("var rc_oldonload = window.onload;");
            output.WriteLine("if (typeof window.onload != 'function') {");
            output.WriteLine("window.onload = showRecaptcha;");
            output.WriteLine("}");
            output.WriteLine("else {");
            output.WriteLine("window.onload = function() {");
            output.WriteLine("rc_oldonload();");
            output.WriteLine("showRecaptcha();");
            output.WriteLine("}");
            output.WriteLine("}");

            output.RenderEndTag();
        }

        #endregion

        /*
        /// <summary>
        /// Generates the challenge URL.
        /// </summary>
        /// <param name="noscript">if set to <c>true</c> [no script].</param>
        /// <returns>The challenge url.</returns>
        private string GenerateChallengeUrl(bool noscript)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(this.Context.Request.IsSecureConnection ? RecaptchaSecureHost : RecaptchaHost);
            urlBuilder.Append(noscript ? "/noscript?" : "/challenge?");
            urlBuilder.AppendFormat("k={0}", this.publicKey);
            if (this.recaptchaResponse != null && this.recaptchaResponse.ErrorCode != string.Empty)
            {
                urlBuilder.AppendFormat("&error={0}", this.recaptchaResponse.ErrorCode);
            }

            return urlBuilder.ToString();
        }
*/
    }
}