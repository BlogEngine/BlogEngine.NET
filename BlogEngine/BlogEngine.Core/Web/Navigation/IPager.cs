namespace BlogEngine.Core.Web.Navigation
{
    /// <summary>
    /// Generic list pager
    /// </summary>
    public interface IPager
    {
        /// <summary>
        /// First page
        /// </summary>
        int First { get; }
        /// <summary>
        /// Previous page
        /// </summary>
        int Previous { get; }
        /// <summary>
        /// Show items on the page from
        /// </summary>
        int From { get; }
        /// <summary>
        /// Show items from the page up to
        /// </summary>
        int To { get; }
        /// <summary>
        /// Next page in the list
        /// </summary>
        int Next { get; }
        /// <summary>
        /// Last page in the list
        /// </summary>
        int Last { get; }
        /// <summary>
        /// Renders pager tag as string
        /// </summary>
        string Render(int page, string callback);
    }
}