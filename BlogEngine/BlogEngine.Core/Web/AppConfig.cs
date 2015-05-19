using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Xml;

namespace BlogEngine.Core.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfig : AppSettingsReader
    {
        private XmlDocument cfgDoc = new XmlDocument();
        public string docName = "web.config";
        private XmlNode node = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AppConfig Instance()
        {
            return new AppConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, string value)
        {
            object _lock = new object();


            lock (_lock)
            {
                loadConfigDoc();
                // retrieve the appSettings node 
                node = cfgDoc.SelectSingleNode("//appSettings");

                if (node == null)
                {
                    throw new InvalidOperationException("appSettings section not found");
                }

                try
                {
                    // XPath select setting "add" element that contains this key    
                    XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + key + "']");
                    if (addElem != null)
                    {
                        addElem.SetAttribute("value", value);
                    }
                    // not found, so we need to add the element, key and value
                    else
                    {
                        XmlElement entry = cfgDoc.CreateElement("add");
                        entry.SetAttribute("key", key);
                        entry.SetAttribute("value", value);
                        node.AppendChild(entry);
                    }
                    //save it
                    saveConfigDoc(docName);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfgDocPath"></param>
        private void saveConfigDoc(string cfgDocPath)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(cfgDocPath, null);
                writer.Formatting = Formatting.Indented;
                cfgDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                return;
            }
            catch (Exception e)
            {
                throw new FileLoadException("Unable to load the web.config file for modification", e.InnerException);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementKey"></param>
        /// <returns></returns>
        public bool removeElement(string elementKey)
        {
            try
            {
                loadConfigDoc();
                // retrieve the appSettings node 
                node = cfgDoc.SelectSingleNode("//appSettings");
                if (node == null)
                {
                    throw new InvalidOperationException("appSettings section not found");
                }
                // XPath select setting "add" element that contains this key to remove   
                node.RemoveChild(node.SelectSingleNode("//add[@key='" + elementKey + "']"));

                saveConfigDoc(docName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void loadConfigDoc()
        {
            // load the config file 
            docName = HttpContext.Current.Server.MapPath(docName);
            cfgDoc.Load(docName);
            return;
        }
    }
}