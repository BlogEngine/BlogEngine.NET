#region using

using System;
using System.Threading;

using BlogEngine.Core;
using BlogEngine.Core.Ping;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;
using System.Collections.Specialized;
using BlogEngine.Core.Providers;

#endregion

/// <summary>
/// Pings all the ping services specified on the 
///     PingServices admin page and send track- and pingbacks
/// </summary>
[Extension("Pings all the ping services specified on the PingServices admin page and send track- and pingbacks", "1.3",
    "BlogEngine.NET")]
public class SendPings
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes static members of the <see cref="SendPings"/> class. 
    ///     Hooks up an event handler to the Post.Saved event.
    /// </summary>
    static SendPings()
    {
        Post.Saved += PostSaved;
        Page.Saved += PostSaved;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the pings from the new thread.
    /// </summary>
    /// <param name="item">
    /// The publishable item.
    /// </param>
    /// <param name="itemUrl">
    /// The item Url.
    /// </param>
    private static void Ping(IPublishable item, Uri itemUrl)
    {
        try
        {
            Thread.Sleep(2000);

            // Ping the specified ping services.
            PingService.Send(itemUrl);

            // Send trackbacks and pingbacks.
            if (!BlogSettings.Instance.EnableTrackBackSend && !BlogSettings.Instance.EnablePingBackSend)
            {
                return;
            }

            if (item.Content.ToUpperInvariant().Contains("\"HTTP"))
            {
                Manager.Send(item, itemUrl);
            }
        }
        catch (Exception)
        {
            // We need to catch this exception so the application doesn't get killed.
        }
    }

    /// <summary>
    /// Handles the Saved event of the Post control.
    /// Sends the pings in a new thread.
    ///     <remarks>
    /// It opens a new thread and executes the pings from there,
    ///         because it takes some time to complete.
    ///     </remarks>
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
    private static void PostSaved(object sender, SavedEventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("SendPings"))
            return;

        if (e.Action == SaveAction.None || e.Action == SaveAction.Delete)
            return;

        var item = (IPublishable)sender;
        if (!item.IsVisibleToPublic)
            return;
        
        var url = item.AbsoluteLink;

        // Need blogSettings to pass to Ping since the current blog instance won't
        // be detectable once in a BG thread.
        Guid blogId = Blog.CurrentInstance.Id;
        ThreadPool.QueueUserWorkItem(state =>
            {
                // because HttpContext is not available within this BG thread
                // needed to determine the current blog instance,
                // set override value here.
                Blog.InstanceIdOverride = blogId;

                Ping(item, url);
            });
    }

    #endregion
}