// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Widget Edit Base
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
    /// Widget Edit Base
    /// </summary>
    public abstract class WidgetEditBase : UserControl
    {
        #region Events

        /// <summary>
        ///     Occurs when [saved].
        /// </summary>
        public static event EventHandler<EventArgs> Saved;

        #endregion

        #region Properties

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
        ///     Gets or sets the widget id.
        /// </summary>
        /// <value>The widget id.</value>
        public Guid WidgetId { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when [saved].
        /// </summary>
        public static void OnSaved()
        {
            if (Saved != null)
            {
                Saved(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get settings from data store
        /// </summary>
        /// <returns>
        /// The settings
        /// </returns>
        public StringDictionary GetSettings()
        {
            var cacheId = string.Format("be_widget_{0}", WidgetId);
            if (Blog.CurrentInstance.Cache[cacheId] == null)
            {
                var ws = new WidgetSettings(WidgetId.ToString());
                Blog.CurrentInstance.Cache[cacheId] = ws.GetSettings();
            }

            return (StringDictionary)Blog.CurrentInstance.Cache[cacheId];
        }

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public abstract void Save();

        #endregion

        #region Methods

        /// <summary>
        /// Saves settings to data store
        /// </summary>
        /// <param name="settings">
        /// The settings
        /// </param>
        protected virtual void SaveSettings(StringDictionary settings)
        {
            var cacheId = string.Format("be_widget_{0}", WidgetId);

            var ws = new WidgetSettings(WidgetId.ToString());
            ws.SaveSettings(settings);

            Blog.CurrentInstance.Cache[cacheId] = settings;
        }

        #endregion
    }
}