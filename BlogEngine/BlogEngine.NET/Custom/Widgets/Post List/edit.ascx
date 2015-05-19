<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.PostList.Edit" Codebehind="edit.ascx.cs" %>

<style type="text/css">
  #body label {display: block; float:left; width:250px}
  #body input {display: block; float:left; }
</style>

<div id="body">
    <label for="<%=txtNumberOfPosts.ClientID %>"><%=Resources.labels.numberOfPosts %></label>
    <asp:TextBox runat="server" ID="txtNumberOfPosts" Width="30" />
    <asp:CompareValidator runat="Server" ControlToValidate="txtNumberOfPosts" Type="Integer" Operator="DataTypeCheck" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="Dynamic" />
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfPosts" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="dynamic" /><br /><br />

    <label for="ddlCategories">Category</label>
    <asp:DropDownList runat="server" ID="ddlCategories"></asp:DropDownList>
    <br /><br />

    <label for="ddlAuthors">Author</label>
    <asp:DropDownList runat="server" ID="ddlAuthors"></asp:DropDownList>
    <br /><br />

    <label for="ddlSortBy">Sort by</label>
    <asp:DropDownList runat="server" ID="ddlSortBy">
      <asp:ListItem Text="Published" />
      <asp:ListItem Text="Comments" />
      <asp:ListItem Text="Alphabetical" />
    </asp:DropDownList>
    <br /><br />

    <label for="<%=cbShowImg.ClientID %>">Display first image as thumbnail</label>
    <asp:CheckBox runat="Server" ID="cbShowImg" />
    <br /><br />

    <label for="<%=cbShowDesc.ClientID %>">Display post description</label>
    <asp:CheckBox runat="Server" ID="cbShowDesc" />
    <br /><br />
</div>