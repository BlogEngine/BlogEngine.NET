using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using System.IO;
using System.Web.Compilation;
using System.Text.RegularExpressions;

public partial class RazorHostSite : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		//TO fix the bug related TO Perisan Culture
		if (System.Threading.Thread.CurrentThread.CurrentCulture.LCID == 1065)
			System.Threading.Thread.CurrentThread.CurrentCulture = new PersianCulture();    
        ParseAndInjectRazor();

    }

    private void ParseAndInjectRazor()
    {
        string parsedRazor = RazorHelpers.ParseRazor(RazorHelpers.RAZOR_HOST_PAGE_VPATH, null);

        if (!string.IsNullOrWhiteSpace(parsedRazor))
        {
            Regex headRgx = new Regex("<head\\b[^>]*>(.*?)</head>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match headMatch = headRgx.Match(parsedRazor);
            if (headMatch.Success)
            {
                string headContent = headMatch.Groups[1].Value;

                // remove the <title> tag.
                headContent = new Regex("<title\\b[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Replace(headContent, string.Empty);

                phRazorHead.Controls.Add(new LiteralControl(headContent));
            }

            Regex bodyRgx = new Regex("<body\\b([^>]*)>(.*?)</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match bodyMatch = bodyRgx.Match(parsedRazor);
            if (bodyMatch.Success)
            {
                string bodyAttributes = bodyMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(bodyAttributes))
                {
                    this.BodyAttributes = " " + bodyAttributes;
                }

                string body = bodyMatch.Groups[2].Value;

                int placeHolderPos = body.IndexOf(RazorHelpers.PAGE_BODY_MARKER, StringComparison.OrdinalIgnoreCase);
                if (placeHolderPos != -1)
                {
                    string beforePlaceholderMarkup = body.Substring(0, placeHolderPos);
                    string afterPlaceholderMarkup = body.Substring(placeHolderPos + RazorHelpers.PAGE_BODY_MARKER.Length);

                    phBeforePageBody.Controls.Add(new LiteralControl(beforePlaceholderMarkup));
                    phAfterPageBody.Controls.Add(new LiteralControl(afterPlaceholderMarkup));
                }
            }
        }
    }

    public string BodyAttributes { get; set; }
}
