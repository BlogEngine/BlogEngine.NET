<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>
  <% if (Location != BlogEngine.Core.ServingLocation.SinglePost)
       {%>
<article class="post post-home" id="post<%=Index %>">
      <%} %>
      <% if (Location == BlogEngine.Core.ServingLocation.SinglePost)
       {%>
<article class="post" id="post<%=Index %>">
      <%} %>
    <header class="post-header">
        <h2 class="post-title">
            <a href="<%=Post.RelativeOrAbsoluteLink %>"><%=Server.HtmlEncode(Post.Title) %></a>
        </h2>
        <div class="post-info clearfix">
            <span class="post-date"><i class="icon-calendar"></i><%=Post.DateCreated.ToString("dd MMMM yyyy") %></span>
            <span class="post-author"><i class=" icon-user"></i><a href="<%=BlogEngine.Core.Utils.AbsoluteWebRoot + "author/" + BlogEngine.Core.Utils.RemoveIllegalCharacters(Post.Author) + BlogEngine.Core.BlogConfig.FileExtension %>"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author %></a></span>
            <span class="post-category"><i class=" icon-folder"></i><%=CategoryLinks(", ") %></span>
            <a rel="nofollow" class="pull-right " href="<%=Post.RelativeOrAbsoluteLink %>#comment"><i class="icon-comment"></i>(<%=Post.ApprovedComments.Count %>)</a>
        </div>
    </header>
    <section class="post-body text">
        <asp:PlaceHolder ID="BodyContent" runat="server" />
        <%=AdminLinks %>
    </section>
    <% if (Location == BlogEngine.Core.ServingLocation.SinglePost)
       {%>
    <footer class="post-footer">
        <div class="post-tags">
            <%=Resources.labels.tags %> : <%=TagLinks(", ") %>
        </div>
        <div class="post-rating">
            <%=Rating %>
        </div>
    </footer>
    <%} %>
</article>