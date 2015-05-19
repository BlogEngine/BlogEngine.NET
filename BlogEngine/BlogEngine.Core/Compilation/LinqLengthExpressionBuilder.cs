// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Linq Length Expression Builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System;
    using System.CodeDom;
    using System.Data.Linq.Mapping;
    using System.Linq;
    using System.Web.Compilation;
    using System.Web.UI;

    /// <summary>
    /// Linq Length Expression Builder
    /// </summary>
    /// <remarks>
    /// The example usage below uses an Employee table with a Name column. Employee may need to be namespace referenced
    ///     like improvGroup.Manager.Data.Employee if the namespace of the page is different than the data entity namespace.
    /// </remarks>
    /// <example>
    /// &lt;asp:TextBox ID="TextBoxName" runat="server" MaxLength='&lt;%$LinqLength:Employee.Name%&gt;' /&gt;
    /// </example>
    [ExpressionPrefix("LinqLength")]
    public class LinqLengthExpressionBuilder : ExpressionBuilder
    {
        #region Public Methods

        /// <summary>
        /// Gets the code expression.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <param name="parsedData">
        /// The parsed data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// A CodeExpression.
        /// </returns>
        public override CodeExpression GetCodeExpression(
            BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            var typeName = BeforeLast(entry.Expression, ".");
            var propertyName = AfterLast(entry.Expression, ".");

            return new CodePrimitiveExpression(PropertyLength(typeName, propertyName));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the end of the string after the last instance of the last.
        /// </summary>
        /// <param name="value">
        /// The string value.
        /// </param>
        /// <param name="last">
        /// The value to search for.
        /// </param>
        /// <returns>
        /// The string after the last instance of last.
        /// </returns>
        private static string AfterLast(string value, string last)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return value.Substring(value.LastIndexOf(last) + last.Length);
        }

        /// <summary>
        /// Get the string up to the last instance of last.
        /// </summary>
        /// <param name="value">
        /// The string value.
        /// </param>
        /// <param name="last">
        /// The instance to search for the last of.
        /// </param>
        /// <returns>
        /// The string up to the last instance of last.
        /// </returns>
        private static string BeforeLast(string value, string last)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (last == null)
            {
                throw new ArgumentNullException("last");
            }

            return value.Substring(0, value.LastIndexOf(last));
        }

        /// <summary>
        /// Gets the string value between the start and the end.
        /// </summary>
        /// <param name="value">
        /// The value to search.
        /// </param>
        /// <param name="startText">
        /// The start text to skip.
        /// </param>
        /// <param name="endText">
        /// The end text to skip.
        /// </param>
        /// <returns>
        /// The middle of the string between start and end.
        /// </returns>
        private static string BetweenFirst(string value, string startText, string endText)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var start = value.IndexOf(startText, StringComparison.OrdinalIgnoreCase) + startText.Length;
            var end = value.IndexOf(endText, start, StringComparison.OrdinalIgnoreCase);

            return value.Substring(start, end - start);
        }

        /// <summary>
        /// Gets the length of the property.
        /// </summary>
        /// <param name="typeName">
        /// Name of the type.
        /// </param>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// <returns>
        /// The length of the property.
        /// </returns>
        private static int PropertyLength(string typeName, string propertyName)
        {
            var attribute =
                (ColumnAttribute)
                BuildManager.GetType(typeName, true).GetProperty(propertyName).GetCustomAttributes(
                    typeof(ColumnAttribute), false).Single();
                
                // <-- look, I just had to use a tiny bit of LINQ in this code!
            return int.Parse(BetweenFirst(attribute.DbType, "char(", ")"));
        }

        #endregion
    }
}