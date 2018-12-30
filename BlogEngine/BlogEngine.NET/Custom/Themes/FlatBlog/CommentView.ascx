<%@ Control Language="C#" EnableViewState="False" Inherits="BlogEngine.Core.Web.Controls.CommentViewBase" %>
<div id="id_<%=Comment.Id %>" class="vcard comment<%= Post.Author.Equals(Comment.Author, StringComparison.OrdinalIgnoreCase) ? " self" : "" %>">
    <div class="comment-contents">
        <div class="comment_header">
            <span class="gravatar Left"><%= Gravatar(32)%></span>
            <div class="visitor">
                <%= Flag %>
                <%= Comment.Website != null ? "<a href=\"" + Comment.Website + "\" rel=\"nofollow\" class=\"url fn\">" + Comment.Author + "</a>" : "<span class=\"fn\">" +Comment.Author + "</span>" %>
                <div>
                    <a class="DateTime"><%= Comment.DateCreated %></a>
                </div>
            </div>
        </div>
        <p><%= Text %></p>
        <p class="author">
            <span class='reply-to'><%=ReplyToLink%></span>
            <%= AdminLinks.Length > 2 ? AdminLinks.Substring(2) : AdminLinks %>
        </p>
    </div>
    <div class="comment-replies" id="replies_<%=Comment.Id %>" <%= (Comment.Comments.Count == 0 || Comment.Email == "pingback" || Comment.Email == "trackback") ? " style=\"display:none;\"" : "" %>>
        <asp:PlaceHolder ID="phSubComments" runat="server" />
    </div>
</div>
