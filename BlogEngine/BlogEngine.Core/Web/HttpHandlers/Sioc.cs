namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using System.Xml;

    /// <summary>
    /// Based on John Dyer's (http://johndyer.name/) extension.
    /// </summary>
    public class Sioc : IHttpHandler
    {
        #region Constants and Fields

        /// <summary>
        ///     The xml namespaces.
        /// </summary>
        private static Dictionary<string, string> xmlNamespaces;

        #endregion

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

        /// <summary>
        ///     Gets SupportedNamespaces.
        /// </summary>
        private static Dictionary<string, string> SupportedNamespaces
        {
            get
            {
                return xmlNamespaces ??
                       (xmlNamespaces =
                        new Dictionary<string, string>
                            {
                                { "foaf", "http://xmlns.com/foaf/0.1/" },
                                { "rss", "http://purl.org/rss/1.0/" },
                                { "admin", "http://webns.net/mvcb/" },
                                { "dc", "http://purl.org/dc/elements/1.1/" },
                                { "dcterms", "http://purl.org/dc/terms/" },
                                { "rdfs", "http://www.w3.org/2000/01/rdf-schema#" },
                                { "rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#" },
                                { "content", "http://purl.org/rss/1.0/modules/content" },
                                { "sioc", "http://rdfs.org/sioc/ns#" }
                            });
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
            context.Response.ContentType = "application/rdf+xml";
            var siocType = context.Request["sioc_type"] + string.Empty;
            var siocId = context.Request["sioc_id"] + string.Empty;

            switch (siocType)
            {
                default:

                    // case "site":
                    var list = Post.Posts.ConvertAll(ConvertToIPublishable);
                    var max = Math.Min(BlogSettings.Instance.PostsPerFeed, list.Count);
                    list = list.GetRange(0, max);
                    WriteSite(context.Response.OutputStream, list);
                    break;

                case "post":
                    Guid postId;
                    if (siocId.TryParse(out postId))
                    {
                        var post = Post.GetPost(postId);
                        if (post != null)
                        {
                            WritePub(context.Response.OutputStream, post);
                        }
                    }

                    break;

                case "comment":
                    Guid commentId;
                    if (siocId.TryParse(out commentId))
                    {
                        // TODO: is it possible to get a single comment?
                        var comment = GetComment(commentId);

                        if (comment != null)
                        {
                            WritePub(context.Response.OutputStream, comment);
                        }
                    }

                    break;

                case "user":
                    WriteAuthor(context.Response.OutputStream, siocId);
                    break;

                    /*
            case "post":
                generator.WriteSite(context.Response.OutputStream);
                break;
            */
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the SHA1.
        /// </summary>
        /// <param name="text">
        /// The text string.
        /// </param>
        /// <param name="enc">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        private static string CalculateSha1(string text, Encoding enc)
        {
            var buffer = enc.GetBytes(text);
            var cryptoTransformSha1 = new SHA1CryptoServiceProvider();
            var hash = BitConverter.ToString(cryptoTransformSha1.ComputeHash(buffer)).Replace("-", string.Empty);

            return hash.ToLower();
        }

        /// <summary>
        /// Closes the writer.
        /// </summary>
        /// <param name="xmlWriter">
        /// The XML writer.
        /// </param>
        private static void CloseWriter(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement(); // rdf:RDF
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        /// <summary>
        /// Converts to publishable interface.
        /// </summary>
        /// <param name="item">
        /// The publishable item.
        /// </param>
        /// <returns>
        /// The publishable interface.
        /// </returns>
        private static IPublishable ConvertToIPublishable(IPublishable item)
        {
            return item;
        }

        /// <summary>
        /// Gets the blog author URL.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// Blog author url.
        /// </returns>
        private static string GetBlogAuthorUrl(string username)
        {
            return string.Format("{0}author/{1}{2}", Utils.AbsoluteWebRoot, HttpUtility.UrlEncode(username), BlogConfig.FileExtension);
        }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <param name="commentId">
        /// The comment id.
        /// </param>
        /// <returns>
        /// The comment.
        /// </returns>
        private static Comment GetComment(Guid commentId)
        {
            var posts = Post.Posts;
            return posts.SelectMany(post => post.Comments).FirstOrDefault(comm => comm.Id == commentId);
        }

        /// <summary>
        /// Gets the sioc author URL.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The SIOC Author Url.
        /// </returns>
        private static string GetSiocAuthorUrl(string username)
        {
            return string.Format(
                "{0}sioc.axd?sioc_type=user&sioc_id={1}", Utils.AbsoluteWebRoot, HttpUtility.UrlEncode(username));
        }

        /// <summary>
        /// Gets the sioc authors URL.
        /// </summary>
        /// <returns>
        /// The SIOC Author Url.
        /// </returns>
        private static string GetSiocAuthorsUrl()
        {
            return string.Format("{0}sioc.axd?sioc_type=site#authors", Utils.AbsoluteWebRoot);
        }

        /// <summary>
        /// Gets the sioc blog URL.
        /// </summary>
        /// <returns>
        /// The SIOC Blog Url.
        /// </returns>
        private static string GetSiocBlogUrl()
        {
            return string.Format("{0}sioc.axd?sioc_type=site#webblog", Utils.AbsoluteWebRoot);
        }

        /// <summary>
        /// Gets the sioc comment URL.
        /// </summary>
        /// <param name="id">
        /// The comment id.
        /// </param>
        /// <returns>
        /// The SIOC Comment Url.
        /// </returns>
        private static string GetSiocCommentUrl(string id)
        {
            return string.Format("{0}sioc.axd?sioc_type=comment&sioc_id={1}", Utils.AbsoluteWebRoot, id);
        }

        /// <summary>
        /// Gets the sioc post URL.
        /// </summary>
        /// <param name="id">
        /// The SIOC post id.
        /// </param>
        /// <returns>
        /// The SIOC Post Url.
        /// </returns>
        private static string GetSiocPostUrl(string id)
        {
            return string.Format("{0}sioc.axd?sioc_type=post&sioc_id={1}", Utils.AbsoluteWebRoot, id);
        }

        /*
                /// <summary>
                /// Gets the sioc site URL.
                /// </summary>
                /// <returns>
                /// The SIOC Site Url.
                /// </returns>
                private static string GetSiocSiteUrl()
                {
                    return string.Format("{0}sioc.axd?sioc_type=site", Utils.AbsoluteWebRoot);
                }
        */

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The Xml Writer.
        /// </returns>
        private static XmlWriter GetWriter(Stream stream)
        {
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var xmlWriter = XmlWriter.Create(stream, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("rdf", "RDF", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            // "http://xmlns.com/foaf/0.1/");
            foreach (var prefix in SupportedNamespaces.Keys)
            {
                xmlWriter.WriteAttributeString("xmlns", prefix, null, SupportedNamespaces[prefix]);
            }

            return xmlWriter;
        }

        /// <summary>
        /// Writes the author.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="authorName">
        /// Name of the author.
        /// </param>
        private static void WriteAuthor(Stream stream, string authorName)
        {
            var xmlWriter = GetWriter(stream);

            WriteFoafDocument(xmlWriter, "user", GetSiocAuthorUrl(authorName));

            var user = Membership.GetUser(authorName);
            var ap = AuthorProfile.GetProfile(authorName);

            // FOAF:Person
            xmlWriter.WriteStartElement("foaf", "Person", null);
            xmlWriter.WriteAttributeString("rdf", "about", null, GetSiocAuthorUrl(authorName));

            xmlWriter.WriteElementString("foaf", "Name", null, authorName);
            if (ap != null && !ap.Private && ap.FirstName != String.Empty)
            {
                xmlWriter.WriteElementString("foaf", "firstName", null, ap.FirstName);
            }

            if (ap != null && !ap.Private && ap.LastName != String.Empty)
            {
                xmlWriter.WriteElementString("foaf", "surname", null, ap.LastName);
            }

            xmlWriter.WriteElementString(
                "foaf", "mbox_sha1sum", null, (user != null) ? CalculateSha1(user.Email, Encoding.UTF8) : string.Empty);
            xmlWriter.WriteStartElement("foaf", "homepage", null);
            xmlWriter.WriteAttributeString("rdf", "resource", null, Utils.AbsoluteWebRoot.ToString());
            xmlWriter.WriteEndElement(); // foaf:homepage 

            xmlWriter.WriteStartElement("foaf", "holdsAccount", null);
            xmlWriter.WriteAttributeString("rdf", "resource", null, GetBlogAuthorUrl(authorName));
            xmlWriter.WriteEndElement(); // foaf:holdsAccount  

            xmlWriter.WriteEndElement(); // foaf:Person

            // SIOC:User
            xmlWriter.WriteStartElement("sioc", "User", null);
            xmlWriter.WriteAttributeString("rdf", "about", null, GetBlogAuthorUrl(authorName));
            xmlWriter.WriteElementString("foaf", "accountName", null, authorName);
            xmlWriter.WriteElementString("sioc", "name", null, authorName);

            // xmlWriter.WriteElementString("dcterms", "created", null, "TODO:" + authorName);
            xmlWriter.WriteEndElement(); // sioc:User

            CloseWriter(xmlWriter);
        }

        /// <summary>
        /// Writes the foaf document.
        /// </summary>
        /// <param name="xmlWriter">
        /// The XML writer.
        /// </param>
        /// <param name="siocType">
        /// Type of the sioc.
        /// </param>
        /// <param name="url">
        /// The URL string.
        /// </param>
        private static void WriteFoafDocument(XmlWriter xmlWriter, string siocType, string url)
        {
            var title = string.Format("SIOC {0} profile for \"{1}\"", siocType, BlogSettings.Instance.Name);
            const string Description =
                "A SIOC profile describes the structure and contents of a weblog in a machine readable form. For more information please refer to http://sioc-project.org/.";

            xmlWriter.WriteStartElement("foaf", "Document", null);
            xmlWriter.WriteAttributeString("rdf", "about", null, Utils.AbsoluteWebRoot.ToString());

            xmlWriter.WriteElementString("dc", "title", null, title);
            xmlWriter.WriteElementString("dc", "description", null, Description);
            xmlWriter.WriteElementString("foaf", "primaryTopic", null, url);
            xmlWriter.WriteElementString(
                "admin", "generatorAgent", null, string.Format("BlogEngine.NET{0}", BlogSettings.Instance.Version()));

            xmlWriter.WriteEndElement(); // foaf:Document
        }

        /// <summary>
        /// Writes the forum.
        /// </summary>
        /// <param name="xmlWriter">
        /// The xml writer.
        /// </param>
        /// <param name="list">
        /// The enumerable of publishable interface.
        /// </param>
        private static void WriteForum(XmlWriter xmlWriter, IEnumerable<IPublishable> list)
        {
            xmlWriter.WriteStartElement("sioc", "Forum", null);

            xmlWriter.WriteAttributeString("rdf", "about", null, GetSiocBlogUrl());
            xmlWriter.WriteElementString("sioc", "name", null, BlogSettings.Instance.Name);
            xmlWriter.WriteStartElement("sioc", "link", null);
            xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocBlogUrl());
            xmlWriter.WriteEndElement();

            foreach (var pub in list)
            {
                xmlWriter.WriteStartElement("sioc", "container_of", null);
                xmlWriter.WriteStartElement("sioc", "Post", null);
                xmlWriter.WriteAttributeString("rdf", "about", null, pub.AbsoluteLink.ToString());
                xmlWriter.WriteAttributeString("dc", "title", null, pub.Title);

                xmlWriter.WriteStartElement("rdfs", "seeAlso", null);
                xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocPostUrl(pub.Id.ToString()));
                xmlWriter.WriteEndElement(); // sioc:Post

                xmlWriter.WriteEndElement(); // sioc:Post
                xmlWriter.WriteEndElement(); // sioc:Forum
            }

            xmlWriter.WriteEndElement(); // sioc:Forum
        }

        /// <summary>
        /// Writes an IPublishable to a stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="pub">
        /// The publishable interface.
        /// </param>
        private static void WritePub(Stream stream, IPublishable pub)
        {
            var xmlWriter = GetWriter(stream);

            if (pub is Post)
            {
                WriteFoafDocument(xmlWriter, "post", pub.AbsoluteLink.ToString());
            }
            else
            {
                WriteFoafDocument(xmlWriter, "comment", pub.AbsoluteLink.ToString());
            }

            WriteSiocPost(xmlWriter, pub);
            CloseWriter(xmlWriter);
        }

        /// <summary>
        /// Writes SIOC post.
        /// </summary>
        /// <param name="xmlWriter">
        /// The xml writer.
        /// </param>
        /// <param name="pub">
        /// The publishable interface.
        /// </param>
        private static void WriteSiocPost(XmlWriter xmlWriter, IPublishable pub)
        {
            xmlWriter.WriteStartElement("sioc", "Post", null);
            xmlWriter.WriteAttributeString("rdf", "about", null, pub.AbsoluteLink.ToString());

            xmlWriter.WriteStartElement("sioc", "link", null);
            xmlWriter.WriteAttributeString("rdf", "resource", null, pub.AbsoluteLink.ToString());
            xmlWriter.WriteEndElement(); // sioc:link

            xmlWriter.WriteStartElement("sioc", "has_container", null);
            xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocBlogUrl());
            xmlWriter.WriteEndElement(); // sioc:has_container

            xmlWriter.WriteElementString("dc", "title", null, pub.Title);

            // SIOC:USER
            if (pub is Post)
            {
                xmlWriter.WriteStartElement("sioc", "has_creator", null);
                xmlWriter.WriteStartElement("sioc", "User", null);
                xmlWriter.WriteAttributeString("rdf", "about", null, GetBlogAuthorUrl(pub.Author));
                xmlWriter.WriteStartElement("rdfs", "seeAlso", null);
                xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocAuthorUrl(pub.Author));
                xmlWriter.WriteEndElement(); // rdf:about
                xmlWriter.WriteEndElement(); // sioc:User
                xmlWriter.WriteEndElement(); // sioc:has_creator
            }

            // FOAF:maker
            xmlWriter.WriteStartElement("foaf", "maker", null);
            xmlWriter.WriteStartElement("foaf", "Person", null);
            if (pub is Post)
            {
                xmlWriter.WriteAttributeString("rdf", "about", null, GetBlogAuthorUrl(pub.Author));
            }

            xmlWriter.WriteAttributeString("foaf", "name", null, pub.Author);

            if (pub is Post)
            {
                var user = Membership.GetUser(pub.Author);
                xmlWriter.WriteElementString(
                    "foaf", 
                    "mbox_sha1sum", 
                    null, 
                    (user != null) ? CalculateSha1(user.Email, Encoding.UTF8) : string.Empty);
            }
            else
            {
                xmlWriter.WriteElementString(
                    "foaf", "mbox_sha1sum", null, CalculateSha1(((Comment)pub).Email, Encoding.UTF8));
            }

            xmlWriter.WriteStartElement("foaf", "homepage", null);
            if (pub is Post)
            {
                xmlWriter.WriteAttributeString("rdf", "resource", null, Utils.AbsoluteWebRoot.ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("rdf", "resource", null, "TODO:");
            }

            xmlWriter.WriteEndElement(); // foaf:homepage

            if (pub is Post)
            {
                xmlWriter.WriteStartElement("rdfs", "seeAlso", null);
                xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocAuthorUrl(pub.Author));
                xmlWriter.WriteEndElement(); // rdfs:seeAlso
            }

            xmlWriter.WriteEndElement(); // foaf:Person
            xmlWriter.WriteEndElement(); // foaf:maker

            // CONTENT
            // xmlWriter.WriteElementString("dcterms", "created", null, DpUtility.ToW3cDateTime(pub.DateCreated));
            xmlWriter.WriteElementString("sioc", "content", null, Utils.StripHtml(pub.Content));
            xmlWriter.WriteElementString("content", "encoded", null, pub.Content);

            // TOPICS
            if (pub is Post)
            {
                // categories
                foreach (var category in ((Post)pub).Categories)
                {
                    xmlWriter.WriteStartElement("sioc", "topic", null);
                    xmlWriter.WriteAttributeString("rdfs", "label", null, category.Title);
                    xmlWriter.WriteAttributeString("rdf", "resource", null, category.AbsoluteLink.ToString());
                    xmlWriter.WriteEndElement(); // sioc:topic
                }

                // tags are also supposed to be treated as sioc:topic 
                foreach (var tag in ((Post)pub).Tags)
                {
                    xmlWriter.WriteStartElement("sioc", "topic", null);
                    xmlWriter.WriteAttributeString("rdfs", "label", null, tag);
                    xmlWriter.WriteAttributeString("rdf", "resource", null, Utils.AbsoluteWebRoot + "?tag=/" + tag);
                    xmlWriter.WriteEndElement(); // sioc:topic
                }

                // COMMENTS
                foreach (var comment in ((Post)pub).ApprovedComments)
                {
                    xmlWriter.WriteStartElement("sioc", "has_reply", null);
                    xmlWriter.WriteStartElement("sioc", "Post", null);
                    xmlWriter.WriteAttributeString("rdf", "about", null, comment.AbsoluteLink.ToString());

                    xmlWriter.WriteStartElement("rdfs", "seeAlso", null);
                    xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocCommentUrl(comment.Id.ToString()));
                    xmlWriter.WriteEndElement(); // rdfs:seeAlso

                    xmlWriter.WriteEndElement(); // sioc:Post
                    xmlWriter.WriteEndElement(); // sioc:has_reply
                }

                // TODO: LINKS
                var linkMatches = Regex.Matches(pub.Content, @"<a[^(href)]?href=""([^""]+)""[^>]?>([^<]+)</a>");
                var linkPairs = new List<string>();

                foreach (Match linkMatch in linkMatches)
                {
                    var url = linkMatch.Groups[1].Value;
                    var text = linkMatch.Groups[2].Value;

                    if (url.IndexOf(Utils.AbsoluteWebRoot.ToString()) == 0)
                    {
                        continue;
                    }

                    var pair = url + "|" + text;
                    if (linkPairs.Contains(pair))
                    {
                        continue;
                    }

                    xmlWriter.WriteStartElement("sioc", "links_to", null);
                    xmlWriter.WriteAttributeString("rdf", "resource", null, url);
                    xmlWriter.WriteAttributeString("rdfs", "label", null, text);
                    xmlWriter.WriteEndElement(); // sioc:links_to

                    linkPairs.Add(pair);
                }
            }

            xmlWriter.WriteEndElement(); // sioc:Post
        }

        /// <summary>
        /// Writes sioc site.
        /// </summary>
        /// <param name="xmlWriter">
        /// The xml writer.
        /// </param>
        private static void WriteSiocSite(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("sioc", "Site", null);
            xmlWriter.WriteAttributeString("rdf", "about", null, Utils.AbsoluteWebRoot.ToString());

            xmlWriter.WriteElementString("dc", "title", null, BlogSettings.Instance.Name);
            xmlWriter.WriteElementString("dc", "description", null, BlogSettings.Instance.Description);
            xmlWriter.WriteElementString("sioc", "link", null, Utils.AbsoluteWebRoot.ToString());
            xmlWriter.WriteElementString("sioc", "host_of", null, GetSiocBlogUrl());
            xmlWriter.WriteElementString("sioc", "has_group", null, GetSiocAuthorsUrl());

            xmlWriter.WriteEndElement(); // sioc:Site
        }

        /// <summary>
        /// Writes site.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="list">
        /// The enumerable of IPublishable.
        /// </param>
        private static void WriteSite(Stream stream, IEnumerable<IPublishable> list)
        {
            var xmlWriter = GetWriter(stream);

            WriteUserGroup(xmlWriter);
            WriteFoafDocument(xmlWriter, "site", Utils.AbsoluteWebRoot.ToString());
            WriteSiocSite(xmlWriter);
            WriteForum(xmlWriter, list);

            CloseWriter(xmlWriter);
        }

        /// <summary>
        /// Writes the user group.
        /// </summary>
        /// <param name="xmlWriter">
        /// The XML writer.
        /// </param>
        private static void WriteUserGroup(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("sioc", "Usergroup", null);

            xmlWriter.WriteAttributeString("rdf", "about", null, GetSiocAuthorsUrl());
            xmlWriter.WriteElementString("dc", "title", null, "Authors at \"" + BlogSettings.Instance.Name + "\"");

            int count;
            var members = Membership.Provider.GetAllUsers(0, 999, out count);

            foreach (MembershipUser user in members)
            {
                xmlWriter.WriteStartElement("sioc", "has_member", null);
                xmlWriter.WriteStartElement("sioc", "User", null);
                xmlWriter.WriteAttributeString("rdf", "about", null, GetBlogAuthorUrl(user.UserName));
                xmlWriter.WriteAttributeString("rdfs", "label", null, user.UserName);

                xmlWriter.WriteStartElement("sioc", "see_also", null);
                xmlWriter.WriteAttributeString("rdf", "resource", null, GetSiocAuthorUrl(user.UserName));
                xmlWriter.WriteEndElement(); // sioc:see_also

                xmlWriter.WriteEndElement(); // sioc:User
                xmlWriter.WriteEndElement(); // sioc:has_member
            }

            xmlWriter.WriteEndElement(); // foaf:Document
        }

        #endregion
    }
}