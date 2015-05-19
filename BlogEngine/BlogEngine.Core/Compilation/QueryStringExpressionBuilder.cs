// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Query String Expression Builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System.Web;
    using System.Web.Compilation;

    /// <summary>
    /// Query String Expression Builder
    /// </summary>
    [ExpressionPrefix("QueryString")]
    [ExpressionEditor("BlogEngine.Core.Compilation.Design.QueryStringExpressionEditor, BlogEngine.Core")]
    public class QueryStringExpressionBuilder : BaseServerObjectExpressionBuilder
    {
        #region Properties

        /// <summary>
        ///     Gets the name of the source object.
        /// </summary>
        /// <value>The name of the source object.</value>
        public override string SourceObjectName
        {
            get
            {
                return "QueryString";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of this expression builder.
        /// </summary>
        /// <returns>
        /// A new instance of this expression builder.
        /// </returns>
        public static QueryStringExpressionBuilder Instance()
        {
            return new QueryStringExpressionBuilder();
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
        protected override object GetValue(string key)
        {
            return HttpContext.Current.Request.QueryString[key];
        }

        #endregion
    }
}