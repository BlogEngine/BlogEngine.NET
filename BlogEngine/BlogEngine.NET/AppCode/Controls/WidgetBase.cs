// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Widget Base
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BlogEngine.Core;

namespace App_Code.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.Web.UI;

    using BlogEngine.Core.DataStore;

    /// <summary>
    /// Widget Base
    /// </summary>
    public abstract class WidgetBase : UserControl
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether the header is visible. This only takes effect if the widgets isn't editable.
        /// </summary>
        /// <value><c>true</c> if the header is visible; otherwise, <c>false</c>.</value>
        public virtual bool DisplayHeader
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether or not the widget can be edited.
        ///     <remarks>
        ///         The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        ///     </remarks>
        /// </summary>
        public abstract bool IsEditable { get; }

        /// <summary>
        ///     Gets the name. It must be exactly the same as the folder that contains the widget.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
        public bool ShowTitle { get; set; }

        /// <summary>
        ///     Gets or sets the title of the widget. It is mandatory for all widgets to set the Title.
        /// </summary>
        /// <value>The title of the widget.</value>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the widget ID.
        /// </summary>
        /// <value>The widget ID.</value>
        public Guid WidgetId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the containing WidgetZone
        /// </summary>
        public string Zone { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get settings from data store
        /// </summary>
        /// <returns>
        /// The settings
        /// </returns>
        public StringDictionary GetSettings()
        {
            var cacheId = string.Format("be_widget_{0}", this.WidgetId);
            if (Blog.CurrentInstance.Cache[cacheId] == null)
            {
                var ws = new WidgetSettings(this.WidgetId.ToString());
                Blog.CurrentInstance.Cache[cacheId] = ws.GetSettings();
            }

            return (StringDictionary)Blog.CurrentInstance.Cache[cacheId];
        }

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public abstract void LoadWidget();

        #endregion

        #region Methods

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> 
        ///     object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                throw new NullReferenceException("Name must be set on a widget");
            }

            base.Render(writer);
        }

        #endregion
    }
}