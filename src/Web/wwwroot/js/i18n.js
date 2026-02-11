// i18n.js
// Simple client-side lookup for localized strings loaded by _JsLocalization.cshtml.
(function(global) {
    var store = global.__i18n || {};
    global.i18n = global.i18n || {};
    global.i18n.t = function(file, key, fallback) {
        if (store && store[file] && typeof store[file][key] === 'string') {
            return store[file][key];
        }
        return fallback || key;
    };
})(window);
