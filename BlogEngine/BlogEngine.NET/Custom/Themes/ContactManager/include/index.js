if ($.trim($('#widgetzone_Footer-Widget').html()) == '') {
    $(".footer .footer-widgets").css('padding', '0');
}
$(".fade .title-blog").animate({
    top: 0,
    opacity: 1,
}, 2000);
$(".fade .line-header").animate({
    opacity: 1,
}, 1000);
$(".fade .main-section").animate({
    opacity: 1,
}, 2000);
$(".footer").animate({
    opacity: 1,
}, 1000);
(function () {
    var move, p1, p2, p3;
    p1 = 0;
    p2 = 0;
    p3 = 0;
    move = function () {
        p1 += 2;
        p2 += 1;
        p3 += 0.7;
        if (p1 > 795) {
            p1 = 0;
        }
        if (p2 > 778) {
            p2 = 0;
        }
        if (p3 > 962) {
            p3 = 0;
        }
        $('#bg1').css('background-position', p1 + 'px bottom');
        $('#bg2').css('background-position', p2 + 'px bottom');
        return $('#bg3').css('background-position', p3 + 'px bottom');
    };
    setInterval(move, 100);
}).call(this);
$(window).scroll(function () {
    if ($(window).scrollTop() > 200) {
        $(".fixnav .header").addClass("fix-header");
    } else {
        $(".fixnav .header").removeClass("fix-header");
    }
});
$(".btn-toggle-search").click(function () {
    $(".header .search").addClass("open-search");
    $(this).hide();
    $(".header .search input[type='text']").focus();
});
$(".search input[type='text']").blur(function () {
    $(".search").delay(2000).removeClass("open-search");
    $(".btn-toggle-search").delay(2000).show();
});
$(".search input[type='text']").addClass("tooltip-search");
$(".search input[type='text']").attr('title', 'Press Enter to search');
$(window).scroll(function () {
    if ($(window).scrollTop() > 200) {
        $('.scrollup').fadeIn();
    } else {
        $('.scrollup').fadeOut();
    }
});
$('.scrollup').click(function () {
    $("html, body").animate({
        scrollTop: 0
    }, 600);
    return false;
});
$(".footer-widgets .widget:nth-child(3n)").addClass("last-child");
window.selectnav = function () {
    "use strict";
    var n = function (n, t) {
        function s(n) {
            var t;
            n || (n = window.event), n.target ? t = n.target : n.srcElement && (t = n.srcElement), t.nodeType === 3 && (t = t.parentNode), t.value && (window.location.href = t.value)
        }

        function h(n) {
            var t = n.nodeName.toLowerCase();
            return t === "ul" || t === "ol"
        }

        function c(n) {
            for (var t = 1; document.getElementById("selectnav" + t); t++);
            return n ? "selectnav" + t : "selectnav" + (t - 1)
        }

        function l(n) {
            var r, t, d, s, p;
            u++;
            var b = n.children.length,
                i = "",
                w = "",
                k = u - 1;
            if (b) {
                if (k) {
                    while (k--) w += y;
                    w += " "
                }
                for (r = 0; r < b; r++) t = n.children[r].children[0], typeof t != "undefined" && (d = t.innerText || t.textContent, s = "", f && (s = t.className.search(f) !== -1 || t.parentNode.className.search(f) !== -1 ? o : ""), a && !s && (s = t.href === document.URL ? o : ""), i += '<option class="option-nav" value="' + t.href + '" ' + s + ">" + w + d + "<\/option>", v && (p = n.children[r].children[1], p && h(p) && (i += l(p))));
                return u === 1 && e && (i = '<option value="">' + e + "<\/option>" + i), u === 1 && (i = '<select class="selectnav" id="' + c(!0) + '">' + i + "<\/select>"), u-- , i
            }
        }
        var r;
        if ((n = document.getElementById(n), n) && h(n) && "insertAdjacentHTML" in window.document.documentElement) {
            document.documentElement.className += " js";
            var i = t || {},
                f = i.activeclass || "active",
                a = typeof i.autoselect == "boolean" ? i.autoselect : !0,
                v = typeof i.nested == "boolean" ? i.nested : !0,
                y = i.indent || "→",
                e = i.label || "- Navigation -",
                u = 0,
                o = " selected ";
            return n.insertAdjacentHTML("afterend", l(n)), r = document.getElementById(c()), r.addEventListener && r.addEventListener("change", s), r.attachEvent && r.attachEvent("onchange", s), r
        }
    };
    return function (t, i) {
        n(t, i)
    }
}();
$("html").addClass("js");
selectnav('nav');