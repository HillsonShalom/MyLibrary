using MyLibrary.Models;

namespace MyLibrary.Data.ViewModels
{
    public class LibraryToShelves
    {
        public int? Id { get; set; }
        public IEnumerable<Shelf> List {  get; set; } 
    }

    public class ShelfToBooks
    {
        public int? Id { get; set; }
        public IEnumerable<Book> List { get; set; }
    }
}
