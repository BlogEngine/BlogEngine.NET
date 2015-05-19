// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.RecentComments
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
        #region Public Methods

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public override void Save()
        {
            var settings = this.GetSettings();
            settings["numberofcomments"] = this.txtNumberOfPosts.Text;
            this.SaveSettings(settings);
            Blog.CurrentInstance.Cache.Remove("widget_recentcomments");
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
            base.OnPreRender(e);

            if (this.Page.IsPostBack)
            {
                return;
            }

            var settings = this.GetSettings();
            this.txtNumberOfPosts.Text = settings.ContainsKey("numberofcomments") ? settings["numberofcomments"] : "10";
        }

        #endregion
    }
}