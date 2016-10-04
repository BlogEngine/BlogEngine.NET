using System;
using System.Text;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using Resources;
using Page = BlogEngine.Core.Page;

/// <summary>
/// The page.
/// </summary>
public partial class page : BlogBasePage
{
    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        var queryString = Request.QueryString;
        var qsDeletePage = queryString["deletepage"];
        if (qsDeletePage != null && qsDeletePage.Length == 36)
        {
            DeletePage(new Guid(qsDeletePage));
        }

		Guid id = GetPageId();
        if (id != Guid.Empty)
        {
            ServePage(id);
            AddMetaTags();
        }
        else if (!IsCallback)
        {
            Response.Redirect(Utils.RelativeWebRoot);
        }

        base.OnInit(e);
    }

	protected Guid GetPageId()
	{
		string id = Request.QueryString["id"];
		Guid result;
		return id != null && Guid.TryParse(id, out result) ? result : Guid.Empty;
	}

    /// <summary>
    /// Serves the page to the containing DIV tag on the page.
    /// </summary>
    /// <param name="id">
    /// The id of the page to serve.
    /// </param>
    private void ServePage(Guid id)
    {
		var pg = this.Page;

        if (pg == null || (!pg.IsVisible))
        {
            this.Response.Redirect($"{Utils.RelativeWebRoot}error404.aspx", true);
            return; // WLF: ReSharper is stupid and doesn't know that redirect returns this method.... or does it not...?
        }

        this.h1Title.InnerHtml = System.Web.HttpContext.Current.Server.HtmlEncode(pg.Title);

        var arg = new ServingEventArgs(pg.Content, ServingLocation.SinglePage);
        BlogEngine.Core.Page.OnServing(pg, arg);

        if (arg.Cancel)
        {
            this.Response.Redirect("error404.aspx", true);
        }

        if (arg.Body.Contains("[usercontrol", StringComparison.OrdinalIgnoreCase))
        {
            Utils.InjectUserControls(this.divText, arg.Body);
           // this.InjectUserControls(arg.Body);
        }
        else
        {
            this.divText.InnerHtml = arg.Body;
        }
    }

    /// <summary>
    /// Adds the meta tags and title to the HTML header.
    /// </summary>
    private void AddMetaTags()
    {
        if (Page == null)
            return;

        Title = Server.HtmlEncode(Page.Title);
        AddMetaTag("keywords", Server.HtmlEncode(Page.Keywords));

        var desc = BlogSettings.Instance.Name + " - " + BlogSettings.Instance.Description + " - " + Page.Description;
        AddMetaTag("description", Server.HtmlEncode(desc));
    }

    /// <summary>
    /// Deletes the page.
    /// </summary>
    /// <param name="id">
    /// The page id.
    /// </param>
    private void DeletePage(Guid id)
    {
        var page = BlogEngine.Core.Page.GetPage(id);
        if (page == null)
        {
            return;
        }
        if (!page.CanUserDelete)
        {
            Response.Redirect(Utils.RelativeWebRoot);
            return;
        }
        if (page.HasChildPages)
        {
            return;
        }
        page.Delete();
        page.Save();
        this.Response.Redirect(Utils.RelativeWebRoot, true);
    }

	private Page _page;
	private bool _pageLoaded;
    /// <summary>
    ///     The Page instance to render on the page.
    /// </summary>
	public new Page Page
	{
		get
		{
			if (!_pageLoaded)
			{
				_pageLoaded = true;
				Guid id = GetPageId();
				if (id != Guid.Empty)
				{
					_page = Page.GetPage(id);
				}
			}

			return _page;
		}
	}

    /// <summary>
    ///     Gets the admin links to edit and delete a page.
    /// </summary>
    /// <value>The admin links.</value>
    public string AdminLinks
    {
        get
        {
            if (!Security.IsAuthenticated)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (this.Page.CanUserEdit)
            {
                if (sb.Length > 0) { sb.Append(" | "); }

                sb.AppendFormat(
                    "<a href=\"{0}admin/app/editor/editpage.cshtml?id={1}\">{2}</a>",
                    Utils.RelativeWebRoot,
                    this.Page.Id,
                    labels.edit);
            }

            if (this.Page.CanUserDelete && !this.Page.HasChildPages)
            {
                if (sb.Length > 0) { sb.Append(" | "); }

                sb.AppendFormat(
                    String.Concat("<a href=\"javascript:void(0);\" onclick=\"if (confirm('", labels.areYouSureDeletePage, "')) location.href='?deletepage={0}'\">{1}</a>"),
                    this.Page.Id,
                    labels.delete);
            }

            if (sb.Length > 0)
            {
                sb.Insert(0, "<div id=\"admin\">");
                sb.Append("</div>");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets PermaLink.
    /// </summary>
    public string PermaLink
    {
        get
        {
            return $"{Utils.AbsoluteWebRoot}page.aspx?id={Page.Id}";
        }
    }
}