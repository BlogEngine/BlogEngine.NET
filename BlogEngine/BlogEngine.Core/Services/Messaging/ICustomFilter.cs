namespace BlogEngine.Core
{
    /// <summary>
    /// An interface implemented by anti-spam 
    ///     services like Waegis, Akismet etc.
    /// </summary>
    public interface ICustomFilter
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether the comment should be passed to
        ///     the next custom filter
        /// </summary>
        /// <returns>True if next filter should run</returns>
        bool FallThrough { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if comment is spam
        /// </summary>
        /// <param name="comment">
        /// BlogEngine comment
        /// </param>
        /// <returns>
        /// True if comment is spam
        /// </returns>
        bool Check(Comment comment);

        /// <summary>
        /// Initializes anti-spam service
        /// </summary>
        /// <returns>
        /// True if service online and credentials validated
        /// </returns>
        bool Initialize();

        /// <summary>
        /// Report mistakes back to service
        /// </summary>
        /// <param name="comment">
        /// BlogEngine comment
        /// </param>
        void Report(Comment comment);

        #endregion
    }
}