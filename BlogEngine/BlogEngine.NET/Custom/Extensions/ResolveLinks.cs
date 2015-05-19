#region using

using System.Globalization;
using System.Text.RegularExpressions;

using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;

#endregion

/// <summary>
/// Auto resolves URLs in the comments and turn them into valid hyperlinks.
/// </summary>
[Extension("Auto resolves URLs in the comments and turn them into valid hyperlinks.", "1.5", "BlogEngine.NET")]
public class ResolveLinks
{
    #region Constants and Fields

    /// <summary>
    /// The max length.
    /// </summary>
    private const int MaxLength = 50;

    /// <summary>
    /// The link format string.
    /// </summary>
    private const string Link = "<a href=\"{0}{1}\" rel=\"nofollow\">{2}</a>";

    /// <summary>
    ///     The regular expression used to parse links.
    /// </summary>
    private static readonly Regex LinkRegex =
        
        new Regex(
            "((http://|https://|www\\.)([A-Z0-9.\\-]{1,})\\.[0-9A-Z?;~&%\\(\\)#,=\\-_\\./\\+]{2,}[0-9A-Z?~&%#=\\-_/\\+])",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes static members of the <see cref="ResolveLinks"/> class.
    /// </summary>
    static ResolveLinks()
    {
        Comment.Serving += PostCommentServing;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Evaluates the replacement for each link match.
    /// </summary>
    /// <param name="match">
    /// The match.
    /// </param>
    /// <returns>
    /// The evaluator.
    /// </returns>
    public static string Evaluator(Match match)
    {
        var info = CultureInfo.InvariantCulture;
        return string.Format(info, Link, !match.Value.Contains("://") ? "http://" : string.Empty, match.Value, ShortenUrl(match.Value, MaxLength));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the CommentServing event of the Post control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
    private static void PostCommentServing(object sender, ServingEventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("ResolveLinks"))
            return;

        if (string.IsNullOrEmpty(e.Body))
            return;

        e.Body = LinkRegex.Replace(e.Body, new MatchEvaluator(Evaluator));
    }

    /// <summary>
    /// Shortens any absolute URL to a specified maximum length
    /// </summary>
    /// <param name="url">
    /// The url string.
    /// </param>
    /// <param name="max">
    /// The max length.
    /// </param>
    /// <returns>
    /// The shorten url.
    /// </returns>
    private static string ShortenUrl(string url, int max)
    {
        if (url.Length <= max)
        {
            return url;
        }

        // Remove the protocal
        var startIndex = url.IndexOf("://");
        if (startIndex > -1)
        {
            url = url.Substring(startIndex + 3);
        }

        if (url.Length <= max)
        {
            return url;
        }

        // Compress folder structure
        var firstIndex = url.IndexOf("/") + 1;
        var lastIndex = url.LastIndexOf("/");
        if (firstIndex < lastIndex)
        {
            url = url.Remove(firstIndex, lastIndex - firstIndex);
            url = url.Insert(firstIndex, "...");
        }

        if (url.Length <= max)
        {
            return url;
        }

        // Remove URL parameters
        var queryIndex = url.IndexOf("?");
        if (queryIndex > -1)
        {
            url = url.Substring(0, queryIndex);
        }

        if (url.Length <= max)
        {
            return url;
        }

        // Remove URL fragment
        var fragmentIndex = url.IndexOf("#");
        if (fragmentIndex > -1)
        {
            url = url.Substring(0, fragmentIndex);
        }

        if (url.Length <= max)
        {
            return url;
        }

        // Compress page
        firstIndex = url.LastIndexOf("/") + 1;
        lastIndex = url.LastIndexOf(".");
        if (lastIndex - firstIndex > 10)
        {
            var page = url.Substring(firstIndex, lastIndex - firstIndex);
            var length = url.Length - max + 3;
            if (page.Length > length)
            {
                url = url.Replace(page, string.Format("...{0}", page.Substring(length)));
            }
        }

        return url;
    }

    #endregion
}