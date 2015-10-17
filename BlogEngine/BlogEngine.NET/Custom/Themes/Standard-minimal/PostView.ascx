<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>
<%@ Import Namespace="BlogEngine.Core" %>
<% if (Location != ServingLocation.SinglePost)
   {%>
<article class="post post-home" id="post<%=Index %>">
    <%} %>
    <% if (Location == ServingLocation.SinglePost)
       {%>
    <article class="post post-single" id="post<%=Index %>">
        <%} %>
        <header class="post-header">
            <h2 class="post-title">
                <a href="<%=Post.RelativeOrAbsoluteLink %>"><%=Server.HtmlEncode(Post.Title) %></a>
            </h2>
            <div class="post-info">
                <span class="post-date"><time datetime="<%=Post.DateCreated.ToString("dd-mm-yyyy") %>"><%=Post.DateCreated.ToString("dd MMMM yyyy") %></time></span>
                <%--<span class="post-author"><i class=" icon-user"></i><a href="<%=Utils.AbsoluteWebRoot + "author/" + Utils.RemoveIllegalCharacters(Post.Author) + BlogConfig.FileExtension %>"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author %></a></span>--%>
                <span class="post-category"><%=CategoryLinks("/") %></span>
            </div>
        </header>

        <div class="post-body text">
            <asp:PlaceHolder ID="BodyContent" runat="server" />
            <%=AdminLinks %>
        </div>
        <% if (Location == ServingLocation.SinglePost)
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
