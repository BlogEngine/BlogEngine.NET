<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostNavigationBase" %>
<div id="postnavigation" class="navigation-posts row">
    <div class="col-6 text-left prev-post">
        <% if (!string.IsNullOrEmpty(PreviousPostUrl))
           { %>
        <a href="<%=PreviousPostUrl %>" class="nav-prev"><i class="fa fa-chevron-left"></i> Previous Post</a>
        <% } %>
    </div>
    <div class="col-6 text-right next-post">
        <% if (!string.IsNullOrEmpty(NextPostUrl))
           { %>
        <a href="<%=NextPostUrl %>" class="nav-next">Next Post <i class="fa fa-chevron-right"></i></a>
        <% } %>
    </div>
</div>
