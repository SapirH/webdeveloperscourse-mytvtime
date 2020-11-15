using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTvTime.Data;
using System.Security.Claims;
using System;

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
            if (!Convert.ToBoolean(((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value))
            {
                return RedirectToAction(nameof(AuthController.AccessDenied), "Auth");
            }
            return View();
        }

        // GET: Statistics/GenersStatistics

        public async Task<JsonResult> GenersStatisticsAsync()
        {
            var mg = db.MovieGenres.Include(mg => mg.Genre).Select(mg => new
            {
                GenreName = mg.Genre.Name,
            }).AsQueryable();

            var genres_groups = await mg.GroupBy(gm => gm.GenreName).Select(group => new
            {
                Genre = group.Key,
                Count = group.Count(),
            }).ToListAsync();

            return Json(genres_groups);
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
