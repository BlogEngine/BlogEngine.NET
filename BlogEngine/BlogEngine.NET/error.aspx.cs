using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core;
using System.Collections.Generic;

public partial class error_occurred : BlogBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = Resources.labels.Error;
        OutputErrorDetails();
    }

    private void OutputErrorDetails()
    {
        string contextItemKey = "LastErrorDetails";

        if (Security.IsAuthorizedTo(Rights.ViewDetailedErrorMessages) && HttpContext.Current.Items.Contains(contextItemKey))
        { 
            string errorDetails = (string)HttpContext.Current.Items[contextItemKey];

            if (!string.IsNullOrEmpty(errorDetails))
            {
                divErrorDetails.Visible = true;                
                pDetails.InnerHtml = Server.HtmlEncode(errorDetails);
                pDetails.InnerHtml = errorDetails.Replace(Environment.NewLine, "<br /><br />");
            }
        }        
    }
}
