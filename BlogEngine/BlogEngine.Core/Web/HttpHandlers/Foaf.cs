namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Xml;

    /// <summary>
    /// Based on John Dyer's (http://johndyer.name/) extension.
    /// </summary>
    public class Foaf : IHttpHandler
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
                                { "admin", "http://webns.net/mvcb/" },
                                { "rdfs", "http://www.w3.org/2000/01/rdf-schema#" }
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
            // attempt to grab the username from the URL
            // where URL = www.mysite.com/foaf_admin.axd
            // username = 'admin'
            var filename = context.Request.Url.ToString();
            var name =
                filename.Substring(filename.LastIndexOf("/") + 1).Replace(".axd", string.Empty).Replace(
                    "foaf_", string.Empty).Replace("foaf", string.Empty);

            // if no name is specificied, then grab the first user from Membership
            if (name == string.Empty)
            {
                foreach (MembershipUser user in Membership.GetAllUsers())
                {
                    name = user.UserName;
                    break;
                }
            }

            context.Response.ContentType = "application/rdf+xml";
            this.WriteFoaf(context, name);
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
        /// Closes up the FOAF document
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
        /// Creates the necessary startup information for the FOAF document
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The xml writer.
        /// </returns>
        private static XmlWriter GetWriter(Stream stream)
        {
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var xmlWriter = XmlWriter.Create(stream, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("rdf", "RDF", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            foreach (var prefix in SupportedNamespaces.Keys)
            {
                xmlWriter.WriteAttributeString("xmlns", prefix, null, SupportedNamespaces[prefix]);
            }

            return xmlWriter;
        }

        /// <summary>
        /// Writes the FOAF.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="name">
        /// The name of the FOAF.
        /// </param>
        private void WriteFoaf(HttpContext context, string name)
        {
            // begin FOAF
            var writer = GetWriter(context.Response.OutputStream);

            // write DOCUMENT
            writer.WriteStartElement("foaf", "PersonalProfileDocument", null);
            writer.WriteAttributeString("rdf", "about", null, string.Empty);
            writer.WriteStartElement("foaf", "maker", null);
            writer.WriteAttributeString("rdf", "resource", null, "#me");
            writer.WriteEndElement(); // foaf:maker
            writer.WriteStartElement("foaf", "primaryTopic", null);
            writer.WriteAttributeString("rdf", "resource", null, "#me");
            writer.WriteEndElement(); // foaf:primaryTopic
            writer.WriteEndElement(); // foaf:PersonalProfileDocument

            // get main person's data
            var ap = AuthorProfile.GetProfile(name);

            if (!ap.Private)
            {
                // main author object
                var me = new FoafPerson("#me", ap) { Friends = new List<FoafPerson>() };

                // TODO: this really should be it's own data store

                // assume all other authors are friends
                foreach (var user in
                    Membership.GetAllUsers().Cast<MembershipUser>().Where(
                        user => !user.UserName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    var friend = AuthorProfile.GetProfile(user.UserName);
                    if (friend == null)
                    {
                        continue;
                    }

                    me.Friends.Add(new FoafPerson("#" + user.UserName, friend));
                }

                // assume blog roll = friends
                var blogroll = new Data.ViewModels.BlogRollVM();
                foreach (var br in blogroll.BlogRolls)
                {
                    var title = br.Title;
                    var url = br.BlogUrl.ToString();

                    var foaf = new FoafPerson(title) { Name = title, Blog = url };

                    if (Blog.CurrentInstance.Cache[string.Format("foaf:{0}", title)] == null)
                    {
                        var docs = Utils.FindSemanticDocuments(new Uri(url), "foaf");
                        if (docs.Count > 0)
                        {
                            foreach (var key in docs.Keys)
                            {
                                Blog.CurrentInstance.Cache.Insert(string.Format("foaf:{0}", title), key.ToString());
                                break;
                            }
                        }
                        else
                        {
                            Blog.CurrentInstance.Cache.Insert(string.Format("foaf:{0}", title), "0");
                        }
                    }

                    var seeAlso = (string)Blog.CurrentInstance.Cache[string.Format("foaf:{0}", title)];
                    if (seeAlso != null && seeAlso.Contains("://"))
                    {
                        foaf.Rdf = seeAlso;
                    }

                    me.Friends.Add(foaf);
                }

                // begin writing FOAF Persons
                this.WriteFoafPerson(writer, me);
            }

            CloseWriter(writer);
        }

        /// <summary>
        /// Write a FOAF:Person and any friends to the output stream
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="person">The person.</param>
        private void WriteFoafPerson(XmlWriter writer, FoafPerson person)
        {
            writer.WriteStartElement("foaf", "Person", null);

            // if (person.ID != "")
            // {
            // writer.WriteAttributeString("rdf", "ID", null, person.ID);
            // }
            writer.WriteElementString("foaf", "name", null, person.Name);
            if (person.Title != string.Empty)
            {
                writer.WriteElementString("foaf", "title", null, person.Title);
            }

            if (person.Firstname != string.Empty)
            {
                writer.WriteElementString("foaf", "givenname", null, person.Firstname);
            }

            if (person.Lastname != string.Empty)
            {
                writer.WriteElementString("foaf", "family_name", null, person.Lastname);
            }

            if (!string.IsNullOrEmpty(person.Email))
            {
                writer.WriteElementString("foaf", "mbox_sha1sum", null, CalculateSha1(person.Email, Encoding.UTF8));
            }

            if (!string.IsNullOrEmpty(person.Homepage))
            {
                writer.WriteStartElement("foaf", "homepage", null);
                writer.WriteAttributeString("rdf", "resource", null, person.Homepage);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(person.Blog))
            {
                writer.WriteStartElement("foaf", "weblog", null);
                writer.WriteAttributeString("rdf", "resource", null, person.Blog);
                writer.WriteEndElement();
            }

            if (person.Rdf != string.Empty && person.Rdf != HttpContext.Current.Request.Url.ToString())
            {
                writer.WriteStartElement("rdfs", "seeAlso", null);
                writer.WriteAttributeString("rdf", "resource", null, person.Rdf);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(person.Birthday))
            {
                writer.WriteElementString("foaf", "birthday", null, person.Birthday);
            }

            if (!string.IsNullOrEmpty(person.PhotoUrl))
            {
                writer.WriteStartElement("foaf", "depiction", null);
                writer.WriteAttributeString("rdf", "resource", null, person.PhotoUrl);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(person.Phone))
            {
                writer.WriteElementString("foaf", "phone", null, person.Phone);
            }

            if (person.Friends != null && person.Friends.Count > 0)
            {
                foreach (var friend in person.Friends)
                {
                    writer.WriteStartElement("foaf", "knows", null);

                    this.WriteFoafPerson(writer, friend);

                    writer.WriteEndElement(); // foaf:knows
                }
            }

            writer.WriteEndElement(); // foaf:Person
        }

        #endregion
    }

    /// <summary>
    /// Temporary class for transmitting FOAF attributes
    /// </summary>
    public class FoafPerson
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FoafPerson"/> class.
        /// </summary>
        /// <param name="id">
        /// The FOAF person id.
        /// </param>
        public FoafPerson(string id)
        {
            this.Birthday = string.Empty;
            this.Blog = string.Empty;
            this.Email = string.Empty;
            this.Firstname = string.Empty;
            this.Homepage = string.Empty;
            this.Image = string.Empty;
            this.Lastname = string.Empty;
            this.Name = string.Empty;
            this.Phone = string.Empty;
            this.PhotoUrl = string.Empty;
            this.Rdf = string.Empty;
            this.Title = string.Empty;
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoafPerson"/> class.
        /// </summary>
        /// <param name="id">
        /// The FOAF id.
        /// </param>
        /// <param name="ap">
        /// The AuthorProfile.
        /// </param>
        public FoafPerson(string id, AuthorProfile ap)
        {
            this.Homepage = string.Empty;
            this.Title = string.Empty;
            this.Id = string.IsNullOrEmpty(id) ? "#me" : id;
            this.Name = ap.FullName;
            this.Email = ap.EmailAddress;

            // no homepage
            // this website = blog
            this.Blog = Utils.AbsoluteWebRoot.ToString();
            this.Rdf = string.Format("{0}foaf_{1}.axd", Utils.AbsoluteWebRoot, ap.UserName);
            this.Firstname = ap.FirstName;
            this.Lastname = ap.LastName;
            this.Image = ap.PhotoUrl;
            this.Birthday = ap.Birthday.ToString("yyyy-MM-dd");
            this.Phone = ap.PhoneMain;
            this.PhotoUrl = ap.PhotoUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoafPerson"/> class.
        /// </summary>
        /// <param name="id">
        /// The FOAF person id.
        /// </param>
        /// <param name="name">
        /// The FOAF person name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="homepage">
        /// The homepage.
        /// </param>
        /// <param name="blog">
        /// The FOAF person blog.
        /// </param>
        /// <param name="rdf">
        /// The FOAF person rdf.
        /// </param>
        /// <param name="firstname">
        /// The firstname.
        /// </param>
        /// <param name="lastname">
        /// The lastname.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="birthday">
        /// The birthday.
        /// </param>
        /// <param name="phone">
        /// The phone.
        /// </param>
        public FoafPerson(
            string id,
            string name,
            string title,
            string email,
            string homepage,
            string blog,
            string rdf,
            string firstname,
            string lastname,
            string image,
            string birthday,
            string phone)
        {
            this.PhotoUrl = string.Empty;
            this.Id = id;
            this.Name = name;
            this.Title = title;
            this.Email = email;
            this.Homepage = homepage;
            this.Blog = blog;
            this.Rdf = rdf;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Image = image;
            this.Birthday = birthday;
            this.Phone = phone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the birthday.
        /// </summary>
        /// <value>The birthday.</value>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets the blog.
        /// </summary>
        /// <value>The FOAF blog.</value>
        public string Blog { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the firstname.
        /// </summary>
        /// <value>The firstname.</value>
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the friends.
        /// </summary>
        /// <value>The friends.</value>
        public List<FoafPerson> Friends { get; set; }

        /// <summary>
        /// Gets or sets the homepage.
        /// </summary>
        /// <value>The homepage.</value>
        public string Homepage { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The FOAF id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the lastname.
        /// </summary>
        /// <value>The lastname.</value>
        public string Lastname { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The FOAF name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the photo URL.
        /// </summary>
        /// <value>The photo URL.</value>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the RDF.
        /// </summary>
        /// <value>The FOAF RDF.</value>
        public string Rdf { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        #endregion
    }
}
