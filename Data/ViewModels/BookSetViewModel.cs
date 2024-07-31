using MyLibrary.Models;

namespace MyLibrary.Data.ViewModels
{
    public class BookSetViewModel
    {
        public string SetName { get; set; }
        public double Height { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
