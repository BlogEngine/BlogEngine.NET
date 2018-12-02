<%@ Page Language="C#" AutoEventWireup="true" Inherits="store" ValidateRequest="false" Codebehind="store.aspx.cs" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">
  <div id="store" class="store-page page-global">

    <div id="divForm" data-id="divForm" runat="server">
      <h2 class="store-page-title page-global-title"><%=Resources.labels.store %></h2>
      <div class="store-page-message"><%=BlogSettings.Instance.ContactFormMessage %></div>
        <div class="form-group">
          <label for="<%=txtName.ClientID %>"><%=Resources.labels.name %></label>
          <asp:TextBox runat="server" id="txtName" cssclass="field form-control" data-id="txtName" />
            <asp:requiredfieldvalidator runat="server" CssClass="required-field" controltovalidate="txtName" ErrorMessage="<%$Resources:labels, required %>" validationgroup="store" />
        </div>
        <div class="form-group">
          <label for="<%=txtEmail.ClientID %>"><%=Resources.labels.email %></label> 
          <asp:TextBox runat="server" id="txtEmail" cssclass="field form-control" data-id="txtEmail" />
            <asp:requiredfieldvalidator runat="server"  CssClass="required-field" controltovalidate="txtEmail" ErrorMessage="<%$Resources:labels, required %>" validationgroup="store" /> <asp:RegularExpressionValidator runat="server"   CssClass="required-field" ControlToValidate="txtEmail" display="dynamic" ErrorMessage="<%$Resources:labels, enterValidEmail %>" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" validationgroup="store" />       
        </div>
        <div class="form-group">
          <label for="<%=txtSubject.ClientID %>"><%=Resources.labels.subject %></label> 
          <asp:TextBox runat="server" id="txtSubject" cssclass="field form-control" data-id="txtSubject" />
            <asp:requiredfieldvalidator runat="server"  CssClass="required-field" controltovalidate="txtSubject" ErrorMessage="<%$Resources:labels, required %>" validationgroup="store" />
        </div>
        <div class="form-group">
          <label for="<%=txtMessage.ClientID %>"><%=Resources.labels.message %></label> 
          <asp:TextBox runat="server" id="txtMessage" textmode="multiline" cssclass="form-control" rows="5" columns="30" data-id="txtMessage"/>
            <asp:requiredfieldvalidator runat="server"  CssClass="required-field" controltovalidate="txtMessage" ErrorMessage="<%$Resources:labels, required %>" display="dynamic" validationgroup="store" />    
        </div>
        <div class="form-group">
          <asp:placeholder runat="server" id="phAttachment">      
            <label for="<%=txtAttachment.ClientID %>"><%=Resources.labels.attachFile %></label>
            <asp:FileUpload runat="server" id="txtAttachment" data-id="txtAttachment"/>
          </asp:placeholder>
        </div>
      <blog:RecaptchaControl runat="server" ID="recaptcha" />
      <asp:HiddenField runat="server" ID="hfCaptcha" />
      <div class="text-right btn-wrapper">
        <asp:button runat="server" id="btnSend" class="btn btn-primary" Text="<%$Resources:labels, send %>" OnClientClick="return beginSendMessage();" validationgroup="store" data-id="btnSend" />    
        <asp:label runat="server" id="lblStatus" visible="false"><%=BlogSettings.Instance.ContactErrorMessage %>.</asp:label>
      </div>
    </div>


    <div id="thanks">
      <div id="divThank" data-id="divThank" runat="Server" visible="False">      
      <div><%=BlogSettings.Instance.ContactThankMessage %></div>
      </div>
    </div>
  </div>


  <script type="text/javascript" src="<%=Utils.ApplicationRelativeWebRoot %>Scripts/store.js"></script>
</asp:content>