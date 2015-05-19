namespace BlogEngine.Core.Web.Controls
{
    using System.IO;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// The RewriteFormHtmlTextWriter class implements Form action tag rewriting for rewritten pages 
    ///     on Mono.
    /// </summary>
    public class RewriteFormHtmlTextWriter : HtmlTextWriter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RewriteFormHtmlTextWriter"/> class.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public RewriteFormHtmlTextWriter(Html32TextWriter writer)
            : base(writer)
        {
            this.InnerWriter = writer.InnerWriter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RewriteFormHtmlTextWriter"/> class.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public RewriteFormHtmlTextWriter(TextWriter writer)
            : base(writer)
        {
            this.InnerWriter = writer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the specified markup attribute and value to the output stream, and, if specified, writes the value encoded.
        /// </summary>
        /// <param name="name">
        /// The markup attribute to write to the output stream.
        /// </param>
        /// <param name="value">
        /// The value assigned to the attribute.
        /// </param>
        /// <param name="encode">
        /// true to encode the attribute and its assigned value; otherwise, false.
        /// </param>
        public override void WriteAttribute(string name, string value, bool encode)
        {
            // Mono has issues identifying relative paths when the url is rewritten,
            // so we need to place the full path in the form tag's action attribute
            // or postbacks won't work in rewritten pages.
            if (Utils.IsMono)
            {
                if (name == "action")
                {
                    if (HttpContext.Current.Items["ActionAlreadyWritten"] == null)
                    {
                        value = Utils.AbsoluteWebRoot + value;
                        HttpContext.Current.Items["ActionAlreadyWritten"] = true;
                    }
                }
            }
            else
            {
                if (name == "action")
                {
                    if (HttpContext.Current.Items["ActionAlreadyWritten"] == null)
                    {
                        value = HttpContext.Current.Request.RawUrl;
                        HttpContext.Current.Items["ActionAlreadyWritten"] = true;
                    }
                }
            }

            base.WriteAttribute(name, value, encode);
        }

        #endregion
    }
}