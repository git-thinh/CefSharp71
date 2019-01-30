/*============================================================
/ DATA: CONTRACTOR - CREATE EVENT DATA CHANGE
/============================================================*/

///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
___SCREENS = {
    NAV_BOTTOM: { Id: 'navi-bottom-1001', className: 'screen-hook-nav-bottom', Components: 'com-nav-bottom', overlayVisiable: false, Footer: { buttonOk: false, buttonCancel: false } },
    DICTIONARY: { Id: 'screen-dictionary-1001', className: 'screen-dictionary', Components: 'com-dictionary', overlayVisiable: false, Footer: { buttonOk: false, buttonCancel: false } },
};
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
function CoreInterface() {
    this._timeOutAutoSave = 15000;
    this._hasChangeData = false;
    this._elSuggestion = document.getElementById('gt-src-is');
    this._elInputTranslate = document.getElementById('source');
}
CoreInterface.prototype = {
    setup: function () {
        var _self = this;
        //-----------------------------------------------------------------
        _self.autoSave_RunInterval();
        //-----------------------------------------------------------------
        var header = document.getElementById('gb');
        if (header) header.style.display = 'none';
        //-----------------------------------------------------------------
        //document.body.addEventListener('DOMSubtreeModified', function () {
        //    var time = 'DOM Changed at ' + new Date();
        //    console.log(time);
        //}, false);

        if (this._elSuggestion) this._elSuggestion.addEventListener('DOMSubtreeModified', this.suggestion_eventChange, false);
        //-----------------------------------------------------------------
        var elSource = document.getElementById('source');
        if (elSource) {
            elSource.setAttribute('onkeypress', '___CORE.___inputKeypress(event, this.value.trim())');
            elSource.setAttribute('onkeydown', '___CORE.___inputKeydown(event)');
        }
        //-----------------------------------------------------------------
        setTimeout(function () {
            console.log('STATE_READY');
        }, 100);
        setTimeout(function () {
            ___screenOpen(___SCREENS.NAV_BOTTOM);
        }, 300);
    },
    //========================================================================
    // TRANSLATE
    translate_Execute: function (text) {
        var _self = this;
        _self._elInputTranslate.value = text;
    },
    //========================================================================
    // AUTO_SAVE
    autoSave_RunInterval: function () {
        var _self = this;
        setInterval(function () { _self.autoSave_Update(); }, _self._timeOutAutoSave);
    },
    autoSave_Update: function () {
        var _self = this;
        if (_self._hasChangeData) {
            //Boolean f_api_writeFile(String file, String data)
            //___API.f_api_writeFile();
            _self._hasChangeData = false;
            f_log('AUTO_SAVE: ', ___DATA[___DATA_BROADCAST.Dictionary]);
        }
    },
    //========================================================================
    // DICTIONARY
    dictionary_Add: function (text, mean) {
        var _self = this;
        if (text == null || mean == null || text.length == 0 || mean.length == 0) return false;
        text = text.toLowerCase().trim();
        if (___DATA[___DATA_BROADCAST.Dictionary][text] == null) {
            ___DATA[___DATA_BROADCAST.Dictionary][text] = mean;
            _self._hasChangeData = true;
            f_log('DICTIONARY_ADD: ', text);
            ___sendBroadcastData(___DATA_BROADCAST.Dictionary);
            return true;
        }
        return false;
    },
    //========================================================================
    ___getMeanText: function () {
        var el = document.querySelector('.text-wrap.tlid-copy-target');
        if (el) return el.textContent;
        return '';
    },
    ___inputKeydown: function (e) {
        var key = e.keyCode || e.charCode;
        if (key == 8 || key == 46) {
            this.___inputProcess('DELETE_CHAR');
            return;
        }
    },
    ___inputKeypress: function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            this.___inputProcess('ENTER');
            return;
        }
        this.___inputProcess('DATA');
    },
    ___inputProcess: function (key) {
        setTimeout(function () {
            //var _input = document.getElementById('source').value;
            //console.log(key, _input);
            //___CORE.suggestion_getData();
        }, 100);
    },
    //========================================================================
    suggestion_eventChange: function (e) {
        ___CORE.suggestion_getData();
    },
    suggestion_getData: function () {
        var _self = this;
        var ell = this._elSuggestion.querySelectorAll('.gt-is-itm .gt-is-sg');
        var elr = this._elSuggestion.querySelectorAll('.gt-is-itm .gt-is-tr');
        for (var i = 0; i < ell.length; i++) {
            var text = ell[i].textContent;
            var mean = elr[i].textContent;
            //console.log(text, mean);
            _self.dictionary_Add(text, mean);
        }
    },
    suggestion_Show: function () {
        //this._elSuggestion.style.display = 'inline-block';
        ___screenOpen(___SCREENS.DICTIONARY);
    }
};
___CORE = new CoreInterface();
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
___CORE_INTERFACE_MIXIN = {
    methods: {
        translate_Execute: function (text) {
            ___CORE.translate_Execute(text);
        },
        suggestion_Show: function () {
            ___CORE.suggestion_Show();
        }
    }
};
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////

