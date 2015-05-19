// Inspired by and interface heavily borrowed from Filip Stanek's ( http://www.bloodforge.com ) Recaptcha extension for blogengine.net
// SimpleCaptcha created by Aaron Stannard (http://www.aaronstannard.com )

namespace App_Code.Controls
{
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;
    using System;
    using System.Collections.Generic;
    using BlogEngine.Core;

    /// <summary>
    /// Builds the SimpleCaptcha control
    /// </summary>
    [Extension("Settings for the SimpleCaptcha control", "1.0", 
        "<a href=\"http://www.aaronstannard.com\">Aaron Stannard</a>", 2)]
    public class SimpleCaptcha
    {
        #region Constants and Fields

        /// <summary>
        ///     The maximum length of a SimpleCaptcha expected value.
        /// </summary>
        public const int MaxCaptchaLength = 30;

        /// <summary>
        ///     The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "SimpleCaptcha" /> class.
        /// </summary>
        public SimpleCaptcha()
        {
            InitSettings();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        protected static Dictionary<Guid, ExtensionSettings> blogsSettings = new Dictionary<Guid, ExtensionSettings>();

        private static ExtensionSettings Settings
        {
            get
            {
                Guid blogId = Blog.CurrentInstance.Id;
                ExtensionSettings settings = null;
                blogsSettings.TryGetValue(blogId, out settings);

                if (settings == null)
                {
                    lock (syncRoot)
                    {
                        blogsSettings.TryGetValue(blogId, out settings);

                        if (settings == null)
                        {
                            settings = new ExtensionSettings("SimpleCaptcha") { IsScalar = true };

                            settings.AddParameter("CaptchaLabel", "Your captcha's label", 30, true, true, ParameterType.String);
                            settings.AddValue("CaptchaLabel", "5+5 = ");

                            settings.AddParameter(
                                "CaptchaAnswer", "Your captcha's expected value", MaxCaptchaLength, true, true, ParameterType.String);
                            settings.AddValue("CaptchaAnswer", "10");

                            settings.AddParameter(
                                "ShowForAuthenticatedUsers",
                                "Show Captcha For Authenticated Users",
                                1,
                                true,
                                false,
                                ParameterType.Boolean);
                            settings.AddValue("ShowForAuthenticatedUsers", false);

                            settings.Help =
                                @"To get started with SimpleCaptcha, just provide some captcha instructions for your users in the <b>CaptchaLabel</b>
                                field and the value you require from your users in order to post a comment in the <b>CaptchaAnswer</b> field.";

                            blogsSettings[blogId] = ExtensionManager.InitSettings("SimpleCaptcha", settings);
                            // ExtensionManager.SetStatus("SimpleCaptcha", false);
                        }
                    }
                }

                return settings;
            }
        }

        #endregion

        #region Methods

        public void InitSettings()
        {
            // call Settings getter so default settings are loaded on application start.
            var s = Settings;
        }

        #endregion
    }
}