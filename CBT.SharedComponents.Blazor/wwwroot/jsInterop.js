// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}
export function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

export function writeAuthCookie(value) {
    document.cookie = value;
}

export function readAuthCookie() {
    return document.cookie;
}
sok.onerror = function (evt) { console.log(evt); }