
//crating hub
ServerUrl = "https://localhost:44365/";
var Url = ServerUrl + "signalr";

var SignalrConnection = $.hubConnection(Url, {
    useDefaultPath: false
});

var Proxy = SignalrConnection.createHubProxy("myHyb");

//connecting to hub
SignalrConnection.start().done(function () {
    var userId = document.getElementById('MyId').value;
    console.log(userId);
    Proxy.invoke('connect', userId);
})
    .fail(function () {
        alert("failed in connecting to the signalr server");
});


Proxy.on("receiveRequest", function (senderId) {
    {
        var x = document.getElementById("toast");

        x.style.display = 'block';

        setTimeout(function () {
            var x = document.getElementById('toast');
            x.style.display = 'none';
        }, 30000);
    };
});

Proxy.on("receiveInvite", function (eventId) {
    {
        var x = document.getElementById("toast");

        x.style.display = 'block';

        setTimeout(function () {
            var x = document.getElementById('toast');
            x.style.display = 'none';
        }, 30000);
    };
});



document.getElementById('toastClose').addEventListener('click', function () {
    var x = document.getElementById('toast');
    x.style.display = 'none';
});


document.getElementById('btnLogout').addEventListener('click', function () {
    Proxy.invoke('disconect');
});