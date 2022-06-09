/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.filebrowserUploadMethod = 'form';
    //config.skin = "office2013";
    config.removePlugins = 'easyimage';
    config.removePlugins = 'uploadimage';
    config.language = "ar";
    config.customConfig = '';
    config.filebrowserImageUploadUrl = '/cpanel/UploadFiles?type=Images';
    //config.uploadUrl = '/JsonUploadFiles';
    //config.filebrowserBrowseUrl = '/Content/ckfinder/ckfinder.html';
    // config.filebrowserImageBrowseUrl = '/Content/ckfinder/ckfinder.html?type=Images';
    //config.filebrowserUploadUrl = '/Content/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    //config.filebrowserImageUploadUrl = '/Content/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    //config.disallowedContent = 'img{width,height,float}';
    config.extraAllowedContent = 'img[width,height,align];span{background}';
    config.extraPlugins = 'colorbutton,font,justify,print,pastebase64,tableresize,uploadfile,pastefromword,liststyle,slideshow';
    //var roxyFileman = "/fileman/Index.html";
    //config.filebrowserBrowseUrl = roxyFileman;
    //config.filebrowserImageBrowseUrl = roxyFileman + '?type=image';
    config.removeDialogTabs = "link:upload;image:upload";
}
