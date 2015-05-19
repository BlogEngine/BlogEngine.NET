namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;
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
        /// Deletes a Category
        /// </summary>
        /// <param name="category">
        /// Must be a valid Category object.
        /// </param>
        public override void DeleteCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Remove(category);

            if (Category.Categories.Contains(category))
            {
                Category.Categories.Remove(category);
            }

            this.WriteToFile();
        }

        /// <summary>
        /// Fills an unsorted list of categories.
        /// </summary>
        /// <returns>
        /// A List&lt;Category&gt; of all Categories.
        /// </returns>
        public override List<Category> FillCategories(Blog blog)
        {
            var fileName = HostingEnvironment.MapPath(blog.StorageLocation + "categories.xml");
            if (!File.Exists(fileName))
            {
                return null;
            }

            var doc = new XmlDocument();
            doc.Load(fileName);
            var categories = new List<Category>();

            foreach (XmlNode node in doc.SelectNodes("categories/category"))
            {
                var category = new Category { Id = new Guid(node.Attributes["id"].InnerText), Title = node.InnerText };

                if (node.Attributes["description"] != null)
                {
                    category.Description = node.Attributes["description"].InnerText;
                }
                else
                {
                    category.Description = string.Empty;
                }

                if (node.Attributes["parent"] != null)
                {
                    if (String.IsNullOrEmpty(node.Attributes["parent"].InnerText))
                    {
                        category.Parent = null;
                    }
                    else
                    {
                        category.Parent = new Guid(node.Attributes["parent"].InnerText);
                    }
                }
                else
                {
                    category.Parent = null;
                }

                categories.Add(category);
                category.MarkOld();
            }

            return categories;
        }

        /// <summary>
        /// Inserts a Category
        /// </summary>
        /// <param name="category">
        /// Must be a valid Category object.
        /// </param>
        public override void InsertCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Add(category);
            categories.Sort();

            this.WriteToFile();
        }

        /// <summary>
        /// Gets a Category based on a Guid
        /// </summary>
        /// <param name="id">
        /// The category's Guid.
        /// </param>
        /// <returns>
        /// A matching Category
        /// </returns>
        public override Category SelectCategory(Guid id)
        {
            var categories = Category.Categories;

            var category = categories.FirstOrDefault(cat => cat.Id == id) ?? new Category();
            category.MarkOld();
            return category;
        }

        /// <summary>
        /// Updates a Category
        /// </summary>
        /// <param name="category">
        /// Must be a valid Category object.
        /// </param>
        public override void UpdateCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Remove(category);
            categories.Add(category);
            categories.Sort();

            this.WriteToFile();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the Categories to disk.
        /// </summary>
        private void WriteToFile()
        {
            var categories = Category.Categories;
            var fileName = string.Format("{0}categories.xml", this.Folder);

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("categories");

                foreach (var cat in categories)
                {
                    writer.WriteStartElement("category");
                    writer.WriteAttributeString("id", cat.Id.ToString());
                    writer.WriteAttributeString("description", cat.Description);
                    writer.WriteAttributeString("parent", cat.Parent.ToString());
                    writer.WriteValue(cat.Title);
                    writer.WriteEndElement();
                    cat.MarkOld();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}