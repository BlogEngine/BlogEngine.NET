<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="Widgets.Search.Edit" %>
<div>
    <label style="display: block; margin: 10px 0 5px 0" for="<%=txtButtonText %>"><%=Resources.labels.buttonText %></label>
    <asp:TextBox runat="server" ID="txtButtonText" Width="300" />
</div>
<div>
    <label style="display: block; margin: 10px 0 5px 0" for="<%=txtFieldText %>"><%=Resources.labels.searchFieldText %></label>
    <asp:TextBox runat="server" ID="txtFieldText" Width="300" />
</div>