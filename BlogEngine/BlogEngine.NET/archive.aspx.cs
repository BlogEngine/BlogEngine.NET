using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

public partial class archive : BlogEngine.Core.Web.Controls.BlogBasePage
{
	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack && !IsCallback)
		{
			CreateMenu();
			CreateArchive();
			AddTotals();
		}

		Page.Title = Server.HtmlEncode(Resources.labels.archive);
		base.AddMetaTag("description", Resources.labels.archive + " | " + BlogSettings.Instance.Name);
	}

	/// <summary>
	/// Creates the category top menu.
	/// </summary>
	private void CreateMenu()
	{
        var categories = new List<string>();

		foreach (Category cat in Category.ApplicableCategories)
		{
            if (cat.Posts.Count > 0)
            {
                if (!categories.Contains(cat.Title))
                {
                    AddCategoryToMenu(cat.Title);
                    categories.Add(cat.Title);
                }
            }
		}
	}

	private void AddCategoryToMenu(string title)
	{
		HtmlAnchor a = new HtmlAnchor();
		a.InnerHtml = Server.HtmlEncode(title);
		a.HRef = string.Format("{0}archive{1}#cat-{2}", Blog.CurrentInstance.RelativeWebRoot, BlogConfig.FileExtension, Utils.RemoveIllegalCharacters(title));
		a.Attributes.Add("rel", "directory");

		HtmlGenericControl li = new HtmlGenericControl("li");
		li.Controls.Add(a);
		ulMenu.Controls.Add(li);
	}

	/// <summary>
	/// Sorts the categories.
	/// </summary>
	/// <param name="categories">The categories.</param>
	private SortedDictionary<string, Guid> SortCategories(Dictionary<Guid, string> categories)
	{
		SortedDictionary<string, Guid> dic = new SortedDictionary<string, Guid>();
		foreach (Category cat in Category.Categories)
		{
			bool postsExist = cat.Posts.FindAll(delegate(Post post)
			{
				return post.IsVisible;
			}).Count > 0;

			if (postsExist)
				dic.Add(cat.Title, cat.Id);
		}

		return dic;
	}

	private void CreateArchive()
	{
        var categories = new List<string>();

		foreach (Category cat in Category.ApplicableCategories)
		{
            if (cat.Posts.Count > 0)
            {
                if (!categories.Contains(cat.Title))
                {
                    string name = cat.Title;
                    List<Post> list = cat.Posts.FindAll(delegate(Post p) { return p.IsVisible; });

                    HtmlGenericControl h2 = CreateRowHeader(cat, name, list.Count);
                    phArchive.Controls.Add(h2);

                    HtmlTable table = CreateTable(name);
                    foreach (Post post in list)
                    {
                        CreateTableRow(table, post);
                    }

                    phArchive.Controls.Add(table);
                    categories.Add(cat.Title);
                }
            }
		}

		List<Post> noCatList = Post.ApplicablePosts.FindAll(delegate(Post p) { return p.Categories.Count == 0 && p.IsVisible; });
		if (noCatList.Count > 0)
		{
			string name = Resources.labels.uncategorized;
			HtmlGenericControl h2 = CreateRowHeader(null, name, noCatList.Count);
			phArchive.Controls.Add(h2);

			HtmlTable table = CreateTable(name);
			foreach (Post post in noCatList)
			{
				CreateTableRow(table, post);
			}

			phArchive.Controls.Add(table);

			AddCategoryToMenu(name);
		}
	}

	private static HtmlGenericControl CreateRowHeader(Category cat, string name, int count)
	{
		HtmlGenericControl h2 = new HtmlGenericControl("h2");
		h2.Attributes["id"] = "cat-" + Utils.RemoveIllegalCharacters(name);

        if (cat != null)
		{
			HtmlAnchor feed = new HtmlAnchor();
            feed.HRef = cat.FeedRelativeLink;

			HtmlImage img = new HtmlImage();
            img.Src = Utils.RelativeWebRoot + "Content/images/blog/rssButton.png";
			img.Alt = "RSS";
			feed.Controls.Add(img);
			h2.Controls.Add(feed);
		}

		Control header = new LiteralControl(name + " (" + count + ")");
		h2.Controls.Add(header);
		return h2;
	}

	private static void CreateTableRow(HtmlTable table, Post post)
	{
		HtmlTableRow row = new HtmlTableRow();

		HtmlTableCell date = new HtmlTableCell();
		date.InnerHtml = post.DateCreated.ToString("yyyy-MM-dd");
		date.Attributes.Add("class", "date");
		row.Cells.Add(date);

		HtmlTableCell title = new HtmlTableCell();
		title.InnerHtml = string.Format("<a href=\"{0}\">{1}</a>", post.RelativeLink, post.Title);
		title.Attributes.Add("class", "title");
		row.Cells.Add(title);

		if (BlogSettings.Instance.IsCommentsEnabled)
		{
			HtmlTableCell comments = new HtmlTableCell();

            if (BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.Disqus)
            {
                comments.InnerHtml = string.Format("<span><a href=\"{0}#comment\">{1}</a></span>", post.PermaLink, Resources.labels.comments);
            }
            else if(BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.Facebook)
            {
                comments.InnerHtml = string.Format("<fb:comments-count href=\"{0}#comment\"></fb:comments-count> {1}", post.PermaLink, Resources.labels.comments);
            }
            else
            {
                comments.InnerHtml = post.ApprovedComments.Count.ToString();
            }
			comments.Attributes.Add("class", "comments");
			row.Cells.Add(comments);
		}

		if (BlogSettings.Instance.EnableRating)
		{
			HtmlTableCell rating = new HtmlTableCell();
			rating.InnerHtml = post.Raters == 0 ? "None" : Math.Round(post.Rating, 1).ToString();
			rating.Attributes.Add("class", "rating");
			row.Cells.Add(rating);
		}

		table.Rows.Add(row);
	}

	private HtmlTable CreateTable(string name)
	{
		HtmlTable table = new HtmlTable();
		table.Attributes.Add("summary", name);

		HtmlTableRow header = new HtmlTableRow();

		HtmlTableCell date = new HtmlTableCell("th");
        date.InnerHtml = Utils.Translate("date");
		header.Cells.Add(date);

		HtmlTableCell title = new HtmlTableCell("th");
        title.InnerHtml = Utils.Translate("title");
		header.Cells.Add(title);

		if (BlogSettings.Instance.IsCommentsEnabled)
		{
			HtmlTableCell comments = new HtmlTableCell("th");
			comments.InnerHtml = Utils.Translate("comments");
			comments.Attributes.Add("class", "comments");
			header.Cells.Add(comments);
		}

		if (BlogSettings.Instance.EnableRating)
		{
			HtmlTableCell rating = new HtmlTableCell("th");
            rating.InnerHtml = Utils.Translate("rating");
			rating.Attributes.Add("class", "rating");
			header.Cells.Add(rating);
		}

		table.Rows.Add(header);

		return table;
	}

	private void AddTotals()
	{
		int comments = 0;
		int raters = 0;
		List<Post> posts = Post.ApplicablePosts.FindAll(delegate(Post p) { return p.IsVisible; });
		foreach (Post post in posts)
		{
			comments += post.ApprovedComments.Count;
			raters += post.Raters;
		}

		ltPosts.Text = posts.Count + " " + Resources.labels.posts.ToLowerInvariant();
		if (BlogSettings.Instance.IsCommentsEnabled && BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.BlogEngine)
			ltComments.Text = "<span>" + comments + " " + Resources.labels.comments.ToLowerInvariant() + "</span><br />";

		if (BlogSettings.Instance.EnableRating)
			ltRaters.Text = raters + " " + Resources.labels.raters.ToLowerInvariant();
	}
}
