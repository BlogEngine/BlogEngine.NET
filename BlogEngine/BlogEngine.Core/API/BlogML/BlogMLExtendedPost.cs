namespace BlogEngine.Core.API.BlogML
{
    using global::BlogML.Xml;
    using System.Collections.Generic;

    /// <summary>
    /// Extended BlogML post
    /// </summary>
    public class BlogMlExtendedPost
    {
        /// <summary>
        /// Gets or sets blog post
        /// </summary>
        public BlogMLPost BlogPost { get; set; }

        /// <summary>
        /// Gets or sets post URL
        /// </summary>
        public string PostUrl { get; set; }
        
        /// <summary>
        /// Gets or sets post tags
        /// </summary>
        public StateList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets post categories
        /// </summary>
        public StateList<Category> Categories { get; set; }

        /// <summary>
        /// Post comments
        /// </summary>
        public List<Comment> Comments { get; set; }
    }
}
