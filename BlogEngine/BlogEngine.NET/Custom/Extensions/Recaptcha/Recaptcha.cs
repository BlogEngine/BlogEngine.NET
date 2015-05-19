namespace Recaptcha
{
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;
    using System.Collections.Generic;
    using System;
    using BlogEngine.Core;

    /// <summary>
    /// Builds the recaptcha control ( http://www.Recaptcha.net )
    /// </summary>
    [Extension("Settings for the Recaptcha control", "1.1", "<a href=\"http://www.bloodforge.com\">Bloodforge.com</a>")]
    public class Recaptcha
    {
        #region Constants and Fields

        /// <summary>
        ///     The sync root.
        /// </summary>
        private static readonly object syncRoot = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Recaptcha" /> class.
        /// </summary>
        public Recaptcha()
        {
            this.InitSettings();
            this.InitLogSettings();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets JScript.
        /// </summary>
        public static string JScript
        {
            get
            {
                const string Result =
                    @"function showRecaptchaLog() {
        window.scrollTo(0, 0);
        var width = document.documentElement.clientWidth + document.documentElement.scrollLeft;
        var height = document.documentElement.clientHeight + document.documentElement.scrollTop;

        var layer = document.createElement('div');
        layer.style.zIndex = 1002;
        layer.id = 'RecaptchaLogLayer';
        layer.style.position = 'absolute';
        layer.style.top = '0px';
        layer.style.left = '0px';
        layer.style.height = document.documentElement.scrollHeight + 'px';
        layer.style.width = width + 'px';
        layer.style.backgroundColor = 'black';
        layer.style.opacity = '.6';
        layer.style.filter += ('progid:DXImageTransform.Microsoft.Alpha(opacity=60)');
        document.body.style.position = 'static';
        document.body.appendChild(layer);

        var size = { 'height': 500, 'width': 750 };
        var iframe = document.createElement('iframe');
        iframe.name = 'Recaptcha Log Viewer';
        iframe.id = 'RecaptchaLogDetails';
        iframe.src = '../Pages/RecaptchaLogViewer.aspx';
        iframe.style.height = size.height + 'px';
        iframe.style.width = size.width + 'px';
        iframe.style.position = 'fixed';
        iframe.style.zIndex = 1003;
        iframe.style.backgroundColor = 'white';
        iframe.style.border = '4px solid silver';
        iframe.frameborder = '0';

        iframe.style.top = ((height + document.documentElement.scrollTop) / 2) - (size.height / 2) + 'px';
        iframe.style.left = (width / 2) - (size.width / 2) + 'px';

        document.body.appendChild(iframe);
        return false;
    }";
                return Result;
            }
        }

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
                            settings = new ExtensionSettings("Recaptcha") { IsScalar = true };

                            settings.AddParameter("PublicKey", "Public Key", 50, true, true, ParameterType.String);
                            settings.AddValue("PublicKey", "YOURPUBLICKEY");

                            settings.AddParameter("PrivateKey", "Private Key", 50, true, true, ParameterType.String);
                            settings.AddValue("PrivateKey", "YOURPRIVATEKEY");

                            settings.AddParameter(
                                "ShowForAuthenticatedUsers",
                                "Show Captcha For Authenticated Users",
                                1,
                                true,
                                false,
                                ParameterType.Boolean);
                            settings.AddValue("ShowForAuthenticatedUsers", false);

                            settings.AddParameter(
                                "MaxLogEntries",
                                "Logging: Maximum successful recaptcha attempts to store (set to 0 to disable logging)",
                                4,
                                true,
                                false,
                                ParameterType.Integer);
                            settings.AddValue("MaxLogEntries", 50);

                            settings.AddParameter("Theme", "Theme", 20, true, false, ParameterType.DropDown);
                            settings.AddValue("Theme", new[] { "red", "white", "blackglass", "clean" }, "white");

                            settings.AddParameter("Language", "Language", 5, true, false, ParameterType.DropDown);
                            settings.AddValue("Language", new[] { "en|English", "nl|Dutch", "fr|French", "de|German", "pt|Portuguese", "ru|Russian", "es|Spanish", "tr|Turkish" }, "en");

                            settings.Help =
                                string.Format(
                                    "\n<script type='text/javascript'>\n{0}\n</script>\nYou can create your own public key at <a href='http://www.Recaptcha.net'>http://www.Recaptcha.net</a>. This is used for communication between your website and the recapcha server.<br /><br />Please rememeber you need to <span style=\"color:red\">enable extension</span> for reCaptcha to show up on the comments form.<br /><br />You can see some statistics on Captcha solving by storing successful attempts. If you're getting spam, this should also confirm that the spammers are at least solving the captcha.<br /><br /><a href='../Pages/RecaptchaLogViewer.aspx' target='_blank' onclick='return showRecaptchaLog()'>Click here to view the log</a>",
                                    JScript);

                            blogsSettings[blogId] = ExtensionManager.InitSettings("Recaptcha", settings);

                            ExtensionManager.SetStatus("Recaptcha", false);
                        }
                    }
                }

                return settings;
            }
        }

        #endregion

        #region Public Methods

        public void InitSettings()
        {
            // call Settings getter so default settings are loaded on application start.
            var s = Settings;
        }

        /// <summary>
        /// The init log settings.
        /// </summary>
        public void InitLogSettings()
        {
            var settings = new ExtensionSettings("RecaptchaLog");

            settings.AddParameter("Response");
            settings.AddParameter("Challenge");
            settings.AddParameter("CommentID");
            settings.AddParameter("TimeToComment");
            settings.AddParameter("TimeToSolveCapcha");
            settings.AddParameter("NumberOfAttempts");
            settings.AddParameter("Enabled");
            settings.AddParameter("Necessary");
            settings.Hidden = true;

            ExtensionManager.InitSettings("Recaptcha", settings);
        }

        #endregion
    }
}