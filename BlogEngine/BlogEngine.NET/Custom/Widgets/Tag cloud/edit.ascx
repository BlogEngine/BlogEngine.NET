<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.TagCloud.Edit" Codebehind="edit.ascx.cs" %>
<%@ Reference VirtualPath="~/Custom/Widgets/Tag cloud/widget.ascx" %>

<label for="<%=ddlMinimumPosts.ClientID %>"><%=Resources.labels.tagMinimumPosts %></label><br />
<asp:DropDownList runat="server" ID="ddlMinimumPosts">
  <asp:ListItem Value="1" Text="<%$ Resources:labels, defaultMinTag %>" />
  <asp:ListItem Text="2" />
  <asp:ListItem Text="3" />
  <asp:ListItem Text="4" />
  <asp:ListItem Text="5" />
  <asp:ListItem Text="6" />
  <asp:ListItem Text="7" />
  <asp:ListItem Text="8" />
  <asp:ListItem Text="9" />
  <asp:ListItem Text="10" />
</asp:DropDownList>
<br /><br />
<label for="<%=ddlCloudSize.ClientID %>"><%=Resources.labels.tagCloudMaxSize %></label><br />
<asp:DropDownList runat="server" ID="ddlCloudSize">
  <asp:ListItem Value="-1" Text="<%$ Resources:labels, unlimited %>" />
  <asp:ListItem Text="10" />
  <asp:ListItem Text="25" />
  <asp:ListItem Text="50" />
  <asp:ListItem Text="75" />
  <asp:ListItem Text="100" />
  <asp:ListItem Text="125" />
  <asp:ListItem Text="150" />
</asp:DropDownList>