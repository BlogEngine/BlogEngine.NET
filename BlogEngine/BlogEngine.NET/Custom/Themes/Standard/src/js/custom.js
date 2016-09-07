$(document).ready(function () {
    //
    var blogAuthor = $(".blog-author");
    if ($.trim(blogAuthor.html()).length) {
        $(blogAuthor).show();
    }

    //
    var socialNetwork = $(".social-network li a");
    for (i = 0; i < socialNetwork.length; ++i) {
        link = socialNetwork[i];

        if ($(link).attr("href") != "") {
            $(link).parent().css("display", "inline-block");
        }
    }

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
    $(".widget a:has(img)").addClass("no-border");

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

    // support theme
    var adminNav = $(".item-admin");
    var adminAlert = $(".admin-alerts-true");
    var adminName = $(".post-author a").html();

    console.log(adminName);
    var adminAlertHtml = '<div class="support-theme visible-md visible-lg alert alert-info clearfix"><p class="pull-left">Hi ' + adminName + ', You can read <a href="http://francis.bio/notes/blogengine-standard-2016-theme/" target="_blank" rel="nofollow"><b>This Article</b></a> that will help you customize this theme easily.</p><a href="admin/#/custom/themes" class="pull-right visible-lg"><i class="fa fa-times" data-toggle="tooltip" data-placement="left" title="remove from theme options"></i></a></div>';
    if (adminNav.length && adminAlert.length && (location.pathname == '/' || location.pathname == '/default.aspx')) {
        adminAlert.prepend(adminAlertHtml);
    }

    //
    $(".blog-nav li a, [data-toggle=tooltip]").tooltip();

});
