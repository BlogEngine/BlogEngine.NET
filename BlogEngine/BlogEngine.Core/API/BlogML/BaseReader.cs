namespace BlogEngine.Core.API.BlogML
{
    /// <summary>
    /// The base reader.
    /// </summary>
    public class BaseReader
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "BaseReader" /> class.
        /// </summary>
        public BaseReader()
        {
            this.Author = string.Empty;
            this.RemoveDuplicates = false;
            this.ApprovedCommentsOnly = false;
            this.Message = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether ApprovedCommentsOnly.
        /// </summary>
        public bool ApprovedCommentsOnly { get; set; }

        /// <summary>
        ///     Gets or sets Author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether RemoveDuplicates.
        /// </summary>
        public bool RemoveDuplicates { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Imports this instance.
        /// </summary>
        /// <returns>
        /// Always returns false.
        /// </returns>
        public virtual bool Import()
        {
            return false;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>
        /// Always returns false.
        /// </returns>
        public virtual bool Validate()
        {
            return false;
        }

        #endregion
    }
}