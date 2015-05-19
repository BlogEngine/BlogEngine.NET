// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Edit widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.ModeComments
{
    using System;

    using App_Code.Controls;
    using BlogEngine.Core;

    /// <summary>
    /// Edit widget.
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
            settings["avatarsize"] = this.txtSize.Text;
            settings["numberofvisitors"] = this.txtNumber.Text;
            settings["days"] = this.txtDays.Text;
            settings["showcomments"] = this.cbShowComments.Checked.ToString();
            this.SaveSettings(settings);

            Blog.CurrentInstance.Cache.Remove("most_comments");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnPreRender(EventArgs e)
        {
            if (this.Page.IsPostBack)
            {
                return;
            }

            this.txtNumber.Text = @"3";
            this.txtSize.Text = @"50";
            this.txtDays.Text = @"60";
            this.cbShowComments.Checked = true;

            var settings = this.GetSettings();
            if (settings.ContainsKey("avatarsize"))
            {
                this.txtSize.Text = settings["avatarsize"];
            }

            if (settings.ContainsKey("numberofvisitors"))
            {
                this.txtNumber.Text = settings["numberofvisitors"];
            }

            if (settings.ContainsKey("days"))
            {
                this.txtDays.Text = settings["days"];
            }

            if (settings.ContainsKey("showcomments"))
            {
                this.cbShowComments.Checked = settings["showcomments"].Equals(
                    "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion
    }
}