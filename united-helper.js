// Helper for booking United Airlines flights via Hipmunk

var __printUrl_original = printUrl;
printUrl = function (url, name, desc) {
    if (name == "United") {
        url = "https://ita-matrix-helper.azurewebsites.net/united-hipmunk.ashx?url=" + encodeURIComponent(url);
        desc = null;
    }
    __printUrl_original(url, name, desc);
};

var __printUrlInline_original = printUrlInline;
printUrlInline = function (url, name, desc, nth) {
    if (name == "United") {
        url = "https://ita-matrix-helper.azurewebsites.net/united-hipmunk.ashx?url=" + encodeURIComponent(url);
        desc = null;
    }
    __printUrlInline_original(url, name, desc, nth);
};
