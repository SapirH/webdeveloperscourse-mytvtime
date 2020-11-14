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
        private readonly TVContext db;

        public StatisticsController(TVContext context)
        {
            db = context;
        }

        // GET: Statistics/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Statistics/GenersStatistics

        public async Task<JsonResult> GenersStatisticsAsync()
        {
            var movies_groups = await db.MovieGenres.GroupBy(movieG => movieG.GenreID).Select(group => new
            {
                Genre = group.First().Genre.Name,
                Count = group.Count(),
            }).ToListAsync();
            return Json(movies_groups);
        }

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
