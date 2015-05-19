// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widgets_ recent posts_edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.RecentPosts
{
    using System;
    using System.Web;

    using App_Code.Controls;
    using BlogEngine.Core;

    /// <summary>
    /// The widgets_ recent posts_edit.
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
            settings["numberofposts"] = this.txtNumberOfPosts.Text;
            settings["showcomments"] = this.cbShowComments.Checked.ToString();
            settings["showrating"] = this.cbShowRating.Checked.ToString();
            this.SaveSettings(settings);
            Blog.CurrentInstance.Cache.Remove("widget_recentposts");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Page.IsPostBack)
            {
                return;
            }
        
            var settings = this.GetSettings();
            this.txtNumberOfPosts.Text = settings.ContainsKey("numberofposts") ? settings["numberofposts"] : "10";

            this.cbShowComments.Checked = !settings.ContainsKey("showcomments") ||
                                          settings["showcomments"].Equals("true", StringComparison.OrdinalIgnoreCase);

            this.cbShowRating.Checked = !settings.ContainsKey("showrating") || settings["showrating"].Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}