

function beginSendMessage() {
    //if ($('[data-id="txtAttachment"]').length > 0 && $('[data-id="txtAttachment"]').val().length > 0)
    //    return true;

    if (!Page_ClientValidate('store'))
        return false;

    var recaptchaResponseField = $('#g-recaptcha-response');
    var recaptchaResponse = recaptchaResponseField.length > 0 ? recaptchaResponseField.val() : "";

    var testArg = {
        name: $('[data-id="txtName"]').val(),
        email: $('[data-id="txtEmail"]').val(),
        subject: $('[data-id="txtSubject"]').val(),
        message: $('[data-id="txtMessage"]').val()
    };

    var name = $('[data-id="txtName"]').val();
    var email = $('[data-id="txtEmail"]').val();
    var subject = $('[data-id="txtSubject"]').val();
    var message = $('[data-id="txtMessage"]').val();
    var sep = '-||-';

    var arg = name + sep + email + sep + subject + sep + message + sep + recaptchaResponse;

    /* WebForm_DoCallback parameters follow:
    target: The name of a server Control that handles the client callback.The control must 
        implement the ICallbackEventHandler interface and provide a RaiseCallbackEvent method.
    argument: An argument passed from the client script to the server RaiseCallbackEvent 
        method.
    clientCallback: The name of the client event handler that receives the result of the 
        successful server event.
    context: Client script that is evaluated on the client prior to initiating the callback.
        The result of the script is passed back to the client event handler.
    clientErrorCallback: The name of the client event handler that receives the result when 
        an error occurs in the server event handler.
    useAsync: true to perform the callback asynchronously; false to perform the callback 
        synchronously.   */
    WebForm_DoCallback('__Page', arg, clientCallback, 'store', clientErrorCallback, false)

    $('[data-id="btnSend"]').attr("disabled", true);

    return false;
}

function clientCallback(returnMessage, context) {

    if (returnMessage == "RecaptchaIncorrect") {
        displayIncorrectCaptchaMessage();
        $('[data-id="btnSend"]').attr("disabled", "");

        if ($('#recaptcha_response_field').length > 0) {
            Recaptcha.reload();
        }
    }
    else {
        if ($("#spnCaptchaIncorrect")) $("#spnCaptchaIncorrect").css("display", "none");

        $('[data-id="btnSend"]').attr("disabled", "");
        var form = $('[data-id="divForm"]');
        var thanks = $('#thanks');

        form.css("display", "none");
        thanks.html(returnMessage);
    }
}

function displayIncorrectCaptchaMessage() {
    if ($("#spnCaptchaIncorrect")) $("#spnCaptchaIncorrect").css("display", "none");
}

function clientErrorCallback(err, context) {
    if ($('#recaptcha_response_field')) {
        Recaptcha.reload();
    }
    $('[data-id="btnSend"]').css("display", "none");
    alert("Sorry, but the following occurred while attemping to send your message: " + err);
}
