// BillKrat.2018.08.25 - adapted for Google recaptcha V2 ( http://AdventuresOnTheEdge.net )

function beginSendMessage() {
    if ($('[data-id="txtAttachment"]').length > 0 && $('[data-id="txtAttachment"]').val().length > 0)
        return true;

    if (!Page_ClientValidate('contact'))
        return false;


    var para = {
        name: $('[data-id="txtName"]').val(),
        email: $('[data-id="txtEmail"]').val(),
        subject: $('[data-id="txtSubject"]').val(),
        message: $('[data-id="txtMessage"]').val(),
        recaptchaResponse: $('#g-recaptcha-response').val(),
        recaptchaChallenge: "not-used"
    };
    var arg = JSON.stringify(para);

    $('[data-id="btnSend"]').attr("disabled", true);
    $('#recaptchaMessage').css("display", "none");

    WebForm_DoCallback('__Page', arg, endSendMessage, 'contact', onSendError, false);

    return false;
}

function endSendMessage(arg, context) {

    if (arg === "RecaptchaIncorrect") {
        displayIncorrectCaptchaMessage();
        $('[data-id="btnSend"]').attr("disabled", false);
        $('#recaptchaMessage').css("display", "");
    }
    else {
        if ($("#spnCaptchaIncorrect")) $("#spnCaptchaIncorrect").css("display", "none");

        $('[data-id="btnSend"]').attr("disabled", false);
        var form = $('[data-id="divForm"]');
        var thanks = $('#thanks');

        form.css("display", "none");
        thanks.html(arg);
    }
}

function displayIncorrectCaptchaMessage() {
    if ($("#spnCaptchaIncorrect")) $("#spnCaptchaIncorrect").css("display", "none");
}

function onSendError(err, context) {
    $('[data-id="btnSend"]').css("display", "none");
    alert("Sorry, but the following occurred while attemping to send your message: " + err);
}
