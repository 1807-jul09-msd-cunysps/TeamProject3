$(document).ready(function () {
    var slideNav = $("div.slideNav");
    slideNav.fadeTo(0, 0);

    slideNav.hover(function () {
        if (this.style.opacity = "0.25") {
            $(this).fadeTo(300, 1);
        }
    },
        function () {
            if (this.style.opacity = "0.25") {
                $(this).fadeTo(300, 0);
            }
        });
});