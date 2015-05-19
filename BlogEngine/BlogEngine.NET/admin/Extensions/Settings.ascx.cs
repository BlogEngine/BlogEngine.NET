namespace Admin.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;

    using Resources;

    /// <summary>
    /// The user_controls_xmanager_ parameters.
    /// </summary>
    public partial class UserControlSettings : UserControl
    {
        #region Private members

        /// <summary>
        ///     The settings.
        /// </summary>
        protected ExtensionSettings Settings;

        #endregion

        /// <summary>
        /// Extension name
        /// </summary>
        public string ExtensionName { get; set; }

        /// <summary>
        ///     Gets or sets SettingName.
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        ///     Gets or sets the generate edit button.
        /// </summary>
        public bool GenerateEditButton { get; set; }

        /// <summary>
        ///     Gets or sets the generate delete button.
        /// </summary>
        public bool GenerateDeleteButton { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref = "UserControlSettings" /> class.
        /// </summary>
        public UserControlSettings()
        {
            GenerateDeleteButton = true;
            GenerateEditButton = true;
            SettingName = string.Empty;
            ExtensionName = string.Empty;
        }

        /// <summary>
        /// Dynamically loads form controls or
        ///     data grid and binds data to controls
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);

            ExtensionName = System.Web.HttpUtility.UrlEncode(Request.QueryString["ext"]);
            SettingName = this.ID;
            if (string.IsNullOrEmpty(ExtensionName))
                ExtensionName = SettingName;

            Settings = ExtensionManager.GetSettings(ExtensionName, SettingName);

            GenerateDeleteButton = Settings.ShowDelete;
            GenerateEditButton = Settings.ShowEdit;

            if (Settings.ShowAdd)
            {
                CreateFormFields();
            }

            if (!this.Page.IsPostBack)
            {
                if (this.Settings.IsScalar)
                {
                    this.BindScalar();
                }
                else
                {
                    this.CreateTemplatedGridView();
                    this.BindGrid();
                }
            }

            if (this.Settings.IsScalar)
            {
                this.btnAdd.Text = labels.save;
            }
            else
            {
                if (this.Settings.ShowAdd)
                {
                    this.grid.RowEditing += this.GridRowEditing;
                    this.grid.RowUpdating += this.GridRowUpdating;
                    this.grid.RowCancelingEdit += (o, args) => this.Response.Redirect(this.Request.RawUrl);
                    this.grid.RowDeleting += this.GridRowDeleting;
                    this.btnAdd.Text = labels.add;
                }
                else
                {
                    this.btnAdd.Visible = false;
                }
            }

            this.btnAdd.Click += this.BtnAddClick;
        }

        /// <summary>
        /// Handles the Click event of the btnAdd control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void BtnAddClick(object sender, EventArgs e)
        {
            if (!this.IsValidForm())
            {
                return;
            }

            foreach (Control ctl in this.phAddForm.Controls)
            {
                switch (ctl.GetType().Name)
                {
                    case "TextBox":
                        {
                            var txt = (TextBox)ctl;

                            if (this.Settings.IsScalar)
                            {
                                this.Settings.UpdateScalarValue(txt.ID, txt.Text);
                            }
                            else
                            {
                                this.Settings.AddValue(txt.ID, txt.Text);
                            }
                        }

                        break;
                    case "CheckBox":
                        {
                            var cbx = (CheckBox)ctl;
                            this.Settings.UpdateScalarValue(cbx.ID, cbx.Checked.ToString());
                        }

                        break;
                    case "DropDownList":
                        {
                            var dd = (DropDownList)ctl;
                            this.Settings.UpdateSelectedValue(dd.ID, dd.SelectedValue);
                        }

                        break;
                    case "ListBox":
                        {
                            var lb = (ListBox)ctl;
                            this.Settings.UpdateSelectedValue(lb.ID, lb.SelectedValue);
                        }

                        break;
                    case "RadioButtonList":
                        {
                            var rbl = (RadioButtonList)ctl;
                            this.Settings.UpdateSelectedValue(rbl.ID, rbl.SelectedValue);
                        }

                        break;
                }
            }

            ExtensionManager.SaveSettings(this.ExtensionName, this.Settings);
            if (this.Settings.IsScalar)
            {
                this.InfoMsg.InnerHtml = labels.theValuesSaved;
                this.InfoMsg.Visible = true;
            }
            else
            {
                this.BindGrid();
            }
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
            var paramIndex = ParameterValueIndex(sender, e.RowIndex);

            foreach (var par in this.Settings.Parameters)
            {
                par.DeleteValue(paramIndex);
            }

            ExtensionManager.SaveSettings(this.ExtensionName, this.Settings);
            this.Response.Redirect(this.Request.RawUrl);
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
            // extract and store input values in the collection
            var updateValues = new StringCollection();
            foreach (var txt in (from DataControlFieldCell cel in this.grid.Rows[e.RowIndex].Controls
                                 from Control ctl in cel.Controls
                                 where ctl.GetType().Name == "TextBox"
                                 select ctl).Cast<TextBox>())
            {
                updateValues.Add(txt.Text);
            }

            var paramIndex = ParameterValueIndex(sender, e.RowIndex);

            for (var i = 0; i < this.Settings.Parameters.Count; i++)
            {
                var parName = this.Settings.Parameters[i].Name;
                if (this.Settings.IsRequiredParameter(parName) && string.IsNullOrEmpty(updateValues[i]))
                {
                    // throw error if required field is empty
                    this.ErrorMsg.InnerHtml = string.Format(
                        "\"{0}\" {1}", this.Settings.GetLabel(parName), labels.isRequiredField);
                    this.ErrorMsg.Visible = true;
                    e.Cancel = true;
                    return;
                }

                if (parName == this.Settings.KeyField && this.Settings.IsKeyValueExists(updateValues[i]))
                {
                    // check if key value was changed; if not, it's ok to update
                    if (!this.Settings.IsOldValue(parName, updateValues[i], paramIndex))
                    {
                        // trying to update key field with value that already exists
                        this.ErrorMsg.InnerHtml = string.Format("\"{0}\" {1}", updateValues[i], labels.isAlreadyExists);
                        this.ErrorMsg.Visible = true;
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    this.Settings.Parameters[i].Values[paramIndex] = updateValues[i];
                }
            }

            ExtensionManager.SaveSettings(this.ExtensionName, this.Settings);
            this.Response.Redirect(this.Request.RawUrl);
        }

        /// <summary>
        /// Returns index of the parameter calculated 
        ///     based on the page number and size
        /// </summary>
        /// <param name="sender">
        /// GridView object
        /// </param>
        /// <param name="rowindex">
        /// Index of the row in the grid
        /// </param>
        /// <returns>
        /// Index of the parameter
        /// </returns>
        private static int ParameterValueIndex(object sender, int rowindex)
        {
            var paramIndex = rowindex;
            var gv = (GridView)sender;
            if (gv.PageIndex > 0)
            {
                paramIndex = gv.PageIndex * gv.PageSize + rowindex;
            }

            return paramIndex;
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
        /// Handles the PageIndexChanging event of the grid control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewPageEventArgs"/> instance containing the event data.
        /// </param>
        protected void GridPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grid.PageIndex = e.NewPageIndex;
            this.grid.DataSource = this.Settings.GetDataTable();
            this.grid.DataBind();
        }

        /// <summary>
        /// Binds settings values formatted as
        ///     data table to grid view
        /// </summary>
        private void BindGrid()
        {
            if (this.GenerateEditButton)
            {
                this.grid.AutoGenerateEditButton = true;
            }

            if (this.GenerateDeleteButton)
            {
                this.grid.AutoGenerateDeleteButton = true;
            }

            this.grid.DataKeyNames = new[] { this.Settings.KeyField };
            this.grid.DataSource = this.Settings.GetDataTable();
            this.grid.DataBind();
        }

        /// <summary>
        /// Binds single value parameters
        ///     to text boxes
        /// </summary>
        private void BindScalar()
        {
            foreach (var par in this.Settings.Parameters)
            {
                foreach (Control ctl in this.phAddForm.Controls)
                {
                    switch (ctl.GetType().Name)
                    {
                        case "CheckBox":
                            {
                                var cbx = (CheckBox)ctl;
                                if (cbx.ID.ToLower() == par.Name.ToLower())
                                {
                                    if (par.Values != null && par.Values.Count > 0)
                                    {
                                        cbx.Checked = bool.Parse(par.Values[0]);
                                    }
                                }
                            }

                            break;
                        case "TextBox":
                            {
                                var txt = (TextBox)ctl;
                                if (txt.ID.ToLower() == par.Name.ToLower())
                                {
                                    if (par.Values != null)
                                    {
                                        txt.Text = par.Values.Count == 0 ? string.Empty : par.Values[0];
                                    }
                                }
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates template for data grid view
        /// </summary>
        private void CreateTemplatedGridView()
        {
            foreach (var col in
                this.Settings.Parameters.Select(par => new BoundField { DataField = par.Name, HeaderText = par.Name }))
            {
                this.grid.Columns.Add(col);
            }
        }

        /// <summary>
        /// Dynamically add controls to the form
        /// </summary>
        private void CreateFormFields()
        {
            foreach (var par in this.Settings.Parameters)
            {
                this.ErrorMsg.InnerHtml = string.Empty;
                this.ErrorMsg.Visible = false;
                this.InfoMsg.InnerHtml = string.Empty;
                this.InfoMsg.Visible = false;

                // add label
                if (par.ParamType != ParameterType.Boolean)
                {
                    this.AddLabel(par.Label, string.Empty);
                }

                if (par.ParamType == ParameterType.Boolean)
                {
                    // add checkbox
                    var cb = new CheckBox { Checked = false, ID = par.Name, CssClass = "mgrCheck" };
                    this.phAddForm.Controls.Add(cb);
                    this.AddLabel(par.Label, "mgrCheckLbl");
                }
                else if (par.ParamType == ParameterType.DropDown)
                {
                    // add dropdown
                    var dd = new DropDownList();
                    foreach (var item in par.Values)
                    {
                        string[] parts = item.Split('|');
                        ListItem li = new ListItem();
                        li.Text = parts.Length == 1 ? parts[0] : parts[1];
                        li.Value = parts[0];

                        dd.Items.Add(li);
                    }

                    dd.SelectedValue = par.SelectedValue;
                    dd.ID = par.Name;
                    dd.Width = 250;
                    dd.CssClass = "form-control";
                    this.phAddForm.Controls.Add(dd);
                }
                else if (par.ParamType == ParameterType.ListBox)
                {
                    var lb = new ListBox { Rows = par.Values.Count };
                    foreach (var item in par.Values)
                    {
                        lb.Items.Add(item);
                    }

                    lb.SelectedValue = par.SelectedValue;
                    lb.ID = par.Name;
                    lb.Width = 250;
                    this.phAddForm.Controls.Add(lb);
                }
                else if (par.ParamType == ParameterType.RadioGroup)
                {
                    var rbl = new RadioButtonList();
                    foreach (var item in par.Values)
                    {
                        rbl.Items.Add(item);
                    }

                    rbl.SelectedValue = par.SelectedValue;
                    rbl.ID = par.Name;
                    rbl.RepeatDirection = RepeatDirection.Horizontal;
                    rbl.CssClass = "mgrRadioList";
                    this.phAddForm.Controls.Add(rbl);
                }
                else
                {
                    // add textbox
                    var bx = new TextBox
                        {
                           Text = string.Empty, ID = par.Name, Width = new Unit(250), MaxLength = par.MaxLength, CssClass = "form-control"
                        };
                    this.phAddForm.Controls.Add(bx);
                }

                using (var br2 = new Literal { Text = @"<br />" })
                {
                    this.phAddForm.Controls.Add(br2);
                }
            }
        }

        /// <summary>
        /// Adds the label.
        /// </summary>
        /// <param name="txt">
        /// The text of the label.
        /// </param>
        /// <param name="cls">
        /// The css class.
        /// </param>
        private void AddLabel(string txt, string cls)
        {
            using (var lbl = new Label())
            {
                lbl.Width = new Unit("250");
                lbl.Text = txt;
                if (!string.IsNullOrEmpty(cls))
                {
                    lbl.CssClass = cls;
                }

                this.phAddForm.Controls.Add(lbl);
            }

            using (var br = new Literal { Text = @"<br />" })
            {
                this.phAddForm.Controls.Add(br);
            }
        }

        /// <summary>
        /// Validate the form
        /// </summary>
        /// <returns>
        /// True if valid
        /// </returns>
        private bool IsValidForm()
        {
            var rval = true;
            this.ErrorMsg.InnerHtml = string.Empty;
            foreach (var txt in
                this.phAddForm.Controls.Cast<Control>().Where(ctl => ctl.GetType().Name == "TextBox").Cast<TextBox>())
            {
                // check if required
                if (this.Settings.IsRequiredParameter(txt.ID) && string.IsNullOrEmpty(txt.Text.Trim()))
                {
                    this.ErrorMsg.InnerHtml = string.Format(
                        "\"{0}\" {1}", this.Settings.GetLabel(txt.ID), labels.isRequiredField);
                    this.ErrorMsg.Visible = true;
                    rval = false;
                    break;
                }

                // check data type
                if (!string.IsNullOrEmpty(txt.Text) && !this.ValidateType(txt.ID, txt.Text))
                {
                    this.ErrorMsg.InnerHtml = string.Format(
                        "\"{0}\" must be a {1}", this.Settings.GetLabel(txt.ID), this.Settings.GetParameterType(txt.ID));
                    this.ErrorMsg.Visible = true;
                    rval = false;
                    break;
                }

                if (txt.Text != null &&
                    (!this.Settings.IsScalar &&
                     (this.Settings.KeyField == txt.ID && this.Settings.IsKeyValueExists(txt.Text.Trim()))))
                {
                    this.ErrorMsg.InnerHtml = string.Format("\"{0}\" {1}", txt.Text, labels.isAlreadyExists);
                    this.ErrorMsg.Visible = true;
                    rval = false;
                    break;
                }
            }

            return rval;
        }

        /// <summary>
        /// Validates the type.
        /// </summary>
        /// <param name="parameterName">
        /// Name of the parameter.
        /// </param>
        /// <param name="val">
        /// The value.
        /// </param>
        /// <returns>
        /// The validate type.
        /// </returns>
        protected bool ValidateType(string parameterName, object val)
        {
            var retVal = true;
            try
            {
                switch (this.Settings.GetParameterType(parameterName))
                {
                    case ParameterType.Boolean:
                        bool.Parse(val.ToString());
                        break;
                    case ParameterType.Integer:
                        int.Parse(val.ToString());
                        break;
                    case ParameterType.Long:
                        long.Parse(val.ToString());
                        break;
                    case ParameterType.Float:
                        float.Parse(val.ToString());
                        break;
                    case ParameterType.Double:
                        double.Parse(val.ToString());
                        break;
                    case ParameterType.Decimal:
                        decimal.Parse(val.ToString());
                        break;
                }
            }
            catch (Exception)
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Handles the RowDataBound event of the grid control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.
        /// </param>
        protected void GridRowDataBound(object sender, GridViewRowEventArgs e)
        {
            AddConfirmDelete((GridView)sender, e);
        }

        /// <summary>
        /// Adds confirmation box to delete buttons
        ///     in the data grid
        /// </summary>
        /// <param name="gv">
        /// Data grid view
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        protected static void AddConfirmDelete(GridView gv, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            foreach (var dcf in
                e.Row.Cells.Cast<DataControlFieldCell>().Where(dcf => string.IsNullOrEmpty(dcf.Text.Trim())))
            {
                foreach (var deleteButton in
                    dcf.Controls.Cast<Control>().Select(ctrl => ctrl as LinkButton).Where(
                        deleteButton => deleteButton != null && deleteButton.Text == labels.delete))
                {
                    deleteButton.Attributes.Add(
                        "onClick", string.Format("return confirm('{0}');", labels.areYouSureDeleteRow));
                    break;
                }

                break;
            }
        }
    }
}