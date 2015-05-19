#region using

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;
using Resources;

#endregion

/// <summary>
/// Breaks a post where [more] is found in the body and adds a link to full post.
/// </summary>
[Extension(
    "Breaks a post where [more] is found in the body and adds a link to full post",
    "1.4",
    "BlogEngine.NET",
    1010)]
public class BreakPost
{
    #region Constants and Fields

    /// <summary>
    /// The closed tag regex.
    /// </summary>
    private static readonly Regex ClosedTagRegex = new Regex(
        @"</([A-Z][A-Z0-9]*?)\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// The opening tag regex.
    /// </summary>
    private static readonly Regex OpeningTagRegex = new Regex(
        @"<([A-Z][A-Z0-9]*?)\b[^>/]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes static members of the <see cref="BreakPost"/> class. 
    ///     Hooks up an event handler to the Post.Serving event.
    /// </summary>
    static BreakPost()
    {
        Post.Serving += PostServing;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Replaces the [more] string with a hyperlink to the full post.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void AddMoreLink(object sender, ServingEventArgs e)
    {
        var post = (Post)sender;
        var index = e.Body.IndexOf("[more]");
        var link = string.Format("<a class=\"more\" href=\"{0}#continue\">{1}...</a>", post.RelativeLink, labels.more);
        var newBody = e.Body.Substring(0, index);

        // Need to close any open HTML tags in NewBody where the matching close tags have been truncated.
        var closingTagsToAppend = string.Empty;
        var openingTagsCollection = OpeningTagRegex.Matches(newBody);

        if (openingTagsCollection.Count > 0)
        {
            // Copy the opening tags in MatchCollection to a generic list.
            var openingTags = openingTagsCollection.Cast<Match>().Where(openTag => openTag.Groups.Count == 2).Select(
                openTag => openTag.Groups[1].Value).ToList();

            var closingTagsCollection = ClosedTagRegex.Matches(newBody);

            // Iterate through closed tags and remove the first matching open tag from the openingTags list.
            foreach (var indexToRemove in from Match closedTag in closingTagsCollection
                                          where closedTag.Groups.Count == 2
                                          select openingTags.FindIndex(
                                              openTag =>
                                              openTag.Equals(
                                                  closedTag.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase))
                                          into indexToRemove
                                          where indexToRemove != -1
                                          select indexToRemove)
            {
                openingTags.RemoveAt(indexToRemove);
            }

            // A closing tag needs to be created for any remaining tags in the openingTags list.
            if (openingTags.Count > 0)
            {
                // Reverse the order of the tags so tags opened later are closed first.
                openingTags.Reverse();
                closingTagsToAppend = string.Format("</{0}>", string.Join("></", openingTags.ToArray()));
            }
        }

        e.Body = newBody + link + closingTagsToAppend;
    }

    /// <summary>
    /// Handles the Serving event of the Post control.
    /// Handles the Post.Serving event to take care of the [more] keyword.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
    private static void PostServing(object sender, ServingEventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("BreakPost"))
            return;

        if (!e.Body.Contains("[more]"))
            return;

        switch (e.Location)
        {
            case ServingLocation.PostList:
                AddMoreLink(sender, e);
                break;
            case ServingLocation.SinglePost:
                PrepareFullPost(e);
                break;
            case ServingLocation.Feed:
                e.Body = e.Body.Replace("[more]", string.Empty);
                break;
        }
    }

    /// <summary>
    /// Replaces the [more] string on the full post page.
    /// </summary>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void PrepareFullPost(ServingEventArgs e)
    {
        var request = HttpContext.Current.Request;
        e.Body = request.UrlReferrer == null || request.UrlReferrer.Host != request.Url.Host
                     ? e.Body.Replace("[more]", string.Empty)
                     : e.Body.Replace("[more]", "<span id=\"continue\"></span>");
    }

    #endregion
}