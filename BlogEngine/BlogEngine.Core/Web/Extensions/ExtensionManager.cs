namespace BlogEngine.Core.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Xml.Serialization;

    using BlogEngine.Core.Web.Controls;

    /// <summary>
    /// Extension Manager - top level object in the hierarchy
    ///     Holds collection of extensions and methods to manipulate
    ///     extensions
    /// </summary>
    [XmlRoot]
    public class ExtensionManager
    {
        #region Constants and Fields

        /// <summary>
        ///     The extensions.
        /// </summary>
		private static Dictionary<string, ManagedExtension> extensions = new Dictionary<string, ManagedExtension>();

        /// <summary>
        ///     The new extensions.
        /// </summary>
        private static StringCollection newExtensions = new StringCollection();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a collection of extensions
        /// </summary>
        [XmlElement]
		public static Dictionary<string, ManagedExtension> Extensions
        {
            get { return extensions; }
        }

        /// <summary>
        ///     Gets or sets an exeption thrown when extension can not be serialized because of
        ///     file access permission. Not serializable, used by UI to show error message.
        /// </summary>
        [XmlIgnore]
        public static Exception FileAccessException { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to change extension status
        /// </summary>
        /// <param name="extension">
        /// Extensio Name
        /// </param>
        /// <param name="enabled">
        /// If true, enables extension
        /// </param>
        public static void ChangeStatus(string extension, bool enabled)
        {
            foreach (var x in extensions.Where(x => x.Key == extension))
            {
                x.Value.Enabled = enabled;
                var xs = new DataStore.ExtensionSettings(x.Key);
                xs.SaveSettings(x.Value);
                SaveToCache();
                break;
            }
            Utils.RecycleIIS();
        }

        /// <summary>
        /// Tell if manager already has this extension
        /// </summary>
        /// <param name="type">
        /// Extension Type
        /// </param>
        /// <returns>
        /// True if already has
        /// </returns>
        public static bool Contains(Type type)
        {
            return extensions.Any(extension => extension.Key == type.Name);
        }

        /// <summary>
        /// Enabled / Disabled
        /// </summary>
        /// <param name="extensionName">
        /// Name of the extension.
        /// </param>
        /// <returns>
        /// True if enabled
        /// </returns>
        public static bool ExtensionEnabled(string extensionName)
        {
            LoadExtensions();
            ManagedExtension extension = GetExtension(extensionName);

            if (Blog.CurrentInstance.IsPrimary)
            {
                return extension == null ? false : extension.Enabled;
            }
            else
            {
                if (extension != null)
                {
                    // if the extension is disabled at the primary blog, then
                    // it is automatically considered disabled for all child blogs.
                    if (!extension.Enabled) { return false; }

                    if (extension.Blogs != null && extension.Blogs.Contains(Blog.CurrentInstance.Id))
                    {
                        // when non-primary blog disables extension,
                        // this blog added to Blogs list for this extension
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Returns extension to the client based on name
        /// </summary>
        /// <param name="extensionName">
        /// Extension name
        /// </param>
        /// <returns>
        /// Extension object or null
        /// </returns>
        public static ManagedExtension GetExtension(string extensionName)
        {
            ManagedExtension extension;
            extensions.TryGetValue(extensionName, out extension);
            return extension;
        }

        /// <summary>
        /// Returns settings for specified extension
        /// </summary>
        /// <param name="extensionName">
        /// Extension Name
        /// </param>
        /// <param name="settingName">
        /// Settings Name
        /// </param>
        /// <returns>
        /// Settings object
        /// </returns>
        public static ExtensionSettings GetSettings(string extensionName, string settingName = "")
        {
            var extension = GetExtension(extensionName);
            if (extension == null)
                return null;

            if (string.IsNullOrEmpty(settingName))
                settingName = extensionName;

            if (!Blog.CurrentInstance.IsPrimary && extension.SubBlogEnabled)
            {
                return extension.Settings.FirstOrDefault(setting => setting != null
                    && setting.Name == settingName
                    && setting.BlogId == Blog.CurrentInstance.Id);
            }

            var primId = Blog.Blogs.FirstOrDefault(b => b.IsPrimary).BlogId;
            return extension.Settings.FirstOrDefault(setting => setting != null
                    && setting.Name == settingName
                    && (setting.BlogId == primId || setting.BlogId == null));
        }

        /// <summary>
        /// Do initial import here.
        ///     If already imported, let extension manager take care of settings
        ///     To reset, blogger has to delete all settings in the manager
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The import settings.
        /// </returns>
        public static bool ImportSettings(ExtensionSettings settings)
        {
            return ImportSettings(settings.Name, settings);
        }

        /// <summary>
        /// Imports the settings.
        /// </summary>
        /// <param name="extensionName">
        /// Name of the extension.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// If import settings works.
        /// </returns>
        public static bool ImportSettings(string extensionName, ExtensionSettings settings)
        {
            var ext = extensions.FirstOrDefault(x => x.Key == extensionName && !x.Value.Initialized(settings));
            if (ext.Value != null)
            {
                ext.Value.InitializeSettings(settings);
            }

            SaveToCache();

            // return SaveToStorage();
            return true;
        }

        /// <summary>
        /// Initializes settings by importing default parameters
        /// </summary>
        /// <param name="extensionName">
        /// Extension Name
        /// </param>
        /// <param name="settings">
        /// Settings object
        /// </param>
        /// <returns>
        /// The Extension Settings
        /// </returns>
        public static ExtensionSettings InitSettings(string extensionName, ExtensionSettings settings)
        {
            ImportSettings(extensionName, settings);
            return GetSettings(extensionName, settings.Name);
        }

        /// <summary>
        /// Will serialize and cache ext. mgr. object
        /// </summary>
        public static void Save()
        {
            SaveToStorage();
            SaveToCache();
        }

        /// <summary>
        /// Will save settings (add to extension object, then
        ///     cache and serialize all object hierarhy to XML)
        /// </summary>
        /// <param name="settings">
        /// Settings object
        /// </param>
        public static void SaveSettings(ExtensionSettings settings)
        {
            SaveSettings(settings.Name, settings);
        }

        /// <summary>
        /// The save settings.
        /// </summary>
        /// <param name="extensionName">
        /// The extension name.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public static void SaveSettings(string extensionName, ExtensionSettings settings)
        {
            var ext = extensions.FirstOrDefault(x => x.Key == extensionName);
            if (ext.Value != null)
            {
                ext.Value.SaveSettings(settings);
            }

            Save();
        }

        /// <summary>
        /// Saves ext. manager object to XML file
        ///     or database table using provider model
        /// </summary>
        /// <returns>
        /// True if successful
        /// </returns>
        public static bool SaveToStorage()
        {
            foreach (var ext in extensions)
            {
                var xs = new DataStore.ExtensionSettings(ext.Key);
                xs.SaveSettings(ext.Value);
            }

            return true;
        }

        /// <summary>
        /// Save individual extension to storage
        /// </summary>
        /// <param name="ext">
        /// The Extension
        /// </param>
        /// <returns>
        /// True if saved
        /// </returns>
        public static bool SaveToStorage(ManagedExtension ext)
        {
            var xs = new DataStore.ExtensionSettings(ext.Name);
            xs.SaveSettings(ext);
            return true;
        }

        /// <summary>
        /// A way to let extension author to use custom
        ///     admin page. Will show up as link on extensions page
        /// </summary>
        /// <param name="extension">
        /// Extension Name
        /// </param>
        /// <param name="url">
        /// Path to custom admin page
        /// </param>
        public static void SetAdminPage(string extension, string url)
        {
            var x = extensions.FirstOrDefault(ext => ext.Key == extension);
            if (x.Value == null)
            {
                return;
            }

            x.Value.AdminPage = url;
            SaveToStorage();
            SaveToCache();
        }

        /// <summary>
        /// Only change status on first load;
        ///     This allows to enable/disable extension on
        ///     initial load and then be able to override it with
        ///     change status from admin interface
        /// </summary>
        /// <param name="extension">
        /// Extension Name
        /// </param>
        /// <param name="enabled">
        /// Enable/disable extension on initial load
        /// </param>
        public static void SetStatus(string extension, bool enabled)
        {
            if (IsNewExtension(extension))
            {
                ChangeStatus(extension, enabled);
            }
        }

        /// <summary>
        /// Show of hide settings in the admin/extensions list
        /// </summary>
        /// <param name="extensionName">
        /// Extension name
        /// </param>
        /// <param name="flag">
        /// True of false
        /// </param>
        public static void ShowSettings(string extensionName, bool flag)
        {
            var extension = extensions.FirstOrDefault(ext => ext.Key == extensionName);
            if (extension.Value == null)
            {
                return;
            }

            extension.Value.ShowSettings = flag;
            Save();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns extension object
        /// </summary>
        /// <param name="name">
        /// Extension name
        /// </param>
        /// <returns>
        /// The Extension
        /// </returns>
        private static ManagedExtension DataStoreExtension(string name)
        {
            ManagedExtension ex = null;
            var xs = new DataStore.ExtensionSettings(name);
            var serializer = new XmlSerializer(typeof(ManagedExtension));
            var o = xs.GetSettings();

            if (o != null)
            {
                if (o.GetType().Name == "FileStream")
                {
                    Stream stm = (FileStream)o;
                    ex = (ManagedExtension)serializer.Deserialize(stm);
                    stm.Close();
                }
                else
                {
                    if (!string.IsNullOrEmpty((string)o))
                    {
                        using (var reader = new StringReader(o.ToString()))
                        {
                            ex = (ManagedExtension)serializer.Deserialize(reader);
                        }
                    }
                }
            }

            return ex;
        }

        /// <summary>
        /// Extension is "new" if it is loaded from assembly
        ///     but not yet saved to the disk. This state is needed
        ///     so that we can initialize extension and its settings
        ///     on the first load and then override it from admin
        /// </summary>
        /// <param name="name">
        /// Extension name
        /// </param>
        /// <returns>
        /// True if new
        /// </returns>
        private static bool IsNewExtension(string name)
        {
            return newExtensions.Contains(name);
        }

        /// <summary>
        /// If extensions not in the cache will load
        ///     from the XML file. If file not exists
        ///     will load from assembly using reflection
        /// </summary>
        private static void LoadExtensions()
        {
            if (HttpRuntime.Cache["Extensions"] != null &&
                ((Dictionary<string, ManagedExtension>)HttpRuntime.Cache["Extensions"]).Count != 0)
            {
                return;
            }

            var codeAssemblies = Utils.CodeAssemblies();

            var meta = DataStoreExtension("MetaExtension");
            if (meta == null)
            {
                extensions.Add("MetaExtension", new ManagedExtension("MetaExtension", "1.0", "Meta extension", "BlogEngine.net"));
            }
            else
            {
				if (!extensions.ContainsKey("MetaExtension"))
                {
                    extensions.Add("MetaExtension", meta);
                }
            }

            foreach (Assembly a in codeAssemblies)
            {
                var types = a.GetTypes();
                foreach (var type in types)
                {
                    var attributes = type.GetCustomAttributes(typeof(ExtensionAttribute), false);
                    foreach (var xa in attributes.Cast<ExtensionAttribute>())
                    {
                        // try to load from storage
                        try
                        {
                            var x = DataStoreExtension(type.Name);

                            // if nothing, crete new extension
                            if (x == null)
                            {
                                x = new ManagedExtension(type.Name, xa.Version, xa.Description, xa.Author);
                                newExtensions.Add(type.Name);
                                SaveToStorage(x);
                            }
                            else
                            {
                                // update attributes from assembly
                                x.Version = xa.Version;
                                x.Description = xa.Description;
                                x.Author = xa.Author;

                                if (x.Priority == 0)
                                {
                                    x.Priority = xa.Priority;
                                }

                                if (!x.SubBlogEnabled)
                                {
                                    x.SubBlogEnabled = xa.SubBlogEnabled;
                                }
                            }
							if (!extensions.ContainsKey(x.Name))
								extensions.Add(x.Name, x);
                        }
                        catch (Exception e)
                        {
                            Utils.Log($"Can not load {type.Name}: {e.Message}");
                        }
                    }
                }
            }

            // SaveToStorage();
            SaveToCache();
        }

        /// <summary>
        /// Caches for performance. If manager cached
        ///     and not updates done, chached copy always 
        ///     returned
        /// </summary>
        private static void SaveToCache()
        {
            HttpRuntime.Cache.Remove("Extensions");
            HttpRuntime.Cache["Extensions"] = extensions;
        }

        #endregion
    }
}