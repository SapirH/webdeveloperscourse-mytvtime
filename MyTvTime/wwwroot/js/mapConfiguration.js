var map = null;
var userLocation = null;
var users = @Html.Raw(Json.Serialize(Model));
var usersPinsArray = [];  // Created this because iterating over map entities is a pain in the ass and also in order to distinguish between user-pin entities over other possible ones

$('tr').click(function () {
    var userId = $(this)[0].children[0].innerText;
    focusOnUser(userId);
});

function LoadMap() {
    createMap();
    setUserLocation();
    createInfoBox();

    Microsoft.Maps.loadModule('Microsoft.Maps.SpatialMath', function () { });  // Needed for distance calculation
    Microsoft.Maps.loadModule('Microsoft.Maps.Search', function () {
        searchManager = new Microsoft.Maps.Search.SearchManager(map);
        createUseresPins(setMapInitialView);  // Call setMapInitialView() only after all pins get created
    });
}

function createMap() {
    map = new Microsoft.Maps.Map('#articleMap', {
        credentials: "AglcOvofpKm1X-WOXDY3_Cr0bxdAbKzUid-4bp7Em3BxbNPK_kT-X8iWI4B1RF0H",
        showMapTypeSelector: true,
        showZoomButtons: true,
        showLocateMeButton: true,
        showBreadcrumb: true
    });
}

function createInfoBox() {
    infobox = new Microsoft.Maps.Infobox(map.getCenter(), {
        visible: false
    });
    infobox.setMap(map);
    Microsoft.Maps.Events.addHandler(map, 'click', function () { infobox.setOptions({ visible: false }); });
}

function setUserLocation() {
    // If getting current position is not possible user the map center as the user location
    userLocation = map.getCenter();
    navigator.geolocation.getCurrentPosition(function (position) {
        userLocation = new Microsoft.Maps.Location(
            position.coords.latitude,
            position.coords.longitude);
    });
}

// The geocode function which converts address into location calls the requestOptions callback function.
// This messes up the sync and makes it hard to point out when the function truly finished creating all pins.
// To solve this is the _callback parameter, it indicates which function should be called when all user pins were truly created.
function createUseresPins(_callback) {
    $.each(users, function (idx, user) {
        var address = user.country;
        var requestOptions = {
            bounds: map.getBounds(),
            where: address,
            callback: function (answer, userData) {
                createUserPin(user, answer.results[0].location);
                if (usersPinsArray.length == users.length) {  // Was last user pin created and this is the function's last iteration
                    _callback();
                }
            }
        };
        searchManager.geocode(requestOptions);  // Calls the requestOptions callback function
    });
}

function createUserPin(user, location) {
    var pin = new Microsoft.Maps.Pushpin(location, {
        title: user.username,
        text: user.id.toString(),
        enableHoverStyle: true
    });
    pin.metadata = {
        userId: user.id.toString(),
        username: user.username,
        email: user.email,
    }
    setPinWeather(pin);
    setPinDistance(pin);
    Microsoft.Maps.Events.addHandler(pin, 'mouseover', pushpinMouseOver);
    map.entities.push(pin);
    usersPinsArray.push(pin);
}

// Set the initial view to the closest user if possible, if not use user location
function setMapInitialView() {
    var closestUser = findClosestUser();
    var bestViewLocation = (closestUser) ? closestUser.getLocation() : userLocation;
    map.setView({ center: bestViewLocation, zoom: 17 });
}

function findClosestUser() {
    return usersPinsArray.reduce(function (prev, curr) {
        return prev.metadata.distance < curr.metadata.distance ? prev : curr;
    });
}

function focusOnUser(userId) {
    var pin = usersPinsArray.find(function (p) { return p.metadata.userId == userId; })
    if (pin) {
        map.setView({ center: pin.getLocation(), zoom: 17 });
        showPinInfo(pin);
    }
}

function pushpinMouseOver(e) {
    showPinInfo(e.target)
}

function showPinInfo(pin) {
    if (pin.metadata) {
        var metadata = pin.metadata;
        infobox.setOptions({
            location: pin.getLocation(),
            title: pin.getTitle(),
            description: ("User: " + metadata.username + " | E-Mail: " + metadata.email + " | Weather in their place: " + metadata.weather),
            visible: true  // Clicking anywhere on the map should bring the visibility back to false
        });
    }
}

function setPinDistance(pin) {
    if (userLocation) {
        pin.metadata.distance = Microsoft.Maps.SpatialMath.getDistanceTo(pin.getLocation(), userLocation, Microsoft.Maps.SpatialMath.DistanceUnits.Miles);
    } else {
        pin.metadata.distance = Infinity;
    }
}

function setPinWeather(pin) {
    var key = 'e972e7f0d681d56210ca361472ee82e8';
    var location = pin.getLocation();
    fetch('https://api.openweathermap.org/data/2.5/weather?lat=' + location.latitude + '&lon=' + location.longitude + '&appid=' + key)
        .then(function (resp) { return resp.json() }) // Convert data to json
        .then(function (data) {
            var celcius = Math.round(parseFloat(data.main.temp) - 273.15);
            var weatherDesc = data.weather[0].description;
            pin.metadata.weather = weatherDesc + " " + celcius + '&deg;';
        })
        .catch(function () {
            pin.metadata.weather = "no weather data";
        });
}