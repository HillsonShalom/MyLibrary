using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Data;
using MyLibrary.Data.ViewModels;
using MyLibrary.Models;

namespace MyLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(int? id)
        {
            IEnumerable<Book> groupedList;
            if (id != null)
            {
                groupedList = _context.Book.Include(s => s.Shelf)
                    .Where(s => s.ShelfId == id);
                ViewData["ShelfId"] = id;
            }
            else
            {
                groupedList = _context.Book.Include(s => s.Shelf);
                ViewData["ShelfId"] = null;
            }
            ShelfToBooks model = new ShelfToBooks()
            {
                Id = id,
                List = groupedList
            };
            return View(model);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id");
            return View();
        }

        public IActionResult CreateSet(int id)
        {
            ViewData["ShelfId"] = id;
            return View();
        }

        // לבדוק את האינקלוד של המדפים כשלא מכניסים מזהה בקישור
        [HttpPost]
        public IActionResult CreateSet(BookSetViewModel model)
        {
            model.Books = model.Books.Where(s => s.Name != null && s.Width != 0).ToList();
            Shelf shelf = _context.Shelf.Find(model.ShelfId);
            if (ModelState.IsValid)
            {
                foreach (var book in model.Books)
                {
                    book.SetName = model.SetName;
                    book.Height = model.Height;
                    book.Shelf = shelf;
                    book.ShelfId = model.ShelfId;
                    _context.Book.Add(book);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Height,Width,SetName,ShelfId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", book.ShelfId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", book.ShelfId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Height,Width,SetName,ShelfId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", book.ShelfId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
