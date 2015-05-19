#region using

using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;

#endregion

/// <summary>
/// Converts ASCII smilies into real smilies in the comments.
/// </summary>
/// <remarks>
/// Based on the extension by John Knipper - http://www.happytocode.com
/// </remarks>
[Extension("Converts ASCII smilies into real smilies in the comments", "1.3", "BlogEngine.NET")]
public class Smilies
{
    #region Constants and Fields

    /// <summary>
    /// The link string.
    /// </summary>
    private const string Link =
        "<img src=\"{0}admin/editors/tinymce/plugins/emoticons/img/smiley-{1}.gif\" class=\"flag\" alt=\"{2}\" />";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes static members of the <see cref="Smilies"/> class.
    /// </summary>
    static Smilies()
    {
        Comment.Serving += PostCommentServing;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the anchor and inserts the right smiley image.
    /// </summary>
    /// <param name="name">The name of the link.</param>
    /// <param name="alt">The alternate string.</param>
    /// <returns>The link image.</returns>
    private static string Convert(string name, string alt)
    {
        return string.Format(Link, Utils.RelativeWebRoot, name, alt);
    }

    /// <summary>
    /// Handles the CommentServing event of the Post control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
    private static void PostCommentServing(object sender, ServingEventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("Smilies"))
            return;

        if (string.IsNullOrEmpty(e.Body))
            return;

        e.Body = e.Body.Replace("(H)", Convert("cool", "Cool"));
        e.Body = e.Body.Replace(":'(", Convert("cry", "Cry"));
        e.Body = e.Body.Replace(":$", Convert("embarassed", "Embarassed"));
        e.Body = e.Body.Replace(":|", Convert("foot-in-mouth", "Foot"));
        e.Body = e.Body.Replace(":(", Convert("frown", "Frown"));
        e.Body = e.Body.Replace("(A)", Convert("innocent", "Innocent"));
        e.Body = e.Body.Replace("(K)", Convert("kiss", "Kiss"));
        e.Body = e.Body.Replace(":D", Convert("laughing", "Laughing"));
        e.Body = e.Body.Replace("($)", Convert("money-mouth", "Money"));
        e.Body = e.Body.Replace(":-#", Convert("sealed", "Sealed"));
        e.Body = e.Body.Replace(":)", Convert("smile", "Smile"));
        e.Body = e.Body.Replace(":-)", Convert("smile", "Smile"));
        e.Body = e.Body.Replace(":-O", Convert("surprised", "Surprised"));
        e.Body = e.Body.Replace(":P", Convert("tongue-out", "Tong"));
        e.Body = e.Body.Replace("*-)", Convert("undecided", "Undecided"));
        e.Body = e.Body.Replace(";-)", Convert("wink", "Wink"));
        e.Body = e.Body.Replace("8o|", Convert("yell", "Yell"));
    }

    #endregion
}