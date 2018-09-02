<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>
<article class="post" id="post<%=Index %>">
    <h2 class="post-title">
        <a href="<%=Post.RelativeOrAbsoluteLink %>" class="taggedlink"><%=Server.HtmlEncode(Post.Title) %></a>
    </h2>
    <div class="post-info Clear">
        <span class="post-date"><%=Post.DateCreated.ToString("d. MMMM yyyy") %> <span class="separator"></span></span>
        <span class="post-author"><a href="<%=BlogEngine.Core.Utils.AbsoluteWebRoot + "author/" + BlogEngine.Core.Utils.RemoveIllegalCharacters(Post.Author) + BlogEngine.Core.BlogConfig.FileExtension %>"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author %></a> <span class="separator"></span></span>
        <span class="post-category"><%=CategoryLinks(" , ") %> </span>
        <a class="post-comment" rel="nofollow" href="<%=Post.RelativeOrAbsoluteLink %>#comment"><%=Resources.labels.comments %> (<%=Post.ApprovedComments.Count %>)</a>
        <script type="text/javascript">$('#post<%=Index %> .post-category:has(a)').append('<span class="separator"></span>');</script>
    </div>
    <div class="post-body text">
        <asp:PlaceHolder ID="BodyContent" runat="server" />
    </div>
    <% if (Location == BlogEngine.Core.ServingLocation.SinglePost)
       {%>
    <div class="post-footer clearfix">
        <%=Rating %>
        <div class="post-tags">
            <%=Resources.labels.tags %>: <%=TagLinks(" , ") %>
        </div>
    </div>
    <script type="text/javascript">$(".post").addClass("post-single");</script>
    <%  }%>
    <%=AdminLinks %>
</article>
