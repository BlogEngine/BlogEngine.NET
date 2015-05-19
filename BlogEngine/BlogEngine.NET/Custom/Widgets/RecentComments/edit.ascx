<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.RecentComments.Edit" Codebehind="edit.ascx.cs" %>

<style type="text/css">
  #body label {display: block; float:left; width:150px}
  #body input {display: block; float:left; }
</style>

<div id="body">

<label for="<%=txtNumberOfPosts.ClientID %>"><%=Resources.labels.numberOfComments %></label>
<asp:TextBox runat="server" ID="txtNumberOfPosts" Width="30" />
<asp:CompareValidator runat="Server" ControlToValidate="txtNumberOfPosts" Type="Integer" Operator="DataTypeCheck" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="Dynamic" />
<asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfPosts" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="dynamic" />

</div>