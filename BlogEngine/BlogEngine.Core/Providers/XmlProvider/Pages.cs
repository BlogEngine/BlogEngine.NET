namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
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
        /// Deletes a Page from the data store specified by the provider.
        /// </summary>
        /// <param name="page">The page to delete.</param>
        public override void DeletePage(Page page)
        {
            var fileName = string.Format("{0}pages{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, page.Id);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            if (Page.Pages.Contains(page))
            {
                Page.Pages.Remove(page);
            }
        }

        /// <summary>
        /// Retrieves all pages from the data store
        /// </summary>
        /// <returns>
        /// List of Pages
        /// </returns>
        public override List<Page> FillPages()
        {
            var folder = string.Format("{0}pages{1}", this.Folder, Path.DirectorySeparatorChar);
            if (Directory.Exists(folder))
            {
                return (from file in Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly)
                        select new FileInfo(file) into info
                        select info.Name.Replace(".xml", string.Empty) into id
                        select Page.Load(new Guid(id))).ToList();
            }
            else
            {
                return new List<Page>();
            }
        }

        /// <summary>
        /// Inserts a new Page into the data store specified by the provider.
        /// </summary>
        /// <param name="page">The page to insert.</param>
        public override void InsertPage(Page page)
        {
            if (!Directory.Exists(string.Format("{0}pages", this.Folder)))
            {
                Directory.CreateDirectory(string.Format("{0}pages", this.Folder));
            }

            var fileName = string.Format("{0}pages{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, page.Id);
            var settings = new XmlWriterSettings { Indent = true };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("page");

                writer.WriteElementString("title", page.Title);
                writer.WriteElementString("description", page.Description);
                writer.WriteElementString("content", page.Content);
                writer.WriteElementString("keywords", page.Keywords);
                writer.WriteElementString("slug", page.Slug);
                writer.WriteElementString("parent", page.Parent.ToString());
                writer.WriteElementString("isfrontpage", page.IsFrontPage.ToString());
                writer.WriteElementString("showinlist", page.ShowInList.ToString());
                writer.WriteElementString("ispublished", page.IsPublished.ToString());
                writer.WriteElementString("isdeleted", page.IsDeleted.ToString());
                writer.WriteElementString("sortorder", page.SortOrder.ToString());
                writer.WriteElementString(
                    "datecreated", 
                    page.DateCreated.AddHours(-BlogSettings.Instance.Timezone).ToString(
                        "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteElementString(
                    "datemodified", 
                    page.DateModified.AddHours(-BlogSettings.Instance.Timezone).ToString(
                        "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Retrieves a Page from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The Page id.</param>
        /// <returns>The Page object.</returns>
        public override Page SelectPage(Guid id)
        {
            var fileName = string.Format("{0}pages{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, id);
            var doc = new XmlDocument();
            doc.Load(fileName);

            var page = new Page
                {
                    Title = doc.SelectSingleNode("page/title").InnerText,
                    Description = doc.SelectSingleNode("page/description").InnerText,
                    Content = doc.SelectSingleNode("page/content").InnerText,
                    Keywords = doc.SelectSingleNode("page/keywords").InnerText
                };

            if (doc.SelectSingleNode("page/slug") != null)
            {
                page.Slug = doc.SelectSingleNode("page/slug").InnerText;
            }

            if (doc.SelectSingleNode("page/parent") != null)
            {
                page.Parent = new Guid(doc.SelectSingleNode("page/parent").InnerText);
            }

            if (doc.SelectSingleNode("page/isfrontpage") != null)
            {
                page.IsFrontPage = bool.Parse(doc.SelectSingleNode("page/isfrontpage").InnerText);
            }

            if (doc.SelectSingleNode("page/showinlist") != null)
            {
                page.ShowInList = bool.Parse(doc.SelectSingleNode("page/showinlist").InnerText);
            }

            if (doc.SelectSingleNode("page/ispublished") != null)
            {
                page.IsPublished = bool.Parse(doc.SelectSingleNode("page/ispublished").InnerText);
            }

            if (doc.SelectSingleNode("page/isdeleted") != null)
            {
                page.IsDeleted = bool.Parse(doc.SelectSingleNode("page/isdeleted").InnerText);
            }

            if (doc.SelectSingleNode("page/sortorder") != null)
            {
                page.SortOrder = Int32.Parse(doc.SelectSingleNode("page/sortorder").InnerText);
            }

            page.DateCreated = DateTime.Parse(
                doc.SelectSingleNode("page/datecreated").InnerText, CultureInfo.InvariantCulture);
            page.DateModified = DateTime.Parse(
                doc.SelectSingleNode("page/datemodified").InnerText, CultureInfo.InvariantCulture);

            return page;
        }

        /// <summary>
        /// Updates an existing Page in the data store specified by the provider.
        /// </summary>
        /// <param name="page">The page to update.</param>
        public override void UpdatePage(Page page)
        {
            InsertPage(page);
        }

        #endregion
    }
}