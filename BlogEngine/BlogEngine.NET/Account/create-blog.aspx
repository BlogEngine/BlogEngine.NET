<%@ Page Title="" Language="C#" MasterPageFile="account.master" AutoEventWireup="true" Inherits="Account.CreateBlog" Codebehind="create-blog.aspx.cs" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<%@ Import Namespace="BlogEngine.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="page-header clearfix">
        <h3 id="CreateHdr"><%=Resources.labels.createBlog %></h3>
    </div>
    <div class="accountInfo">

        <div class="form-group ltr-dir">
            <label style="font-weight:normal;"><%=BlogEngine.Core.Utils.AbsoluteWebRoot %></label><span id="blogId" style="font-weight: bold"></span>
            <asp:TextBox ID="BlogName" runat="server" CssClass="textEntry form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="UserLabel" runat="server" AssociatedControlID="UserName"><%=Resources.labels.userName %>:</asp:Label>
            <asp:TextBox ID="UserName" runat="server" CssClass="textEntry form-control ltr-dir"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email"><%=Resources.labels.email %>:</asp:Label>
            <asp:TextBox ID="Email" runat="server" CssClass="textEntry form-control ltr-dir"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><%=String.Format(Resources.labels.passwordMinimumCharacters, Membership.MinRequiredPasswordLength) %></asp:Label>
            <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry form-control ltr-dir" TextMode="Password"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"><%=Resources.labels.confirmPassword %>:</asp:Label>
            <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry form-control ltr-dir" TextMode="Password"></asp:TextBox>
        </div>
        <div class="form-group">
            <blog:RecaptchaControl ID="recaptcha" runat="server" />
        </div>
        <hr />
        <div class="btn-wrapper text-right">
             <a href="<%= Utils.RelativeWebRoot %>Account/login.aspx" class="btn btn-default"><%=Resources.labels.cancel %></a>
            <asp:Button ID="CreateUserButton" runat="server" Text="Create" CssClass="btn btn-primary" OnClientClick="return ValidateNewBlog()" OnClick="CreateUserButton_Click" />
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='BlogName']").focus();

            $(function () {
                $('#<%= BlogName.ClientID %>').keyup(function () {
                    $('#blogId').text($(this).val());
                });
            });
        });
    </script>
</asp:Content>

