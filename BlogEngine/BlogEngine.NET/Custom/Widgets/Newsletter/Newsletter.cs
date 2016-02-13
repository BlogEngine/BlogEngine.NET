using BlogEngine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Hosting;
using System.Xml;

namespace BlogEngine.NET.Custom.Widgets
{
    public class Newsletter
    {
        private static readonly object syncRoot = new object();
        private static string filename = HostingEnvironment.MapPath(
            Path.Combine(Blog.CurrentInstance.StorageLocation, "newsletter.xml"));

        /// <summary>
        /// Add subscriber emal
        /// </summary>
        /// <param name="email">Email address</param>
        public static void AddEmail(string email)
        {
            var doc = GetXml();
            var node = doc.SelectSingleNode(string.Format("emails/email[text()='{0}']", email));
            if (node == null)
            {
                node = doc.CreateElement("email");
                node.InnerText = email;
                doc.FirstChild.AppendChild(node);
                SaveEmails(doc);
            }
        }

        /// <summary>
        /// Remove email from subscription
        /// </summary>
        /// <param name="email">Email address</param>
        public static void RemoveEmail(string email)
        {
            var doc = GetXml();
            var node = doc.SelectSingleNode(string.Format("emails/email[text()='{0}']", email));
            if (node != null)
            {
                doc.FirstChild.RemoveChild(node);
                SaveEmails(doc);
            }
        }

        /// <summary>
        /// Load emails
        /// </summary>
        /// <returns>List of emails</returns>
        public static List<string> LoadEmails()
        {
            var emailList = new List<string>();
            var doc = GetXml();
            var emails = doc.SelectNodes("emails/email");
            if (emails != null)
            {
                foreach (XmlNode node in emails)
                {
                    emailList.Add(node.InnerText.Trim());
                }
            }
            return emailList.OrderBy(e => e).ToList();
        }

        /// <summary>
        /// Send emails to newsletter subscribers
        /// </summary>
        public static void SendEmails(IPublishable publishable)
        {
            var emails = LoadEmails();
            if (emails != null && emails.Count > 0)
            {
                foreach (var email in emails)
                {
                    if (!Utils.StringIsNullOrWhitespace(email) && Utils.IsEmailValid(email))
                    {
                        MailMessage message = CreateEmail(publishable);
                        message.To.Add(email);
                        try
                        {
                            Utils.SendMailMessage(message);
                            //Utils.Log("sent to " + email + " on - " + publishable.Title);
                        }
                        catch (Exception ex)
                        {
                            Utils.Log("Custom.Widgets.Newsletter.SendEmails", ex);
                        }
                    }
                }
            }
        }

        #region Private methods

        private static XmlDocument GetXml()
        {
            var doc = new XmlDocument();
            try
            {
                lock (syncRoot)
                {
                    if (File.Exists(filename))
                    {
                        doc.Load(filename);
                    }
                    else
                    {
                        doc.LoadXml("<emails></emails>");
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Custom.Widgets.Newsletter.GetXml", ex);
            }
            return doc;
        }

        private static void SaveEmails(XmlDocument doc)
        {
            lock (syncRoot)
            {
                using (var ms = new MemoryStream())
                using (var fs = File.Open(filename, FileMode.Create, FileAccess.Write))
                {
                    doc.Save(ms);
                    ms.WriteTo(fs);
                }
            }
        }

        private static string FormatBodyMail(IPublishable publishable)
        {
            var body = new StringBuilder();
            var urlbase = Path.Combine(
                Path.Combine(Utils.AbsoluteWebRoot.AbsoluteUri, "themes"), BlogSettings.Instance.Theme);
            var filePath = string.Format("~/Custom/Themes/{0}/newsletter.html", BlogSettings.Instance.Theme);
            filePath = HostingEnvironment.MapPath(filePath);
            if (File.Exists(filePath))
            {
                body.Append(File.ReadAllText(filePath));
            }
            else
            {
                // if custom theme doesn't have email template
                // use email template from standard theme
                filePath = HostingEnvironment.MapPath("~/Custom/Themes/Standard/newsletter.html");
                if (File.Exists(filePath))
                {
                    body.Append(File.ReadAllText(filePath));
                }
                else
                {
                    Utils.Log(
                        "When sending newsletter, newsletter.html does not exist " +
                        "in theme folder, and does not exist in the Standard theme " +
                        "folder.");
                }
            }

            body = body.Replace("[TITLE]", publishable.Title);
            body = body.Replace("[LINK]", publishable.AbsoluteLink.AbsoluteUri);
            body = body.Replace("[LINK_DESCRIPTION]", publishable.Description);
            body = body.Replace("[WebRoot]", Utils.AbsoluteWebRoot.AbsoluteUri);
            body = body.Replace("[httpBase]", urlbase);
            return body.ToString();
        }

        private static MailMessage CreateEmail(IPublishable publishable)
        {
            var subject = publishable.Title;
            //var settings = GetSettings();

            //if (settings["subjectPrefix"] != null)
            //    subject = settings["subjectPrefix"] + subject;

            var mail = new MailMessage
            {
                Subject = subject,
                Body = FormatBodyMail(publishable),
                From = new MailAddress(BlogSettings.Instance.Email, BlogSettings.Instance.Name)
            };
            return mail;
        }

        #endregion
    }
}