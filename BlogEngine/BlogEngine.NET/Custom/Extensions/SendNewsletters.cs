using System;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;

/// <summary>
/// Sends emails to newsletter subscribers
/// </summary>
[Extension("Sends emails to newsletter subscribers", "3.3.0.0", "BlogEngine.NET")]
public class SendNewsletters
{
    #region Constructors and Destructors

    static SendNewsletters()
    {
        Post.Published += Post_Published;
    }

    private static void Post_Published(object sender, EventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("SendNewsletters"))
            return;

        var publishable = (IPublishable)sender;
        BlogEngine.NET.Custom.Widgets.Newsletter.SendEmails(publishable);
    }

    #endregion
}