var map;
var marker;
var geocoder;
function initMap() {
    var options = {
        zoom: 17,
        center: { lat: 43.32472, lng: 21.90333 },

    }
    map = new google.maps.Map(document.getElementById('map'), options);
    var marker = new google.maps.Marker({
        position: { lat: 43.32472, lng: 21.90333 },
        map: map
    });
}

document.getElementById('Location').addEventListener('change', function () {
    geocoder = new google.maps.Geocoder();
    var address = document.getElementById('Location').value;
    axios.get('https://maps.googleapis.com/maps/api/geocode/json', {
        params: {
            address: address,
            key: 'AIzaSyBzDvV8lc4g4XVi8xPZHzCsZ4jfjUUONVU'
        }
    })
.then(function (response) {

    var lat = response.data.results[0].geometry.location.lat;
    var lng = response.data.results[0].geometry.location.lng;

    var options = {
        zoom: 17,
        center: { lat: lat, lng: lng },

    }
    map = new google.maps.Map(document.getElementById('map'), options);

    marker = new google.maps.Marker({
        position: { lat: lat, lng: lng },
        map: map
    });

    document.getElementById('lat').value = lat;
    document.getElementById('lng').value = lng;

})
.catch(function (error) {
});
});