using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using BlogEngine.Core.Notes;

namespace BlogEngine.Core.Providers
{
    public partial class XmlBlogProvider : BlogProvider
    {
        readonly string notesFileName = "quicknotes.xml";
        readonly string settingsFileName = "quicksettings.xml";

        /// <summary>
        /// Save quick note
        /// </summary>
        /// <param name="note">Quick note</param>
        public override void SaveQuickNote(QuickNote note)
        {
            var fileName = Path.Combine(this.Folder, notesFileName);

            var notes = FillQuickNotes(note.Author) ?? new List<QuickNote>();
            int idx = -1;

            for (int index = 0; index < notes.Count; index++)
            {
                var n = notes[index];
                if (n.Id == note.Id)
                {
                    idx = index;
                    break;
                }
            }

            if (idx >= 0)
                notes.RemoveAt(idx);

            notes.Insert(0, note);

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("QuickNotes");

                foreach (var n in notes)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", n.Id.ToString());
                    writer.WriteAttributeString("blogid", Blog.CurrentInstance.Id.ToString());
                    writer.WriteAttributeString("note", n.Note);
                    writer.WriteAttributeString("author", n.Author);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Save quick setting
        /// </summary>
        /// <param name="setting">Quick setting</param>
        public override void SaveQuickSetting(QuickSetting setting)
        {
            var fileName = Path.Combine(this.Folder, settingsFileName);

            var settings = FillQuickSettings(setting.Author) ?? new List<QuickSetting>();
            int idx = -1;

            for (int index = 0; index < settings.Count; index++)
            {
                var s = settings[index];
                if (s.Author == setting.Author && s.SettingName == setting.SettingName)
                {
                    idx = index;
                    break;
                }
            }

            if (idx >= 0)
                settings.RemoveAt(idx);

            settings.Add(setting);

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("QuickSettings");

                foreach (var s in settings)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("blogid", Blog.CurrentInstance.Id.ToString());
                    writer.WriteAttributeString("author", s.Author);
                    writer.WriteAttributeString("name", s.SettingName);
                    writer.WriteAttributeString("value", s.SettingValue);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Fill quick notes
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user notes</returns>
        public override List<QuickNote> FillQuickNotes(string userId)
        {
            var fileName = Path.Combine(this.Folder, notesFileName);

            if (!File.Exists(fileName))
                return new List<QuickNote>();

            var doc = new XmlDocument();
            doc.Load(fileName);

            return (from XmlNode node in doc.SelectNodes("QuickNotes/item")
                    select new QuickNote
                    {
                        Id = new Guid(node.Attributes["id"].InnerText),
                        BlogId = new Guid(node.Attributes["blogid"].InnerText),
                        Author = node.Attributes["author"].InnerText,
                        Note = node.Attributes["note"].InnerText
                    }).Where(s => s.Author == userId && s.BlogId == Blog.CurrentInstance.Id).ToList();
        }
        /// <summary>
        /// Fill quick settings
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user settings</returns>
        public override List<QuickSetting> FillQuickSettings(string userId)
        {
            var fileName = Path.Combine(this.Folder, settingsFileName);

            if (!File.Exists(fileName))
                return new List<QuickSetting>();

            var doc = new XmlDocument();
            doc.Load(fileName);

            return (from XmlNode node in doc.SelectNodes("QuickSettings/item")
                    select new QuickSetting
                    {
                        BlogId = new Guid(node.Attributes["blogid"].InnerText),
                        Author = node.Attributes["author"].InnerText,
                        SettingName = node.Attributes["name"].InnerText,
                        SettingValue = node.Attributes["value"].InnerText
                    }).Where(s => s.Author == userId && s.BlogId == Blog.CurrentInstance.Id).ToList();
        }
        /// <summary>
        /// Delete quick note
        /// </summary>
        /// <param name="noteId">Note ID</param>
        public override void DeleteQuickNote(Guid noteId)
        {
            var fileName = Path.Combine(this.Folder, notesFileName);
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(fileName);
            var nodes = xmlDoc.SelectNodes("QuickNotes/item");

            if (nodes != null && nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Attributes != null && nodes[i].Attributes["id"] != null)
                    {
                        if (noteId.ToString() == nodes[i].Attributes["id"].InnerText)
                        {
                            if (nodes[i].ParentNode != null)
                                nodes[i].ParentNode.RemoveChild(nodes[i]);
                        }
                    }
                }
            }
            xmlDoc.Save(fileName);
        }
    }
}
