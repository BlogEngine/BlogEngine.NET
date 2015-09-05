using System;
using System.Web.UI;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Collections.Generic;

public partial class post : BlogBasePage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
        {
            Response.Redirect(Utils.RelativeWebRoot);
        }

        bool shouldThrow404 = false;

        ucCommentList.Visible = ShowBlogengineComments;

        var requestId = Request.QueryString["id"];
        Guid id;

        if ((!Utils.StringIsNullOrWhitespace(requestId)) && requestId.TryParse(out id))
        {

            Post post = Post.ApplicablePosts.Find(p => p.Id == id);

            if (post != null)
            {
                if (!Page.IsPostBack && !Page.IsCallback && Request.RawUrl.Contains("?id="))
                {
                    // If there's more than one post that has the same RelativeLink
                    // this post has then don't do a 301 redirect.

                    if (Post.Posts.FindAll(delegate(Post p)
                    { return p.RelativeLink.Equals(post.RelativeLink); }
                    ).Count < 2)
                    {
                        Response.Clear();
                        Response.StatusCode = 301;
                        Response.AppendHeader("location", post.RelativeLink.ToString());
                        Response.End();
                    }
                }
                else if (!post.IsVisible)
                {
                    Response.Redirect(Utils.RelativeWebRoot + "default.aspx", true);
                    //shouldThrow404 = true;
                }
                else
                {
                    this.Post = post;

                    // SEO redirct, discussion #446011
                    int idx = Request.RawUrl.IndexOf("?");
                    var rawUrl = idx > 0 ? Request.RawUrl.Substring(0, idx) : Request.RawUrl;
                    if (rawUrl != post.RelativeLink.ToString())
                    {
                        Response.Clear();
                        Response.StatusCode = 301;
                        Response.AppendHeader("location", post.RelativeLink.ToString());
                        Response.End();
                    }

                    var settings = BlogSettings.Instance;
                    string encodedPostTitle = Server.HtmlEncode(Post.Title);
                    string path = Utils.ApplicationRelativeWebRoot + "Custom/Themes/" + BlogSettings.Instance.GetThemeWithAdjustments(null) + "/PostView.ascx";

                    if (!System.IO.File.Exists(Server.MapPath(path)))
                        path = string.Format("{0}Custom/Controls/Defaults/PostView.ascx", Utils.ApplicationRelativeWebRoot);

                    PostViewBase postView = (PostViewBase)LoadControl(path);
                    postView.Post = Post;
                    postView.ID = Post.Id.ToString().Replace("-", string.Empty);
                    postView.Location = ServingLocation.SinglePost;
                    pwPost.Controls.Add(postView);

                    // Related posts
                    if (settings.EnableRelatedPosts)
                    {
                        try
                        {
                            // Try to load RelatedPosts view from theme folder
                            string relatedPath = Utils.ApplicationRelativeWebRoot + "Custom/Themes/" + BlogSettings.Instance.GetThemeWithAdjustments(null) + "/RelatedPosts.ascx";
                            RelatedPostsBase relatedView = (RelatedPostsBase)LoadControl(relatedPath);
                            relatedView.PostItem = this.Post;
                            phRelatedPosts.Controls.Add(relatedView);
                        }
                        catch (Exception)
                        {
                            // fall back to legacy code
                            related.Visible = true;
                            related.Item = this.Post;
                        }
                    }

                    ucCommentList.Post = Post;

                    Page.Title = encodedPostTitle;
                    AddMetaKeywords();
                    AddMetaDescription();
                    base.AddMetaTag("author", Server.HtmlEncode(Post.AuthorProfile == null ? Post.Author : Post.AuthorProfile.FullName));

                    List<Post> visiblePosts = Post.Posts.FindAll(delegate(Post p) { return p.IsVisible; });
                    if (visiblePosts.Count > 0)
                    {
                        AddGenericLink("last", visiblePosts[0].Title, visiblePosts[0].RelativeLink);
                        AddGenericLink("first", visiblePosts[visiblePosts.Count - 1].Title, visiblePosts[visiblePosts.Count - 1].RelativeLink);
                    }

                    InitNavigationLinks();

                    phRDF.Visible = settings.EnableTrackBackReceive;

                    base.AddGenericLink("application/rss+xml", "alternate", encodedPostTitle + " (RSS)", postView.CommentFeed + "?format=ATOM");
                    base.AddGenericLink("application/rss+xml", "alternate", encodedPostTitle + " (ATOM)", postView.CommentFeed + "?format=ATOM");

                    if (BlogSettings.Instance.EnablePingBackReceive)
                    {
                        Response.AppendHeader("x-pingback", "http://" + Request.Url.Authority + Utils.RelativeWebRoot + "pingback.axd");
                    }

                    string commentNotificationUnsubscribeEmailAddress = Request.QueryString["unsubscribe-email"];
                    if (!string.IsNullOrEmpty(commentNotificationUnsubscribeEmailAddress))
                    {
                        if (Post.NotificationEmails.Contains(commentNotificationUnsubscribeEmailAddress))
                        {
                            Post.NotificationEmails.Remove(commentNotificationUnsubscribeEmailAddress);
                            Post.Save();
                            phCommentNotificationUnsubscription.Visible = true;
                        }
                    }
                }

            }

        }

        else
        {
            shouldThrow404 = true;
        }

        if (shouldThrow404)
        {
            Response.Redirect(Utils.RelativeWebRoot + "error404.aspx", true);
        }

    }

	/// <summary>
	/// Gets the next post filtered for invisible posts.
	/// </summary>
	private Post GetNextPost(Post post)
	{
		if (post.Next == null)
			return null;

		if (post.Next.IsVisible)
			return post.Next;

		return GetNextPost(post.Next);
	}

	/// <summary>
	/// Gets the prev post filtered for invisible posts.
	/// </summary>
	private Post GetPrevPost(Post post)
	{
		if (post.Previous == null)
			return null;

		if (post.Previous.IsVisible)
			return post.Previous;

		return GetPrevPost(post.Previous);
	}

	/// <summary>
	/// Inits the navigation links above the post and in the HTML head section.
	/// </summary>
	private void InitNavigationLinks()
	{
		if (BlogSettings.Instance.ShowPostNavigation)
		{
			Post next = GetNextPost(Post);
			Post prev = GetPrevPost(Post);

            if ((next != null && !next.Deleted) || (prev != null && !prev.Deleted))
            {
                try
                {
                    // Try to load PostNavigation from theme folder
                    var template = BlogSettings.Instance.IsRazorTheme ? "PostNavigation.cshtml" : "PostNavigation.ascx";

                    var path = string.Format("{0}Custom/Themes/{1}/{2}", 
                        Utils.ApplicationRelativeWebRoot, BlogSettings.Instance.Theme, template);

                    if (!System.IO.File.Exists(Server.MapPath(path)))
                        path = Utils.ApplicationRelativeWebRoot + "Custom/Controls/Defaults/PostNavigation.ascx";
                    else
                        path = Utils.ApplicationRelativeWebRoot + "Custom/Themes/" + BlogSettings.Instance.GetThemeWithAdjustments(null) + "/PostNavigation.ascx";
                    
                    var navView = (PostNavigationBase)LoadControl(path);
                    navView.CurrentPost = this.Post;
                    phPostNavigation.Controls.Add(navView);
                    phPostNavigation.Visible = true;
                }
                catch (Exception ex)
                {
                    Utils.Log("Error loading PostNavigation template", ex);
                }
            }
		}
	}

	/// <summary>
	/// Adds the post's description as the description metatag.
	/// </summary>
	private void AddMetaDescription()
	{
        var desc = BlogSettings.Instance.Name + " - " + BlogSettings.Instance.Description + " - " + Post.Description;
        base.AddMetaTag("description", Server.HtmlEncode(desc));
	}

	/// <summary>
	/// Adds the post's tags as meta keywords.
	/// </summary>
	private void AddMetaKeywords()
	{
        if (Post.Tags.Count > 0)
		{
            base.AddMetaTag("keywords", Server.HtmlEncode(string.Join(",", Post.Tags.ToArray())));
		}
	}

	public Post Post;

    public static bool ShowBlogengineComments
    {
        get
        {
            return BlogSettings.Instance.IsCommentsEnabled &&
                BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.BlogEngine;
        }
    }
    public static bool ShowDisqusComments
    {
        get
        {
            return BlogSettings.Instance.IsCommentsEnabled &&
                BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.Disqus;
        }
    }
    public static bool ShowFacebookComments
    {
        get
        {
            return BlogSettings.Instance.IsCommentsEnabled &&
                BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.Facebook;
        }
    }

}
