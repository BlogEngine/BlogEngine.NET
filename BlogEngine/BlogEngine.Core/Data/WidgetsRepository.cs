using BlogEngine.Core.Data.ViewModels;
using BlogEngine.Core.Data.Contracts;
using System.Collections.Generic;
using System.Xml;
using BlogEngine.Core.DataStore;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Widgets repository
    /// </summary>
    public class WidgetsRepository : IWidgetsRepository
    {
        /// <summary>
        /// Get theme widgets
        /// </summary>
        /// <returns>Widgets view model</returns>
        public WidgetsVM Get()
        {
            if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
                throw new System.UnauthorizedAccessException();

            return new WidgetsVM();
        }

        /// <summary>
        /// Update widget zones
        /// </summary>
        /// <param name="items">List of zones</param>
        /// <returns>True on success</returns>
        public bool Update(List<WidgetZone> items)
        {
            if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
                throw new System.UnauthorizedAccessException();

            try
            {
                foreach (var zone in items)
                {
                    XmlDocument doc = new XmlDocument();
                    var widgets = doc.CreateElement("widgets");
                    doc.AppendChild(widgets);

                    foreach (var widget in zone.Widgets)
                    {
                        XmlNode node = doc.CreateElement("widget");
                        node.InnerText = widget.Name;

                        var id = doc.CreateAttribute("id");
                        // in javascript, new id initiated as random number
                        // replace with new GUID before saving to back-end
                        if(widget.Id.Length < 30)
                        {
                            widget.Id = System.Guid.NewGuid().ToString();
                        }
                        id.InnerText = widget.Id;
                        node.Attributes.Append(id);

                        var title = doc.CreateAttribute("title");
                        title.InnerText = widget.Title;
                        node.Attributes.Append(title);

                        var show = doc.CreateAttribute("showTitle");
                        show.InnerText = "True";
                        node.Attributes.Append(show);

                        widgets.AppendChild(node);
                    }
                    SaveXmlDocument(doc, zone.Id);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Utils.Log("WidgetsRepository.Update", ex);
                return false;
            }
        }

        private void SaveXmlDocument(XmlDocument doc, string zone)
        {
            var ws = new WidgetSettings(zone) { SettingsBehavior = new XmlDocumentBehavior() };
            ws.SaveSettings(doc);
            Blog.CurrentInstance.Cache[zone] = doc;
        }
    }
}
