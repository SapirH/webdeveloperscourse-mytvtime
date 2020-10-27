using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTvTime.Data;
using MyTvTime.Models;

namespace MyTvTime.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
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
                _context.Add(user);
                await _context.SaveChangesAsync();
                await SignInAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,username,password,email,birthdate")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IsUserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Reset password view
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
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
                    await SignInAsync(users.First());
                    return View("~/Views/Movies/Index.cshtml");
                }
                return NotFound();
            }
            return View();
        }

        //Login view
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        //Login post function
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {

                    var users = _context.User.Where(a => a.username.Equals(username) && a.password.Equals(password)).ToList();
                    if (users != null && users.Count() > 0)
                    {
                        await SignInAsync(users.First());
                        return View("~/Views/Movies/Index.cshtml");
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
        public ActionResult Logout()
        {
            //remove session
            //HttpContext.Session.Clear();
            //HttpContext.Session.SetString("Logged", "false");

            return RedirectToAction("Login");
        }
        private bool IsUserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        //SignIn to cookies
        [AllowAnonymous]
        private async Task SignInAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim("Email", user.email),
                new Claim("UserId", user.Id.ToString()),
                new Claim("isAdmin", user.isAdmin.ToString())
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1)
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity),
                authProperties
            );
            HttpContext.Session.SetString("Logged", "true");
            HttpContext.Session.SetString("UserName", user.username);
        }
    }
}