var _SELECT_OBJ = { x: 0, y: 0, text: '', id: '' };
////////////////////////////////////////////////////////////
var _CLIENT_ID = 1;
var _GET_ID = function () { var date = new Date(); var id = _CLIENT_ID + ("0" + (date.getMonth() + 1)).slice(-2) + ("0" + date.getDate()).slice(-2) + ("0" + date.getHours()).slice(-2) + ("0" + date.getMinutes()).slice(-2) + ("0" + date.getSeconds()).slice(-2) + (date.getMilliseconds() + Math.floor(Math.random() * 100)).toString().substring(0, 3); return parseInt(id); };
var APP_INFO = { Width: $(window).width() };
////////////////////////////////////////////////////////////

function f_english_Translate() {
    if (_SELECT_OBJ != null) {
        var otran = JSON.parse(JSON.stringify(_SELECT_OBJ));
        f_log('TRANSLATE ', otran);
        var text = otran.text.toLowerCase().trim();

        //___API.speechWord(text, 1);
        //___API.playMp3FromUrl('https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3', 1);
        //___API.playMp3FromUrl('http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3', 1);

        //var audio = document.createElement('embed');
        ////audio.style.display = 'none';
        //audio.id = 'iframeAudio';
        //audio.width = '100px';
        //audio.height = '100px';
        //audio.autostart = 'true';
        //audio.autoplay = 'true';
        //audio.setAttribute('src', 'https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3');
        ////audio.play();
        //document.body.appendChild(audio);

        // <iframe src="audio/source.mp3" allow="autoplay" style="display:none" id="iframeAudio"></iframe> 
        // <audio autoplay loop  id="playAudio"><source src="audio/source.mp3"></audio>
        // 

        ////if (_.some(_words, function (w) { return w == text; }) == false) _words.push(text);

        ////f_post('//api/translate/v1', otran.text, function (_res) {
        ////    f_log('OK', _res);
        ////    if (_res && _res.length > 0) {
        ////        otran.mean_vi = _res;
        ////        f_english_TranslateShowResult(otran);
        ////    } else {

        ////    }
        ////}, function (_err) {
        ////    f_log('ERR', _err);
        ////})
    }
}

