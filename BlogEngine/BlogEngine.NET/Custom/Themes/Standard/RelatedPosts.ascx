<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.RelatedPostsBase" %>
<div id="relatedPosts" class="well-global">
    <h3 class="well-global-title"><%=Resources.labels.relatedPosts %></h3>
    <ul class="related-posts">
        <%foreach (var item in RelatedPostList)
            {%>
        <li class="related-posts-item">
            <a href="<%=item.Link %>"><%=item.Title %></a>
            <p><%=item.Description %></p>
        </li>
        <% } %>
    </ul>
</div>
