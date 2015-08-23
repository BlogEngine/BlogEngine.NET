// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Session Expression Editor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation.Design
{
    using System;
    using System.Web.UI.Design;

    /// <summary>
    /// Session Expression Editor
    /// </summary>
    public class SessionExpressionEditor : ExpressionEditor
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
            return string.Concat("[Session:", expression, "]");
        }

        #endregion
    }
}