// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Server Variable Expression Builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core.Compilation
{
    using System.Web;
    using System.Web.Compilation;

    /// <summary>
    /// Server Variable Expression Builder
    /// </summary>
    [ExpressionPrefix("ServerVariable")]
    [ExpressionEditor("BlogEngine.Core.Compilation.Design.ServerVariableExpressionEditor, BlogEngine.Core")]
    public class ServerVariableExpressionBuilder : BaseServerObjectExpressionBuilder
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
                return "ServerVariable";
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
        public static ServerVariableExpressionBuilder Instance()
        {
            return new ServerVariableExpressionBuilder();
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
            return HttpContext.Current.Request.ServerVariables[key];
        }

        #endregion
    }
}