using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyTvTime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using MyTvTime.Data;
using System.Security.Claims;

namespace MyTvTime.Controllers
{
    public class HomeController : Controller
    {
        private readonly TVContext db;

        public HomeController(TVContext context)
        {
            db = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            ViewData["IsUserAdmin"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value;
            ViewData["UserName"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "username").Value;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode.HasValue ? statusCode.ToString() : null
            };

            if (statusCode.HasValue)
            {
                if (statusCode == 400)
                {
                    error.AdditionalInfo = "Bad request.";
                }
                else if (statusCode == 404)
                {
                    error.AdditionalInfo = "Page not found or Resource not found.";
                }
                else if (statusCode == 500)
                {
                    error.AdditionalInfo = "Server error, something went wrong.";
                }
                else
                {
                    error.AdditionalInfo = "Unknown error.";
                }
            }

            return View(error);
        }
    }
}
