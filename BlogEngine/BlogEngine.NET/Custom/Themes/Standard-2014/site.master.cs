using BlogEngine.Core;
using System;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class StandardSite2014 : System.Web.UI.MasterPage
{
    private static Regex reg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");

    protected static string ShRoot = Utils.ApplicationRelativeWebRoot + "scripts/syntaxhighlighter/";

    protected void Page_Load(object sender, EventArgs e)
    {
		//TO fix the bug related TO Perisan Culture
		if (System.Threading.Thread.CurrentThread.CurrentCulture.LCID == 1065)
			System.Threading.Thread.CurrentThread.CurrentCulture = new PersianCulture();
        // for supported of RTL languages
        if (Resources.labels.LangDirection.Equals("rtl", StringComparison.OrdinalIgnoreCase))
        {
            var lc = new LiteralControl("<link href=\"" + Utils.RelativeWebRoot + "Custom/Themes/Standard/css/rtl.css\" rel=\"stylesheet\" />");
            HeadContent.Controls.Add(lc);
        }

        // needed to make <%# %> binding work in the page header
        Page.Header.DataBind();

        if (Security.IsAuthenticated)
        {
            aLogin.InnerText = Resources.labels.logoff;
            aLogin.HRef = Utils.RelativeWebRoot + "Account/login.aspx?logoff";
        }
        else
        {
            aLogin.HRef = Utils.RelativeWebRoot + "Account/login.aspx";
            aLogin.InnerText = Resources.labels.login;
        }
    }
}
