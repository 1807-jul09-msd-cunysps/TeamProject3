//script for slideshow
var slide1 = document.getElementById("slide1"); //0
var slide2 = document.getElementById("slide2"); //1
var slide3 = document.getElementById("slide3"); //2
var slideNumber = 0;
var slideFreeze = 0;

//Call function to start SlideShow when page loades
slideit();

function slideit() {
    if (!slideFreeze) {
        slide();
        if (slideNumber != 0) {
            this.handlebtn(false);
        }
        slideNumber++;
    }
    if (slideNumber == 5) {
        slideNumber = 1;
    }
    this.handlebtn(true);
    slideFreeze = 0;
    setTimeout("slideit()", 8000);
}


function slideNext() {
    slide();
    this.handlebtn(false);
    slideNumber++;
    if (slideNumber == 5) {
        slideNumber = 1;
    }
    this.handlebtn(true);
    slideFreeze = 1;
}

function slidePrev() {
    this.handlebtn(false);
    switch (slideNumber) {
        case 1:
            slideNumber = 4;
            $(slide1).fadeToggle(1000);
            $(slide2).fadeToggle(0);
            $(slide3).fadeToggle(0);
            break;
        case 2:
            slideNumber = 1;
            $(slide1).fadeToggle(1000);
            break;
        case 3:
            slideNumber = 2;
            $(slide2).fadeToggle(1000);
            break;
        case 4:
            slideNumber = 3;
            $(slide3).fadeToggle(1000);
            break;
    }
    if (slideNumber == 5) {
        slideNumber = 1;
    }
    this.handlebtn(true);
    slideFreeze = 1;
}

function slidebtn(slide) {
    if (slideNumber == slide) {
        console.log("slide: " + slide);
        return;
    } else {
        this.handlebtn(false);
        this.jumpToSlide(slide);
        slideNumber = slide;
        console.log("slideNumber: " + slideNumber);
        if (slideNumber == 5) {
            slideNumber = 1;
        }
        this.handlebtn(true);
        slideFreeze = 1;
    }
}

function slide() {
    switch (slideNumber) {
        case 1:
            $(slide1).fadeToggle(1000);
            break;
        case 2:
            $(slide2).fadeToggle(1000);
            break;
        case 3:
            $(slide3).fadeToggle(1000);
            break;
        case 4:
            $(slide1).fadeToggle(1000);
            $(slide2).fadeToggle(1000);
            $(slide3).fadeToggle(1000);
            break;
    }
}
function jumpToSlide(slide) {
    switch (slideNumber) {
        case 1:
            if (slide == 2) {
                $(slide1).fadeToggle(1000);
            } else if (slide == 3) {
                $(slide1).fadeToggle(1000);
                $(slide2).fadeToggle(1000);
            } else if (slide == 4) {
                $(slide1).fadeToggle(1000);
                $(slide2).fadeToggle(1000);
                $(slide3).fadeToggle(1000);
            }
            break;
        case 2:
            if (slide == 1) {
                $(slide1).fadeToggle(1000);
            } else if (slide == 3) {
                $(slide2).fadeToggle(1000);
            } else if (slide == 4) {
                $(slide2).fadeToggle(1000);
                $(slide3).fadeToggle(1000);
            }
            break;
        case 3:
            if (slide == 1) {
                $(slide1).fadeToggle(1000);
                $(slide2).fadeToggle(1000);
            } else if (slide == 2) {
                $(slide2).fadeToggle(1000);
            } else if (slide == 4) {
                $(slide3).fadeToggle(1000);
            }
            break;
        case 4:
            if (slide == 1) {
                $(slide1).fadeToggle(1000);
                $(slide2).fadeToggle(1000);
                $(slide3).fadeToggle(1000);
            } else if (slide == 2) {
                $(slide2).fadeToggle(1000);
                $(slide3).fadeToggle(1000);
            } else if (slide == 3) {
                $(slide3).fadeToggle(1000);
            }
            break;
    }
}
function handlebtn(addRemove) {
    var element = document.getElementById("slidebtn" + slideNumber);
    if (addRemove) {
        element.classList.add('slidebtnactive');
    } else {
        element.classList.remove('slidebtnactive');
    }
}
