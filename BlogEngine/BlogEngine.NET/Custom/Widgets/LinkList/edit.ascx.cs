// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widgets link list_edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.LinkList
{
    using System;
    using System.Data;
    using System.Web.UI.WebControls;
    using System.Xml;

    using App_Code.Controls;

    /// <summary>
    /// The widgets link list_edit.
    /// </summary>
    public partial class Edit : WidgetEditBase
    {
        #region Public Methods

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public override void Save()
        {
            var doc = this.Doc();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        
            if (!this.Page.IsPostBack)
            {
                this.BindGrid();
            }

            this.grid.RowEditing += this.GridRowEditing;
            this.grid.RowUpdating += this.GridRowUpdating;
            this.grid.RowCancelingEdit += (o, args) => this.grid.EditIndex = -1;
            this.grid.RowDeleting += this.GridRowDeleting;
            this.btnAdd.Click += this.BtnAddClick;
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            var doc = this.Doc();
            var list = doc.SelectNodes("//link");
            if (list == null || list.Count <= 0)
            {
                return;
            }

            using (var reader = new XmlTextReader(doc.OuterXml, XmlNodeType.Document, null))
            {
                var ds = new DataSet();
                ds.ReadXml(reader);
                this.grid.DataSource = ds;
                this.grid.DataKeyNames = new[] { "id" };
                this.grid.DataBind();
                ds.Dispose();
            }
        }

        /// <summary>
        /// Gets the xml document.
        /// </summary>
        /// <returns>The xml document.</returns>
        private XmlDocument Doc()
        {
            var settings = this.GetSettings();
            var doc = new XmlDocument();
            if (settings["content"] != null)
            {
                doc.InnerXml = settings["content"];
            }

            return doc;
        }

        /// <summary>
        /// Saves the specified xml document.
        /// </summary>
        /// <param name="doc">The xml document.</param>
        private void Save(XmlNode doc)
        {
            var settings = this.GetSettings();
            settings["content"] = doc.InnerXml;
            this.SaveSettings(settings);
        }

        /// <summary>
        /// Handles the Click event of the btnAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnAddClick(object sender, EventArgs e)
        {
            var doc = this.Doc();
            var links = doc.SelectSingleNode("links");
            if (links == null)
            {
                links = doc.CreateElement("links");
                doc.AppendChild(links);
            }

            XmlNode link = doc.CreateElement("link");

            var id = doc.CreateAttribute("id");
            id.InnerText = Guid.NewGuid().ToString();
            if (link.Attributes != null)
            {
                link.Attributes.Append(id);
            }

            var title = doc.CreateAttribute("title");
            title.InnerText = this.txtTitle.Text.Trim();

            if (link.Attributes != null)
            {
                link.Attributes.Append(title);
            }

            var url = doc.CreateAttribute("url");
            url.InnerText = this.txtUrl.Text.Trim();
            if (link.Attributes != null)
            {
                link.Attributes.Append(url);
            }

            var newwindow = doc.CreateAttribute("newwindow");
            newwindow.InnerText = this.cbNewWindow.Checked.ToString();
            if (link.Attributes != null)
            {
                link.Attributes.Append(newwindow);
            }

            links.AppendChild(link);
            this.Save(doc);
            this.BindGrid();
        }

        /// <summary>
        /// Handles the RowDeleting event of the grid control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.
        /// </param>
        private void GridRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var doc = this.Doc();
            var row = this.grid.DataKeys[e.RowIndex];
            if (row != null)
            {
                var id = (string)row.Value;
                var node = doc.SelectSingleNode(string.Format("//link[@id=\"{0}\"]", id));
                if (node == null)
                {
                    return;
                }

                if (node.ParentNode != null)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
            
            this.Save(doc);
            this.BindGrid();
        }

        /// <summary>
        /// Handles the RowEditing event of the grid control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.
        /// </param>
        private void GridRowEditing(object sender, GridViewEditEventArgs e)
        {
            this.grid.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }

        /// <summary>
        /// Handles the RowUpdating event of the grid control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewUpdateEventArgs"/> instance containing the event data.
        /// </param>
        private void GridRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var doc = this.Doc();
            var row = this.grid.DataKeys[e.RowIndex];
            if (row != null)
            {
                var id = (string)row.Value;
                var textboxTitle = (TextBox)this.grid.Rows[e.RowIndex].FindControl("txtTitle");
                var textboxUrl = (TextBox)this.grid.Rows[e.RowIndex].FindControl("txtUrl");
                var checkboxNewWindow = (CheckBox)this.grid.Rows[e.RowIndex].FindControl("cbNewWindow");
                var node = doc.SelectSingleNode(string.Format("//link[@id=\"{0}\"]", id));

                if (node == null)
                {
                    return;
                }

                if (node.Attributes != null)
                {
                    node.Attributes["title"].InnerText = textboxTitle.Text;
                    node.Attributes["url"].InnerText = textboxUrl.Text;
                    node.Attributes["newwindow"].InnerText = checkboxNewWindow.Checked.ToString();
                }
            }

            this.grid.EditIndex = -1;
            this.Save(doc);
            this.BindGrid();
        }

        #endregion
    }
}