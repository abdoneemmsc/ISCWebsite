// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    'use strict'

    $('[data-toggle="offcanvas"]').on('click', function () {
        $('.offcanvas-collapse').toggleClass('open')
    });
    $(".change-language").click(function () {
        var pathname = window.location.pathname;
        var lang = pathname.split('/')[1];
        if (lang.toLowerCase() === 'en') {
            pathname = pathname.replace('/' + lang + '/', '/ar/');
            location.href = pathname;
        } else if (lang.toLowerCase() === 'ar') {
            pathname = pathname.replace('/' + lang + '/', '/en/');
            location.href = pathname;
        } else {
            pathname = 'en' + pathname;
            location.href = pathname;
        }
    });
    var isShown = false;
    $(".navbar-toggler").click(function () {
        var width = $(".navbar-collapse:first").css("width");
        if (!isShown) {
            $(".page-overlay").show();
            $(".page-overlay").css("left", width);
            isShown = true;
            debugger;
        } else {
            $(".page-overlay").hide();
            isShown = false;
            debugger;
        }
    })
});
$(document).on('click', '.dropdown-menu', function (e) {
    e.stopPropagation();
});

// make it as accordion for smaller screens
if ($(window).width() < 992) {
    $('.dropdown-menu a').click(function (e) {
        //e.preventDefault();
        if ($(this).next('.submenu').length) {
            $(this).next('.submenu').toggle();
        }
        $('.dropdown').on('hide.bs.dropdown', function () {
            $(this).find('.submenu').hide();
        })
    });
}
