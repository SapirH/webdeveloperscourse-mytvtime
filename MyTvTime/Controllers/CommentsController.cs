using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTvTime.Data;
using MyTvTime.Models;

namespace MyTvTime.Controllers
{
    public class CommentsController : Controller
    {
        private readonly TVContext db;

        public CommentsController(TVContext context)
        {
            db = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index(int movieID)
        {
            var tvContext = db.Comment.Where(m => m.MovieID == movieID).Include(c => c.User);
            return PartialView(await tvContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await db.Comment
                .Include(c => c.Movie)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        
        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Add(comment);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieID"] = new SelectList(db.Movie, "ID", "ID", comment.MovieID);
            ViewData["UserID"] = new SelectList(db.User, "Id", "country", comment.UserID);
            return View(comment);
        }
        

        [HttpPost]
        public async Task<IActionResult> AddComment(int movieID, string commentText)
        {
            if (!string.IsNullOrEmpty(commentText) && HttpContext.User.Identity.IsAuthenticated)
            {
                db.Add(new Comment { UserID = int.Parse(HttpContext.User.Identity.Name), MovieID = movieID, Text = commentText, Date = DateTime.Now });
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Movies", new { id = movieID });
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await db.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["MovieID"] = new SelectList(db.Movie, "ID", "ID", comment.MovieID);
            ViewData["UserID"] = new SelectList(db.User, "Id", "country", comment.UserID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int movieID, string newCommentText)
        {
            if (!string.IsNullOrWhiteSpace(newCommentText))
            {
                var comment = await db.Comment.FindAsync(id);
                if (comment != null)
                {
                    comment.Date = DateTime.Now;
                    comment.Text = newCommentText;
                    db.Update(comment);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Details", "Movies", new { id = movieID });
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await db.Comment
                .Include(c => c.Movie)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int movieID)
        {
            var comment = await db.Comment.FindAsync(id);
            db.Comment.Remove(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Movies", new { id = movieID });
        }

        private bool CommentExists(int id)
        {
            return db.Comment.Any(e => e.ID == id);
        }
    }
}