function f_event_processCenter(event) {
    var type = event.type,
        el = event.target,
        tagName = el.tagName,
        id = el.id,
        text = el.innerText,
        textSelect = '';

    if (id == null || id.trim().length == 0) {
        var id = _GET_ID();
        el.setAttribute('id', id);
    }

    textSelect = window.getSelection().toString();
    switch (type) {
        case 'mousedown':
            //if (console.clear) console.clear();

            var elbox = document.getElementById('___box_tran');
            if (elbox) elbox.style.display = 'none';

            _SELECT_OBJ = { id: id, cached: false, x: event.x, y: event.y };
            if (el.className == '___translated') _SELECT_OBJ.cached = true;

            break;
        case 'mouseup':
            if (_SELECT_OBJ != null) {
                _SELECT_OBJ.x = event.x;
                _SELECT_OBJ.y = event.y;
                if (textSelect && textSelect.trim().length > 0) _SELECT_OBJ.text = textSelect;
            }
            break;
        case 'click':
            if (_SELECT_OBJ != null) {
                if (textSelect && textSelect.trim().length > 0) _SELECT_OBJ.text = textSelect;
            }
            break;
        case 'dblclick':
            if (_SELECT_OBJ != null) {
                if (textSelect && textSelect.trim().length > 0) _SELECT_OBJ.text = textSelect;
            }
            break;
    }

    //f_log(tagName + '.' + type + ': ' + JSON.stringify(_SELECT_OBJ));

    if (_SELECT_OBJ != null) {
        if (_SELECT_OBJ.cached == true) {
            f_displayTranslateCache(_SELECT_OBJ);
            _SELECT_OBJ = null;
        } else {
            if (_SELECT_OBJ.text && _SELECT_OBJ.text.length > 0) {
                f_english_Translate();
                _SELECT_OBJ = null;
            }
        }
    }

    //f_log(tagName + '.' + type + ': ' + id + ' \r\nSELECT= ' + textSelect + ' \r\nTEXT= ', text);
    //event.preventDefault();
    //event.stopPropagation();
}

if (window.addEventListener) {
    window.addEventListener("mouseup", f_event_processCenter, true);
    window.addEventListener("mousedown", f_event_processCenter, true);
    window.addEventListener("click", f_event_processCenter, true);
    window.addEventListener("dblclick", f_event_processCenter, true);
}

///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////

var ___SCREENS_COMMON_MIXIN = {},
    ___SCREEN_EVENT_SUBMIT_ID = {},
    ___PROPS_DATA_SHARED = '',
    ___COMS_MIXIN = {
        data: {
            _eleID: null
        },
        ready: function () {
            var _id = this.$el.id;
            if (_id == null || _id.length == 0) {
                _id = '___vue-com-' + this._uid;
                if (this.$el && this.$el.setAttribute) {
                    this.$el.setAttribute('id', _id);
                }
            }
            this._eleID = _id;
            ___COMS_ID.push(_id);
        },
        props: ___DATA_SHARED,
        computed: {
        },
        methods: {
            f_base_show: function () {
                var el = document.getElementById(this.el_id);
                if (el) el.style.opacity = 1;
                f_log(this.el_id, 'SHOW');
            },
            f_base_hide: function () {
                var el = document.getElementById(this.el_id);
                if (el) el.style.opacity = 0;
                f_log(this.el_id, 'HIDE');
            },
            screenEmit: function (screenId) {
                var _self = this;
                //console.log('[1]', _self.screenInfo);
                if (screenId == null && _self.screenInfo) screenId = _self.screenInfo.Id;
                if (screenId == null) return;
                //console.log('[2]',screenId, ___SCREEN_EVENT_SUBMIT_ID[screenId], ___SCREEN_EVENT_SUBMIT_ID);
                if (___SCREEN_EVENT_SUBMIT_ID[screenId] == null) return;

                var screenInfoSubmit = _self.getScreenInfo(screenId);
                ___SCREEN_EVENT_SUBMIT_ID[screenId].forEach(function (id) {
                    var el = document.getElementById(id);
                    if (el && el.__vue__) el.__vue__.$emit(screenId, screenInfoSubmit);
                });
            },
            addScreenEmit: function (screenId, funcCallback) {
                var _self = this;
                //console.log(screenId, _self._eleID);
                if (___SCREEN_EVENT_SUBMIT_ID[screenId] == null)
                    ___SCREEN_EVENT_SUBMIT_ID[screenId] = [_self._eleID];
                else
                    ___SCREEN_EVENT_SUBMIT_ID[screenId].push(_self._eleID);
                this.$on(screenId, funcCallback);
            },
            getScreenInfo: function (screenId) {
                var _self = this;
                if (_self.objScreens != null && _self.objScreens[screenId] != null) {
                    return _self.objScreens[screenId];
                }
                return null;
            },
            getScreenHomeUI: function () {
                return vueHome;
            },
            getWidgetArea: function () {
                return vueHome.getWidgetArea();
            },
            screenDialogClose: function () {
                var dialog = this.$root;
                if (dialog && typeof dialog['screenDialogClose'] == 'function') dialog.screenDialogClose();
            },
            screenDialogCloseNoCallback: function () {
                var dialog = this.$root;
                if (dialog && typeof dialog['screenDialogClose'] == 'function') dialog.screenDialogCloseNoCallback();
            },
            getDataTextSynchronous: function (url) {
                //if (url.indexOf('/') != 0) url = '/' + url;
                //if (url.indexOf('http') != 0) url = 'http://' + ___NODEJS_HOST + url;
                //f_log_kit(url);
                var r = new XMLHttpRequest();
                r.open('GET', url, false);
                r.send(null);
                if (r.status === 200) return r.responseText;
                return '';
            }
        }
    };
