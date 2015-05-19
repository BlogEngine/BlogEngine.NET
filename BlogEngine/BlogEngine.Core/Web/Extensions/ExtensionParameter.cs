namespace BlogEngine.Core.Web.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Xml.Serialization;

    /// <summary>
    /// Enumeration for parameter data types
    /// </summary>
    [Flags]
    public enum ParameterType
    {
        /// <summary>
        /// The string.
        /// </summary>
        String,

        /// <summary>
        /// The boolean.
        /// </summary>
        Boolean,

        /// <summary>
        /// The integer.
        /// </summary>
        Integer,

        /// <summary>
        /// The long number.
        /// </summary>
        Long,

        /// <summary>
        /// The float.
        /// </summary>
        Float,

        /// <summary>
        /// The double.
        /// </summary>
        Double,

        /// <summary>
        /// The decimal.
        /// </summary>
        Decimal,

        /// <summary>
        /// The drop down.
        /// </summary>
        DropDown,

        /// <summary>
        /// The list box.
        /// </summary>
        ListBox,

        /// <summary>
        /// The radio group.
        /// </summary>
        RadioGroup
    }

    /// <summary>
    /// Extension Parameter - serializable object
    ///     that holds parameter attributes and collection
    ///     of values
    /// </summary>
    [Serializable]
    public class ExtensionParameter
    {
        #region Constants and Fields

        /// <summary>
        /// The extension parameter name.
        /// </summary>
        private string name = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionParameter"/> class. 
        ///     Default constructor required for serialization
        /// </summary>
        public ExtensionParameter()
        {
            this.SelectedValue = string.Empty;
            this.ParamType = ParameterType.String;
            this.MaxLength = 100;
            this.Label = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionParameter"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// Parameter Name
        /// </param>
        public ExtensionParameter(string name)
        {
            this.SelectedValue = string.Empty;
            this.ParamType = ParameterType.String;
            this.MaxLength = 100;
            this.Label = string.Empty;
            this.name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether is Primary Key field
        /// </summary>
        [XmlElement]
        public bool KeyField { get; set; }

        /// <summary>
        ///     Gets or sets Used as label in the UI controls
        /// </summary>
        [XmlElement]
        public string Label { get; set; }

        /// <summary>
        ///     Gets or sets Maximum number of characters stored in the value fields
        /// </summary>
        [XmlElement]
        public int MaxLength { get; set; }

        /// <summary>
        ///     Gets or sets Parameter Name, often used as ID in the UI
        /// </summary>
        [XmlElement]
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value.Trim().Replace(" ", string.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets Parameter Type
        /// </summary>
        [XmlElement]
        //// [XmlElement(IsNullable = true)]
        public ParameterType ParamType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether values for parameter required
        /// </summary>
        [XmlElement]
        public bool Required { get; set; }

        /// <summary>
        ///     Gets or sets selected value in parameter lists (dropdown, listbox etc.)
        /// </summary>
        [XmlElement]
        public string SelectedValue { get; set; }

        /// <summary>
        ///     Gets or sets a collection of values for given parameter
        /// </summary>
        [XmlElement]
        public StringCollection Values { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add single value to value collection
        /// </summary>
        /// <param name="val">
        /// String Value
        /// </param>
        public void AddValue(string val)
        {
            if (this.Values == null)
            {
                this.Values = new StringCollection();
            }

            this.Values.Add(val);
        }

        /// <summary>
        /// Add single value to collection
        /// </summary>
        /// <param name="val">
        /// Value object
        /// </param>
        public void AddValue(object val)
        {
            if (this.Values == null)
            {
                this.Values = new StringCollection();
            }

            this.Values.Add(val.ToString());
        }

        /// <summary>
        /// Delete value in parameter value collection
        /// </summary>
        /// <param name="rowIndex">
        /// Row Index.
        /// </param>
        public void DeleteValue(int rowIndex)
        {
            this.Values.RemoveAt(rowIndex);
        }

        /// <summary>
        /// Update value for scalar (single value) parameter
        /// </summary>
        /// <param name="val">
        /// Scalar Value
        /// </param>
        public void UpdateScalarValue(string val)
        {
            if (this.Values == null || this.Values.Count == 0)
            {
                this.AddValue(val);
            }
            else
            {
                this.Values[0] = val;
            }
        }

        /// <summary>
        /// Update value for scalar (single value) parameter
        /// </summary>
        /// <param name="val">
        /// Scalar Value
        /// </param>
        public void UpdateScalarValue(object val)
        {
            if (this.Values == null || this.Values.Count == 0)
            {
                this.AddValue(val);
            }
            else
            {
                this.Values[0] = val.ToString();
            }
        }

        #endregion
    }
}