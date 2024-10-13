document.getElementById('invite').addEventListener('click', () => {
    var checkboxes = document.querySelectorAll('.custom-control-input');
    var friends = [];
    var id = document.getElementById('eventId').value;

    checkboxes.forEach((x) => {
        if (x.checked) {
            var parent = x.parentNode.parentNode.parentNode;
            var idUser = parent.querySelector(".id");
            friends.push(idUser.value);
        }
    });

    Proxy.invoke('invite',id,friends);
})