// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widgets tag cloud edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.TagCloud
{
    using System;

    using App_Code.Controls;

    /// <summary>
    /// The widgets tag cloud edit.
    /// </summary>
    public partial class Edit : WidgetEditBase
    {
        #region Public Methods

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public override void Save()
        {
            var settings = this.GetSettings();
            settings["minimumposts"] = this.ddlMinimumPosts.SelectedValue;
            settings["tagcloudsize"] = this.ddlCloudSize.SelectedValue;
            this.SaveSettings(settings);
            WidgetsTagCloudWidget.Reload();
        }

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
            var settings = this.GetSettings();
            var minimumPosts = "1";
            if (settings.ContainsKey("minimumposts"))
            {
                minimumPosts = settings["minimumposts"];
            }

            var tagcloudsize = "-1";
            if (settings.ContainsKey("tagcloudsize"))
            {
                tagcloudsize = settings["tagcloudsize"];
            }

            this.ddlMinimumPosts.SelectedValue = minimumPosts;
            this.ddlCloudSize.SelectedValue = tagcloudsize;

            base.OnInit(e);
        }

        #endregion
    }
}