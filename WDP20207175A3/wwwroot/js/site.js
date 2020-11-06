// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function makeImageBlur(obj) {
    obj.style.opacity = 0.5;
}

function makeImageUnblur(obj) {
    obj.style.opacity = 1;
    //document.getElementById("bgimage").style.background = "lightgrey";
}

function displayBigImage(obj) {
    document.getElementById("bgimage").style.backgroundImage = obj.style.backgroundImage;
    document.getElementById("bgimage").style.backgroundSize = "100%";
    document.getElementById("bgimage").style.backgroundRepeat = no - repeat;
    document.getElementById("bgimage").style.opacity = 1; //unnecessary code?

}