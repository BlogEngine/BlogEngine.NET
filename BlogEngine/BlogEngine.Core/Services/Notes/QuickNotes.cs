using System;
using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core.Providers;

namespace BlogEngine.Core.Notes
{
    /// <summary>
    /// Quick notes
    /// </summary>
    public class QuickNotes
    {
        private string author = "";
        private string cacheKey = "";
        private string cacheKeySettings = "";

        /// <summary>
        /// Constructs a QuickNotes for the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public QuickNotes(string user)
        {
            author = user;
            cacheKey = user + "_" + "QuickNotes";
            cacheKeySettings = user + "_" + "QuickSettings";
        }

        /// <summary>
        /// Collection of notes
        /// </summary>
        public List<QuickNote> Notes
        {
            get
            {
                if (Blog.CurrentInstance.Cache[cacheKey] == null)
                {
                    var n = BlogService.FillQuickNotes(author);
                    Blog.CurrentInstance.Cache[cacheKey] = n;
                }
                return (List<QuickNote>)Blog.CurrentInstance.Cache[cacheKey];
            }
        }

        /// <summary>
        /// Settings for quick notes
        /// </summary>
        public QuickSettings Settings
        {
            get
            {
                if (Blog.CurrentInstance.Cache[cacheKeySettings] == null)
                {
                    var s = new QuickSettings(author);
                    Blog.CurrentInstance.Cache[cacheKeySettings] = s;
                }
                return (QuickSettings)Blog.CurrentInstance.Cache[cacheKeySettings];
            }
        }
        /// <summary>
        /// The note
        /// </summary>
        /// <param name="id">Note ID</param>
        /// <returns>Quick note object</returns>
        public QuickNote Note(Guid id)
        {
            return Notes.Where(n => n.Id == id).FirstOrDefault();
        }

        #region Methods

        /// <summary>
        /// Save note to collection
        /// and data storage
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="note">Note</param>
        public QuickNote SaveNote(string id, string note)
        {
            var qnote = new QuickNote { Id = Guid.NewGuid(), Author = author, Note = note };
            var idx = Notes.FindIndex(n => n.Id.ToString() == id);
            
            if (idx >= 0)
                qnote.Id = Notes[idx].Id;

            BlogService.SaveQuickNote(qnote);
            Blog.CurrentInstance.Cache.Remove(cacheKey);
            return qnote;
        }
        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="category">Default post category id</param>
        /// <param name="tags">Default tags</param>
        public void SaveSettings(string category, string tags)
        {
            var cat = new QuickSetting { Author = author, SettingName = "category", SettingValue = category };
            var tag = new QuickSetting { Author = author, SettingName = "tags", SettingValue = tags };
            // save to collection
            this.Settings.Save(cat);
            this.Settings.Save(tag);
            // save to storage
            BlogService.SaveQuickSetting(cat);
            BlogService.SaveQuickSetting(tag);
            // reset cache
            Blog.CurrentInstance.Cache.Remove(cacheKeySettings);
        }

        /// <summary>
        /// Mark note as deleted
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>True if deleted</returns>
        public bool Delete(string id)
        {
            BlogService.DeleteQuickNote(new Guid(id));
            Notes.RemoveAll(n => n.Id.ToString() == id);
            Blog.CurrentInstance.Cache.Remove(cacheKey);
            return true;
        }

        #endregion
    }
}