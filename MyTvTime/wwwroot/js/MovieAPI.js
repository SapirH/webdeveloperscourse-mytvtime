var settings = {
	"async": true,
	"crossDomain": true,
	"url": "https://movies-tvshows-data-imdb.p.rapidapi.com/?page=1&type=get-airingtoday-shows",
	"method": "GET",
	"headers": {
		"x-rapidapi-host": "movies-tvshows-data-imdb.p.rapidapi.com",
		"x-rapidapi-key": "93f2bbe6bdmsh4a12d4d9e5b771dp14b702jsn8bc0f393b0ca"
	}
}

$	.ajax(settings).done(function (response) {
	console.log(response);
});