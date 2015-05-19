<%@ Page Language="C#" MasterPageFile="account.master" AutoEventWireup="true" Inherits="Account.ChangePassword" Codebehind="change-password.aspx.cs" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="page-header clearfix">
        <h3>
            <%=Resources.labels.changePassword %></h3>
    </div>
    <p>
        <%=String.Format(Resources.labels.requiredPasswordLength,Membership.MinRequiredPasswordLength) %>
    </p>
    <br />
    <asp:ChangePassword ID="ChangeUserPassword" runat="server" CancelDestinationPageUrl="~/"
        EnableViewState="false" RenderOuterTable="false">
        <ChangePasswordTemplate>
            <div class="account-content">
                <div class="form-group">
                    <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"><%=Resources.labels.oldPassword %>:</asp:Label>
                    <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry form-control ltr-dir" TextMode="Password"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"><%=Resources.labels.newPassword %>:</asp:Label>
                    <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry form-control ltr-dir" TextMode="Password"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"><%=Resources.labels.confirmNewPassword %>:</asp:Label>
                    <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry form-control ltr-dir" TextMode="Password"></asp:TextBox>
                </div>
                <hr />
                <div class="btn-wrapper">
                    <asp:Button ID="ChangePasswordPushButton" CssClass="btn btn-block btn-primary btn-lg" runat="server" CommandName="ChangePassword" Text="<%$Resources:labels,changePassword %>" OnClick="ChangePasswordPushButton_Click" OnClientClick="return ValidateChangePassword();" />
                </div>
            </div>
        </ChangePasswordTemplate>
    </asp:ChangePassword>
    <asp:HiddenField ID="hdnPassLength" runat="server" />
</asp:Content>
