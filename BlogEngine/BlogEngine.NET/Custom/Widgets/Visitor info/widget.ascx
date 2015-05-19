<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.VisitorInfo.Widget" Codebehind="widget.ascx.cs" %>

<span runat="server" id="pName" />
<br /><br />
<span runat="server" id="pComment" />

<asp:PlaceHolder runat="Server" ID="phScript" Visible="false">
  <div id="visitor_widget_apml" style="margin-top:5px">
  
  </div>
  
  <script type="text/javascript">
		function checkApml()
		{
			var apml_url = '<asp:literal runat="server" id="ltWebsite" />';
			BlogEngine.createCallback(BlogEngineRes.webRoot + 'widgets/visitor info/apmlchecker.ashx?url=' + apml_url, 
			function(response) 
			{
				if (response.length > 10)
				{
					var text = '<%=Resources.labels.visitorInfoApmlFound %> <a href=\"'+apml_url+'">'+apml_url+'</a>. ';
					text += '<%=Resources.labels.visitorInfoApmlQuestion %> ';
					text += '<a href="javascript:void(location.href=\''+BlogEngineRes.webRoot+'?apml='+encodeURIComponent(response)+'\')"><%=Resources.labels.visitorInfoApplyFilter %></a>';
					BlogEngine.$('visitor_widget_apml').innerHTML = text;
				}
			});
		}

		setTimeout('checkApml()', 1000);
  </script>
</asp:PlaceHolder>