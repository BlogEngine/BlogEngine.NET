<%@ Import Namespace="BlogEngine.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.BlogRoll.Widget" Codebehind="widget.ascx.cs" %>
<blog:Blogroll ID="Blogroll1" runat="server" />
<a href="<%=Utils.AbsoluteWebRoot %>opml.axd" style="display: block; text-align: right"
    title="<%=Resources.labels.downloadOPML %>"><%=Resources.labels.downloadOPML %> <img src="<%=Utils.ApplicationRelativeWebRoot %>Content/images/blog/opml.png" width="12" height="12"
        alt="OPML" /></a>