using Microsoft.EntityFrameworkCore;
using MyLibrary.Models;

namespace MyLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<MyLibrary.Models.Library> Library { get; set; } = default!;
        public DbSet<MyLibrary.Models.Shelf> Shelf { get; set; } = default!;
        public DbSet<MyLibrary.Models.Book> Book { get; set; } = default!;
    }
}




