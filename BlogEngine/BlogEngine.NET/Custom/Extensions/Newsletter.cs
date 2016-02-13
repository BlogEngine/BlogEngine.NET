using System;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;

/// <summary>
/// Sends emails to newsletter subscribers
/// </summary>
[Extension("Sends emails to newsletter subscribers", "3.3.0.0", "BlogEngine.NET")]
public class Newsletter
{
    #region Constructors and Destructors

    static Newsletter()
    {
        Post.Published += Post_Published;
    }

    private static void Post_Published(object sender, EventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("Newsletter"))
            return;

        var publishable = (IPublishable)sender;
        BlogEngine.NET.Custom.Widgets.Newsletter.SendEmails(publishable);
    }

    #endregion
}