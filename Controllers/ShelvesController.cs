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
    public class ShelvesController : Controller
    {
        private readonly AppDbContext _context;

        public ShelvesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Shelves
        public async Task<IActionResult> Index(int? id)
        {
            IEnumerable<Shelf> groupedList;
            if (id != null)
            {
                groupedList = _context.Shelf.Include(s => s.Library)
                    .Where(s => s.LibraryId == id);
                Library lib = await _context.Library.FindAsync(id);
                ViewData["LibraryName"] = lib.Genre;
            } else {
                groupedList = _context.Shelf.Include(s => s.Library);
            }
            LibraryToShelves model = new LibraryToShelves()
            {
                Id = id, List = groupedList
            };
            return View(model);
        }

        // GET: Shelves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelf
                .Include(s => s.Library)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        // GET: Shelves/Create
        public IActionResult Create(int id)
        {
            ViewData["LibraryId"] = id;
            return View();
        }

        // POST: Shelves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]  // יש פה שינויים
        public async Task<IActionResult> Create([Bind("Height,Width,LibraryId")] Shelf shelf)
        {
            shelf.AvailableWidth = shelf.Width;
            shelf.Library = await _context.Library.FindAsync(shelf.LibraryId);
            var otherShelves = _context.Shelf.Where(s => s.LibraryId == shelf.LibraryId).ToList();
            int maxNum = shelf.Library.FirstShelf ?? 1;
            foreach (var item in otherShelves)
            {
                if (item.Number > maxNum) { maxNum = item.Number; }
            }
            shelf.Number = maxNum + 1;
            if (ModelState.IsValid)
            {
                _context.Shelf.Add(shelf);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = shelf.LibraryId });
            }
            ViewData["LibraryId"] = shelf.LibraryId;
            return View(shelf);
        }

        // GET: Shelves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelf.FindAsync(id);
            if (shelf == null)
            {
                return NotFound();
            }
            ViewData["LibraryId"] = new SelectList(_context.Library, "Id", "Id", shelf.LibraryId);
            return View(shelf);
        }

        // POST: Shelves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Height,Width,AvailableWidth,LibraryId")] Shelf shelf)
        {
            if (id != shelf.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shelf);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShelfExists(shelf.Id))
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
            ViewData["LibraryId"] = new SelectList(_context.Library, "Id", "Id", shelf.LibraryId);
            return View(shelf);
        }

        // GET: Shelves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelf
                .Include(s => s.Library)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        // POST: Shelves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shelf = await _context.Shelf.FindAsync(id);
            if (shelf != null)
            {
                _context.Shelf.Remove(shelf);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShelfExists(int id)
        {
            return _context.Shelf.Any(e => e.Id == id);
        }
    }
}
