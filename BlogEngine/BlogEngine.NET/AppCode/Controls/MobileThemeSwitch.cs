namespace App_Code.Controls
{
    using System;
    using System.Web;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;

    /// <summary>
    /// The <see cref="MobileThemeSwitch"/>
    ///   class is used to provide the UI support for switching between the regular site theme and the mobile site theme.
    /// </summary>
    public class MobileThemeSwitch : LinkButton
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MobileThemeSwitch" /> class.
        /// </summary>
        public MobileThemeSwitch()
        {
            String forceMainThemeText;

            try
            {
                forceMainThemeText = Resources.labels.forceMainTheme;
            }
            catch (Exception)
            {
                forceMainThemeText = "Regular Site";
            }

            ShowRegularSiteText = forceMainThemeText;

            String allowMobileThemeText;

            try
            {
                allowMobileThemeText = Resources.labels.allowMobileTheme;
            }
            catch (Exception)
            {
                allowMobileThemeText = "Mobile Site";
            }

            ShowMobileSiteText = allowMobileThemeText;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.WebControls.Button.Click"/> event of the <see cref="T:System.Web.UI.WebControls.Button"/> control.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected override void OnClick(EventArgs e)
        {
            Boolean isForced = Utils.ShouldForceMainTheme(Page.Request);

            if (isForced)
            {
                HttpCookie allowMobileThemeCookie = new HttpCookie(Utils.ForceMainThemeCookieName, "false");

                // Remove the forced cookie to allow the regular theme settings to take control
                HttpCookie requestCookie = Page.Request.Cookies[Utils.ForceMainThemeCookieName];

                if (requestCookie != null)
                {
                    requestCookie.Value = allowMobileThemeCookie.Value;
                }
                else
                {
                    Page.Response.Cookies.Add(allowMobileThemeCookie);
                }

                HttpCookie responseCookie = Page.Response.Cookies[Utils.ForceMainThemeCookieName];
                
                if (responseCookie != null)
                {
                    responseCookie.Value = allowMobileThemeCookie.Value;
                }
                else
                {
                    Page.Response.Cookies.Add(allowMobileThemeCookie);
                }
            }
            else
            {
                HttpCookie forceCookie = new HttpCookie(Utils.ForceMainThemeCookieName, "true");

                Page.Request.Cookies.Add(forceCookie);
                Page.Response.Cookies.Add(forceCookie);
            }

            // The page processing has already started with the prior theme 
            // so we need a redirect to ensure that the correct theme is used for the full page processing lifecycle
            Page.Response.Redirect(Page.Request.RawUrl, true);

            base.OnClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnPreRender(EventArgs e)
        {
            Boolean isForced = Utils.ShouldForceMainTheme(Page.Request);

            if (isForced)
            {
                Text = ShowMobileSiteText;
            }
            else
            {
                Text = ShowRegularSiteText;
            }

            base.OnPreRender(e);
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the server control persists its view state, and the view state of any child controls it contains, to the requesting client.
        /// </summary>
        /// <returns>true if the server control maintains its view state; otherwise false. The default is true.
        /// </returns>
        public override bool EnableViewState
        {
            get
            {
                return false;
            }

            set
            {
                // Ignonored
            }
        }

        /// <summary>
        ///   Gets or sets the show mobile site text.
        /// </summary>
        /// <value>
        ///   The show mobile site text.
        /// </value>
        public String ShowMobileSiteText
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the show regular site text.
        /// </summary>
        /// <value>
        ///   The show regular site text.
        /// </value>
        public String ShowRegularSiteText
        {
            get;
            set;
        }
    }
}