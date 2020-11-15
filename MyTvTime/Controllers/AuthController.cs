using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTvTime.Data;
using MyTvTime.Models;

namespace MyTvTime.Controllers
{
    public class AuthController : Controller
    {
        private readonly TVContext _context;

        public AuthController(TVContext context)
        {
            _context = context;
        }

        //Login view
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //Login post function
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {

                    var users = _context.User.Where(a => a.username.Equals(username) && a.password.Equals(password)).ToList();
                    if (users != null && users.Count() > 0)
                    {
                        await _SignInAsync(users.First());
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    else
                    {
                        ViewBag.error = "Login failed";
                        return View();
                    }
                }
            }
            return View();
        }


        //Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        //Reset password view
        [AllowAnonymous]
        public IActionResult ResetPassword(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //Reset password post function
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                var users = _context.User.Where(a => a.email.Equals(email)).ToList();
                if (newPassword == confirmPassword && users != null && users.Count() > 0)
                {
                    var user = users[0];
                    user.password = newPassword;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    await _SignInAsync(users.First());
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                return RedirectToAction(nameof(AuthController.AccessDenied), "Auth");
            }
            return View();
        }
        //Create view
        [AllowAnonymous]
        public IActionResult Create(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,username,password,email,birthdate, sex, country, language")] User user)
        {
            if (ModelState.IsValid)
            {
                if(user.username == "admin")
                {
                    user.isAdmin = true;
                }
                _context.Add(user);
                await _context.SaveChangesAsync();
                await _SignInAsync(user);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View(user);
        }

        //AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

        //SignIn to cookies
        [AllowAnonymous]
        private async Task _SignInAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim("Email", user.email),
                new Claim("username", user.username),
                new Claim("isAdmin", user.isAdmin.ToString()),
                new Claim("UserId", user.Id.ToString()),
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity),
                authProperties
            );
        }
    }
}
