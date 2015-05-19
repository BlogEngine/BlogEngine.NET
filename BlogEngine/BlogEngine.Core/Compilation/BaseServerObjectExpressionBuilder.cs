// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Base Server Object Expression Builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System;
    using System.CodeDom;
    using System.ComponentModel;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.UI;

    /// <summary>
    /// Base Server Object Expression Builder
    /// </summary>
    public abstract class BaseServerObjectExpressionBuilder : ExpressionBuilder
    {
        #region Properties

        /// <summary>
        ///     Gets the name of the source object.
        /// </summary>
        /// <value>The name of the source object.</value>
        public abstract string SourceObjectName { get; }

        /// <summary>
        ///     When overridden in a derived class, returns a value indicating whether the current <see cref = "T:System.Web.Compilation.ExpressionBuilder" /> object supports no-compile pages.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.Compilation.ExpressionBuilder" /> supports expression evaluation; otherwise, false.
        /// </returns>
        public override bool SupportsEvaluate
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, returns an object that represents an evaluated expression.
        /// </summary>
        /// <param name="target">
        /// The object containing the expression.
        /// </param>
        /// <param name="entry">
        /// The object that represents information about the property bound to by the expression.
        /// </param>
        /// <param name="parsedData">
        /// The object containing parsed data as returned by <see cref="M:System.Web.Compilation.ExpressionBuilder.ParseExpression(System.String,System.Type,System.Web.Compilation.ExpressionBuilderContext)"/>.
        /// </param>
        /// <param name="context">
        /// Contextual information for the evaluation of the expression.
        /// </param>
        /// <returns>
        /// An object that represents the evaluated expression; otherwise, null if the inheritor does not implement <see cref="M:System.Web.Compilation.ExpressionBuilder.EvaluateExpression(System.Object,System.Web.UI.BoundPropertyEntry,System.Object,System.Web.Compilation.ExpressionBuilderContext)"/>.
        /// </returns>
        public override object EvaluateExpression(
            object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return this.GetRequestedValue(entry.Expression.Trim(), target.GetType(), entry.PropertyInfo.Name);
        }

        /// <summary>
        /// When overridden in a derived class, returns code that is used during page execution to obtain the evaluated expression.
        /// </summary>
        /// <param name="entry">
        /// The object that represents information about the property bound to by the expression.
        /// </param>
        /// <param name="parsedData">
        /// The object containing parsed data as returned by <see cref="M:System.Web.Compilation.ExpressionBuilder.ParseExpression(System.String,System.Type,System.Web.Compilation.ExpressionBuilderContext)"/>.
        /// </param>
        /// <param name="context">
        /// Contextual information for the evaluation of the expression.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.CodeDom.CodeExpression"/> that is used for property assignment.
        /// </returns>
        public override CodeExpression GetCodeExpression(
            BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            var inputParams = new CodeExpression[]
                {
                    new CodePrimitiveExpression(entry.Expression.Trim()), new CodeTypeOfExpression(entry.DeclaringType), 
                    new CodePrimitiveExpression(entry.PropertyInfo.Name)
                };

            // Return a CodeMethodInvokeExpression that will invoke the GetRequestedValue method using the specified input parameters
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(this.GetType()), "Instance().GetRequestedValue", inputParams);
        }

        /// <summary>
        /// Gets the requested value.
        /// </summary>
        /// <param name="key">
        /// The key of the requested value.
        /// </param>
        /// <param name="targetType">
        /// Type of the target.
        /// </param>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// <returns>
        /// The requested value.
        /// </returns>
        public object GetRequestedValue(string key, Type targetType, string propertyName)
        {
            // First make sure that the server object will be available
            if (HttpContext.Current == null)
            {
                return null;
            }

            // Get the value
            var value = this.GetValue(key);

            // Make sure that the value exists
            if (value == null)
            {
                throw new InvalidOperationException(
                    string.Format("{0} field '{1}' not found.", this.SourceObjectName, key));
            }

            // If the value is being assigned to a control property we may need to convert it
            if (targetType != null)
            {
                var propDesc = TypeDescriptor.GetProperties(targetType)[propertyName];

                // Type mismatch - make sure that the value can be converted
                if (propDesc != null && propDesc.PropertyType != value.GetType() && propDesc.Converter != null)
                {
                    if (propDesc.Converter.CanConvertFrom(value.GetType()) == false)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "{0} value '{1}' cannot be converted to type {2}.",
                                this.SourceObjectName,
                                key,
                                propDesc.PropertyType));
                    }

                    return propDesc.Converter.ConvertFrom(value);
                }
            }

            // If we reach here, no type mismatch - return the value
            return value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">
        /// The key of the value to retrieve.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        protected abstract object GetValue(string key);

        #endregion
    }
}