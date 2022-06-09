
/*--This JavaScript method for Print command--*/
function printData(contentId) {
    var head = document.getElementsByTagName('head')[0].innerHTML;
    var divToPrint = document.getElementById(contentId);
    newWin = window.open('');
    newWin.document.write(head);
    newWin.document.write(divToPrint.outerHTML);
    newWin.document.close();
    newWin.focus();
    newWin.print();
    setTimeout(window.close, 0);

}