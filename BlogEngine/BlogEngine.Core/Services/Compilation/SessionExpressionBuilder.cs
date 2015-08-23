// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Session Expression Builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System.Web;
    using System.Web.Compilation;

    /// <summary>
    /// Session Expression Builder
    /// </summary>
    [ExpressionPrefix("Session")]
    [ExpressionEditor("BlogEngine.Core.Compilation.Design.SessionExpressionEditor, BlogEngine.Core")]
    public class SessionExpressionBuilder : BaseServerObjectExpressionBuilder
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
                return "Session";
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
        public static SessionExpressionBuilder Instance()
        {
            return new SessionExpressionBuilder();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">
        /// The key of the value to get.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        protected override object GetValue(string key)
        {
            return HttpContext.Current.Session[key];
        }

        #endregion
    }
}