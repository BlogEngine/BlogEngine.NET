namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// A storage provider for BlogEngine that uses XML files.
    ///     <remarks>
    /// To build another provider, you can just copy and modify
    ///         this one. Then add it to the web.config's BlogEngine section.
    ///     </remarks>
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// Deletes a BlogRoll
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void DeleteBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = FillBlogRoll().Where(r => r.Id != blogRollItem.Id).ToList();
            WriteBlogRollFile(blogRolls);
        }

        /// <summary>
        /// Fills an unsorted list of BlogRolls.
        /// </summary>
        /// <returns>
        /// A List&lt;BlogRoll&gt; of all BlogRolls
        /// </returns>
        public override List<BlogRollItem> FillBlogRoll()
        {
            var fileName = Folder + "blogroll.xml";
            if (!File.Exists(fileName))
                return null;

            var doc = new XmlDocument();
            doc.Load(fileName);
            var blogRoll = new List<BlogRollItem>();

            var largestSortIndex = -1;
            var nodes = doc.SelectNodes("blogRoll/item");
            if (nodes != null)
            {
                foreach (var br in from XmlNode node in nodes
                    select new BlogRollItem
                    {
                        Id = node.Attributes["id"] == null ? Guid.NewGuid() : new Guid(node.Attributes["id"].InnerText),
                        Title = node.Attributes["title"] == null ? null : node.Attributes["title"].InnerText,
                        Description = node.Attributes["description"] == null ? null : node.Attributes["description"].InnerText,
                        BlogUrl = node.Attributes["htmlUrl"] == null ? null : new Uri(node.Attributes["htmlUrl"].InnerText),
                        FeedUrl = node.Attributes["xmlUrl"] == null ? null : new Uri(node.Attributes["xmlUrl"].InnerText), 
                        Xfn = node.Attributes["xfn"] == null ? null : node.Attributes["xfn"].InnerText,
                        SortIndex = node.Attributes["sortIndex"] == null ? (blogRoll.Count == 0 ? 0 : largestSortIndex + 1) : int.Parse(node.Attributes["sortIndex"].InnerText)
                    })
                {
                    if (br.SortIndex > largestSortIndex)
                    {
                        largestSortIndex = br.SortIndex;
                    }

                    blogRoll.Add(br);
                }
            }
            return blogRoll;
        }

        /// <summary>
        /// Inserts a BlogRoll
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void InsertBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = FillBlogRoll();
            if(blogRolls == null)
            {
                blogRolls = new List<BlogRollItem>();
            }

            blogRolls.Add(blogRollItem);
            WriteBlogRollFile(blogRolls);
        }

        /// <summary>
        /// Gets a BlogRoll based on a Guid.
        /// </summary>
        /// <param name="id">
        /// The BlogRoll's Guid.
        /// </param>
        /// <returns>
        /// A matching BlogRoll
        /// </returns>
        public override BlogRollItem SelectBlogRollItem(Guid id)
        {
            var blogRoll = FillBlogRoll().Find(br => br.Id == id) ?? new BlogRollItem();
            return blogRoll;
        }

        /// <summary>
        /// Updates a BlogRoll
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void UpdateBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = FillBlogRoll();
            if (blogRolls == null || blogRolls.Count < 1)
            {
                blogRolls = new List<BlogRollItem>();
                blogRolls.Add(blogRollItem);
            }
            else
            {
                foreach (var br in blogRolls)
                {
                    if(br.Id == blogRollItem.Id)
                    {
                        br.Title = blogRollItem.Title;
                        br.BlogUrl = blogRollItem.BlogUrl;
                        br.Description = blogRollItem.Description;
                        br.FeedUrl = blogRollItem.FeedUrl;
                        br.SortIndex = blogRollItem.SortIndex;
                        br.Xfn = blogRollItem.Xfn;
                        break;
                    }
                }
            }
            WriteBlogRollFile(blogRolls);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The write blog roll file.
        /// </summary>
        /// <param name="blogRollItems">
        /// The blog roll items.
        /// </param>
        private void WriteBlogRollFile(List<BlogRollItem> blogRollItems)
        {
            var fileName = this.Folder + "blogroll.xml";

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("blogRoll");

                foreach (var br in blogRollItems)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", br.Id.ToString());
                    writer.WriteAttributeString("title", br.Title);
                    writer.WriteAttributeString("description", br.Description ?? string.Empty);
                    writer.WriteAttributeString("htmlUrl", br.BlogUrl != null ? br.BlogUrl.ToString() : string.Empty);
                    writer.WriteAttributeString("xmlUrl", br.FeedUrl != null ? br.FeedUrl.ToString() : string.Empty);
                    writer.WriteAttributeString("xfn", br.Xfn ?? string.Empty);
                    writer.WriteAttributeString("sortIndex", br.SortIndex.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}