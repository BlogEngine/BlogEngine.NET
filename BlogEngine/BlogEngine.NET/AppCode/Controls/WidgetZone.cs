using BlogEngine.Core;
using BlogEngine.Core.DataStore;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace App_Code.Controls
{
    /// <summary>
    /// The widget zone.
    /// </summary>
    public class WidgetZone : PlaceHolder
    {
        #region Constants and Fields

        private string zoneName = "be_WIDGET_ZONE";
        static WidgetZone() { }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the data-container used by this instance
        /// </summary>
        public string ZoneName
        {
            get
            {
                return zoneName;
            }

            set
            {
                zoneName = Utils.RemoveIllegalCharacters(value);
            }
        }

        private XmlDocument XmlDocument
        {
            get
            {
                // look up the document by zone name
                return Blog.CurrentInstance.Cache[ZoneName] == null ? null : (XmlDocument)Blog.CurrentInstance.Cache[ZoneName];
            }
        }

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            if (XmlDocument == null)
            {
                var doc = RetrieveXml(ZoneName);
                if (doc != null)
                {
                    Blog.CurrentInstance.Cache[ZoneName] = doc;
                }
            }
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            XmlNodeList zone = null;
            if (XmlDocument != null)
            {
                zone = XmlDocument.SelectNodes("//widget");
            }
            if (zone == null)
            {
                return;
            }
            foreach (XmlNode widget in zone)
            {
                var fileName = string.Format("{0}Custom/Widgets/{1}/widget.cshtml", 
                    Utils.ApplicationRelativeWebRoot, 
                    widget.InnerText);
                try
                {
                    var model = new { Id = widget.Attributes["id"].Value, Title = widget.Attributes["title"].Value };
                    var lit = new Literal { Text = RazorHelpers.ParseRazor(fileName, model) };
                    Controls.Add(lit);
                }
                catch (Exception ex)
                {
                    var lit = new Literal
                    {
                        Text = string.Format("<p style=\"color:red\">Widget {0} not found, check log for details.<p>", widget.InnerText)
                    };
                    Controls.Add(lit);
                    Utils.Log("WidgetZone.OnLoad", ex);
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"widgetzone_{0}\" class=\"widgetzone\">", zoneName);
            base.Render(writer);
            writer.Write("</div>");
        }

        private static XmlDocument RetrieveXml(string zoneName)
        {
            var ws = new WidgetSettings(zoneName) { SettingsBehavior = new XmlDocumentBehavior() };
            var doc = (XmlDocument)ws.GetSettings();
            return doc;
        }

        #endregion
    }
}