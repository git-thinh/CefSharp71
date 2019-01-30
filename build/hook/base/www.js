var ___DATA = { appInfo: {} };
var ___DATA_SHARED = ['appInfo'];
var ___DATA_BROADCAST = {};
var ___CORE;
var ___COMS_ID = [];
function CoreInterface() { }
CoreInterface.prototype = { setup: function () { } };
function ___onDomReady() {
    console.log("DOM loaded ...");
    ___CORE.setup(); 
}
if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", ___onDomReady); else ___onDomReady();
///////////////////////////////////////////////////////////////////////
var ___sendBroadcastData = function (key) {
    ___COMS_ID.forEach(function (comId) {
        var el = document.getElementById(comId);
        if (el && el.__vue__) {
            el.__vue__.$emit(key, ___DATA[key]);
        }
    });
};
function ___screenOpen(screenInfo) {

}