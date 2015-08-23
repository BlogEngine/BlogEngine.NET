// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Implements an expression builder that provides a strongly-typed reference to a type or member info, instead of a string.
//   Targeted properties still receive a string as input, but the expression is validated during page compilation or execution.
//   If the supplied expression cannot be resolved to a type or member info from an assembly loaded into the current domain
//   then an exception is thrown. Note that type and member name resolution is case-sensitive.
//   Only public types and members can be resolved.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System;
    using System.CodeDom;
    using System.Linq;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.UI;

    /// <summary>
    /// Implements an expression builder that provides a strongly-typed reference to a type or member info, instead of a string.
    ///     Targeted properties still receive a string as input, but the expression is validated during page compilation or execution.
    ///     If the supplied expression cannot be resolved to a type or member info from an assembly loaded into the current domain
    ///     then an exception is thrown. Note that type and member name resolution is case-sensitive.
    ///     Only public types and members can be resolved.
    /// </summary>
    /// <remarks>
    /// &lt;add expressionPrefix="Reflect" type="WebApp.Compilation.ReflectExpressionBuilder, WebApp"/&gt;
    ///     Add the above to the web.config.
    ///     The following examples illustrate the usage of the expression in web pages:
    ///     &lt;asp:ObjectDataSource SelectMethod="&lt;%$ Reflect: ComCard.Components.BusinessObjects.Task, Search %&gt;" ... /&gt;
    ///     This will resolve the type ComCard.Components.BusinessObjects.Task, and validate that Search is a public member on that type.
    ///     The member name will be returned for binding to the property.
    ///     &lt;asp:ObjectDataSource DataObjectTypeName="&lt;%$ Reflect: ComCard.Components.BusinessObjects.LogEntry %&gt;" ... /&gt;
    ///     This will resolve the type ComCard.Components.BusinessObjects.LogEntry. The type name will be returned for binding.
    /// </remarks>
    [ExpressionPrefix("Reflect")]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ReflectExpressionBuilder : ExpressionBuilder
    {
        #region Properties

        /// <summary>
        ///     Gets a flag that indicates whether the expression builder supports no-compile evaluation.
        ///     Returns true, as the target type can be validated at runtime as well.
        /// </summary>
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
        /// Evaluates the expression at runtime.
        /// </summary>
        /// <param name="target">
        /// The target object.
        /// </param>
        /// <param name="entry">
        /// The entry for the property bound to the expression.
        /// </param>
        /// <param name="parsedData">
        /// The parsed expression data.
        /// </param>
        /// <param name="context">
        /// The current expression builder context.
        /// </param>
        /// <returns>
        /// A string representing the target type or member.
        /// </returns>
        public override object EvaluateExpression(
            object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return parsedData;
        }

        /// <summary>
        /// Returns a <see cref="System.CodeDom"/> expression for invoking the expression from a
        ///     compiled page at runtime.
        /// </summary>
        /// <param name="entry">
        /// The entry for the bound property.
        /// </param>
        /// <param name="parsedData">
        /// The parsed expression data.
        /// </param>
        /// <param name="context">
        /// The expression builder context.
        /// </param>
        /// <returns>
        /// A <see cref="CodeExpression"/> for invoking the expression.
        /// </returns>
        public override CodeExpression GetCodeExpression(
            BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return new CodePrimitiveExpression(parsedData);
        }

        /// <summary>
        /// Parses and validates the expression data and returns a canonical type or member name, 
        ///     or throws an exception if the expression is invalid.
        /// </summary>
        /// <param name="expression">
        /// The raw expression to parse.
        /// </param>
        /// <param name="propertyType">
        /// The target property type.
        /// </param>
        /// <param name="context">
        /// Contextual information for the expression builder.
        /// </param>
        /// <returns>
        /// A string representing the target type or member name for binding.
        /// </returns>
        public override object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context)
        {
            var parsed = false;
            string typeName = null;
            string memberName = null;

            if (!String.IsNullOrEmpty(expression))
            {
                var parts = expression.Split(',');
                if (parts.Length > 0 && parts.Length < 3)
                {
                    switch (parts.Length)
                    {
                        case 1:
                            typeName = parts[0].Trim();
                            break;
                        case 2:
                            typeName = parts[0].Trim();
                            memberName = parts[1].Trim();
                            break;
                    }

                    parsed = true;
                }
            }

            if (!parsed)
            {
                throw new HttpException(String.Format("Invalid Reflect expression - '{0}'.", expression));
            }

            // now validate the expression fields
            return ValidateExpression(typeName, memberName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validates that the specified type and member name can be resolved in the current context.
        ///     Member name resolution is optional.
        /// </summary>
        /// <param name="typeName">
        /// The full name of the type.
        /// </param>
        /// <param name="memberName">
        /// The member name to resolve, or null to ignore.
        /// </param>
        /// <returns>
        /// The type / member name as a string for binding to the target property.
        /// </returns>
        private static string ValidateExpression(string typeName, string memberName)
        {
            // resolve type name first
            Type resolvedType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                resolvedType = assembly.GetType(typeName, false, false);
                if (resolvedType != null)
                {
                    break;
                }
            }

            // if type was not resolved then raise error
            if (resolvedType == null)
            {
                var message =
                    String.Format(
                        "Reflect Expression: Type '{0}' could not be resolved in the current context.", typeName);
                throw new HttpCompileException(message);
            }

            // resolve the member name if provided - don't care about multiple matches
            var bindingValue = typeName;
            if (!String.IsNullOrEmpty(memberName))
            {
                bindingValue = memberName;
                if (!resolvedType.GetMember(memberName).Any())
                {
                    var message = String.Format(
                        "Reflect Expression: Member '{0}' for type '{1}' does not exist.", memberName, typeName);
                    throw new HttpCompileException(message);
                }
            }

            return bindingValue;
        }

        #endregion
    }
}