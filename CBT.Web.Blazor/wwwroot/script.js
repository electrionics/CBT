function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

function writeAuthCookie(value) {
    document.cookie = value;
}

function readAuthCookie() {
    return document.cookie;
}
sok.onerror = function (evt) { console.log(evt); }