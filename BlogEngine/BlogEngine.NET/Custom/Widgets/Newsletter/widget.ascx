<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.Newsletter.Widget" Codebehind="widget.ascx.cs" %>

<div id="newsletterform">
  <p><%=Resources.labels.notifiedPublished %></p>

  <label for="<%=txtEmail.ClientID %>" style="font-weight:bold"><%=Resources.labels.enterEmail %></label><br />
  <asp:TextBox runat="server" ID="txtEmail" Width="98%" CssClass="form-control" />
  <asp:RequiredFieldValidator 
    runat="server" 
    ControlToValidate="txtEmail" 
    ErrorMessage="<%$Resources:labels, enterEmailAddress %>" 
    Display="dynamic" 
    ValidationGroup="newsletter" />
    
  <asp:RegularExpressionValidator 
    runat="server" 
    ControlToValidate="txtEmail" 
    ErrorMessage="<%$Resources:labels, enterValidEmail %>" 
    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
    Display="dynamic" 
    ValidationGroup="newsletter" />
    
  <div style="text-align:center">
    <asp:Button runat="server" ID="btnSave" ValidationGroup="newsletter" CssClass="btn btn-primary" Text="<%$Resources:labels, notifyMe %>" OnClientClick="return beginAddEmail()" />
  </div>
</div>

<div id="newsletterthanks" style="display:none;text-align:center">
  <br /><br />
  <h2 id="newsletteraction"><%=Resources.labels.thankYou %></h2>
  <br /><br />
</div>

<script type="text/javascript">
  function beginAddEmail()
  {
    if(!Page_ClientValidate('newsletter'))
        return false;

    var formAction = '<%=FormAction() %>';
    if (formAction.length > 0) {
        document.getElementById("aspnetForm").action = formAction;
    }
      
    var arg = BlogEngine.$('<%=txtEmail.ClientID %>').value;
    var context = 'newsletter';
    <%=Page.ClientScript.GetCallbackEventReference(this, "arg", "endAddEmail", "context") %>;
    
    return false;
  }
  
  function endAddEmail(arg, context)
  {
    BlogEngine.$('newsletterform').style.display = 'none';
    BlogEngine.$('newsletterthanks').style.display = 'block';
    if (arg == "false")
    {
      BlogEngine.$('newsletteraction').innerHTML = "<%=Resources.labels.nowUnsubscribed %>";
    }
  }
</script>