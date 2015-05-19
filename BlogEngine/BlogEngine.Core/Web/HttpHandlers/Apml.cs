namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Based on John Dyer's (http://johndyer.name/) extension.
    /// </summary>
    public class Apml : IHttpHandler
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            WriteApmlDocument(context.Response.OutputStream);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Finds the max.
        /// </summary>
        /// <param name="dic">The dictionary.</param>
        /// <returns>A list of Concept.</returns>
        private static IEnumerable<Concept> FindMax(Dictionary<string, Concept> dic)
        {
            float max = 0;
            float max1 = max;
            foreach (var concept in dic.Values.Where(concept => concept.Score > max1))
            {
                max = concept.Score;
            }

            // reset values as a percentage of the max
            var list = dic.Keys.Select(key => new Concept(dic[key].LastUpdated, dic[key].Score / max, key)).ToList();

            list.Sort((c1, c2) => c2.Score.CompareTo(c1.Score));

            return list;
        }

        /// <summary>
        /// Closes the writer.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        private static void CloseWriter(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement(); // APML
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        //// modified version of what's happening in the tag cloud
        // private Dictionary<string, Concept> CreateCategoryList()
        // {
        // // get all the tags and count the number of usages
        // Dictionary<string, Concept> dic = new Dictionary<string, Concept>();
        // foreach (Post post in Post.Posts)
        // {
        // if (post.Visible)
        // {
        // foreach (Category cat in post.Categories)
        // {
        // if (dic.ContainsKey(cat.Title))
        // {
        // Concept concept = dic[cat.Title];
        // concept.Score++;
        // if (post.DateModified > concept.LastUpdated)
        // concept.LastUpdated = post.DateModified;
        // dic[cat.Title] = concept;
        // }
        // else
        // {
        // dic[cat.Title] = new Concept(post.DateModified, 1);
        // }
        // }
        // }
        // }

        // return FindMax(dic);
        // }

        /// <summary>
        /// Creates the link list.
        /// </summary>
        /// <returns>A dictionary of string, string.</returns>
        private static Dictionary<string, string> CreateLinkList()
        {
            var dic = new Dictionary<string, string>();

            foreach (var br in BlogRollItem.BlogRolls)
            {
                var title = br.Title;
                var website = br.BlogUrl.ToString();
                dic.Add(title, website);
            }

            return dic;
        }

        /// <summary>
        /// Creates the tag list.
        /// </summary>
        /// <returns>A list of concepts.</returns>
        private static IEnumerable<Concept> CreateTagList()
        {
            // get all the tags and count the number of usages
            var dic = new Dictionary<string, Concept>();
            foreach (var post in Post.Posts)
            {
                if (!post.IsVisible)
                {
                    continue;
                }

                foreach (var tag in post.Tags)
                {
                    if (dic.ContainsKey(tag))
                    {
                        var concept = dic[tag];
                        concept.Score++;
                        if (post.DateModified > concept.LastUpdated)
                        {
                            concept.LastUpdated = post.DateModified;
                        }

                        dic[tag] = concept;
                    }
                    else
                    {
                        dic[tag] = new Concept(post.DateModified, 1, tag);
                    }
                }
            }

            return FindMax(dic);
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>An xml writer.</returns>
        private static XmlWriter GetWriter(Stream stream)
        {
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var xmlWriter = XmlWriter.Create(stream, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("APML");

            return xmlWriter;
        }

        /// <summary>
        /// The write apml document.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        private static void WriteApmlDocument(Stream stream)
        {
            var writer = GetWriter(stream);

            // HEAD
            writer.WriteStartElement("Head");
            writer.WriteElementString(
                "Title", "APML data for " + BlogSettings.Instance.Name + " - " + BlogSettings.Instance.Description);
            writer.WriteElementString("Generator", "BlogEngine.NET " + BlogSettings.Instance.Version());
            writer.WriteElementString("UserEmail", string.Empty);
            writer.WriteElementString("DateCreated", DateTime.Now.ToString());
            writer.WriteEndElement(); // Head

            // BODY
            writer.WriteStartElement("Body");
            writer.WriteAttributeString("defaultProfile", "tags");

            // tags
            writer.WriteStartElement("Profile");
            writer.WriteAttributeString("name", "tags");
            writer.WriteStartElement("ImplicitData");
            writer.WriteStartElement("Concepts");

            var tagList = CreateTagList();
            foreach (var key in tagList)
            {
                writer.WriteStartElement("Concept");
                writer.WriteAttributeString("key", key.Title);
                writer.WriteAttributeString("value", key.Score.ToString());
                writer.WriteAttributeString("from", Utils.AbsoluteWebRoot.ToString());
                writer.WriteAttributeString("updated", key.LastUpdated.ToString());
                writer.WriteEndElement(); // Concept
            }

            writer.WriteEndElement(); // Concepts
            writer.WriteEndElement(); // ImplicitData
            writer.WriteEndElement(); // Profile

            // categories
            // writer.WriteStartElement("Profile");
            // writer.WriteAttributeString("name", "categories");
            // writer.WriteStartElement("ImplicitData");
            // writer.WriteStartElement("Concepts");

            // Dictionary<string, Concept> catList = CreateCategoryList();
            // foreach (string key in catList.Keys)
            // {
            // writer.WriteStartElement("Concept");
            // writer.WriteAttributeString("key", key);
            // writer.WriteAttributeString("value", catList[key].Score.ToString());
            // writer.WriteAttributeString("from", Utils.AbsoluteWebRoot.ToString());
            // writer.WriteAttributeString("updated", catList[key].LastUpdated.ToString());
            // writer.WriteEndElement();  // Concept
            // }

            // writer.WriteEndElement();  // Concepts
            // writer.WriteEndElement();  // ImplicitData
            // writer.WriteEndElement();  // Profile

            // links
            writer.WriteStartElement("Profile");
            writer.WriteAttributeString("name", "links");
            writer.WriteStartElement("ExplicitData");
            writer.WriteStartElement("Concepts");

            var links = CreateLinkList();
            if (links != null)
            {
                foreach (var title in links.Keys)
                {
                    var website = links[title];
                    writer.WriteStartElement("Source");
                    writer.WriteAttributeString("key", website);
                    writer.WriteAttributeString("name", title);
                    writer.WriteAttributeString("value", "1.0");
                    writer.WriteAttributeString("type", "text/html");
                    writer.WriteAttributeString("from", Utils.AbsoluteWebRoot.ToString());
                    writer.WriteAttributeString("updated", DateTime.Now.ToString());

                    writer.WriteStartElement("Author");
                    writer.WriteAttributeString("key", title);
                    writer.WriteAttributeString("value", "1.0");
                    writer.WriteAttributeString("from", Utils.AbsoluteWebRoot.ToString());
                    writer.WriteAttributeString("updated", DateTime.Now.ToString());
                    writer.WriteEndElement(); // Author

                    writer.WriteEndElement(); // Source
                }
            }

            writer.WriteEndElement(); // Concepts
            writer.WriteEndElement(); // ImplicitData
            writer.WriteEndElement(); // Profile
            writer.WriteEndElement(); // Body

            CloseWriter(writer);
        }

        #endregion

        /// <summary>
        /// The concept.
        /// </summary>
        private class Concept
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Concept"/> class.
            /// </summary>
            /// <param name="lastUpdated">
            /// The last updated.
            /// </param>
            /// <param name="score">
            /// The score.
            /// </param>
            /// <param name="title">
            /// The title.
            /// </param>
            public Concept(DateTime lastUpdated, float score, string title)
            {
                this.LastUpdated = lastUpdated;
                this.Score = score;
                this.Title = title;
            }

            #endregion

            #region Properties

            /// <summary>
            ///     Gets or sets the last updated.
            /// </summary>
            /// <value>The last updated.</value>
            public DateTime LastUpdated { get; set; }

            /// <summary>
            ///     Gets or sets the score.
            /// </summary>
            /// <value>The score.</value>
            public float Score { get; set; }

            /// <summary>
            ///     Gets the title.
            /// </summary>
            /// <value>The title.</value>
            public string Title { get; private set; }

            #endregion
        }
    }
}