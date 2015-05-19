<%@ Page Title="Change Password" Language="C#" MasterPageFile="account.master" AutoEventWireup="true" Inherits="Account.ChangePasswordSuccess" Codebehind="change-password-success.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="page-header clearfix">
        <h3>
            <%=Resources.labels.changePassword %>
        </h3>
    </div>
    <div id="ChangePwd" class="alert alert-success">
        <%=Resources.labels.passwordChangeSuccess %>
    </div>
</asp:Content>
