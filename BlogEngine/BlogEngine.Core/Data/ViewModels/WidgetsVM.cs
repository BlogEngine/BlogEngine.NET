using BlogEngine.Core.DataStore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace BlogEngine.Core.Data.ViewModels
{
    /// <summary>
    /// List of zones and widgets
    /// for a current theme
    /// </summary>
    public class WidgetsVM
    {
        /// <summary>
        /// Theme widtegs
        /// </summary>
        public WidgetsVM()
        {
            try
            {
                var packages = Packaging.FileSystem.LoadWidgets();
                AvailableWidgets = new List<WidgetItem>();
                foreach (var pk in packages)
                {
                    AvailableWidgets.Add(new WidgetItem { Id = pk.Id, Name = pk.Title, Title = pk.Title, ShowTitle = false });
                }

                WidgetZones = new List<WidgetZone>();
                WebClient client = new WebClient();
                var html = client.DownloadString(Utils.AbsoluteWebRoot);
                var cnt = 0;
                var tag = "widgetzone_";

                for (int i = 0; i < 10; i++)
                {
                    int from = html.IndexOf(tag, cnt);
                    if (from > 0)
                    {
                        from = from + 11;
                        int to = html.IndexOf("\"", from);
                        var zoneId = html.Substring(from, to - from);
                        var zone = new WidgetZone();
                        zone.Id = zoneId;

                        var xml = RetrieveXml(zoneId);
                        var wd = new WidgetData { Settings = xml.InnerXml };

                        //------------------------------
                        var widgets = xml.SelectSingleNode("widgets");
                        zone.Widgets = new List<WidgetItem>();
                        if (widgets != null)
                        {
                            foreach (XmlNode node in widgets.ChildNodes)
                            {
                                if (node != null && node.Attributes != null)
                                {
                                    var item = new WidgetItem();
                                    item.Id = node.Attributes["id"].InnerText;
                                    item.Name = node.InnerText;
                                    item.Title = node.Attributes["title"].InnerText;
                                    item.ShowTitle = bool.Parse(node.Attributes["showTitle"].InnerText);
                                    zone.Widgets.Add(item);
                                }
                            }
                        }
                        //------------------------------

                        WidgetZones.Add(zone);
                        cnt = to;
                    }
                    else { break; }
                }
            }
            catch (Exception) { }
        }
        /// <summary>
        /// Available widgets
        /// </summary>
        public List<WidgetItem> AvailableWidgets { get; set; }
        /// <summary>
        /// Widget zones
        /// </summary>
        public List<WidgetZone> WidgetZones { get; set; }
        private static XmlDocument RetrieveXml(string zoneName)
        {
            var ws = new WidgetSettings(zoneName) { SettingsBehavior = new XmlDocumentBehavior() };
            var doc = (XmlDocument)ws.GetSettings();
            return doc;
        }
    }

    /// <summary>
    /// Widget zone
    /// </summary>
    public class WidgetZone
    {
        /// <summary>
        /// Zone ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// List of widgets
        /// </summary>
        public List<WidgetItem> Widgets { get; set; }
    }
    /// <summary>
    /// Widget item
    /// </summary>
    public class WidgetItem
    {
        /// <summary>
        /// Widget Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Widget name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Widget Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Show title
        /// </summary>
        public bool ShowTitle { get; set; }
    }
}
