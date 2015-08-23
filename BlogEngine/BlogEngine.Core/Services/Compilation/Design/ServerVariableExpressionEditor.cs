// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Server Variable Expression Editor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.UI.Design;

    /// <summary>
    /// Server Variable Expression Editor
    /// </summary>
    public class ServerVariableExpressionEditor : ExpressionEditor
    {
        #region Public Methods

        /// <summary>
        /// Evaluates an expression string and provides the design-time value for a control property.
        /// </summary>
        /// <param name="expression">
        /// An expression string to evaluate. The expression does not include the expression prefix.
        /// </param>
        /// <param name="parseTimeData">
        /// An object containing additional parsing information for evaluating <paramref name="expression"/>. This typically is provided by the expression builder.
        /// </param>
        /// <param name="propertyType">
        /// The type of the control property to which <paramref name="expression"/> is bound.
        /// </param>
        /// <param name="serviceProvider">
        /// A service provider implementation supplied by the designer host, used to obtain additional design-time services.
        /// </param>
        /// <returns>
        /// The object referenced by the evaluated expression string, if the expression evaluation succeeded; otherwise, null.
        /// </returns>
        public override object EvaluateExpression(
            string expression, object parseTimeData, Type propertyType, IServiceProvider serviceProvider)
        {
            return string.Concat("[ServerVariable:", expression, "]");
        }

        /// <summary>
        /// Returns an expression editor sheet that is associated with the current expression editor.
        /// </summary>
        /// <param name="expression">
        /// The expression string set for a control property, used to initialize the expression editor sheet.
        /// </param>
        /// <param name="serviceProvider">
        /// A service provider implementation supplied by the designer host, used to obtain additional design-time services.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Web.UI.Design.ExpressionEditorSheet"/> that defines the custom expression properties.
        /// </returns>
        public override ExpressionEditorSheet GetExpressionEditorSheet(
            string expression, IServiceProvider serviceProvider)
        {
            return new ServerVariableExpressionEditorSheet(expression, this, serviceProvider);
        }

        #endregion

        /// <summary>
        /// Server Variable Expression Editor Sheet
        /// </summary>
        private class ServerVariableExpressionEditorSheet : ExpressionEditorSheet
        {
            #region Constants and Fields

            /// <summary>
            /// The owner.
            /// </summary>
            private ServerVariableExpressionEditor owner;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ServerVariableExpressionEditorSheet"/> class.
            /// </summary>
            /// <param name="expression">
            /// The expression.
            /// </param>
            /// <param name="owner">
            /// The owner.
            /// </param>
            /// <param name="serviceProvider">
            /// The service provider.
            /// </param>
            public ServerVariableExpressionEditorSheet(
                string expression, ServerVariableExpressionEditor owner, IServiceProvider serviceProvider)
                : base(serviceProvider)
            {
                this.ServerVariableName = expression;
                this.owner = owner;
            }

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value that indicates whether the expression string is valid.
            /// </summary>
            /// <value></value>
            /// <returns>true, if the expression string is valid; otherwise false.
            /// </returns>
            public override bool IsValid
            {
                get
                {
                    return string.IsNullOrEmpty(this.ServerVariableName) == false;
                }
            }

            /// <summary>
            ///     Gets or sets the name of the server variable.
            /// </summary>
            /// <value>The name of the server variable.</value>
            [DefaultValue(""), TypeConverter(typeof(ServerVariableTypeConverter))]
            public string ServerVariableName { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// When overridden in a derived class, returns the expression string that is formed by the expression editor sheet property values.
            /// </summary>
            /// <returns>
            /// The custom expression string for the current property values.
            /// </returns>
            public override string GetExpression()
            {
                return this.ServerVariableName;
            }

            #endregion

            /// <summary>
            /// Server Variable Type Converter
            /// </summary>
            private class ServerVariableTypeConverter : TypeConverter
            {
                #region Constants and Fields

                /// <summary>
                ///     No Server Variable Setting
                /// </summary>
                private const string NoServerVariableSetting = "(None)";

                #endregion

                #region Public Methods

                /// <summary>
                /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
                /// </param>
                /// <param name="sourceType">
                /// A <see cref="T:System.Type"/> that represents the type you want to convert from.
                /// </param>
                /// <returns>
                /// true if this converter can perform the conversion; otherwise, false.
                /// </returns>
                public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                {
                    return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
                }

                /// <summary>
                /// Returns whether this converter can convert the object to the specified type, using the specified context.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
                /// </param>
                /// <param name="destinationType">
                /// A <see cref="T:System.Type"/> that represents the type you want to convert to.
                /// </param>
                /// <returns>
                /// true if this converter can perform the conversion; otherwise, false.
                /// </returns>
                public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                {
                    return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
                }

                /// <summary>
                /// Converts the given object to the type of this converter, using the specified context and culture information.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
                /// </param>
                /// <param name="culture">
                /// The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.
                /// </param>
                /// <param name="value">
                /// The <see cref="T:System.Object"/> to convert.
                /// </param>
                /// <returns>
                /// An <see cref="T:System.Object"/> that represents the converted value.
                /// </returns>
                /// <exception cref="T:System.NotSupportedException">
                /// The conversion cannot be performed.
                /// </exception>
                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                {
                    return value is string
                               ? (string.Compare(value as string, NoServerVariableSetting, true) == 0
                                      ? string.Empty
                                      : value)
                               : base.ConvertFrom(context, culture, value);
                }

                /// <summary>
                /// Converts the given value object to the specified type, using the specified context and culture information.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
                /// </param>
                /// <param name="culture">
                /// A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.
                /// </param>
                /// <param name="value">
                /// The <see cref="T:System.Object"/> to convert.
                /// </param>
                /// <param name="destinationType">
                /// The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.
                /// </param>
                /// <returns>
                /// An <see cref="T:System.Object"/> that represents the converted value.
                /// </returns>
                /// <exception cref="T:System.ArgumentNullException">
                /// The <paramref name="destinationType"/> parameter is null.
                /// </exception>
                /// <exception cref="T:System.NotSupportedException">
                /// The conversion cannot be performed.
                /// </exception>
                public override object ConvertTo(
                    ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                {
                    return value is string
                               ? ((value as string).Length == 0 ? NoServerVariableSetting : value)
                               : base.ConvertTo(context, culture, value, destinationType);
                }

                /// <summary>
                /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.
                /// </param>
                /// <returns>
                /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
                /// </returns>
                public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    if (context != null)
                    {
                        var serverVariableNames = new List<string>
                            {
                                "ALL_HTTP",
                                "ALL_RAW",
                                "APPL_MD_PATH",
                                "APPL_PHYSICAL_PATH",
                                "AUTH_TYPE",
                                "AUTH_USER",
                                "AUTH_PASSWORD",
                                "LOGON_USER",
                                "REMOTE_USER",
                                "CERT_COOKIE",
                                "CERT_FLAGS",
                                "CERT_ISSUER",
                                "CERT_KEYSIZE",
                                "CERT_SECRETKEYSIZE",
                                "CERT_SERIALNUMBER",
                                "CERT_SERVER_ISSUER",
                                "CERT_SERVER_SUBJECT",
                                "CERT_SUBJECT",
                                "CONTENT_LENGTH",
                                "CONTENT_TYPE",
                                "GATEWAY_INTERFACE",
                                "HTTP_HOST",
                                "HTTP_REFERER",
                                "HTTP_USER_AGENT",
                                "HTTPS",
                                "HTTPS_KEYSIZE",
                                "HTTPS_SECRETKEYSIZE",
                                "HTTPS_SERVER_ISSUER",
                                "HTTPS_SERVER_SUBJECT",
                                "INSTANCE_ID",
                                "INSTANCE_META_PATH",
                                "LOCAL_ADDR",
                                "PATH_INFO",
                                "PATH_TRANSLATED",
                                "QUERY_STRING",
                                "REMOTE_ADDR",
                                "REMOTE_HOST",
                                "REMOTE_PORT",
                                "REQUEST_METHOD",
                                "SCRIPT_NAME",
                                "SERVER_NAME",
                                "SERVER_PORT",
                                "SERVER_PORT_SECURE",
                                "SERVER_PROTOCOL",
                                "SERVER_SOFTWARE",
                                "URL"
                            };

                        return new StandardValuesCollection(serverVariableNames);
                    }

                    return base.GetStandardValues(context);
                }

                /// <summary>
                /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive list of possible values, using the specified context.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
                /// </param>
                /// <returns>
                /// true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exhaustive list of possible values; false if other values are possible.
                /// </returns>
                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return false;
                }

                /// <summary>
                /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
                /// </summary>
                /// <param name="context">
                /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
                /// </param>
                /// <returns>
                /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, false.
                /// </returns>
                public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
                {
                    return true;
                }

                #endregion
            }
        }
    }
}