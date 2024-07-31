using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibrary.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public string? SetName { get; set; } = null;
        public int ShelfId { get; set; }
        [ForeignKey(nameof(ShelfId))]
        public Shelf? Shelf { get; set; }

    }
}
