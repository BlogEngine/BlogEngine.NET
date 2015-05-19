#region using

using System;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;

using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;

#endregion

/// <summary>
/// Sends an e-mail to the blog owner whenever a comment is added.
/// </summary>
[Extension("Sends an e-mail to the blog owner whenever a comment is added", "1.3", "BlogEngine.NET")]
public class SendCommentMail
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes static members of the <see cref="SendCommentMail"/> class. 
    ///     Hooks up an event handler to the Post.CommentAdded event.
    /// </summary>
    static SendCommentMail()
    {
        Post.CommentAdded += PostCommentAdded;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the CommentAdded event of the Post control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private static void PostCommentAdded(object sender, EventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("SendCommentMail"))
            return;

        if (!BlogSettings.Instance.SendMailOnComment) 
            return;

        var comment = (Comment)sender;
        if (comment == null) { return; }

        var post = comment.Parent as Post;
        if (post == null || post.CurrentUserOwns)
        {
            return;
        }

        // If moderation is on, send the email if the comment hasn't been moderated (so
        // the blog owner can determine if the comment should be approved/rejected).
        if (BlogSettings.Instance.EnableCommentsModeration &&
            !Utils.StringIsNullOrWhitespace(comment.ModeratedBy))
        {
            return;
        }
        else if (
        // If moderation is off, send the email only if it's been approved.
            !BlogSettings.Instance.EnableCommentsModeration &&
            !comment.IsApproved)
        {
            return;
        }

        // Trackback and pingback comments don't have a '@' symbol in the e-mail field.
        var replyTo = comment.Email.Contains("@") ? comment.Email : BlogSettings.Instance.Email;
        var subject = " comment on ";

        switch (comment.Email)
        {
            case "trackback":
                subject = " trackback on ";
                break;
            case "pingback":
                subject = " pingback on ";
                break;
        }

        var defaultCulture = Utils.GetDefaultCulture();

        var args = new ServingEventArgs(comment.Content, ServingLocation.Email);
        Comment.OnServing(comment, args);
        var body = args.Body;
        body = body.Replace(Environment.NewLine, "<br />");
        body = body.Replace(string.Format("<img src=\"{0}", Utils.RelativeWebRoot), string.Format("<img src=\"{0}", Utils.AbsoluteWebRoot));

        var mail = new MailMessage
            {
                From = new MailAddress(BlogSettings.Instance.Email),
                Subject = BlogSettings.Instance.EmailSubjectPrefix + subject + post.Title
            };
        mail.ReplyToList.Add(new MailAddress(replyTo, HttpUtility.HtmlDecode(comment.Author)));
        mail.To.Add(BlogSettings.Instance.Email);

        var sb = new StringBuilder();
        sb.Append("<div style=\"font: 11px verdana, arial\">");
        sb.AppendFormat("{0}<br /><br />", body);
        sb.AppendFormat(
            "<strong>{0}</strong>: <a href=\"{1}#id_{2}\">{3}</a><br /><br />",
            Utils.Translate("post", null, defaultCulture),
            post.PermaLink,
            comment.Id,
            post.Title);

        var deleteLink = string.Format("{0}?deletecomment={1}", post.AbsoluteLink, comment.Id);
        sb.AppendFormat(
            "<a href=\"{0}\">{1}</a>", deleteLink, Utils.Translate("delete", null, defaultCulture));

        if (BlogSettings.Instance.EnableCommentsModeration)
        {
            var approveLink = string.Format("{0}?approvecomment={1}", post.AbsoluteLink, comment.Id);
            sb.AppendFormat(
                " | <a href=\"{0}\">{1}</a>", approveLink, Utils.Translate("approve", null, defaultCulture));
        }

        sb.Append("<br />_______________________________________________________________________________<br />");
        sb.Append("<h3>Author information</h3>");
        sb.Append("<div style=\"font-size:10px;line-height:16px\">");
        sb.AppendFormat("<strong>Name:</strong> {0}<br />", comment.Author);
        sb.AppendFormat("<strong>E-mail:</strong> {0}<br />", comment.Email);
        sb.AppendFormat("<strong>Website:</strong> <a href=\"{0}\">{0}</a><br />", comment.Website);

        if (comment.Country != null)
        {
            sb.AppendFormat("<strong>Country code:</strong> {0}<br />", comment.Country.ToUpperInvariant());
        }

        if (HttpContext.Current != null)
        {
            sb.AppendFormat("<strong>IP address:</strong> {0}<br />", Utils.GetClientIP());
            sb.AppendFormat("<strong>User-agent:</strong> {0}", HttpContext.Current.Request.UserAgent);
        }

        sb.Append("</div>");
        sb.Append("</div>");

        mail.Body = sb.ToString();

        Utils.SendMailMessageAsync(mail);
    }

    #endregion
}