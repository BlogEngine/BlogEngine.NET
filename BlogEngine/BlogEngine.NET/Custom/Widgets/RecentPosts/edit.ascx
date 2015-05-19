<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.RecentPosts.Edit" Codebehind="edit.ascx.cs" %>

<style type="text/css">
  #body label {display: block; float:left; width:150px}
  #body input {display: block; float:left; }
</style>

<div id="body">

<label for="<%=txtNumberOfPosts.ClientID %>"><%=Resources.labels.numberOfPosts %></label>
<asp:TextBox runat="server" ID="txtNumberOfPosts" Width="30" />
<asp:CompareValidator runat="Server" ControlToValidate="txtNumberOfPosts" Type="Integer" Operator="DataTypeCheck" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="Dynamic" />
<asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfPosts" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="dynamic" /><br /><br />

<label for="<%=cbShowComments.ClientID %>"><%=Resources.labels.showComments %></label>
<asp:CheckBox runat="Server" ID="cbShowComments" />
<br /><br />

<label for="<%=cbShowRating.ClientID %>"><%=Resources.labels.showRating %></label>
<asp:CheckBox runat="Server" ID="cbShowRating" />

</div>