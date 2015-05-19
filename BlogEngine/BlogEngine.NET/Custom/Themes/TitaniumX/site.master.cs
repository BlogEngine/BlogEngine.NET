using System;
using BlogEngine.Core;

public partial class TitaniumXSite : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		//TO fix the bug related TO Perisan Culture
		if (System.Threading.Thread.CurrentThread.CurrentCulture.LCID == 1065)
			System.Threading.Thread.CurrentThread.CurrentCulture = new PersianCulture();    
        if (Security.IsAuthenticated)
        {
            aUser.InnerText = "Welcome " + Page.User.Identity.Name + "!";
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
