<%@ Page Language="C#" MasterPageFile="account.master" AutoEventWireup="true" ClientIDMode="Static" Inherits="Account.Login" CodeBehind="login.aspx.cs" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false" OnAuthenticate="LoginUser_OnAuthenticate">
        <LayoutTemplate>
            <h1 class="page-header"><asp:Label runat="server" ID="lblTitle" Text="<%$Resources:labels,login %>" /></h1>

            <div class="form-group first-child with-icon">
               <span class="icon-form-group"> <img src="../Content/images/blog/icon-user.svg" class="icon-user" /></span>
                <asp:TextBox ID="UserName" runat="server" AutoCompleteType="None" placeholder="username" CssClass="textEntry ltr-dir"></asp:TextBox>
            </div>
            <div class="form-group with-icon">
                <span class="icon-form-group"><img src="../Content/images/blog/icon-pass.svg" class="icon-pass" /></span>
                <asp:TextBox ID="Password" runat="server" placeholder="password" CssClass="passwordEntry ltr-dir" TextMode="Password"></asp:TextBox>
            </div>
            <div class="form-group with-icon">
                <span class="icon-form-group"><asp:CheckBox ID="RememberMe" runat="server" /></span>
                <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="label-title "><%=Resources.labels.rememberMe %></asp:Label>
            </div>
                <asp:Button ID="LoginButton" runat="server" CommandName="Login" CssClass="btn btn-success btn-block btn-lg" Text="<%$Resources:labels,login %>" OnClientClick="return ValidateLogin();" />
            <div class="text-center text-uppercase small-link ">
                <asp:PlaceHolder ID="phResetPassword" runat="server">
                    <asp:HyperLink runat="server" ID="linkForgotPassword" CssClass="text-muted" Text="<%$ Resources:labels,forgotPassword %>" />
                </asp:PlaceHolder>
            </div>
        </LayoutTemplate>
    </asp:Login>
    <% if (BlogEngine.Core.BlogSettings.Instance.EnableSelfRegistration)
        { %>
    <div id="LoginRegister" class="text-center">
        <hr />
        <%=Resources.labels.dontHaveAccount %>
        <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false" />
    </div>
    <% } %>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='UserName']").focus();
        });
    </script>
</asp:Content>
