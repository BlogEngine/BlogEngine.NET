using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace BlogEngine.Core.Data.Services
{
    /// <summary>
    /// Parse Html page output and replace custom fields with appropriate values
    /// </summary>
    public class CustomFieldsParser
    {
        static readonly string _cacheKey = "CACHED_CUSTOM_FIELDS";
        static Regex regBlock = new Regex(@"\[CUSTOMBLOCK.*]", RegexOptions.IgnoreCase);
        static Regex regFields = new Regex(@"\[CUSTOMFIELD.*?]", RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetPageHtml(string html)
        {
            html = RemoveCustomBlocks(html);
            html = ReplaceCustomFields(html);

            return html;
        }

        /// <summary>
        /// Parse string and if it has custom fields markup
        /// return list of custom field objects
        /// </summary>
        /// <param name="html">HTML sring</param>
        /// <returns>List of custom fields</returns>
        public static List<CustomField> GetFieldsFromString(string html)
        {
            var fields = new List<CustomField>();

            foreach (Match m in regFields.Matches(html))
            {
                string[] s = m.Value.Replace("[", "").Replace("/]", "").Split('|');

                if (s.Length == 5)
                {
                    fields.Add(new CustomField { CustomType = s[1], ObjectId = s[2], Key = s[3], Value = s[4] });
                }
            }
            return fields;
        }

        /// <summary>
        /// Lost of custome fields saved to cache
        /// </summary>
        public static IEnumerable<CustomField> CachedFields
        {
            get
            {
                // uncomment this line to disable gallery caching for debugging
                // Blog.CurrentInstance.Cache.Remove(_cacheKey);

                if (Blog.CurrentInstance.Cache[_cacheKey] == null)
                {
                    Blog.CurrentInstance.Cache.Add(
                        _cacheKey,
                        LoadFields(),
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 15, 0),
                        CacheItemPriority.Low,
                        null);
                }
                return (IEnumerable<CustomField>)Blog.CurrentInstance.Cache[_cacheKey];
            }
        }

        /// <summary>
        /// Clear custom fields cache
        /// </summary>
        public static void ClearCache()
        {
            Blog.CurrentInstance.Cache.Remove(_cacheKey);
        }

        #region Private methods

        static string RemoveCustomBlocks(string html)
        {
            foreach (Match m in regBlock.Matches(html))
            {
                html = html.Replace(m.Value, "");
            }
            return html.Replace("[/CUSTOMBLOCK]", "");
        }

        static string ReplaceCustomFields(string html)
        {
            foreach (Match m in regFields.Matches(html))
            {
                string[] s = m.Value.Replace("[", "").Replace("/]", "").Split('|');

                if (s.Length == 5)
                {
                    var postFields = CachedFields.Where(f => f.CustomType.ToLower() == "post");
                    var themeFields = CachedFields.Where(f => f.CustomType.ToLower() == "theme");

                    string id = s[2];
                    string key = s[3];

                    // set default value
                    string val = s[4];

                    if (s[1] == "POST")
                    {
                        var cf = postFields.Where(f => f.Key.ToLower() == key.ToLower()
                            && f.ObjectId.ToLower() == id.ToLower()).FirstOrDefault();

                        if (cf != null)
                            val = cf.Value;
                    }

                    if (s[1] == "THEME")
                    {
                        var cf = themeFields.Where(f => f.Key.ToLower() == key.ToLower()
                            && f.ObjectId.ToLower() == id.ToLower()).FirstOrDefault();

                        if (cf != null)
                            val = cf.Value;
                    }
                    
                    html = html.Replace(m.Value, val);
                }
            }
            return html;
        }

        static List<CustomField> LoadFields()
        {
            var markupFields = new List<CustomField>();

            // read default from templates
            var themeFields = FromThemeTemplates();

            if (themeFields != null && themeFields.Count > 0)
                markupFields.AddRange(themeFields);

            // TODO: post fields

            SaveToRepository(markupFields);

            return FromRepository();
        }

        static List<CustomField> FromThemeTemplates()
        {
            var dirPath = HttpContext.Current.Server.MapPath(
                string.Format("{0}Custom/Themes", Utils.ApplicationRelativeWebRoot));

            var items = new List<CustomField>();

            foreach (var d in Directory.GetDirectories(dirPath))
            {
                if (d.EndsWith("RazorHost"))
                    continue;

                foreach (var f in Directory.GetFiles(d))
                {
                    if (f.Contains(".master", System.StringComparison.OrdinalIgnoreCase)
                        || f.Contains(".ascx", System.StringComparison.OrdinalIgnoreCase)
                        || f.Contains(".cshtml", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var contents = File.ReadAllText(f);

                        if (contents.Contains("[CUSTOMFIELD", System.StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var item in CustomFieldsParser.GetFieldsFromString(contents))
                            {
                                if (FindInCollection(items, item) == null)
                                {
                                    items.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return items;
        }

        static List<CustomField> FromRepository()
        {
            var repoItems = BlogEngine.Core.Providers.BlogService.FillCustomFields();
            return repoItems ?? new List<CustomField>();
        }

        static void SaveToRepository(List<CustomField> markupFields)
        {
            List<CustomField> savedFields = FromRepository();

            // both empty
            if (markupFields.Count == 0 && savedFields.Count == 0)
                return;

            // nothing in markup - return saved
            if (markupFields.Count == 0)
                return;

            // save if not yet in the data store
            foreach (var item in markupFields)
            {
                var oItem = FindInCollection(savedFields, item);
                if (oItem == null)
                {
                    item.BlogId = Blog.CurrentInstance.BlogId;
                    BlogEngine.Core.Providers.BlogService.SaveCustomField(item);
                }
            }
        }

        static CustomField FindInCollection(List<CustomField> items, CustomField item)
        {
            foreach (var i in items)
            {
                if (i.CustomType == item.CustomType && i.ObjectId == item.ObjectId && i.Key == item.Key)
                    return i;
            }
            return null;
        }

        #endregion
    }
}
