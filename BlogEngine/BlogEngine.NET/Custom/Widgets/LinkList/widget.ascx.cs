// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widgets link list_widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.LinkList
{
    using System;
    using System.Web.UI.HtmlControls;
    using System.Xml;

    using App_Code.Controls;

    /// <summary>
    /// The widgets link list_widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not the widget can be edited.
        /// <remarks>
        /// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        /// </remarks>
        /// </summary>
        /// <value></value>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the name. It must be exactly the same as the folder that contains the widget.
        /// </summary>
        /// <value></value>
        public override string Name
        {
            get
            {
                return "LinkList";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        /// data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            var settings = this.GetSettings();
            var doc = new XmlDocument();

            if (settings["content"] != null)
            {
                doc.InnerXml = settings["content"];
            }

            var links = doc.SelectNodes("//link");

            if (links == null)
            {
                return;
            }

            if (links.Count == 0)
            {
                this.ulLinks.Visible = false;
            }
            else
            {
                foreach (XmlNode node in links)
                {
                    var a = new HtmlAnchor();

                    if (node.Attributes != null)
                    {
                        if (node.Attributes["url"] != null)
                        {
                            a.HRef = node.Attributes["url"].InnerText;
                        }

                        if (node.Attributes["title"] != null)
                        {
                            a.InnerText = node.Attributes["title"].InnerText;
                        }

                        if (node.Attributes["newwindow"] != null &&
                            node.Attributes["newwindow"].InnerText.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            a.Target = "_blank";
                        }
                    }

                    var li = new HtmlGenericControl("li");
                    li.Controls.Add(a);
                    this.ulLinks.Controls.Add(li);
                }
            }
        }

        #endregion
    }
}