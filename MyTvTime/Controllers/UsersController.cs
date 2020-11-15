using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTvTime.Data;
using MyTvTime.Models;
using System.Security.Claims;

namespace MyTvTime.Controllers
{
    public class UsersController : Controller
    {
        private readonly TVContext _context;

        public UsersController(TVContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string username, string country)
        {
            if(!Convert.ToBoolean(((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value))
            {
                return RedirectToAction(nameof(AuthController.AccessDenied), "Auth");
            }
            var users = from u in _context.User select u;

            if(!string.IsNullOrWhiteSpace(username))
			{
                users= users.Where(u => u.username == username);
			}

            if(!string.IsNullOrWhiteSpace(country))
			{
                users= users.Where(u => u.country == country);
            }

            List<User> res = await users.ToListAsync();
            return View(res);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["IsUserAdmin"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value;
            ViewData["UserId"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value;
            if (id == null)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = ((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value;
                    id = Int32.Parse(identity);
                }
                else
                {
                    return NotFound();
                }
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
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
            ViewData["UserId"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value;
            ViewData["IsUserAdmin"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value;
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,username,password,email,birthdate,country,language,sex, isAdmin")] User user)
        {
            ViewData["IsUserAdmin"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value;
            ViewData["UserId"] = ((ClaimsIdentity)User.Identity).FindFirst(type: "UserId").Value;
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
                return View("Details",user);
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!Convert.ToBoolean(((ClaimsIdentity)User.Identity).FindFirst(type: "isAdmin").Value))
            {
                return RedirectToAction(nameof(AuthController.AccessDenied), "Auth");
            }
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
          
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        }

        private bool IsUserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
