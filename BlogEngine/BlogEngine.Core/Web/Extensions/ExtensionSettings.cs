namespace BlogEngine.Core.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializable object that holds collection of
    ///     parameters and methods to manipulate them
    /// </summary>
    [Serializable]
    public class ExtensionSettings
    {
        #region Constants and Fields

        /// <summary>
        /// The required fields.
        /// </summary>
        private readonly StringCollection requiredFields = new StringCollection();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionSettings"/> class. 
        ///     Default constructor requried for serialization
        /// </summary>
        public ExtensionSettings()
        {
            this.ShowEdit = true;
            this.ShowDelete = true;
            this.ShowAdd = true;
            this.Name = string.Empty;
            this.Help = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionSettings"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="extension">
        /// The extension.
        /// </param>
        public ExtensionSettings(object extension)
        {
            this.ShowEdit = true;
            this.ShowDelete = true;
            this.ShowAdd = true;
            this.Help = string.Empty;
            this.Name = extension is string ? (string)extension : extension.GetType().Name;

            this.Delimiter = ",".ToCharArray();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets delimiter used to split string into string array, comma by default
        /// </summary>
        [XmlElement]
        public char[] Delimiter { get; set; }

        /// <summary>
        ///     Gets or sets information extension author can save to describe 
        ///     settings usage. If set, shows up in the settings page
        /// </summary>
        [XmlElement]
        public string Help { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether true, settings section will not show in the settings page
        /// </summary>
        [XmlElement]
        public bool Hidden { get; set; }

        /// <summary>
        ///     Gets or sets the order for loading into admin settings page
        /// </summary>
        [XmlElement]
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether grid view will not show for settings
        ///     and only single text box per parameter will
        ///     be added to the form to update and store input
        /// </summary>
        [XmlElement]
        public bool IsScalar { get; set; }

        /// <summary>
        ///     Gets or sets a field used as primary key for settings.
        ///     If not defined, first parameter in the collection
        ///     set as key field. Unique and required by default.
        /// </summary>
        [XmlElement]
        public string KeyField
        {
            get
            {
                var val = this.Parameters.FirstOrDefault(par => par.KeyField);
                string rval = null;
                if (val != null)
                {
                    rval = val.Name;
                }

                if (string.IsNullOrEmpty(rval) && !IsScalar)
                {
                    rval = this.Parameters[0].Name;
                }

                return rval;
            }

            set
            {
                // Don't bother.
            }
        }

        /// <summary>
        ///     Gets or sets the extension name, same as class name
        /// </summary>
        [XmlElement]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets a collection of parameters defined by extension writer.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public List<ExtensionParameter> Parameters { get; set; }

        /// <summary>
        ///     Gets a collection of required parameters
        /// </summary>
        [XmlIgnore]
        public StringCollection RequiredFields
        {
            get
            {
                this.requiredFields.AddRange(
                    this.Parameters.Where(par => par.Required && !this.requiredFields.Contains(par.Name)).Select(
                        par => par.Name).ToArray());

                // key field is required by default
                if (!this.requiredFields.Contains(this.KeyField))
                {
                    if(KeyField != null)
                        requiredFields.Add(KeyField);
                }

                return this.requiredFields;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show add].
        ///     If false, "add" button will not show on the settings form
        /// </summary>
        /// <value><c>true</c> if [show add]; otherwise, <c>false</c>.</value>
        [XmlElement]
        public bool ShowAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show delete].
        ///     If false, "delete" button will not show in the settings form
        /// </summary>
        /// <value><c>true</c> if [show delete]; otherwise, <c>false</c>.</value>
        [XmlElement]
        public bool ShowDelete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show edit].
        ///     If false, "edit" button will not show in the settings form
        /// </summary>
        /// <value><c>true</c> if [show edit]; otherwise, <c>false</c>.</value>
        [XmlElement]
        public bool ShowEdit { get; set; }

        /// <summary>
        /// Blog ID
        /// </summary>
        public Nullable<Guid> BlogId { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add parameter to settings object by name
        ///     rest of the attributes will be set to defaults
        /// </summary>
        /// <param name="name">
        /// Parameter Name
        /// </param>
        public void AddParameter(string name)
        {
            this.AddParameter(name, name);
        }

        /// <summary>
        /// Add Parameter
        /// </summary>
        /// <param name="name">
        /// Parameter Name
        /// </param>
        /// <param name="label">
        /// Parameter Label
        /// </param>
        /// <param name="maxLength">
        /// Maximum Length
        /// </param>
        /// <param name="required">
        /// Set if value in the parameter required when added/apdated
        /// </param>
        /// <param name="keyfield">
        /// Mark field as primary key, unique and required
        /// </param>
        /// <param name="parType">
        /// Parameter type (integer, string, boolean etc.)
        /// </param>
        public void AddParameter(
            string name, string label, int maxLength = 100, bool required = false, bool keyfield = false, ParameterType parType = ParameterType.String)
        {
            if (this.Parameters == null)
            {
                this.Parameters = new List<ExtensionParameter>();
            }

            var par = new ExtensionParameter(name)
                {
                    Label = label,
                    MaxLength = maxLength,
                    Required = required,
                    KeyField = keyfield,
                    ParamType = parType
                };

            this.Parameters.Add(par);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, string val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, bool val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, int val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, long val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, float val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, double val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Appends value to parameter value collection
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void AddValue(string parameterName, decimal val)
        {
            this.AddObjectValue(parameterName, val);
        }

        /// <summary>
        /// Add value to the list data type
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="items">
        /// List of values
        /// </param>
        /// <param name="selected">
        /// Selected value
        /// </param>
        public void AddValue(string parameterName, string[] items, string selected)
        {
            if (items.Length <= 0)
            {
                return;
            }

            var col = new StringCollection();
            foreach (var t in items)
            {
                col.Add(t);
            }

            this.AddValue(parameterName, col, selected);
        }

        /// <summary>
        /// Add value to the list data type
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="items">
        /// List of values
        /// </param>
        /// <param name="selected">
        /// Selected value
        /// </param>
        public void AddValue(string parameterName, StringCollection items, string selected)
        {
            foreach (var par in this.Parameters.Where(par => par.Name == parameterName))
            {
                par.Values = items;
                par.SelectedValue = selected;
                if (par.ParamType == ParameterType.String)
                {
                    // if string was set as a generic default
                    // for lists reset default to dropdown
                    par.ParamType = ParameterType.DropDown;
                }
            }
        }

        /// <summary>
        /// Add values to parameter value collection
        /// </summary>
        /// <param name="values">
        /// Values as array of strings
        /// </param>
        public void AddValues(string[] values)
        {
            if (this.Parameters.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < this.Parameters.Count; i++)
            {
                if (this.Parameters[i].KeyField && IsKeyValueExists(values[i]))
                {
                    string err = string.Format("Dupliate value of '{0}' not allowed for parameter '{1}'", values[i], this.Parameters[i].Label);
                    Utils.Log(err);

                    throw new ApplicationException(err);   
                }
                this.Parameters[i].AddValue(values[i]);
            }
        }

        /// <summary>
        /// Add values to parameter value collection
        /// </summary>
        /// <param name="values">
        /// String collection of values
        /// </param>
        public void AddValues(StringCollection values)
        {
            if (this.Parameters.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < this.Parameters.Count; i++)
            {
                if (IsKeyValueExists(values[i]))
                {
                    string err = string.Format("Dupliate value of '{0}' not allowed for parameter '{1}'", values[i], this.Parameters[i].Label);
                    Utils.Log(err);

                    throw new ApplicationException(err);   
                }
                this.Parameters[i].AddValue(values[i]);
            }
        }

        /// <summary>
        /// Parameters with multiple values formatted
        ///     as data table where column names are parameter
        ///     names and collection of values are data rows.
        ///     In the UI bound to the data grid view
        /// </summary>
        /// <returns>
        /// Data table
        /// </returns>
        public DataTable GetDataTable()
        {
            var objDataTable = new DataTable();
            foreach (var p in this.Parameters)
            {
                objDataTable.Columns.Add(p.Name, string.Empty.GetType());
            }

            if (this.Parameters[0].Values != null)
            {
                // Protect against the situation where parameters[i].Values.Count have different values
                // This would cause a index out of bounce exception
                var minCount = Int32.MaxValue;
                var changeCount = 0;

                for (var j = 0; j < this.Parameters.Count; j++)
                {
                    if (this.Parameters[j].Values.Count < minCount)
                    {
                        minCount = this.Parameters[j].Values.Count;
                        changeCount++;
                    }
                }

                for (var i = 0; i < minCount; i++)
                {
                    var row = new string[this.Parameters.Count];
                    for (var j = 0; j < this.Parameters.Count; j++)
                    {
                        row[j] = this.Parameters[j].Values[i] ?? "Value not found";
                    }

                    objDataTable.Rows.Add(row);
                }

                if (changeCount > 1)
                {
                    // the parameters arrays have different number of elements -> bad
                    var row = new string[this.Parameters.Count];
                    for (var j = 0; j < this.Parameters.Count; j++)
                    {
                        row[j] = "Incorrect data sets";
                    }

                    objDataTable.Rows.Add(row);
                }
            }

            return objDataTable;
        }

        /// <summary>
        /// Dictionary collection of parameters
        /// </summary>
        /// <returns>
        /// Dictionary object
        /// </returns>
        public Dictionary<string, string> GetDictionary()
        {
            var dic = new Dictionary<string, string>();

            if (this.IsScalar)
            {
                foreach (var par in this.Parameters)
                {
                    dic.Add(par.Name, par.Values[0]);
                }
            }

            return dic;
        }

        /// <summary>
        /// Method to get description of the parameter
        /// </summary>
        /// <param name="parameterName">
        /// Parameter Name
        /// </param>
        /// <returns>
        /// Parameter Description(label)
        /// </returns>
        public string GetLabel(string parameterName)
        {
            var par = this.Parameters.FirstOrDefault(p => p.Name.ToLower() == parameterName.ToLower());
            return par == null ? string.Empty : par.Label;
        }

        /// <summary>
        /// Get parameter type. All valid types defined in the ParameterType enum
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <returns>
        /// Parameter type
        /// </returns>
        public ParameterType GetParameterType(string parameterName)
        {
            return this.Parameters.Where(par => par.Name.ToLower() == parameterName.ToLower()).Select(par => par.ParamType).FirstOrDefault();
        }

        /// <summary>
        /// Method to get vaule for scalar parameter
        /// </summary>
        /// <param name="parameterName">
        /// Parameter Name
        /// </param>
        /// <returns>
        /// First value in the values collection
        /// </returns>
        public string GetSingleValue(string parameterName)
        {
            var p = this.Parameters.FirstOrDefault(
                par => par.Name.ToLower() == parameterName.ToLower() && par.Values != null && par.Values.Count > 0);
            return p == null ? string.Empty : (!string.IsNullOrEmpty(p.SelectedValue) ? p.SelectedValue : p.Values[0]);
        }

        /// <summary>
        /// Returns true if value that user entered
        ///     exists in the parameter used as primary key
        /// </summary>
        /// <param name="newVal">
        /// Value entered
        /// </param>
        /// <returns>
        /// True if value exists
        /// </returns>
        public bool IsKeyValueExists(string newVal)
        {
            return this.Parameters.Where(par => par.Name == this.KeyField && par.Values != null).Any(par => par.Values.Cast<string>().Any(val => val == newVal));
        }

        /// <summary>
        /// Compare value in the parameters collection
        ///     with one in the grid. Return true if value 
        ///     in the grid is the same (old value).
        /// </summary>
        /// <param name="parName">
        /// Parameter Name
        /// </param>
        /// <param name="val">
        /// Value in the grid view
        /// </param>
        /// <param name="rowIndex">
        /// Row in the grid view
        /// </param>
        /// <returns>
        /// The is old value.
        /// </returns>
        public bool IsOldValue(string parName, string val, int rowIndex)
        {
            var oldVal = string.Empty;

            var par = this.Parameters.FirstOrDefault(p => p.Name.ToLower() == parName.ToLower());
            if (par != null)
            {
                if (par.Values != null && rowIndex <= par.Values.Count)
                {
                    oldVal = par.Values[rowIndex];
                }

                if (oldVal == val)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if values in the parameter required
        /// </summary>
        /// <param name="paramName">
        /// Parameter Name
        /// </param>
        /// <returns>
        /// The is required parameter.
        /// </returns>
        public bool IsRequiredParameter(string paramName)
        {
            return this.RequiredFields.Contains(paramName);
        }

        /// <summary>
        /// Set parameter type (int, bool etc.)
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="t">
        /// Parameter type
        /// </param>
        public void SetParameterType(string parameterName, ParameterType t)
        {
            var par = this.Parameters.FirstOrDefault(p => p.Name.ToLower() == parameterName.ToLower());
            if (par != null)
            {
                par.ParamType = t;
            }
        }

        /// <summary>
        /// Update parameter that has only one value
        /// </summary>
        /// <param name="parameterName">
        /// Parameter Name
        /// </param>
        /// <param name="val">
        /// The Value.
        /// </param>
        public void UpdateScalarValue(string parameterName, string val)
        {
            var par = this.Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (par != null)
            {
                par.UpdateScalarValue(val);
            }
        }

        /// <summary>
        /// Updates selected value in the Lists
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        public void UpdateSelectedValue(string parameterName, string val)
        {
            var par = this.Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (par != null)
            {
                par.SelectedValue = val;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add value to the parameter and assign data type
        /// </summary>
        /// <param name="parameterName">
        /// Parameter name
        /// </param>
        /// <param name="val">
        /// Parameter value
        /// </param>
        private void AddObjectValue(string parameterName, object val)
        {
            var par = this.Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (par == null)
            {
                return;
            }

            par.AddValue(val);

            // if set to string by default - check if strong data type
            // was used in "AddValue" method and assign appropriate
            // data type. otherwise leave as it is
            if (par.ParamType != ParameterType.String)
            {
                return;
            }

            switch (val.GetType().Name)
            {
                case "Int32":
                    par.ParamType = ParameterType.Integer;
                    break;
                case "Boolean":
                    par.ParamType = ParameterType.Boolean;
                    break;
                case "Int64":
                    par.ParamType = ParameterType.Long;
                    break;
                case "Float":
                    par.ParamType = ParameterType.Float;
                    break;
                case "Double":
                    par.ParamType = ParameterType.Double;
                    break;
                case "Decimal":
                    par.ParamType = ParameterType.Decimal;
                    break;
            }
        }

        #endregion
    }
}