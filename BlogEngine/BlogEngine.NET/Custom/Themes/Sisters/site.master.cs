using System;
using System.Linq;
using System.Web.UI;
using BlogEngine.Core;
using System.Text.RegularExpressions;

public partial class ContactManager : System.Web.UI.MasterPage
{
    private static Regex reg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");

    protected static string ShRoot = Utils.ApplicationRelativeWebRoot + "editors/tiny_mce_3_5_8/plugins/syntaxhighlighter/";
    protected static bool HasRoleAccess(BlogEngine.Core.Page p)
    {
        if(p.CustomFields!=null && p.CustomFields.ContainsKey("Role"))
        {
            var customField = p.CustomFields["Role"];
            var returnValue =  Security.GetCurrentUserRoles().ToList().Contains(customField.Value);
            return returnValue;
        }
        return true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
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
    protected override void Render(HtmlTextWriter writer)
    {
        using (HtmlTextWriter htmlwriter = new HtmlTextWriter(new System.IO.StringWriter()))
        {
            base.Render(htmlwriter);
            writer.Write(htmlwriter.InnerWriter.ToString());
        }
    }

}

