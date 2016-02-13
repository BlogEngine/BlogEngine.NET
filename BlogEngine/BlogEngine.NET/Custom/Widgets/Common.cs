using BlogEngine.Core;
using BlogEngine.Core.DataStore;
using System.Collections.Specialized;

namespace BlogEngine.NET.Custom.Widgets
{
    public class Common
    {
        /// <summary>
        /// Gets widget settings
        /// </summary>
        /// <param name="id">Widget ID</param>
        /// <returns>Settings object</returns>
        public static StringDictionary GetSettings(string id)
        {
            var cacheId = string.Format("be_widget_{0}", id);
            if (Blog.CurrentInstance.Cache[cacheId] == null)
            {
                var ws = new WidgetSettings(id);
                Blog.CurrentInstance.Cache[cacheId] = ws.GetSettings();
            }
            return (StringDictionary)Blog.CurrentInstance.Cache[cacheId];
        }

        /// <summary>
        /// Saves widget settings into datastore
        /// </summary>
        /// <param name="settings">Settings object (key/values)</param>
        /// <param name="widgetId">Widget Id</param>
        public static void SaveSettings(StringDictionary settings, string widgetId)
        {
            var cacheId = string.Format("be_widget_{0}", widgetId);

            var ws = new WidgetSettings(widgetId);
            ws.SaveSettings(settings);

            Blog.CurrentInstance.Cache[cacheId] = settings;
        }
    }
}