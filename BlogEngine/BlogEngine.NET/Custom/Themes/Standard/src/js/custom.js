// add placeholder to newsletter widget.
$("#txtNewsletterEmail").attr("placeholder", "youremail@example.com");

//
$('[data-toggle="tooltip"]').tooltip();


$(".blog-nav ul").each(function () {
    var $this = $(this);
    if ($this.find("li").length) {
        $this.parent().addClass("has-ul").append('<i class="fa fa-chevron-down nav-item-toggle"></i>');
    }
});


function toggleSidebar() {
    $(".blog-nav-toggle").toggleClass("is-active").toggleClass("color-dark").toggleClass("color-white");
    $(".blog-sidebar").toggleClass("active");
    $(".overlay").fadeToggle();
}
//
$(".blog-nav-toggle").on("click", toggleSidebar);
$(".overlay").on("click", toggleSidebar);


//
$(".nav-item-toggle").on("click", function () {
    $(this).toggleClass("is-active");
    $(this).parent().find("ul").toggleClass("is-active");
});

$(function () {
    // social networks
    var socialNetwork = $(".blog-social li a");
    for (i = 0; i < socialNetwork.length; ++i) {
        link = socialNetwork[i];

        if ($(link).attr("href") != "") {
            $(link).parent().css("display", "block");
        }
    }
});

// back up
$(".goup").on('click', function (e) {
    e.preventDefault();
    $('html,body').animate({
        scrollTop: 0
    }, 700);
});


if ($(window).scrollTop() > 100) {
    $(".goup").fadeIn();
} else {
    $(".goup").fadeOut();
}
$(window).scroll(function () {

    if ($(window).scrollTop() > 100) {
        $(".goup").fadeIn();
    } else {
        $(".goup").fadeOut();
    }
});

//
var simpleCap = $("label[for=simpleCaptchaValue]").parent();
simpleCap.hide();
$("#commentCaptcha").append(simpleCap.html());

//
if ($('#comment').length < 1) {
    $("#commentlist").parent().hide();
}


$('input[value="Save comment"]').on('click', function () {
    $("#commentlist").parent().show();
});



const ps = new PerfectScrollbar('.blog-sidebar');