using System;

namespace BlogEngine.Core.Notes
{
    /// <summary>
    /// Quick Note
    /// </summary>
    public class QuickNote
    {
        /// <summary>
        /// constructor
        /// </summary>
        public QuickNote()
        {
            Id = Guid.NewGuid();
        }
        /// <summary>
        /// Note ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Blog ID
        /// </summary>
        public Guid BlogId { get; set; }
        /// <summary>
        /// Note content
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Date and time updated
        /// </summary>
        public DateTime Updated { get; set; }
        /// <summary>
        /// Note title
        /// </summary>
        public string Title
        {
            get
            {
                var title = Utils.StripHtml(Note);
                return title.Length > 42 ? title.Substring(0, 42) + "..." : title;
            }
        }
    }
}
