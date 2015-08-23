using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Providers;
using System.Collections.Generic;

namespace BlogEngine.Core.Notes
{
    /// <summary>
    /// Settings for quick notes
    /// </summary>
    public class QuickSettings
    {
        /// <summary>
        /// Quick settings
        /// </summary>
        /// <param name="user"></param>
        public QuickSettings(string user)
        {
            Settings = BlogService.FillQuickSettings(user);

            if (Settings == null)
                Settings = new List<QuickSetting>();
        }
        /// <summary>
        /// List of settings
        /// </summary>
        public List<QuickSetting> Settings { get; set; }

        /// <summary>
        /// List of categories
        /// </summary>
        public List<BlogEngine.Core.Data.Models.CategoryItem> Categories
        { 
            get 
            {
                var cats = new List<BlogEngine.Core.Data.Models.CategoryItem>();
                foreach (var c in Category.Categories)
                {
                    cats.Add(new BlogEngine.Core.Data.Models.CategoryItem { Id = c.Id, Title = c.Title });
                }
                return cats;
            } 
        }

        #region Methods

        /// <summary>
        /// Save to collection (add or replace)
        /// </summary>
        /// <param name="setting">Setting</param>
        public void Save(QuickSetting setting)
        {
            var idx = Settings.FindIndex(s => s.Author == setting.Author && s.SettingName == setting.SettingName);
            if(idx < 0)
            {
                Settings.Add(setting);
            }
            else
            {
                Settings[idx] = setting;
            }
        }

        #endregion
    }
}