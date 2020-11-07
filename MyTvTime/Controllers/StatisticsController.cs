using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class StatisticsController : Controller
    {
        private readonly UserContext db;

        public StatisticsController(UserContext context)
        {
            db = context;
        }

        // GET: Statistics/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Statistics/GenersStatistics

        //public JsonResult GenersStatistics()
        //{
        //    var movies = db.Movie
        //        .Join(db.Movie,          // Get movie info from movies with 's book ID
        //                movieSelector => movieSelector.ID,
        //                genreSelector => genreSelector.ID,
        //        (movie, genre) => new Movie()
        //        {
        //            // Keep movie data
        //            IMDBID = movie.IMDBID,
        //            Name = movie.Name,
        //            ReleaseDate = movie.ReleaseDate,
        //            Language = movie.Language,
        //            Runtime = movie.Runtime,
        //            Description = movie.Description,

        //            // Get gener data
        //            Genres = movie.Genres,
        //            //genre = genre
        //        }).GroupBy(movie => movie.Genres).Select(group => new
        //        {
        //            Name = group.First().Name,
        //            Language = group.First().Language,
        //            Count = group.Count(),
        //        }).AsQueryable();
        //    return Json(movies);
        //}

        // GET: Statistics/LanguageStatistics
        public async Task<JsonResult> LanguageStatisticsAsync()
        {
            var movies_groups = await db.Movie.GroupBy(movie => movie.Language).Select(group => new
            {
                Language = group.Key,
                Count = group.Count(),
            }).ToListAsync();
            return Json(movies_groups);
        }


    }
}
