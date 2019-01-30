function onDOMReady() {
    console.info("HOOK: DOM loaded");

    //var allElems = document.querySelectorAll('*');
    //for (var i = 0; i < allElems.length; i++) {
    //    //------------------------------------------------------------------------------
    //    if (allElems[i].hasAttribute('style')) allElems[i].removeAttribute('style');
    //    if (allElems[i].hasAttribute('width')) allElems[i].removeAttribute('width');
    //    //------------------------------------------------------------------------------
    //    var tagName = allElems[i].tagName, textContent = allElems[i].textContent;
    //    //------------------------------------------------------------------------------
    //    // Remove footer contain a text "Copyright..."
    //    var posCopyright = textContent.toLowerCase().trim().indexOf('copyright');
    //    if ((tagName == 'DIV' || tagName == 'P')
    //        && textContent != null && posCopyright != -1 && posCopyright < 99
    //    ) {
    //        //console.log(tagName, textContent);
    //        allElems[i].remove();
    //    }
    //    //------------------------------------------------------------------------------
    //}

}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onDOMReady);
} else {
    onDOMReady();
}