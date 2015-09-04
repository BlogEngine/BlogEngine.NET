function beginSendMessage() {
    if (BlogEngine.$('<%=txtAttachment.ClientID %>') && BlogEngine.$('<%=txtAttachment.ClientID %>').value.length > 0)
        return true;

    if (!Page_ClientValidate('contact'))
        return false;

    var recaptchaResponseField = document.getElementById('recaptcha_response_field');
    var recaptchaResponse = recaptchaResponseField ? recaptchaResponseField.value : "";

    var recaptchaChallengeField = document.getElementById('recaptcha_challenge_field');
    var recaptchaChallenge = recaptchaChallengeField ? recaptchaChallengeField.value : "";

    var name = BlogEngine.$('<%=txtName.ClientID %>').value;
    var email = BlogEngine.$('<%=txtEmail.ClientID %>').value;
    var subject = BlogEngine.$('<%=txtSubject.ClientID %>').value;
    var message = BlogEngine.$('<%=txtMessage.ClientID %>').value;
    var sep = '-||-';
    var arg = name + sep + email + sep + subject + sep + message + sep + recaptchaResponse + sep + recaptchaChallenge;
    WebForm_DoCallback('__Page', arg, endSendMessage, 'contact', onSendError, false)

    BlogEngine.$('<%=btnSend.ClientID %>').disabled = true;

    return false;
}

function endSendMessage(arg, context) {

    if (arg == "RecaptchaIncorrect") {
        displayIncorrectCaptchaMessage();
        BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;

        if (document.getElementById('recaptcha_response_field')) {
            Recaptcha.reload();
        }
    }
    else {
        if (document.getElementById("spnCaptchaIncorrect")) document.getElementById("spnCaptchaIncorrect").style.display = "none";

        BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;
        var form = BlogEngine.$('<%=divForm.ClientID %>')
        var thanks = BlogEngine.$('thanks');

        form.style.display = 'none';
        thanks.innerHTML = arg;
    }
}

function displayIncorrectCaptchaMessage() {
    if (document.getElementById("spnCaptchaIncorrect")) document.getElementById("spnCaptchaIncorrect").style.display = "";
}

function onSendError(err, context) {
    if (document.getElementById('recaptcha_response_field')) {
        Recaptcha.reload();
    }
    BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;
    alert("Sorry, but the following occurred while attemping to send your message: " + err);
}