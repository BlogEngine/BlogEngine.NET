<%@ Control Language="C#" AutoEventWireup="true" Inherits="App_Code.Controls.WidgetContainer" %>
<div class="widget clearfix <%= Widget.Name.Replace(" ", String.Empty).ToLowerInvariant() %>" id="widget<%= Widget.WidgetId %>">
    <%= AdminLinks %>
    <% if (this.Widget.ShowTitle)
       { %>
   <div class="widget-title">
        <%= Widget.Title%></div>
    <% } %>
    <div class="widget-body">
        <asp:PlaceHolder ID="phWidgetBody" runat="server"></asp:PlaceHolder>
    </div>
</div>
