$(document).ready(function () {
    $(".btn-nav").click(function () {
        $(".First-Line").slideToggle("fast");
        $(".Second-Line").slideToggle();
    });
    $(".Footer-Widget-Container .Widget:last-child").addClass("Last-Widget");
    $(".Header #searchbox #searchfield").focus(function () {
        $(".Header #searchbox").addClass("SearchboxSelected");
        $(".Header #searchbox input[type='button']").addClass("Btn-Search-Selected");
    });
    $(".Header #searchbox #searchfield").blur(function () {
        $(".Header #searchbox").removeClass("SearchboxSelected");
    });
    $(".categorylist img").attr("src", "/themes/FlatBlog/img/rss-cat.png");
    $("#Social-Network").html($(".Blog-Social").html());
    $('#BackTop').click(function () {
        $('body,html').animate({
            scrollTop: 0
        }, 800);
        return false;
    });
    $(window).resize(function () {
        if ($(window).width() > 640) {
            $(".First-Line").show();
        }
    });

    // Find all YouTube videos
    var $allVideos = $("iframe[src^='http://www.youtube.com']"),
        // The element that is fluid width
        $fluidEl = $("body");
    // Figure out and save aspect ratio for each video
    $allVideos.each(function () {
        $(this)
          .data('aspectRatio', this.height / this.width)
          // and remove the hard coded width/height
          .removeAttr('height')
          .removeAttr('width');
    });
    // When the window is resized
    $(window).resize(function () {
        var newWidth = $fluidEl.width();
        // Resize all videos according to their own aspect ratio
        $allVideos.each(function () {
            var $el = $(this);
            $el
              .width(newWidth)
              .height(newWidth * $el.data('aspectRatio'));
        });
        // Kick off one resize to fix all videos on page load
    }).resize();
});
