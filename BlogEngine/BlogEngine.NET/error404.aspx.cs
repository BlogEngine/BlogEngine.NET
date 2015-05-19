using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core;
using System.Collections.Generic;

public partial class error404 : BlogBasePage
{

    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);
        Response.StatusCode = 404;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        

        if (Request.QueryString["aspxerrorpath"] != null && Request.QueryString["aspxerrorpath"].Contains("/post/"))
        {
            DirectHitSearch();
            divDirectHit.Visible = true;
        }
        else if (Request.UrlReferrer == null)
        {
            divDirectHit.Visible = true;
        }
        else if (Request.UrlReferrer.Host == Request.Url.Host)
        {
            divInternalReferrer.Visible = true;
        }
        else if (GetSearchKey() != string.Empty)
        {
            SearchTerm = GetSearchTerm(GetSearchKey());
            BindSearchResult();
            divSearchEngine.Visible = true;
        }
        else if (Request.UrlReferrer != null)
        {
            divExternalReferrer.Visible = true;
        }

        Page.Title += Server.HtmlEncode(" - " + Resources.labels.PageNotFound);
    }

    private void DirectHitSearch()
    {
        string from = Request.QueryString["aspxerrorpath"];
        int index = from.LastIndexOf("/") + 1;
        string title = from.Substring(index).Replace(".aspx", string.Empty).Replace("-", " ");

        List<IPublishable> items = Search.Hits(title, false);
        if (items.Count > 0)
        {
            LiteralControl result = new LiteralControl(string.Format("<li><a href=\"{0}\">{1}</a></li>", items[0].RelativeLink.ToString(), items[0].Title));
            phSearchResult.Controls.Add(result);
        }
    }

    private void BindSearchResult()
    {
        List<IPublishable> items = Search.Hits(SearchTerm, false);
        int max = 1;
        foreach (IPublishable item in items)
        {
            HtmlAnchor link = new HtmlAnchor();
            link.InnerHtml = item.Title;
            link.HRef = item.RelativeLink.ToString();
            divSearchResult.Controls.Add(link);

            if (!string.IsNullOrEmpty(item.Description))
            {
                HtmlGenericControl desc = new HtmlGenericControl("span");
                desc.InnerHtml = item.Description;

                divSearchResult.Controls.Add(new LiteralControl("<br />"));
                divSearchResult.Controls.Add(desc);
            }

            divSearchResult.Controls.Add(new LiteralControl("<br />"));
            max++;
            if (max == 3)
                break;
        }
    }

    protected string SearchTerm = string.Empty;

    private string GetSearchKey()
    {
        string referrer = Request.UrlReferrer.Host.ToLowerInvariant();
        if (referrer.Contains("google.") && referrer.Contains("q="))
            return "q=";

        if (referrer.Contains("yahoo.") && referrer.Contains("p="))
            return "p=";

        if (referrer.Contains("q="))
            return "q=";

        return string.Empty;
    }

    /// <summary>
    /// Retrieves the search term from the specified referrer string.
    /// </summary>
    private string GetSearchTerm(string key)
    {
        string referrer = Request.UrlReferrer.ToString();
        int start = referrer.IndexOf(key) + key.Length;
        int stop = referrer.IndexOf("&", start);
        if (stop == -1)
            stop = referrer.Length;

        string term = referrer.Substring(start, stop - start);
        return term.Replace("+", " ");
    }
}
