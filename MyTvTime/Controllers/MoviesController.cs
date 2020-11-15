using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MyTvTime.Data;
using MyTvTime.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MyTvTime.Controllers
{
    public class MoviesController : Controller
    {
        private readonly TVContext db;
        private readonly int numMoviesToAdd = 5;

        public MoviesController(TVContext context)
        {
            db = context;
        }

        public async Task GenerateGenresSelectList()
        {
            List<SelectListItem> genresSelectList = new List<SelectListItem>();
          //  genresSelectList.Add(new SelectListItem("", "-1"));
            var allGenres = await (from g in db.Genre select g).ToListAsync();
            foreach (Genre g in allGenres)
                genresSelectList.Add(new SelectListItem(g.Name, g.ID.ToString()));
            ViewData["GenresSelectList"] = genresSelectList;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string title, string language, int releaseYear, string genre)
        {
            var movies = from m in db.Movie select m;

            await GenerateGenresSelectList();

            //No search at all.
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(language) && releaseYear == 0 && string.IsNullOrEmpty(genre))
                return View(await movies.ToListAsync());

            //Search by name was used.
            if (!string.IsNullOrWhiteSpace(title))
            {
                //Show the movies found in our DB And if nothing was found automatically show results from IMDB.
                movies = movies.Where(x => x.Name.Contains(title));
                var moviesRes = await movies.ToListAsync();
                if (!moviesRes.Any())
                {
                    ViewData["isIMDB"] = true;
                    return View(await SearchIMDBAsync(title));
                }
            }
            
            //If more searches were applied further filter the results (Works only on results from our DB).
            if (!string.IsNullOrWhiteSpace(language))
                movies = movies.Where(x => x.Language.Contains(language));
            if (releaseYear > 0)
                movies = movies.Where(x => x.ReleaseDate.Year.Equals(releaseYear));

            int genreID = int.TryParse(genre, out genreID) ? genreID : -1;
            if (genreID != -1)
                movies = from m in movies join g in db.MovieGenres on m.ID equals g.MovieID where g.GenreID == genreID select m;

            List<Movie> res = await movies.ToListAsync();


            return View(res);
        }

        public async Task<IActionResult> IndexIMDB(string title)
        // GET: Movies
        
        {
            ViewData["IsUserAdmin"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value;

            await GenerateGenresSelectList();

            ViewData["isIMDB"] = true;

            if (string.IsNullOrEmpty(title))
                return View("Index", new List<Movie>());
            return View("Index", await SearchIMDBAsync(title));
        }

        public async Task<IActionResult> WatchList() {
            int userId = HttpContext.User.Identity.IsAuthenticated ? int.Parse(((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value) : 0;

            var myWatchList = await db.UserMovie.Where(um => um.UserId == userId).Include(m => m.Movie).Include(m=> m.Movie.Genres).ToListAsync();

            var MoviesId = myWatchList.Select(m => m.MovieId).ToArray();
            var Genre = myWatchList.Select(um => um.Movie.Genres);
            var GenreDistinct = Genre.SelectMany(item => item).Distinct().ToArray();
            var GenereCount = GenreDistinct.GroupBy(item => item.GenreID).Select(group => new
            {
                GenreId = group.Key,
                Count = group.Count()
            }).OrderByDescending(x=>x.Count).ToArray();

            if (GenereCount.Length > 1)
            {
                var RecommendedMovies = db.MovieGenres.Include(m => m.Movie).Where(mg => (mg.GenreID == GenereCount[0].GenreId || mg.GenreID == GenereCount[1].GenreId) && !MoviesId.Contains(mg.MovieID)).Select(mg => mg.Movie).Distinct();

                ViewBag.RecommendedMovies = RecommendedMovies;
            }

            return View(myWatchList);
        }

       

        public async Task<List<Movie>> SearchIMDBAsync(string title)
        {
            List<Movie> movies = new List<Movie>();
            
            for (int i = 1; i <= 3; i++)
            {
                //Send the request through the API.
                var client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/?s=" + title + "&page=" + i + "&r=json&type=movie");
                var request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-key", "93f2bbe6bdmsh4a12d4d9e5b771dp14b702jsn8bc0f393b0ca");
                request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
                var response = await client.ExecuteAsync(request);
                if(response.IsSuccessful)
				{
                    SearchMovieResultRoot resultsRoot = JsonConvert.DeserializeObject<SearchMovieResultRoot>(response.Content);
                    if(resultsRoot.totalResults != null) {
                        if (i * 10 > int.Parse(resultsRoot.totalResults))
                            i = 4;

                        foreach (SearchMovieResult result in resultsRoot.Search)
                            movies.Add(new Movie { ID = -1, Name = result.Title, IMDBID = result.imdbID, ImageURL = result.Poster });
                    }
                }
            }

            return movies;
        }
        
        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await db.Movie.Where(m => m.ID == id).Include(x => x.Comments).ThenInclude(x => x.User).Include(x => x.Genres).ThenInclude(x => x.Genre)
                .FirstOrDefaultAsync();
            if (movie == null)
            {
                return NotFound();
            }
            int userId = HttpContext.User.Identity.IsAuthenticated ? int.Parse(((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value) : 0;
           
            var movieWatched = await db.UserMovie.FirstOrDefaultAsync(um => (um.MovieId == id && um.UserId == userId));

            ViewData["UserID"] = userId;

            if (movieWatched == null)
            {
                ViewBag.inWatchList = false;
            } else
            {
                ViewBag.inWatchList = true;
            }

            return View(movie);
        }

        public async Task<IActionResult> AddMovieAsync(string IMDBID)
        {
            var checkMovie = from m in db.Movie where m.IMDBID == IMDBID select m;
            if (checkMovie.Any())
            {
                return RedirectToAction("Details", new { id = checkMovie.First().ID });
             }
            var client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/?i=" + IMDBID +" &r=json&type=movie&plot=short");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-key", "93f2bbe6bdmsh4a12d4d9e5b771dp14b702jsn8bc0f393b0ca");
            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                IMDBMovieDetails movieDetails = JsonConvert.DeserializeObject<IMDBMovieDetails>(response.Content);

                Movie m = new Movie
                {
                    IMDBID = movieDetails.imdbID,
                    Name = movieDetails.Title,
                    ReleaseDate = movieDetails.GetReleaseDateTime(),
                    Runtime = movieDetails.GetRuntimeAsInt(),
                    Description = movieDetails.Plot,
                    Language = movieDetails.GetFirstLanguage(),
                    ImageURL = movieDetails.Poster
                };

                await db.Movie.AddAsync(m);
                await db.SaveChangesAsync();

                string[] genres = movieDetails.GetGenresArray();
                foreach (string s in genres)
                {
                    var genreFromDB = from g in db.Genre where (g.Name == s) select g;
                    if (!genreFromDB.Any())
                    {
                        await db.Genre.AddAsync(new Genre { Name = s });
                        await db.SaveChangesAsync();
                        genreFromDB = from g in db.Genre where (g.Name == s) select g;
                    }
                    Genre genre = genreFromDB.First();
                    var newMovieGenre = new MovieGenres { MovieID = m.ID, GenreID = genre.ID };
                    await db.MovieGenres.AddAsync(newMovieGenre);
                    await db.SaveChangesAsync();
                }
                await db.SaveChangesAsync();

                return  RedirectToAction("Details", new { id = m.ID });
            }
            return Redirect("~/Error");
        }
 
        [HttpPost, ActionName("AddToWatchList")]
        public async Task<IActionResult> AddToWatchList(string movieId, string isExist)
		{
            int userId = HttpContext.User.Identity.IsAuthenticated ? int.Parse(((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value) : 0;
            if (isExist == "0")
            {
                try
                {
                    await db.UserMovie.AddAsync(new UserMovie { MovieId = int.Parse(movieId), UserId = userId });
                    await db.SaveChangesAsync();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
            else
			{
                var userMovie = await db.UserMovie.FirstOrDefaultAsync(um=> (um.MovieId == int.Parse(movieId) && um.UserId == userId));

                if(userMovie != null)
				{
                    db.UserMovie.Remove(userMovie);
                    await db.SaveChangesAsync();
                }

            }

			return RedirectToAction("Details", new { id = int.Parse(movieId) });
        }
    }
}
