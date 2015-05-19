namespace BlogEngine.Core.API.MetaWeblog
{
    /// <summary>
    /// MetaWeblog Fault struct
    ///     returned when error occurs
    /// </summary>
    internal struct MWAFault
    {
        #region Constants and Fields

        /// <summary>
        ///     Error code of Fault Response
        /// </summary>
        public string faultCode;

        /// <summary>
        ///     Message of Fault Response
        /// </summary>
        public string faultString;

        #endregion
    }
}