___DATA_SHARED.forEach(function (v) { ___PROPS_DATA_SHARED += ' :' + v + '="' + v + '" '; });
___SCREENS_COMMON_MIXIN = {
    methods: {
        screenOpen: function (options) {
            var parentId = '';
            if (this.$el == null) {
                parentId = _SCREENS_ID.HOME;
                options._screenParentIsHomeUI = true;
            }
            else {
                parentId = this.$el.id;
            }

            //console.log('screenOpen: parentId = ', parentId);

            options._screenParentElemID = parentId;
            f_hui_screenOpen(options);
        },
        screenBlankOpen: function () {
            var _self = this;
            _self.screenOpen({
                Id: _SCREENS_ID.BLANK,
                Components: 'blank-screen',
                className: 'blank-screen',
                overlayShow: false,
                Footer: { buttonOk: false, buttonCancel: false }
            });
        },
        screenErrorOpen: function (screenId, message, optionsFooter, components) {
            if (message == null || message.length == 0) return;

            var _self = this;
            if (optionsFooter == null) optionsFooter = {};
            if (optionsFooter.buttonOk == null) optionsFooter.buttonOk = false;
            if (optionsFooter.buttonCancel == null) optionsFooter.buttonCancel = false;

            _self.screenOpen({
                Id: screenId,
                Components: 'error-screen',
                className: 'screen-error',
                overlayShow: false,
                Header: {
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenAlertOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            if (optionsFooter == null) optionsFooter = { buttonOk: true, buttonCancel: false };
            _self.screenOpen({
                Id: screenId,
                className: 'screen-alert',
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenWarningOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            if (optionsFooter == null) optionsFooter = { buttonOk: true, buttonCancel: false };
            _self.screenOpen({
                Id: screenId,
                className: 'screen-alert',
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenConfirmOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            _self.screenOpen({
                Id: screenId,
                className: 'screen-confirm',
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    classNameMessage: 'f-size20',
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenToastOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            if (optionsFooter == null) optionsFooter = { buttonOk: false, buttonCancel: false };
            _self.screenOpen({
                Id: screenId,
                timeoutDisplay: 5000,
                className: 'screen-toast toast-top screen-toast-widget-remove',
                overlayShow: false,
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    Message: message
                },
                Footer: optionsFooter
            });
        }
    }
};
/*=============================================================
/ COMPONENTS
/=============================================================*/
Vue.component('com-nav-bottom', {
    mixins: [___COMS_MIXIN, ___SCREENS_COMMON_MIXIN, ___CORE_INTERFACE_MIXIN],
    template: '<div :id="el_id"></div>',
    data: function () {
        return {
            code: 'com-nav-bottom',
            el_id: 'com-nav-bottom-' + this._uid + '-' + (new Date().getTime()),
            uc_toolbar_name: 'uc_toolbar_name' + this.el_id
        };
    },
    beforeDestroy: function () {
        var _self = this;
        f_log(_self.code + ':: beforeDestroy');
        if (w2ui[_self.uc_toolbar_name]) w2ui[_self.uc_toolbar_name].destroy();
    },
    destroyed: function () {
        var _self = this;
        f_log(_self.code + ':: destroyed');
    },
    beforeCompile: function () {
        var _self = this;
        f_log(_self.code + ':: beforeCompile');
        this.f_base_hide();
    },
    compiled: function () {
        var _self = this;
        f_log(_self.code + ':: compiled');
    },
    ready: function () {
        var _self = this;
        f_log(_self.code + ':: ready');

        _self.f_toolbar_init();

        this.f_base_show();
        //_LOADING.f_hide();
        this.$on(___DATA_BROADCAST.Dictionary, function (data) {
            console.log('??????????? com-nav-bottom = ', data.length);
        });
    },
    methods: {
        f_toolbar_init: function () {
            var _self = this;

            if (!w2ui[_self.uc_toolbar_name]) {
                $(_self.$el).w2toolbar({
                    name: _self.uc_toolbar_name,
                    style: 'background:#fff;',
                    onClick: function (tbEvent) {
                        switch (tbEvent.target) {
                            case 'id_toolbar_suggestion':
                                _self.suggestion_Show();
                                break;
                            default:
                                var id = '#tb_' + this.name + '_item_' + w2utils.escapeId(event.target);
                                $(id).w2overlay('<div style="margin:8px;">Pressed ' + (new Date().getTime()) + '</div>');
                                break;
                        }
                    },
                    items: [
                        {
                            type: 'menu-radio', id: 'item2', icon: 'fa fa-bars',
                            text: function (item) {
                                var text = item.selected;
                                var el = this.get('item2:' + item.selected);
                                return '' + el.text + '';
                            },
                            selected: 'id3',
                            items: [
                                { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                                { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                                { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 }
                            ]
                        },
                        { type: 'button', id: 'id_toolbar_home', text: '', icon: 'icon-basic-home' },
                        { type: 'button', id: 'id_toolbar_search', text: '', icon: 'icon-basic-magnifier' },
                        { type: 'button', id: 'id_toolbar_suggestion', text: '', icon: 'fa fa-list-ol' },
                        { type: 'spacer' },
                        { type: 'button', id: 'id_toolbar_task', text: '', icon: 'icon-basic-mixer2', count: 7, },
                        { type: 'button', id: 'id_toolbar_mail', text: '', icon: 'fa fa-envelope-o', count: 7, },
                        { type: 'button', id: 'id_toolbar_chat', text: '', icon: 'icon-basic-message-multiple', count: 9, },
                        {
                            type: 'drop', id: 'item4', text: '', icon: 'fa fa-bell-o', tooltip: 'Notification Messages', count: 5,
                            html: '<div style="padding: 10px; line-height: 1.5">You can put any HTML in the drop down.<br>Include tables, images, etc.</div>'
                        },
                        {
                            type: 'menu', id: 'id_toolbar_user_menu', text: '', icon: 'fa fa-user-o', items: [
                                { type: 'button', id: 'id_toolbar_user', text: 'Account Infomation', icon: 'icon-basic-info' },
                                { type: 'button', id: 'id_toolbar_help', text: 'Help', icon: 'icon-basic-question' },
                                { type: 'button', id: 'id_toolbar_settings', text: 'Settings', icon: 'icon-basic-gear' },
                                { text: '--' },
                                { text: 'Item 1', icon: 'fa fa-camera', count: 5 },
                                { text: 'Item 2', icon: 'fa fa-picture-o', disabled: true },
                                { text: 'Item 3', icon: 'fa fa-glass', count: 12 },
                                { text: '--' },
                                { type: 'button', id: 'id_toolbar_logout', text: 'Logout', icon: 'fa fa-sign-out' },
                                { type: 'button', id: 'id_toolbar_close', text: 'Exit Application', icon: 'icon-arrows-circle-remove' },
                            ]
                        },

                        ////{
                        ////    type: 'menu', id: 'item1', text: 'Menu', icon: 'fa fa-table', count: 17, items: [
                        ////      { text: 'Item 1', icon: 'fa fa-camera', count: 5 },
                        ////      { text: 'Item 2', icon: 'fa fa-picture-o', disabled: true },
                        ////      { text: 'Item 3', icon: 'fa fa-glass', count: 12 }
                        ////    ]
                        ////},
                        ////{ type: 'break' },
                        ////{
                        ////    type: 'menu-radio', id: 'item2', icon: 'fa fa-star',
                        ////    text: function (item) {
                        ////        var text = item.selected;
                        ////        var el = this.get('item2:' + item.selected);
                        ////        return 'Radio: ' + el.text;
                        ////    },
                        ////    selected: 'id3',
                        ////    items: [
                        ////        { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                        ////        { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                        ////        { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 }
                        ////    ]
                        ////},
                        ////{ type: 'break' },
                        ////{
                        ////    type: 'menu-check', id: 'item3', text: 'Check', icon: 'fa fa-heart',
                        ////    selected: ['id3', 'id4'],
                        ////    onRefresh: function (event) {
                        ////        event.item.count = event.item.selected.length;
                        ////    },
                        ////    items: [
                        ////        { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                        ////        { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                        ////        { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 },
                        ////        { text: '--' },
                        ////        { id: 'id4', text: 'Item 4', icon: 'fa fa-glass' }
                        ////    ]
                        ////},
                        ////{ type: 'break' },
                        ////{
                        ////    type: 'drop', id: 'item4', text: 'Dropdown', icon: 'fa fa-plus',
                        ////    html: '<div style="padding: 10px; line-height: 1.5">You can put any HTML in the drop down.<br>Include tables, images, etc.</div>'
                        ////},
                        ////{ type: 'break', id: 'break3' },
                        ////{
                        ////    type: 'html', id: 'item5',
                        ////    html: function (item) {
                        ////        var html =
                        ////          '<div style="padding: 3px 10px;">' +
                        ////          ' CUSTOM:' +
                        ////          '    <input size="10" onchange="var el = w2ui.toolbar.set(\'item5\', { value: this.value });" ' +
                        ////          '         style="padding: 3px; border-radius: 2px; border: 1px solid silver" value="' + (item.value || '') + '"/>' +
                        ////          '</div>';
                        ////        return html;
                        ////    }
                        ////},
                        ////{ type: 'spacer' },
                        ////{ type: 'button', id: 'item6', text: 'Item 6', icon: 'fa fa-flag' }

                    ]
                });
            }
        }
    }
});

Vue.component('com-dictionary', {
    mixins: [___COMS_MIXIN, ___SCREENS_COMMON_MIXIN, ___CORE_INTERFACE_MIXIN],
    template: '<div :id="el_id"><ul><li v-for="(index, it) in items" @click="itemClick(it)"><label>{{it.text}}</label><p>{{it.mean}}</p><ol><li><em>More</em><li></ol></li></ul></div>',
    data: function () {
        var _self = this;

        //console.log('Dictionary = ', ___DATA.objCore.Translate.Dictionary);
        //console.log('objCore = ', _self.objCore);
        //console.log('screenInfo = ', _self.screenInfo);

        var _items = [];
        for (var key in ___DATA.objCore.Translate.Dictionary) {
            _items.push({ id: key.split(' ').join('_'), text: key, mean: ___DATA.objCore.Translate.Dictionary[key] });
        }
        console.log('_items = ', _items);

        return {
            code: 'com-dictionary',
            el_id: 'com-' + this._uid + '-' + (new Date().getTime()),
            items: _items
        };
    },
    beforeDestroy: function () {
        var _self = this;
        f_log(_self.code + ':: beforeDestroy');
        _self.freeResource();
    },
    destroyed: function () {
        var _self = this;
        f_log(_self.code + ':: destroyed');
    },
    beforeCompile: function () {
        var _self = this;
        f_log(_self.code + ':: beforeCompile');
        this.f_base_hide();
    },
    compiled: function () {
        var _self = this;
        f_log(_self.code + ':: compiled');
    },
    ready: function () {
        var _self = this;
        f_log(_self.code + ':: ready');
        _self.setup();
    },
    methods: {
        setup: function () {
        },
        freeResource: function () {
        },
        itemClick: function (item) {
            var _self = this;
            console.log('SELECTED = ', JSON.stringify(item));
            _self.translate_Execute(item.text);
        }
    }
});
