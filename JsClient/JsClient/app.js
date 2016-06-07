/// <reference path="oidc-client.js" />

document.getElementById("login").addEventListener("click", doLogin, false);
document.getElementById("callApi").addEventListener("click", callApi, false);


function doLogin() {

}


function callApi() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "http://localhost:48791/test");
    xhr.onload = function () {
        var data = JSON.parse(xhr.responseText);
        log("API Result", data);
    }
    xhr.send();
}


function log() {
    document.getElementById('out').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('out').innerHTML += msg + '\r\n';
    });
}

