<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.Twitter.Edit" Codebehind="edit.ascx.cs" %>
<%@ Reference Control="~/Custom/Widgets/Twitter/widget.ascx" %>

<label for="<%=txtAccountUrl %>"><%=Resources.labels.twitterAccountUrl %></label><br />
<asp:TextBox runat="server" ID="txtAccountUrl" Width="300" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtAccountUrl" ErrorMessage="<%$Resources:labels, enterValidUrl %>" Display="dynamic" /><br /><br />

<label for="<%=txtUrl %>"><%=Resources.labels.twitterRssFeedUrl %></label><br />
<asp:TextBox runat="server" ID="txtUrl" Width="300" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtUrl" ErrorMessage="<%$Resources:labels, enterValidUrl %>" Display="dynamic" /><br /><br />

<label for="<%=txtTwits %>"><%=Resources.labels.twitterNumberOfDisplayedTwits %></label><br />
<asp:TextBox runat="server" ID="txtTwits" Width="30" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtTwits" ErrorMessage="<%$Resources:labels, enterValidNumber %>" Display="dynamic" />
<asp:CompareValidator runat="server" ControlToValidate="txtTwits" Type="Integer" Operator="dataTypeCheck" ErrorMessage="<%$Resources:labels, noValidNumber %>" /><br /><br />

<label for="<%=txtPolling %>"><%=Resources.labels.twitterPollingInterval %></label><br />
<asp:TextBox ID="txtPolling" runat="server" Width="30" />
<asp:CompareValidator runat="server" ControlToValidate="txtPolling" Type="Integer" Operator="dataTypeCheck" ErrorMessage="<%$Resources:labels, noValidNumber %>" /><br /><br />

<label for="<%=txtFollowMe %>"><%=Resources.labels.twitterFollowMeText %></label><br />
<asp:TextBox runat="server" ID="txtFollowMe" Width="300" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtFollowMe" ErrorMessage="<%$Resources:labels, enterValidString %>" Display="dynamic" /><br /><br />
