document.getElementById('toggle-button').addEventListener('click', function () {
    var nav = document.getElementById('navbarColor02');
    if (!nav.classList.contains('show')) {
        document.getElementById('navbarColor02').classList.add('show');
    }
    else {
        document.getElementById('navbarColor02').classList.remove('show');
    }
});

