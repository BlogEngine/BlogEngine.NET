<%@ Page Title="Password Retrieval" Language="C#" MasterPageFile="~/Account/account.master" AutoEventWireup="true" Inherits="Account.PasswordRetrieval" Codebehind="password-retrieval.aspx.cs" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   <%-- <div class="page-header clearfix">
        <h1 style="font-size: 24px; margin: 0"><%=Resources.labels.passwordRetrieval %></h1>
    </div>
    <p>
        <%=Resources.labels.passwordRetrievalInstructionMessage %>
    </p>--%>
        <div class="form-group with-icon first-child">
               <span class="icon-form-group"> <img src="../Content/images/blog/icon-user.svg" class="icon-user" /></span>
            <asp:TextBox ID="txtUser" runat="server" placeholder="User name" AutoCompleteType="None" CssClass="textEntry form-control "></asp:TextBox>
        </div>
            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="<%$Resources:labels,send %>" CssClass="btn btn-block btn-success" OnClick="LoginButton_Click" OnClientClick="return ValidatePasswordRetrieval()" />

    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='txtUser']").focus();
        });
    </script>
</asp:Content>
