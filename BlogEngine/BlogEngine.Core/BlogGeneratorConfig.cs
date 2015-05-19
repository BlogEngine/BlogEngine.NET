using BlogEngine.Core.DataStore;
using System.IO;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace BlogEngine.Core
{
    /// <summary>
    /// Reads default values for a new blog from configuration file
    /// </summary>
    public class BlogGeneratorConfig
    {
        static string TemplateLocation = HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "blogs/_new"));
        static string ConfigFile = "setup.xml";

        /// <summary>
        /// Descripton of the blog
        /// </summary>
        public static string BlogDescription { get; set; }
        /// <summary>
        /// File name of the post
        /// </summary>
        public static string PostsFileName { get; set; }
        /// <summary>
        /// Post title
        /// </summary>
        public static string PostTitle { get; set; }
        /// <summary>
        /// Post content
        /// </summary>
        public static string PostContent { get; set; }

        /// <summary>
        /// Reads widget zone from file in the template folder and serializes it 
        /// as WidgetData to format that can be saved  to database
        /// </summary>
        public static string WidgetZone 
        { 
            get 
            {
                var filePath = Path.Combine(TemplateLocation, "datastore", "widgets", "be_WIDGET_ZONE.xml");
                var content = "";

                var reader = new StreamReader(filePath);
                var x = new XmlSerializer(typeof(XmlDocument));
                var xml = (XmlDocument)x.Deserialize(reader);
                var wd = new WidgetData { Settings = xml.InnerXml };
                var xs = new XmlSerializer(wd.GetType());
                
                using (var sw = new StringWriter())
                {
                    xs.Serialize(sw, wd);
                    content = sw.ToString();
                }
                reader.Close();
                return content;
            } 
        }

        /// <summary>
        /// Default settings from blogs/_new/settings.xml
        /// </summary>
        public static StringDictionary NewBlogSettings
        {
            get
            {
                var filename = Path.Combine(TemplateLocation, "settings.xml");
                var dic = new StringDictionary();

                var doc = new XmlDocument();
                doc.Load(filename);

                var settings = doc.SelectSingleNode("settings");
                if (settings != null)
                {
                    foreach (XmlNode settingsNode in settings.ChildNodes)
                    {
                        var name = settingsNode.Name;
                        var value = settingsNode.InnerText;

                        dic.Add(name, value);
                    }
                }
                return dic;
            }
        }

        /// <summary>
        /// Sets default values for new blog
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>True if successful</returns>
        public static bool SetDefaults(out string message)
        {
            message = "";

            if (!Directory.Exists(TemplateLocation))
            {
                message = "Template directory does not exist";
                return false;
            }

            string file = Path.Combine(TemplateLocation, ConfigFile);

            if (!File.Exists(file))
            {
                CreateConfig(file);
            }

            Init(file);

            return true;
        }

        /// <summary>
        /// Initiates properties by reading values from config file
        /// </summary>
        /// <param name="fileName">File name</param>
        static void Init(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);

            if (doc.SelectSingleNode("//Settings//Description") != null)
            {
                BlogDescription = doc.SelectSingleNode("//Settings//Description").InnerText;
            }

            if (doc.SelectSingleNode("//Posts//FileName") != null)
            {
                PostsFileName = doc.SelectSingleNode("//Posts//FileName").InnerText;
            }

            if (doc.SelectSingleNode("//Posts//Title") != null)
            {
                PostTitle = doc.SelectSingleNode("//Posts//Title").InnerText;
            }

            if (doc.SelectSingleNode("//Posts//Post") != null)
            {
                PostContent = doc.SelectSingleNode("//Posts//Post").InnerText;
            }
        }

        /// <summary>
        /// Generates conig file if one does not exist
        /// </summary>
        /// <param name="fileName">File name</param>
        static void CreateConfig(string fileName)
        {
            var settings = new XmlWriterSettings { Indent = true };
            var blogPost = "Dear {0}, welcome to you new blog! &lt;br/&gt;&lt;br/&gt;Log in &lt;a href=\"{1}\"&gt;here&lt;/a&gt; to start using it. &lt;br/&gt;&lt;br/&gt;Happy blogging!";

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("NewBlog");

                writer.WriteStartElement("Settings");
                writer.WriteElementString("Description", "Short description of the blog");
                writer.WriteEndElement();

                writer.WriteStartElement("Posts");
                writer.WriteElementString("FileName", "c3b491e5-59ac-4f6a-81e5-27e971b903ed.xml");
                writer.WriteElementString("Title", "Welcome to {0}'s blog");
                writer.WriteElementString("Post", blogPost);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}
