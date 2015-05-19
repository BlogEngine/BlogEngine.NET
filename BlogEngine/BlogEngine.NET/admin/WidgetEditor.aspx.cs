// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Xml;

    using App_Code.Controls;

    using BlogEngine.Core;
    using BlogEngine.Core.DataStore;
    using BlogEngine.Core.Providers;

    using Resources;

    using Page = System.Web.UI.Page;

    /// <summary>
    /// The widget editor.
    /// </summary>
    public partial class WidgetEditor : Page
    {
        /*
        /// <summary>
        /// The file name.
        /// </summary>
        private static readonly string FileName =
            HostingEnvironment.MapPath(string.Format("{0}widgetzone.xml", BlogSettings.Instance.StorageLocation));
        */
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
            {
                Response.StatusCode = 403;
                Response.Clear();
                Response.End();
            }

            var widget = Request.QueryString["widget"];
            var id = Request.QueryString["id"];
            var move = Request.QueryString["move"];
            var add = Request.QueryString["add"];
            var remove = Request.QueryString["remove"];
            var zone = Request.QueryString["zone"];
            var getMoveItems = Request.QueryString["getmoveitems"];

            if (!string.IsNullOrEmpty(widget) && !string.IsNullOrEmpty(id))
            {
                InitEditor(widget, id, zone);
            }

            if (!string.IsNullOrEmpty(move))
            {
                MoveWidgets(move);
            }

            if (!string.IsNullOrEmpty(add))
            {
                AddWidget(add, zone);
            }

            if (!string.IsNullOrEmpty(remove) && remove.Length == 36)
            {
                RemoveWidget(remove, zone);
            }

            if (!string.IsNullOrEmpty(getMoveItems))
            {
                GetMoveItems(getMoveItems);
            }

            base.OnInit(e);
        }
    
        /// <summary>
        /// Adds a widget of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type of widget.
        /// </param>
        /// <param name="zone">
        /// The zone a widget is being added to.
        /// </param>
        private void AddWidget(string type, string zone)
        {
            var widget =
                (WidgetBase)LoadControl(string.Format("{0}Custom/Widgets/{1}/widget.ascx", Utils.ApplicationRelativeWebRoot, type));
            widget.WidgetId = Guid.NewGuid();
            widget.ID = widget.WidgetId.ToString().Replace("-", string.Empty);
            widget.Title = type;
            widget.Zone = zone;
            widget.ShowTitle = widget.DisplayHeader;
            widget.LoadWidget();

            // Load a widget container for this new widget.  This will return the WidgetContainer
            // with the new widget in it.
            var widgetContainer = WidgetContainer.GetWidgetContainer(widget);
            widgetContainer.RenderContainer();

            // need to manually invoke the loading process, since the Loading process for this Http Request
            // page lifecycle has already fired.
            widgetContainer.ProcessLoad();

            Response.Clear();
            try
            {
                using (var sw = new StringWriter())
                {
                    widgetContainer.RenderControl(new HtmlTextWriter(sw));

                    // Using ? as a delimiter. ? is a safe delimiter because it cannot appear in a
                    // zonename because ? is one of the characters removed by Utils.RemoveIllegalCharacters().
                    Response.Write(string.Format("{0}?{1}", zone, sw));
                }
            }
            catch (HttpException)
            {
                Response.Write("reload");
            }

            SaveNewWidget(widget, zone);
            WidgetEditBase.OnSaved();
            Response.End();
        }

        /// <summary>
        /// Sends a list of the widget zones and containing widgets in the Response.
        /// </summary>
        /// <param name="zoneNames">
        /// The list of zones to retrieve a list of widgets for.
        /// </param>
        private void GetMoveItems(string zoneNames)
        {
            var moveWidgetTo = (string)GetGlobalResourceObject("labels", "moveWidgetTo") ?? string.Empty;

            var sb = new StringBuilder();
            sb.AppendFormat("{{ moveWidgetTo: '{0}', zones: [", moveWidgetTo.Replace("'", "\\'"));
            if (!string.IsNullOrEmpty(zoneNames))
            {
                var firstZone = true;
                var zones = zoneNames.Split(',');
                foreach (var zone in zones)
                {
                    if (!firstZone)
                    {
                        sb.Append(",");
                    }

                    // Zone names won't have single quotation marks in their names, because they were ran
                    // through Utils.RemoveIllegalCharacters().  Therefore, escaping is unnecessary.
                    sb.AppendFormat(" {{ zoneName: '{0}', widgets: [ ", zone);

                    var doc = GetXmlDocument(zone);

                    var firstWidget = true;
                    var widgets = doc.SelectSingleNode("widgets");
                    if (widgets != null)
                    {
                        foreach (XmlNode node in widgets.ChildNodes)
                        {
                            if (!firstWidget)
                            {
                                sb.Append(",");
                            }

                            // Shorten excessively long titles.  Since the title/description is only
                            // used for display purposes in the "move to" dropdown list, this is okay.
                            // Also strip HTML.  The Visitor Widget (for example) has an <img> tag in
                            // its Title by default which causes issues when added an an option to the
                            // dropdown list.
                            if (node.Attributes != null)
                            {
                                var title = Utils.StripHtml(node.Attributes["title"].InnerText);
                                if (title.Length > 20)
                                {
                                    title = string.Format("{0}...", title.Substring(0, 20));
                                }

                                // Need to escape single quotation marks in widget type and widget title.
                                var description = Utils.StripHtml(node.InnerText).Replace("'", "\\'") +
                                                  (!string.IsNullOrEmpty(title) &&
                                                   bool.Parse(node.Attributes["showTitle"].InnerText)
                                                       ? " (" + title.Replace("'", "\\'") + ")"
                                                       : string.Empty);

                                sb.AppendFormat(
                                    " {{ id: '{0}',  desc: '{1}' }} ", node.Attributes["id"].InnerText, description);
                            }

                            firstWidget = false;
                        }
                    }

                    sb.Append(" ] } ");
                    firstZone = false;
                }
            }

            sb.Append(" ] } ");
            Response.Clear();
            Response.Write(sb.ToString());
            Response.End();
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <param name="zone">
        /// The zone Xml Document to get.
        /// </param>
        /// <returns>
        /// An Xml Document.
        /// </returns>
        private XmlDocument GetXmlDocument(string zone)
        {
            XmlDocument doc;
            if (Blog.CurrentInstance.Cache[zone] == null)
            {
                var ws = new WidgetSettings(zone) { SettingsBehavior = new XmlDocumentBehavior() };
                doc = (XmlDocument)ws.GetSettings();
                if (doc.SelectSingleNode("widgets") == null)
                {
                    XmlNode widgets = doc.CreateElement("widgets");
                    doc.AppendChild(widgets);
                }

                Blog.CurrentInstance.Cache[zone] = doc;
            }

            return (XmlDocument)Blog.CurrentInstance.Cache[zone];
        }

        /// <summary>
        /// Inititiates the editor for widget editing.
        /// </summary>
        /// <param name="type">
        /// The type of widget to edit.
        /// </param>
        /// <param name="id">
        /// The id of the particular widget to edit.
        /// </param>
        /// <param name="zone">
        /// The zone the widget to be edited is in.
        /// </param>
        private void InitEditor(string type, string id, string zone)
        {
            var doc = GetXmlDocument(zone);
            var node = doc.SelectSingleNode(string.Format("//widget[@id=\"{0}\"]", id));
            var fileName = string.Format("{0}Custom/Widgets/{1}/edit.ascx", Utils.ApplicationRelativeWebRoot, type);

            if (File.Exists(Server.MapPath(fileName)))
            {
                var edit = (WidgetEditBase)LoadControl(fileName);
                if (node != null && node.Attributes != null)
                {
                    edit.WidgetId = new Guid(node.Attributes["id"].InnerText);
                    edit.Title = node.Attributes["title"].InnerText;
                    edit.ShowTitle = bool.Parse(node.Attributes["showTitle"].InnerText);
                }

                edit.ID = "widget";
                phEdit.Controls.Add(edit);
            }

            if (!Page.IsPostBack)
            {
                if (node != null && node.Attributes != null)
                {
                    cbShowTitle.Checked = bool.Parse(node.Attributes["showTitle"].InnerText);
                    txtTitle.Text = node.Attributes["title"].InnerText;
                }

                txtTitle.Focus();
                btnSave.Text = labels.save;
            }

            btnSave.Click += BtnSaveClick;
        }

        /// <summary>
        /// Moves the widgets as specified while dragging and dropping.
        /// </summary>
        /// <param name="moveData">
        /// Data containing which widget is moving, where it's moving from and where it's moving to.
        /// </param>
        private void MoveWidgets(string moveData)
        {
            var responseData = string.Empty;
            var data = moveData.Split(',');

            if (data.Length == 4)
            {
                var oldZone = data[0];
                var widgetToMoveId = data[1];
                var newZone = data[2];
                var moveBeforeWidgetId = data[3];

                // Ensure widgetToMoveId and moveBeforeWidgetId are not the same.
                if (!widgetToMoveId.Equals(moveBeforeWidgetId))
                {
                    var oldZoneDoc = GetXmlDocument(oldZone);
                    var newZoneDoc = GetXmlDocument(newZone);

                    // If a widget is moving within its own widget, oldZoneDoc and newZoneDoc will
                    // be referencing the same XmlDocument.  This is okay.
                    if (oldZoneDoc != null && newZoneDoc != null)
                    {
                        // Make sure we can find all required elements before moving anything.
                        var widgetToMove =
                            oldZoneDoc.SelectSingleNode(string.Format("//widget[@id=\"{0}\"]", widgetToMoveId));

                        // If a Zone was selected from the dropdown box (rather than a Widget), moveBeforeWidgetId
                        // will be null.  In this case, the widget is moved to the bottom of the new zone.
                        XmlNode moveBeforeWidget = null;
                        if (!string.IsNullOrEmpty(moveBeforeWidgetId))
                        {
                            moveBeforeWidget =
                                newZoneDoc.SelectSingleNode(string.Format("//widget[@id=\"{0}\"]", moveBeforeWidgetId));
                        }

                        if (widgetToMove != null)
                        {
                            // If the XmlNode is moving into a different XmlDocument, need to ImportNode() to
                            // create a copy of the XmlNode that is compatible with the new XmlDocument.
                            var widgetToMoveIntoNewDoc = newZoneDoc.ImportNode(widgetToMove, true);

                            if (widgetToMove.ParentNode != null)
                            {
                                widgetToMove.ParentNode.RemoveChild(widgetToMove);

                                if (moveBeforeWidget == null)
                                {
                                    var widgets = newZoneDoc.SelectSingleNode("widgets");
                                    if (widgets != null)
                                    {
                                        widgets.AppendChild(widgetToMoveIntoNewDoc);
                                    }
                                }
                                else
                                {
                                    if (moveBeforeWidget.ParentNode != null)
                                    {
                                        moveBeforeWidget.ParentNode.InsertBefore(
                                            widgetToMoveIntoNewDoc, moveBeforeWidget);
                                    }
                                }
                            }

                            SaveXmlDocument(oldZoneDoc, oldZone);

                            if (!oldZone.Equals(newZone))
                            {
                                SaveXmlDocument(newZoneDoc, newZone);
                            }

                            WidgetEditBase.OnSaved();

                            // Pass back the same data that was sent in to indicate success.
                            responseData = moveData;
                        }
                    }
                }
            }

            Response.Clear();
            Response.Write(responseData);
            Response.End();
        }

        /// <summary>
        /// Removes the widget from the XML file.
        /// </summary>
        /// <param name="id">
        /// The id of the widget to remove.
        /// </param>
        /// <param name="zone">
        /// The zone a widget is being removed from.
        /// </param>
        private void RemoveWidget(string id, string zone)
        {
            var doc = GetXmlDocument(zone);
            var node = doc.SelectSingleNode(string.Format("//widget[@id=\"{0}\"]", id));
            if (node == null)
            {
                return;
            }

            // remove widget reference in the widget zone
            if (node.ParentNode != null)
            {
                node.ParentNode.RemoveChild(node);
            }

            SaveXmlDocument(doc, zone);

            // remove widget itself
            BlogService.RemoveFromDataStore(ExtensionType.Widget, id);
            Blog.CurrentInstance.Cache.Remove(string.Format("be_widget_{0}", id));

            WidgetEditBase.OnSaved();

            Response.Clear();
            Response.Write(id + zone);
            Response.End();
        }

        /// <summary>
        /// Saves the new widget to the XML file.
        /// </summary>
        /// <param name="widget">
        /// The widget to add.
        /// </param>
        /// <param name="zone">
        /// The zone a widget is being added to.
        /// </param>
        private void SaveNewWidget(WidgetBase widget, string zone)
        {
            var doc = GetXmlDocument(zone);
            XmlNode node = doc.CreateElement("widget");
            node.InnerText = widget.Name;

            var id = doc.CreateAttribute("id");
            id.InnerText = widget.WidgetId.ToString();
            if (node.Attributes != null)
            {
                node.Attributes.Append(id);
            }

            var title = doc.CreateAttribute("title");
            title.InnerText = widget.Title;
            if (node.Attributes != null)
            {
                node.Attributes.Append(title);
            }

            var show = doc.CreateAttribute("showTitle");
            show.InnerText = "True";
            if (node.Attributes != null)
            {
                node.Attributes.Append(show);
            }

            var widgets = doc.SelectSingleNode("widgets");
            if (widgets == null)
            {
                widgets = doc.CreateElement("widgets");
                doc.AppendChild(widgets);
            }
            widgets.AppendChild(node);

            SaveXmlDocument(doc, zone);
        }

        /// <summary>
        /// Saves the XML document.
        /// </summary>
        /// <param name="doc">
        /// The document.
        /// </param>
        /// <param name="zone">
        /// The zone to save the Xml Document for.
        /// </param>
        private void SaveXmlDocument(XmlDocument doc, string zone)
        {
            var ws = new WidgetSettings(zone) { SettingsBehavior = new XmlDocumentBehavior() };
            ws.SaveSettings(doc);
            Blog.CurrentInstance.Cache[zone] = doc;
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void BtnSaveClick(object sender, EventArgs e)
        {
            var widget = (WidgetEditBase)FindControl("widget");
            var zone = Request.QueryString["zone"];

            if (widget != null)
            {
                widget.Save();
            }

            var doc = GetXmlDocument(zone);
            var node = doc.SelectSingleNode(string.Format("//widget[@id=\"{0}\"]", Request.QueryString["id"]));
            var changed = false;

            if (node != null && node.Attributes != null)
            {
                if (node.Attributes["title"].InnerText != txtTitle.Text.Trim())
                {
                    node.Attributes["title"].InnerText = txtTitle.Text.Trim();
                    changed = true;
                }

                if (node.Attributes["showTitle"].InnerText != cbShowTitle.Checked.ToString())
                {
                    node.Attributes["showTitle"].InnerText = cbShowTitle.Checked.ToString();
                    changed = true;
                }
            }

            if (changed)
            {
                SaveXmlDocument(doc, zone);
            }

            WidgetEditBase.OnSaved();
            Blog.CurrentInstance.Cache.Remove(string.Format("widget_{0}", Request.QueryString["id"]));

            // To avoid JS errors with TextBox widget loading tinyMce scripts while
            // the edit window is closing, don't output phEdit.
            phEdit.Visible = false;

            const string Script = "PostEdit();";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "closeWindow", Script, true);
        }

        #endregion
    }
}