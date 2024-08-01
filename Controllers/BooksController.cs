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
                Shelf shelf = await _context.Shelf.FindAsync(id);
                Library library = await _context.Library.FindAsync(shelf.LibraryId);
                ViewData["LibraryName"] = library.Genre;
                ViewData["LibraryId"] = library.Id;
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
        public IActionResult Create(int id)
        {
            ViewData["ShelfId"] = id; // מחקתי מה שהיה כתוב והשוויתי את זה לפונקצייה הבאה
            return View();
        }

        //POST: Books/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Height,Width,ShelfId")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.Shelf = await _context.Shelf.FindAsync(book.ShelfId);
                if (book.Shelf.AvailableWidth >= book.Width && book.Shelf.Height >= book.Height)
                {
                    //if (book.Shelf.Height >= book.Height + 10) ViewData["Message"] = "The book is too low";
                    book.Shelf.AvailableWidth -= book.Width;
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = book.ShelfId });
                }
                else
                {
                    if (book.Shelf.AvailableWidth < book.Width && book.Shelf.Height < book.Height) ViewData["Message"] = "The shelf is too Small and the book is too high";
                    else if (book.Shelf.AvailableWidth < book.Width) ViewData["Message"] = "The shelf is too Small";
                    else if (book.Shelf.Height < book.Height) ViewData["Message"] = "The book is too high";
                    ViewData["ShelfId"] = book.ShelfId;
                    return View(book);
                }
            }
            ViewData["ShelfId"] = book.ShelfId;
            return View(book);
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
            double width = 0;
            foreach (var item in model.Books)
            {
                width += item.Width;
            }
            if (ModelState.IsValid)
            {
                if (shelf.AvailableWidth >= width && shelf.Height >= model.Height)
                {
                    shelf.AvailableWidth -= width;
                    foreach (var book in model.Books)
                    {
                        book.SetName = model.SetName;
                        book.Height = model.Height;
                        book.Shelf = shelf;
                        book.ShelfId = model.ShelfId;
                        _context.Book.Add(book);
                    }
                    _context.SaveChanges();
                    return RedirectToAction("Index", new { id = model.ShelfId });
                }
                else
                {
                    if (shelf.AvailableWidth < width && shelf.Height < model.Height) ViewData["Message"] = "The shelf is too Small and the set is to high";
                    else if (shelf.AvailableWidth < width) ViewData["Message"] = "The shelf is too Small";
                    else if (shelf.Height < model.Height) ViewData["Message"] = "The book is too high";
                    ViewData["ShelfId"] = model.ShelfId;
                    return View(model);
                }
            }
            ViewData["ShelfId"] = model.ShelfId;
            return View(model);
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
