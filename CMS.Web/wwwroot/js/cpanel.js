// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("select").select2({
        theme: 'bootstrap4'
    });
    $('.datepicker').datepicker({
        format: 'mm/dd/yyyy',
        startDate: '-3d',
        language: 'ar'
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
    })

});
