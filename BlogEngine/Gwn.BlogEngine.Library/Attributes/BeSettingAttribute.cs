using System;

namespace Gwn.BlogEngine.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Class)]
    public class BeSettingAttribute : Attribute
    {
        #region Fields
        private readonly string _name;
        private readonly string _label;
        private readonly string _maxLength;
        private readonly string _required;
        private readonly string _keyField;
        private readonly string _parameterType;
        #endregion 

        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return _name; } }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get { return _label; } }
        
        /// <summary>
        /// Gets the length of the max.
        /// </summary>
        /// <value>The length of the max.</value>
        public string MaxLength { get { return _maxLength; } }
        
        /// <summary>
        /// Gets the required.
        /// </summary>
        /// <value>The required.</value>
        public string Required { get { return _required; } }
        
        /// <summary>
        /// Gets the key field.
        /// </summary>
        /// <value>The key field.</value>
        public string KeyField { get { return _keyField; } }
        
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public string ParameterType{get { return _parameterType; }}
        #endregion 

        #region Constructors 
        /// <summary>
        /// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="label">The label.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="required">The required.</param>
        /// <param name="keyField">The key field.</param>
        /// <param name="parType">Type of the par.</param>
        public BeSettingAttribute(string name, string label, string maxLength, string required, string keyField, string parType)
        {
            _name = name;
            _label = label;
            _maxLength = maxLength;
            _required = required;
            _keyField = keyField;
            _parameterType = parType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="label">The label.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="required">The required.</param>
        /// <param name="keyField">The key field.</param>
        public BeSettingAttribute(string name, string label, string maxLength, string required, string keyField)
            : this(name,label,maxLength,required,keyField, null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="label">The label.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="required">The required.</param>
        public BeSettingAttribute(string name, string label, string maxLength, string required)
            : this(name, label, maxLength, required, null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="label">The label.</param>
        /// <param name="maxLength">Length of the max.</param>
        public BeSettingAttribute(string name, string label, string maxLength) 
            : this(name,label,maxLength,null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="label">The label.</param>
        public BeSettingAttribute(string name, string label) 
            : this(name,label,null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BeSettingAttribute(string name)
            :this(name,null){ }
        #endregion 
    }
}
