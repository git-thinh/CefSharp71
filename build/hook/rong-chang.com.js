CoreInterface.prototype = {
    setup: function () {
        console.info("HOOK.setup(): ...");

        var allElems = document.querySelectorAll('*');
        for (var i = 0; i < allElems.length; i++) {
            //------------------------------------------------------------------------------
            if (allElems[i].hasAttribute('style')) allElems[i].removeAttribute('style');
            if (allElems[i].hasAttribute('width')) allElems[i].removeAttribute('width');
            //------------------------------------------------------------------------------
            var tagName = allElems[i].tagName, textContent = allElems[i].textContent;
            //------------------------------------------------------------------------------
            // Remove footer contain a text "Copyright..."
            var posCopyright = textContent.toLowerCase().trim().indexOf('copyright');
            if ((tagName == 'DIV' || tagName == 'P')
                && textContent != null && posCopyright != -1 && posCopyright < 99
            ) {
                //console.log(tagName, textContent);
                allElems[i].remove();
            }
            //------------------------------------------------------------------------------
        }
    }
};
___core = new CoreInterface();