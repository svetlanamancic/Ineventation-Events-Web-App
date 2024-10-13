document.getElementById('btnsend').addEventListener('click', function () {
    var idFriend = document.getElementById('idFriend').value;
    Proxy.invoke('sendRequest', idFriend);
    var buttonSend = document.getElementById('btnsend');
    buttonSend.innerHTML = "Sent";
});