<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>
<%@ Import Namespace="BlogEngine.Core"%>
<div class="post xfolkentry" id="post<%=Index %>">
  <h1><a href="<%=Post.RelativeLink %>" class="taggedlink"><%=Server.HtmlEncode(Post.Title) %></a></h1>
  <span class="author">by <a href="<%=VirtualPathUtility.ToAbsolute("~/") + "author/" + BlogEngine.Core.Utils.RemoveIllegalCharacters(Post.Author) + BlogEngine.Core.BlogConfig.FileExtension %>"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author %></a></span>
  <span class="pubDate"><%=Post.DateCreated.ToString("d. MMMM yyyy HH:mm") %></span>
  
  <div class="text"><asp:PlaceHolder ID="BodyContent" runat="server" /></div>
  <div class="bottom">
    <%=Rating %>
    <p class="tags">Tags: <%=TagLinks(", ") %></p>
    <p class="categories"><%=CategoryLinks(" | ") %></p>
  </div>

  <div class="footer">    
    <div class="bookmarks">
      <a rel="nofollow" href="mailto:?subject=<%=Post.Title %>&amp;body=I thought that you might find this interesting: <%=Post.AbsoluteLink.ToString() %>">E-Mail</a> | 
      <a rel="nofollow" href="http://www.dotnetkicks.com/submit?url=<%=Server.UrlEncode(Post.AbsoluteLink.ToString()) %>&amp;title=<%=Server.UrlEncode(Post.Title) %>">Kick it!</a> | 
      <a rel="nofollow" href="http://www.dzone.com/links/add.html?url=<%=Server.UrlEncode(Post.AbsoluteLink.ToString()) %>&amp;title=<%=Server.UrlEncode(Post.Title) %>">DZone it!</a> | 
      <a rel="nofollow" href="http://del.icio.us/post?url=<%=Server.UrlEncode(Post.AbsoluteLink.ToString()) %>&amp;title=<%=Server.UrlEncode(Post.Title) %>">del.icio.us</a>      
    </div>   
    <%=AdminLinks %>   
    <% if (BlogSettings.Instance.CommentProvider == BlogSettings.CommentsBy.BlogEngine) { %>
    <a rel="bookmark" href="<%=Post.PermaLink %>" title="<%=Server.HtmlEncode(Post.Title) %>">Permalink</a> |
    <a rel="nofollow" href="<%=Post.RelativeLink %>#comment"><%=Resources.labels.comments %> (<%=Post.ApprovedComments.Count %>)</a>   
	<%} else { %>
    <a rel="nofollow" href="<%=Post.PermaLink %>#comment"><%=Resources.labels.comments %></a> 
    <%} %>
  </div>
</div>