<%@ Control Language="C#" EnableViewState="False" Inherits="BlogEngine.Core.Web.Controls.CommentViewBase" %>
<ul id="id_<%=Comment.Id %>">
    <li class="comment-item">
        <div class="comment-content <%= Post.Author.Equals(Comment.Author, StringComparison.OrdinalIgnoreCase) ? " self" : "" %>">
            <%= Gravatar(48)%>
            <div class="comment-author">
                <%= Comment.Website != null ? "<a href=\"" + Comment.Website + "\" rel=\"nofollow\" class=\"url fn\">" + Comment.Author + "</a>" : "<a class=\"fn\">" +Comment.Author + "</a>" %>
            </div>
            <div class="comment-text">
                <%= Text %>
            </div>
            <div class="comment-replylink">
                <span><%=Comment.DateCreated.ToString("dd MMMM yyyy") %></span> - <%=ReplyToLink%>
            </div>
            <div class="comment-adminlinks">
                 <%= AdminLinks.Length > 2 ? AdminLinks.Substring(2) : AdminLinks %>
            </div>
        </div>
        <div id="replies_<%=Comment.Id %>" <%= (Comment.Comments.Count == 0 || Comment.Email == "pingback" || Comment.Email == "trackback") ? " style=\"display:none;\"" : "" %>>
            <asp:PlaceHolder ID="phSubComments" runat="server" />
        </div>
    </li>
</ul>
