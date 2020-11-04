using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyTvTime.Models;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace MyTvTime.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Message"] = "Who we are:";

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
