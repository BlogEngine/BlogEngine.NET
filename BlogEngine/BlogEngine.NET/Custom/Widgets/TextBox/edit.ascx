<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.TextBox.Edit" Codebehind="edit.ascx.cs" %>
<%@ Import Namespace="BlogEngine.Core" %>

<script type="text/javascript" src="<%=Utils.RelativeWebRoot %>admin/editors/tinymce/tinymce.min.js"></script>
<script type="text/javascript">
	tinyMCE.init({
		selector: "#<%=txtText.ClientID %>",
	    plugins: [
            "advlist autolink lists link image charmap print preview anchor",
            "searchreplace visualblocks code fullscreen",
            "insertdatetime media table contextmenu paste sh4tinymce"
	    ],
	    toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | sh4tinymce",
	    autosave_ask_before_unload: false,
	    max_height: 300,
	    min_height: 160,
	    height: 240
	});
</script>
<style>.mce-ico { height: 20px !important }</style>
<asp:TextBox runat="server" ID="txtText" TextMode="multiLine" Columns="100" Rows="10" style="width:700px;height:280px" /><br />