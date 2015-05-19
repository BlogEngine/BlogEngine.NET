<%@ Page Language="C#" AutoEventWireup="true" Inherits="Admin.WidgetEditor"
    ValidateRequest="false" Codebehind="WidgetEditor.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Widget Editor</title>
    <style type="text/css">
        body
        {
            font: 11px verdana;
            margin: 0;
            overflow: hidden;
        }
        #title
        {
            background: #F1F1F1;
            border-bottom: 1px solid silver;
            padding: 10px;
        }
        label
        {
            font-weight: bold;
        }
        #phEdit
        {
            padding: 10px;
            height: 390px;
            overflow: auto;
            overflow-x: hidden;
        }
        #bottom
        {
            background: #F1F1F1;
            border-top: 1px solid silver;
            padding: 10px;
            text-align: right;
        }
    </style>
</head>
<body scroll="no" onkeypress="ESCclose(event)">
    <script type="text/javascript">
        function ESCclose(evt) {
            if (!evt) evt = window.event;

            if (evt && evt.keyCode == 27)
                window.parent.BlogEngine.widgetAdmin.closeEditor();
        }
        function PostEdit() {
            if (parent && typeof parent.BlogEngine != 'undefined' &&
            typeof parent.BlogEngine.widgetAdmin != 'undefined' &&
            typeof parent.BlogEngine.widgetAdmin.clearWidgetList != 'undefined') {

                parent.BlogEngine.widgetAdmin.clearWidgetList();
            }

            top.location.reload(false);
        }
    </script>
    <form id="form1" runat="server">
    <div id="title">
        <label for="<%=txtTitle.ClientID %>">
            <%=Resources.labels.title %></label>&nbsp;&nbsp;&nbsp;
        <asp:TextBox runat="server" ID="txtTitle" Width="300px" />
        <asp:CheckBox runat="Server" ID="cbShowTitle" Text="<%$Resources:labels, showTitle %>" />
    </div>
    <div runat="server" id="phEdit" />
    <div id="bottom">
        <asp:Button runat="server" ID="btnSave" Text="Save" />
        <input type="button" value="<%=Resources.labels.cancel %>" onclick="parent.BlogEngine.widgetAdmin.closeEditor()" />
    </div>
    </form>
</body>
</html>
