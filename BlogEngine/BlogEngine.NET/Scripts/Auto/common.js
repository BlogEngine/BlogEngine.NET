//parent namespace
var common = {
    //enable features by jquery
    enabling: {
        /*function that creates tooltipster for an element*/
        tooltipster: function (elm) {
            /*it's turning, argh!*/
            var e = elm != null && elm != 0 ? $(elm) : $(this);

            var enable = e.attr("data-tooltipster");
            if (/true/i.test(enable) == false || e.hasClass("tooltipstered") == true) return;

            var title = e.attr("data-tooltipster-title");
            title = title == null || title == undefined ? e.attr("title") : title;
            title = title == null || title == undefined ? e.attr("data-title") : title;

            if (title != "") {
                var offsetX = e.attr("data-tooltipster-offsetx");
                var offsetY = e.attr("data-tooltipster-offsety");
                var position = e.attr("data-tooltipster-position");
                //tooltipster init
                e.tooltipster({ "offsetY": offsetY, "offsetX": offsetX, position: position, content: title, });
            }

            e.attr("data-tooltipster", null);
            e.attr("title", null);
        },
        //enables yoxview
        yoxview: function (selector) {
            $(selector).find(".yoxview-activate").each(
                function () {
                    var elm = $(this);
                    elm.yoxview({
                        allowedUrls: /\/image.axd\?picture=(.*)\&size=(large|original|medium|small)$/i,
                        lang: $("html").attr("lang"),
                        titleAttribute: "data-title",
                    });
                    elm.removeClass("yoxview-activate");
                }
            );
        },
    },
    //prepares images for yoxview
    yoxview: function (selector) {
        $(selector).find("img[src^='/image.axd']").each(
            function () {
                var img = $(this);
                var src = img.attr("src");
                //does nothing, if scaling is not allowed
                if (/[?|&]scaling=false/i.test(src) == true) return;

                if (/(&|\?)size=(\w*)/i.test(src) == true) src = src.replace(/size=\w*/i, "size=original");
                else src += (/\?/i.test(src) ? "&" : "?") + "size=original";

                var alt = img.attr("alt");

                var title = img.attr("title");
                if (title == null || title == undefined) title = img.attr("data-title");
                if (title == null || title == undefined) title = img.attr("alt");

                img.attr("title", null);
                img.attr("width", null);
                img.attr("height", null);
                img.attr("data-title", title);

                //vertical align
                img.addClass("valign");
                img.wrap("<div class='yoxview yoxview-activate' style='text-align:center;' data-tooltipster='true' " + (title != null && title != undefined ? " data-tooltipster-title='" + title + "'" : "") + "><a class='no-link' href='" + src + "'></a></div>");
            }
        );
    },
};