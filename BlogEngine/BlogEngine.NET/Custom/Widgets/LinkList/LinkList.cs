using System;
using System.Collections.Generic;
using System.Xml;

namespace BlogEngine.NET.Custom.Widgets
{
    public class LinkList
    {
        private string _id;
        public LinkList(string id)
        {
            _id = id;
        }

        public void SaveLink(string id, string title, string url, string target)
        {
            var doc = Doc();
            var links = doc.SelectSingleNode("links");
            if (links == null)
            {
                links = doc.CreateElement("links");
                doc.AppendChild(links);
            }

            var node = doc.SelectSingleNode(string.Format("links/link[@id='{0}']", id));
            if (node == null)
            {
                node = doc.CreateElement("link");

                var atId = doc.CreateAttribute("id");
                var atUrl = doc.CreateAttribute("url");
                var atTarget = doc.CreateAttribute("target");

                atId.Value = Guid.NewGuid().ToString();
                atUrl.Value = url;
                atTarget.Value = target;
                
                node.Attributes.Append(atId);
                node.Attributes.Append(atUrl);
                node.Attributes.Append(atTarget);
                node.InnerText = title;

                links.AppendChild(node);
            }
            else
            {
                node.Attributes["url"].Value = url;
                node.Attributes["target"].Value = target;
                node.InnerText = title; 
            }
            SaveDoc(doc);
        }

        public void RemoveLink(string id)
        {
            var doc = Doc();
            var links = doc.SelectSingleNode("links");
            if (links == null)
            {
                return;
            }
            var node = doc.SelectSingleNode(string.Format("links/link[@id='{0}']", id));
            if (node != null)
            {
                links.RemoveChild(node);
                SaveDoc(doc);
            }
        }

        public List<LinkItem> GetLinks()
        {
            var items = new List<LinkItem>();
            var doc = Doc();
            var links = doc.SelectSingleNode("links");
            if (links != null)
            {
                if(links.ChildNodes != null && links.ChildNodes.Count > 0)
                {
                    foreach (var node in links.ChildNodes)
                    {
                        var item = new LinkItem();
                        var x = (XmlNode)node;
                        item.Id = x.Attributes["id"].Value;
                        item.Url = x.Attributes["url"].Value;
                        item.Target = x.Attributes["target"].Value;
                        item.Title = x.InnerText;
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public LinkItem GetLinkById(string id)
        {
            var item = new LinkItem();
            var doc = Doc();
            var links = doc.SelectSingleNode("links");
            if (links == null)
            {
                return item;
            }
            var node = doc.SelectSingleNode(string.Format("links/link[@id='{0}']", id));
            if (node != null)
            {
                item.Id = node.Attributes["id"].Value;
                item.Url = node.Attributes["url"].Value;
                item.Target = node.Attributes["target"].Value;
                item.Title = node.InnerText;
            }
            return item;
        }

        #region Private methods

        private XmlDocument Doc()
        {
            var settings = Common.GetSettings(_id);
            var doc = new XmlDocument();
            if (settings["content"] != null)
            {
                doc.InnerXml = settings["content"];
            }
            return doc;
        }

        private void SaveDoc(XmlDocument doc)
        {
            var settings = Common.GetSettings(_id);
            if(settings != null)
            {
                settings["content"] = doc.InnerXml;
            }
            Common.SaveSettings(settings, _id);
        }
        
        #endregion
    }

    public class LinkItem
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public string Title { get; set; }
    }
}