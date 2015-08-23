// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Code Expression Builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System.CodeDom;
    using System.Web.Compilation;
    using System.Web.UI;

    /// <summary>
    /// Code Expression Builder
    /// </summary>
    /// <example>
    /// page.aspx:
    ///     &lt;asp:CheckBox id="chk1" runat="server" Text='&lt;%$ code: DateTime.Now %&gt;' /&gt;
    /// web.config:
    ///     &lt;compilation debug="true"&gt;
    ///         &lt;expressionBuilders&gt;
    ///             &lt;add expressionPrefix="code" type="WebApp.Compilation.CodeExpressionBuilder, WebApp" /&gt;
    ///         &lt;/expressionBuilders&gt;
    ///     &lt;/compilation&gt;
    /// </example>
    [ExpressionPrefix("Code")]
    [ExpressionEditor("BlogEngine.Core.Compilation.Design.CodeExpressionEditor, BlogEngine.Core")]
    public class CodeExpressionBuilder : ExpressionBuilder
    {
        #region Public Methods

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
            return new CodeSnippetExpression(entry.Expression.Trim());
        }

        #endregion
    }
}