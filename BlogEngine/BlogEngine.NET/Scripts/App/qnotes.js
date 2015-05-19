$(document).ready(function () {
    $("body").prepend('<div id="q-notes"></div>');
    QuickNotes();
    $(document).on("click","#open", function() {
        $("div#q-panel").slideDown("slow");
        if ($('.q-area') && $('.q-area').val()) {
            var len = $('.q-area').val().length;
            $('.q-area').selectRange(len, len);
        }
        else {
            if ($('.q-area')) {
                $('.q-area').focus();
            }
        }
    });
    $(document).on("click","#close", function() {
        $("div#q-panel").slideUp("slow");
    });
    $(document).on("click",".closeup", function() {
        $("div#q-panel").slideUp("slow");
        $("#q-toggle a").toggle();
    });
    $(document).on("click","#q-toggle a", function() {
        $("#q-toggle a").toggle();
    });
});

function QuickNotes() {
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/GetQuickNotes",
        data: "{ }",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            ClearCookies();
            $('#q-notes').setTemplateURL(BlogEngineRes.applicationWebRoot + 'Modules/QuickNotes/Templates/Panel.htm', null, { filter_data: false });
            $('#q-notes').processTemplate(msg);
        }
    });  
}

// NOTE
function GetNote(id) {
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/GetNote",
        data: "{'id':'" + id + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (result) {
            if (id.length > 0) {
                var rt = result.d;
                SetCookies(rt.Id, rt.Note);
                $('#q-panel').setTemplateURL(BlogEngineRes.applicationWebRoot + 'Modules/QuickNotes/Templates/Note.htm', null, { filter_data: false });
                $('#q-panel').processTemplate(result);
                $('.q-area').val(rt.Note);
            }
            else {
                $('#q-panel').setTemplateURL(BlogEngineRes.applicationWebRoot + 'Modules/QuickNotes/Templates/Note.htm', null, { filter_data: false });
                $('#q-panel').processTemplate(result);
                $('.q-area').val(JSON.parse($.cookie('quck-note-current')).Note);
            }
            var len = $('.q-area').val().length;
            $('.q-area').selectRange(len, len);
            $('#q-loading').hide();
            return false;
        }
    });
}

// LIST
function GetNotes() {
    if ($('.q-area').val()) {
        $.cookie('quck-note-current', JSON.stringify({ "Id": JSON.parse($.cookie('quck-note-current')).Id, "Note": $('.q-area').val() }), { expires: 2 });
    }
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/GetNotes",
        data: "{ }",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            $('#q-panel').setTemplateURL(BlogEngineRes.applicationWebRoot + 'Modules/QuickNotes/Templates/List.htm', null, { filter_data: false });
            $('#q-panel').processTemplate(msg);
        }
    });
}

function SaveNote() {
    if ($.trim($('.q-area').val()) == '') {
        ShowWarning("Note can not be empty");
        return false;
    }
    ShowLoader();
    var id = '';
    if ($.cookie('quck-note-current')) {
        id = JSON.parse($.cookie('quck-note-current')).Id;
    }
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/SaveNote",
        data: "{'id':'" + id + "', 'note':'" + $('.q-area').val() + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (result) {
            var rt = result.d;
            SetCookies(rt.Id, rt.Note);
            GetNote(rt.Id);
        }
    });
}

function DeleteNote() {
    id = JSON.parse($.cookie('quck-note-current')).Id;
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/DeleteNote",
        data: "{'id':'" + id + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            NewNote();
        }
    });
}

function NewNote() {
    ClearCookies();
    GetNote('');
}

function IsNewNote() {
    if (JSON.parse($.cookie('quck-note-current')).Id.length > 0) {
        return false;
    }
    else {
        return true;
    }
}

function SaveQuickPost() {
    if ($.trim($('.q-area').val()) == '') {
        ShowWarning("Note can not be empty");
        return false;
    }

    ShowLoader();

    var dto = {
        "content": $('.q-area').val()
    };

    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/SaveQuickPost",
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        data: JSON.stringify(dto),
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                ShowSuccess("Published!");
                DeleteNote();
            }
            else {
                ShowWarning(rt.Message);
            }
        }
    });
    return false;
}

// SETTINGS
function GetSettings() {
    if ($('.q-area').val()) {
        $.cookie('quck-note-current', JSON.stringify({ "Id": JSON.parse($.cookie('quck-note-current')).Id, "Note": $('.q-area').val() }), { expires: 2 });
    }
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/GetSettings",
        data: "{ }",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            $('#q-panel').setTemplateURL(BlogEngineRes.applicationWebRoot + 'Modules/QuickNotes/Templates/Settings.htm', null, { filter_data: false });
            $('#q-panel').processTemplate(msg);
        }
    });
}

function SaveSettings() {
    ShowLoader();
    $.ajax({
        url: BlogEngineRes.applicationWebRoot + "Modules/QuickNotes/Qnotes.asmx/SaveSettings",
        data: "{'category':'" + $('#selCategory').val() + "', 'tags':'" + $('#txtTags').val() + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (result) {
            ShowSuccess("Saved!");
        }
    });
}

function ClearCookies() {
    SetCookies('', '');
}

function SetCookies(id, note) {
    $.cookie('quck-note-current', JSON.stringify({ "Id": id, "Note": note }), { expires: 7 });
}

function ShowLoader() {
    $("#q-loader").removeClass("hidden");
    $("#q-loader").addClass("loader");
}

function HideLoader() {
    var q = $("#q-loader");
    q.removeClass("loader");
    q.removeClass("warning");
    q.addClass("hidden");
}

function ShowSuccess(msg) {
    var q = $("#q-loader");
    q.removeClass("loader");
    q.removeClass("warning");
    q.html(msg);
    q.addClass("success");
    q.fadeIn(1000);
    q.fadeOut(1000);
}

function ShowWarning(msg) {
    var q = $("#q-loader");
    q.removeClass("loader");
    q.removeClass("hidden");
    q.html(msg);
    q.addClass("warning");
}

$.fn.selectRange = function (start, end) {
    return this.each(function () {
        if (this.setSelectionRange) {
            this.focus();
            this.setSelectionRange(start, end);
        } else if (this.createTextRange) {
            var range = this.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
        }
    });
};

function onAjaxBeforeSend(jqXHR, settings) {

    // AJAX calls need to be made directly to the real physical location of the
    // web service/page method.  For this, SiteVars.ApplicationRelativeWebRoot is used.
    // If an AJAX call is made to a virtual URL (for a blog instance), although
    // the URL rewriter will rewrite these URLs, we end up with a "405 Method Not Allowed"
    // error by the web service.  Here we set a request header so the call to the server
    // is done under the correct blog instance ID.

    jqXHR.setRequestHeader('x-blog-instance', BlogEngineRes.blogInstanceId);
}