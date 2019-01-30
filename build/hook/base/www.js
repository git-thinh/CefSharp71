var ___DATA = {};
var ___DATA_SHARED = ['appInfo'];
var ___DATA_BROADCAST = [];

___DATA.appInfo = {};
console.info('___DATA =', JSON.stringify(___DATA));

///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
var ___COMS_ID = [], ___CORE, ___SCREENS, ___CORE_INTERFACE_MIXIN;
___CORE_INTERFACE_MIXIN = { methods: {} };
function ___screenOpen(screenInfo) {
    if (screenInfo == null || screenInfo.Id == null) {
        console.error('Function f_hui_screenOpen(screenInfo) must be screenInfo = { Id: _SCREENS_ID.xxx, ... } ', screenInfo);
        return;
    }

    //#region [ Build Screen Template ]

    //console.log('OPEN SCREEN: ', screenInfo);
    var com_names = screenInfo.Components,
        header = screenInfo.Header,
        footer = screenInfo.Footer,
        bodyHeader = screenInfo.bodyHeader;

    ///////////////////////////////////////////////////////////////////////
    /* Contractor */
    var div = document.createElement('div');
    //div.style.opacity = 0;
    document.body.appendChild(div);
    if (screenInfo.overlayVisiable == null) screenInfo.overlayVisiable = true;
    if (screenInfo.overlayShow == null) screenInfo.overlayShow = true;
    if (screenInfo.scrollVertical == null) screenInfo.scrollVertical = true;
    if (footer == null) footer = {};
    if (footer.callbackOnItSelf == null) footer.callbackOnItSelf = true;
    if (footer.buttonOkActive == null) footer.buttonOkActive = true;
    if (footer.buttonOk == null) footer.buttonOk = true;
    if (footer.buttonCancel == null) footer.buttonCancel = true;
    screenInfo._screenElemID = 'dl-screen-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return r.toString(16); });
    var temp = '<div name="' + screenInfo.Id + '" id="' + screenInfo._screenElemID + '" class="dialog ' + (screenInfo.className == null ? '' : screenInfo.className) + '"><div class="dialog-layout">';

    ///////////////////////////////////////////////////////////////////////
    /* Overlay background */
    if (screenInfo.overlayVisiable == true) {
        //if (screenInfo.timeoutDisplay != null && screenInfo.timeoutDisplay > 0) { } else {
        var overlay = document.createElement('div');
        overlay.className = 'dialog-overlay ___do_action ___do_dialog ' + (screenInfo.overlayShow == true ? '' : ' dialog-overlay-transparent') + ' ' + (screenInfo.className == null ? '' : screenInfo.className + '-overlay');
        overlay.id = 'dl-bg-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
        var ___IS_TOUCH = ('ontouchstart' in window);
        if (screenInfo.overlayShow != true) overlay.addEventListener(___IS_TOUCH ? 'touchstart' : 'click', function () {
            var _dl = document.getElementById(screenInfo._screenElemID);
            if (_dl && _dl.__vue__ && _dl.__vue__['screenDialogClose']) {
                _dl.__vue__['screenDialogClose']();
            }
        });
        document.body.appendChild(overlay);
        screenInfo._screenElemOverlayID = overlay.id;
        //}
    }
    ///////////////////////////////////////////////////////////////////////
    /* Header: { Message: '', className: '', headerIcon: '', classNameMessage: ''  } */
    var sheader = '';
    if (header) {
        var sheaderMessage = '',
            sheaderIcon = '',
            sheaderClass = '',
            sheaderMessageClass = '';

        if (header.className && header.className.length > 0) sheaderClass = header.className;
        if (header.headerIcon && header.headerIcon.length > 0) sheaderIcon = '<i class="dialog-icon icon-sprite ' + header.headerIcon + '"></i>';
        if (header.classNameMessage && header.classNameMessage.length > 0) sheaderMessageClass = header.classNameMessage;

        if (header && header.Message) sheaderMessage = '<p class="dialog-message ' + sheaderMessageClass + (sheaderIcon.length == 0 ? ' icon-none' : '') + ' color-black70">' + header.Message + '</p>';
        if (sheaderIcon.length > 0) sheader += sheaderIcon;
        if (sheaderMessage.length > 0) sheader += sheaderMessage;
    }
    if (sheader.length > 0) {
        screenInfo._screenElemHeaderID = 'dl-header-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
        sheader = '<div id="' + screenInfo._screenElemHeaderID + '" class="dialog-header ' + sheaderClass + '">' + sheader + '</div>';
    }
    if (sheader.length > 0) temp += sheader;

    ///////////////////////////////////////////////////////////////////////
    /* BodyHeader */
    var sbodyHeader = '';
    if (bodyHeader && bodyHeader.length > 0) {
        bodyHeader.forEach(function (hi, index_) { sbodyHeader += '<li class="bheader-title f-size18 bheader-' + index_ + '">' + hi + '</li>'; });
        screenInfo._screenElemBodyHeaderID = 'dl-bheader-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
        sbodyHeader = '<div id="' + screenInfo._screenElemBodyHeaderID + '" class="dialog-body-header"><ul class="bheader">' + sbodyHeader + '</ul></div>';
    }
    if (sbodyHeader.length > 0) temp += sbodyHeader;

    ///////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    /* Components: ['test','..'] | 'test' */
    var scom = '';
    if (com_names != null && com_names.length > 0) {
        screenInfo._screenComponentRef = 'ref_scrollv_yxxx_xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); }).toUpperCase();
        if (typeof com_names === 'string') scom = '<div class="___com ' + com_names + '"><' + com_names + ' ' + ___PROPS_DATA_SHARED + ' ref="' + screenInfo._screenComponentRef + '"></' + com_names + '></div>';
        //else com_names.forEach(function (ci) { scom += '<' + ci + '></' + ci + '>'; });
    }
    if (scom.length > 0) {
        screenInfo._screenElemBodyID = 'dl-content-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
        screenInfo._screenElemBodyScrollVerticalREF = 'ref_scrollv_yxxx_xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); }).toUpperCase();
        temp += '<div id="' + screenInfo._screenElemBodyID + '" class="dialog-content">' + scom + '</div>';
        //temp += '<scroll-vertical id="' + screenInfo._screenElemBodyScrollVerticalREF + '" ref="' + screenInfo._screenElemBodyScrollVerticalREF + '" targetscrollid="' + screenInfo._screenElemBodyID + '" targetsizeid="' + screenInfo._screenElemBodyID + '">' + scom + '</scroll-vertical>';
    }
    ///////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    /* Footer: {
     * buttonOk: true|false,
     * buttonOkActive: true|false,
     * buttonCancel: true|false, 
     * buttonOkText: '', 
     * buttonCancelText: '',
     * callbackOk: function(){}, 
     * callbackCancel: function(){} } */
    var sfooter = '', sok = 'Ok', scancel = 'Cancel';
    if (footer != null && footer.buttonOkText != null) sok = footer.buttonOkText;
    if (footer != null && footer.buttonCancelText != null) scancel = footer.buttonCancelText;
    //var sbtnOk = '<button class="' + (footer.buttonOkActive ? 'is-active' : 'is-disable') + ' btn btn-ok ___do_action" @touchstart="callbackOk">' + sok + '</button>';
    var sbtnOk = '<button :class="[ screenInfo.Footer.buttonOkActive == true ? \'is-active\' : \'is-disable\', \'btn btn-ok ___do_action\']" @touchstart="callbackOk">' + sok + '</button>';
    var sbtnCancel = '<button class="btn btn-cancel ___do_action" @touchstart="callbackCancel">' + scancel + '</button>';
    if (footer.buttonOk == false) sbtnOk = '';
    if (footer.buttonCancel == false) sbtnCancel = '';
    if (sbtnOk.length > 0 || sbtnCancel.length > 0) {
        if (sbtnCancel.length > 0) sfooter += sbtnCancel;
        if (sbtnOk.length > 0) sfooter += sbtnOk;
    }
    if (sfooter.length > 0) sfooter = '<div class="dialog-footer">' + sfooter + '</div>';
    if (sfooter.length > 0) temp += sfooter;

    temp += '</div></div>';

    //#endregion

    ///////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    //console.log(temp);
    var vueScreenExtend = Vue.extend({
        mixins: [___COMS_MIXIN, ___SCREENS_COMMON_MIXIN, ___CORE_INTERFACE_MIXIN],
        template: temp,
        created: function () {
        },
        mounted: function () {
        },
        beforeDestroy: function () {
        },
        computed: {
            screenId: function () { return screenInfo.Id; },
            screenInfo: function () {
                return screenInfo;
            }
        },
        destroyed: function () {
            var _self = this;
            var options = _self.screenInfo;

            var _remove = function (a) {
                setTimeout(function (_a) {
                    JSON.parse(_a).forEach(function (id) {
                        var el = document.getElementById(id);
                        if (el) el.remove();
                    });
                }, 150, JSON.stringify(a));
            };

            var _screenElemOverlay = document.getElementById(_self.screenInfo._screenElemOverlayID),
                _screenElem = document.getElementById(_self.screenInfo._screenElemID);
            if (_screenElem) _screenElem.style.opacity = 0;
            if (_screenElemOverlay) _screenElemOverlay.style.opacity = 0;

            _remove([_self.screenInfo._screenElemOverlayID, _self.screenInfo._screenElemID]);

            ///////////////////////////////////////////////////////////////////////////////// 
            if (options.Footer && options.Footer.eventClosing && typeof options.Footer.eventClosing == 'function') {
                console.log('execute closing dialog ...');
                options.Footer.eventClosing(this);
            }
            ///////////////////////////////////////////////////////////////////////////////// 
        },
        methods: {
            open: function () {
                var _self = this, options = _self.screenInfo;
                //console.log('_HOMEUI_DIALOG_VUE_MIXIN_COMS.open =', options);
                if (options == null) return;

                var _screenElemOverlay = document.getElementById(options._screenElemOverlayID),
                    _screenElemHeader = document.getElementById(options._screenElemHeaderID),
                    _screenElemBodyHeader = document.getElementById(options._screenElemBodyHeaderID),
                    _screenElemBody = document.getElementById(options._screenElemBodyID),
                    _screenElem = document.getElementById(options._screenElemID);
                if (_screenElemOverlay) _screenElemOverlay.style.display = 'inline-block';
                if (_screenElem) {
                    var hi_header = 0;
                    if (_screenElemHeader) hi_header = _screenElemHeader.getBoundingClientRect().height;
                    var hi_bheader = 0;
                    if (_screenElemBodyHeader) hi_bheader = _screenElemBodyHeader.getBoundingClientRect().height;

                    setTimeout(function () {
                        _screenElem.style.opacity = 1;
                    }, 10);

                    if (options.timeoutDisplay != null && options.timeoutDisplay > 0) {
                        setTimeout(function (self_) { self_.screenDialogClose(); }, options.timeoutDisplay, _self);
                    }
                }

                //console.log('_HOMEUI_DIALOG_VUE_MIXIN_COMS.callbackOpen =', this.Options);
                if (options.Footer && options.Footer.callbackOpen) {
                    if (options.Footer.callbackOnItSelf == true) {
                        if (options._screenComponentRef != null
                            && _self.$refs[options._screenComponentRef]
                            && _self.$refs[options._screenComponentRef][options.Footer.callbackOpen]) {
                            _self.$refs[options._screenComponentRef][options.Footer.callbackOpen](this);
                        }
                    } else {
                        var pa = document.getElementById(options._screenParentElemID);
                        if (pa && pa.__vue__ && pa.__vue__[options.Footer.callbackOpen]) {
                            //var data = JSON.parse(JSON.stringify(this.$data));
                            //pa.__vue__[options.Footer.callbackOpen](data);
                            pa.__vue__[options.Footer.callbackOpen](this);
                        }
                    }
                }
            },
            screenDialogCloseNoCallback: function () {
                var _self = this;
                var options = _self.screenInfo;
                ///////////////////////////////////////////////////////////////////////////////////
                //_screenElem = document.getElementById(this.Options._screenElemID);
                ////if (_screenElem) _screenElem.style.opacity = 0;
                _self.$destroy();
            },
            screenDialogClose: function () {
                var _self = this;
                var options = _self.screenInfo;

                if (options.Footer && options.Footer.callbackCancel) {
                    if (options.Footer.callbackOnItSelf == true) {
                        if (options._screenComponentRef != null
                            && _self.$refs[options._screenComponentRef]
                            && _self.$refs[options._screenComponentRef][options.Footer.callbackCancel]) {
                            _self.$refs[options._screenComponentRef][options.Footer.callbackCancel](this);
                        }
                    } else {
                        var pa = document.getElementById(options._screenParentElemID);
                        if (pa && pa.__vue__ && pa.__vue__[options.Footer.callbackCancel]) {
                            pa.__vue__[options.Footer.callbackCancel](this);
                        }
                    }
                }

                _self.screenDialogCloseNoCallback();
            },
            callbackOk: function () {
                var _self = this;
                var options = _self.screenInfo;

                //console.log('_HOMEUI_DIALOG_VUE_MIXIN_COMS.callbackOk =', this.Options);
                if (options.Footer && options.Footer.buttonOkActive == false) {
                    //console.log('Footer.buttonOkActive = false -> break ...');
                    return;
                }
                //console.log('Footer.buttonOkActive = true -> execute ...');

                if (options._screenComponentRef && this.$refs[options._screenComponentRef]) {
                    _self.dataComponent = this.$refs[options._screenComponentRef].$data;
                    //console.log('?????????????????');
                }

                if (options.Footer && options.Footer.callbackOk) {
                    if (options.Footer.callbackOnItSelf == true) {
                        if (options._screenComponentRef != null
                            && _self.$refs[options._screenComponentRef]
                            && _self.$refs[options._screenComponentRef][options.Footer.callbackOk]) {
                            _self.$refs[options._screenComponentRef][options.Footer.callbackOk](this);
                        }
                    } else {
                        var pa = document.getElementById(options._screenParentElemID);
                        if (pa && pa.__vue__ && pa.__vue__[options.Footer.callbackOk]) {
                            pa.__vue__[options.Footer.callbackOk](this);
                        }
                    }
                }
                this.screenDialogClose();
            },
            callbackCancel: function () {
                //console.log('_HOMEUI_DIALOG_VUE_MIXIN_COMS.callbackCancel =', this.Options);
                this.screenDialogClose();
            }
        }
    });

    ///////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    screenInfo.bodyVue = {};
    var _screenId = screenInfo;//.Id.split('-').join('_');
    //___registerDataBroadcast(_screenId);
    ___DATA[_screenId] = screenInfo;

    var frameImpl = new vueScreenExtend({ data: ___DATA });
    var frameVue = frameImpl.$mount(div);

    ___DATA[_screenId].bodyVue = frameVue;

    frameVue.open();
}
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////

function ___onDomReady() {
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
if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", ___onDomReady); else ___onDomReady(); 























