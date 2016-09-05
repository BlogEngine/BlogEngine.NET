//
var simpleCap = $("label[for=simpleCaptchaValue]").parent();
simpleCap.hide();
$("#commentCaptcha").append(simpleCap.html());

//
var logintext = $(".item-login span").text();
var aLogin = $(".item-login");
aLogin.attr("title", logintext);
var aLoginAttr = aLogin.attr("href");
if (aLoginAttr == "/admin/") {
    aLogin.removeClass("item-login");
    aLogin.addClass("item-admin");
    aLogin.attr("title", logintext);
}

//
$(".blog-nav li a").tooltip();

//
if (location.pathname !== '/') {
    $('.blog-nav ul li a[href*="/' + location.pathname.split("/")[1] + '"]').addClass('active');
} else {
    $('.blog-nav ul li a[href="/"]').addClass('active');
}

//
var postAdminLinks = $(".post-adminlinks a");
if (postAdminLinks.length == 2) {
    $(".post-adminlinks a:nth-child(1)").addClass("item-edit");
    $(".post-adminlinks a:nth-child(2)").addClass("item-delete");
}
if (postAdminLinks.length == 4) {
    $(".post-adminlinks a:nth-child(1)").addClass("hidden");
    $(".post-adminlinks a:nth-child(2)").addClass("item-approve");
    $(".post-adminlinks a:nth-child(3)").addClass("item-edit");
    $(".post-adminlinks a:nth-child(4)").addClass("item-delete");
}

//
if (!$.trim($('#commentlist').html()).length) {
    $("#commentlist").parent().hide();
}
$("#btnSaveAjax").click(function () {
    $("#commentlist").parent().show();
});