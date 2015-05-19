<%@ Page Title="Register" Language="C#" MasterPageFile="account.master" AutoEventWireup="true" Inherits="Account.Register" Codebehind="register.aspx.cs" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:CreateUserWizard ID="RegisterUser" runat="server" EnableViewState="false" OnCreatedUser="RegisterUser_CreatedUser"
        OnCreatingUser="RegisterUser_CreatingUser">
        <WizardSteps>
            <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server">
                <ContentTemplate>
                    <div class="page-header clearfix">
                        <h3 id="CreateHdr">
                            <%=Resources.labels.createAccount %></h3>

                    </div>
                    <div class="account-content">
                        <div class="form-group">
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><%=Resources.labels.userName %>:</asp:Label>
                            <div class="boxRound">
                                <asp:TextBox ID="UserName" runat="server" CssClass="textEntry form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email"><%=Resources.labels.email %>:</asp:Label>
                            <div class="boxRound">
                                <asp:TextBox ID="Email" runat="server" CssClass="textEntry form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><%=String.Format(Resources.labels.passwordMinimumCharacters, Membership.MinRequiredPasswordLength) %></asp:Label>
                            <div class="boxRound">
                                <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry form-control" TextMode="Password"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"><%=Resources.labels.confirmPassword %>:</asp:Label>
                            <div class="boxRound">
                                <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry form-control" TextMode="Password"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <blog:RecaptchaControl ID="recaptcha" runat="server" />
                        </div>
                    </div>
                    <hr />
                    <div class="btn-wrapper text-center">
                        <asp:Button ID="CreateUserButton" CssClass="btn btn-primary btn-block btn-lg" runat="server" CommandName="MoveNext" Text="<%$Resources:labels,createUser %>" OnClientClick="return ValidateNewUser()" />
                        <hr />
                        <p>
                            <span>
                                <%=Resources.labels.alreadyHaveAccount %>
                                <a id="HeadLoginStatus" runat="server"><%=Resources.labels.loginNow %></a></span>
                        </p>
                    </div>
                </ContentTemplate>
                <CustomNavigationTemplate>
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
    <asp:HiddenField ID="hdnPassLength" runat="server" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='UserName']").focus();
        });
    </script>
</asp:Content>
