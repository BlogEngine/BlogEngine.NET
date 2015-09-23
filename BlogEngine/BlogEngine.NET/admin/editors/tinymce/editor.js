var editorGetHtml = function () {
    return tinymce.activeEditor.getContent();
}

var htmlContent;

var editorSetHtml = function (html) {
    if (tinymce.activeEditor) {
        tinymce.activeEditor.setContent(html);
    }
    else {
        // If not initialized yet, we need to delay it
        htmlContent = html;
    }
}

tinymce.init({
    selector: '#txtContent',
    plugins: [
        "advlist autolink lists link image charmap print preview anchor",
        "searchreplace visualblocks code fullscreen textcolor imagetools",
        "insertdatetime media table contextmenu paste sh4tinymce filemanager"
    ],
    toolbar: "styleselect forecolor | bold italic | alignleft aligncenter alignright | bullist numlist | link media sh4tinymce | fullscreen code | filemanager",
    autosave_ask_before_unload: false,
    max_height: 400,
    min_height: 160,
    height: 400,
    menubar: false,
    relative_urls: false,
    browser_spellcheck: true,
    setup: function (editor) {
        editor.on('init', function (e) {
            if (htmlContent) {
                editor.setContent(htmlContent);
            }
        });
    }
});
