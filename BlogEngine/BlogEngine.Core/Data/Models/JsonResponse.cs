namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// JSON response.
    /// </summary>
    public class JsonResponse
    {
        #region Properties

        /// <summary>
        ///     Gets or sets Data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        ///     Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Success.
        /// </summary>
        public bool Success { get; set; }

        #endregion
    }

    /// <summary>
    /// JSON response with a strongly typed data.
    /// </summary>
    public class JsonResponse<T>
    {
        #region Properties

        /// <summary>
        ///     Gets or sets Data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///     Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Success.
        /// </summary>
        public bool Success { get; set; }

        #endregion
    }
}
