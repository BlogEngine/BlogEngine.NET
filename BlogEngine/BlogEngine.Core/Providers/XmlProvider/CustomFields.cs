using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BlogEngine.Core.Providers
{
    /// <summary>
    /// Custom fields
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        readonly string _customFieldsFile = "customfields.xml";

        /// <summary>
        /// Saves custom field
        /// </summary>
        /// <param name="field">Object custom field</param>
        public override void SaveCustomField(Data.Models.CustomField field)
        {
            var fileName = Path.Combine(this.Folder, _customFieldsFile);

            var x = FillCustomFields();

            var items = FillCustomFields() ?? new List<Data.Models.CustomField>();
            int idx = -1;

            for (int index = 0; index < items.Count; index++)
            {
                var n = items[index];
                if (n.CustomType == field.CustomType && n.BlogId == field.BlogId && field.ObjectId == n.ObjectId && n.Key == field.Key)
                {
                    idx = index;
                    break;
                }
            }

            if (idx >= 0)
                items.RemoveAt(idx);

            items.Add(field);

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("CustomFields");

                foreach (var n in items)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("customtype", n.CustomType);
                    writer.WriteAttributeString("objectid", n.ObjectId);
                    writer.WriteAttributeString("blogid", Blog.CurrentInstance.Id.ToString());
                    writer.WriteAttributeString("key", n.Key);
                    writer.WriteAttributeString("value", n.Value);
                    writer.WriteAttributeString("attribute", n.Attribute);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Fills list of custom fields for a blog
        /// </summary>
        /// <param name="blog">Current blog</param>
        /// <returns>List of custom fields</returns>
        public override List<Data.Models.CustomField> FillCustomFields()
        {
            var fileName = Path.Combine(this.Folder, _customFieldsFile);

            if (!File.Exists(fileName))
                return new List<Data.Models.CustomField>();

            var doc = new XmlDocument();
            doc.Load(fileName);

            return (from XmlNode node in doc.SelectNodes("CustomFields/item")
                    select new Data.Models.CustomField
                    {
                        CustomType = node.Attributes["customtype"].InnerText,
                        ObjectId = node.Attributes["objectid"].InnerText,
                        BlogId = new Guid(node.Attributes["blogid"].InnerText),
                        Key = node.Attributes["key"].InnerText,
                        Value = node.Attributes["value"].InnerText //,
                        //Attribute = node.Attributes["attribute"].InnerText
                    }).ToList();
        }

        /// <summary>
        /// Deletes custom field
        /// </summary>
        /// <param name="field">Object field</param>
        public override void DeleteCustomField(Data.Models.CustomField field)
        {
            var fileName = Path.Combine(this.Folder, _customFieldsFile);
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(fileName);
            var items = xmlDoc.SelectNodes("CustomFields/item");

            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Attributes != null 
                        && items[i].Attributes["customtype"] != null
                        && items[i].Attributes["blogid"] != null
                        && items[i].Attributes["objectid"] != null
                        && items[i].Attributes["key"] != null)
                    {
                        if (field.CustomType == items[i].Attributes["customtype"].InnerText
                            && field.BlogId.ToString() == items[i].Attributes["blogid"].InnerText
                            && field.ObjectId == items[i].Attributes["objectid"].InnerText
                            && field.Key == items[i].Attributes["key"].InnerText)
                        {
                            if (items[i].ParentNode != null)
                                items[i].ParentNode.RemoveChild(items[i]);
                        }
                    }
                }
            }
            xmlDoc.Save(fileName);
        }

        /// <summary>
        /// Clear custom fields for a type (post, theme etc)
        /// </summary>
        /// <param name="blogId">Blog id</param>
        /// <param name="customType">Custom type</param>
        /// <param name="objectType">Custom object</param>
        public override void ClearCustomFields(string blogId, string customType, string objectType)
        {
            var fileName = Path.Combine(this.Folder, _customFieldsFile);
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(fileName);
            var items = xmlDoc.SelectNodes("CustomFields/item");

            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Attributes != null
                        && items[i].Attributes["customtype"] != null
                        && items[i].Attributes["blogid"] != null
                        && items[i].Attributes["objectid"] != null)
                    {
                        if (items[i].Attributes["customtype"].InnerText == customType
                            && items[i].Attributes["blogid"].InnerText == blogId
                            && items[i].Attributes["objectid"].InnerText == objectType)
                        {
                            if (items[i].ParentNode != null)
                                items[i].ParentNode.RemoveChild(items[i]);
                        }
                    }
                }
            }
            xmlDoc.Save(fileName);
        }
    }
}
