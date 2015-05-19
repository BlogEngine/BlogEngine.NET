// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.Twitter
{
    using System;
    using System.Web;

    using App_Code.Controls;
    using BlogEngine.Core;

    /// <summary>
    /// The edit.
    /// </summary>
    public partial class Edit : WidgetEditBase
    {
        #region Constants and Fields

        /// <summary>
        /// The twitter settings cache key.
        /// </summary>
        private const string TwitterSettingsCacheKey = "twitter-settings"; // same key used in widget.ascx.cs.

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public override void Save()
        {
            var settings = this.GetSettings();
            settings["feedurl"] = this.txtUrl.Text;
            settings["accounturl"] = this.txtAccountUrl.Text;
            settings["maxitems"] = this.txtTwits.Text;
            settings["pollinginterval"] = this.txtPolling.Text;
            settings["followmetext"] = this.txtFollowMe.Text;
            this.SaveSettings(settings);

            // Don't need to clear Feed out of cache because when the Settings are cleared,
            // the last modified date (i.e. TwitterSettings.LastModified) will reset to
            // DateTime.MinValue and Twitter will be re-queried.
            Blog.CurrentInstance.Cache.Remove(TwitterSettingsCacheKey);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            var settings = this.GetSettings();
            if (settings.ContainsKey("feedurl"))
            {
                this.txtUrl.Text = settings["feedurl"];
                this.txtAccountUrl.Text = settings["accounturl"];
                this.txtTwits.Text = settings["maxitems"];
                this.txtPolling.Text = settings["pollinginterval"];
                this.txtFollowMe.Text = settings["followmetext"];
            }

            base.OnInit(e);
        }

        #endregion
    }
}