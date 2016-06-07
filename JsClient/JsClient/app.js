/// <reference path="oidc-client.js" />

document.getElementById("login").addEventListener("click", doLogin, false);
document.getElementById("callApi").addEventListener("click", callApi, false);

var settings = {
    authority: 'http://localhost:5000',
    client_id: 'spa',
    redirect_uri: 'http://localhost:51961/index.html',
    response_type: 'id_token token',
    scope: 'openid profile api1',
};

var manager = new Oidc.UserManager(settings);
var userobj;

function doLogin() {
    log("login");
    manager.signinRedirect();
}

if (window.location.hash) {
    manager.signinRedirectCallback().then(function (userobjthis) {
        userobj = userobjthis;
        log("userObj", userobjthis);
        window.location = "index.html";
    })
} else {
    manager.getUser().then(function (userobjthat) {
        userobj = userobjthat;
        log("userObj", userobjthat);
    })
}





function callApi() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "http://localhost:48791/test");
    xhr.onload = function () {
        var data = JSON.parse(xhr.responseText);
        log("API Result", data);
    }
    xhr.setRequestHeader("Authorization", "Bearer " + userobj.access_token);
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

