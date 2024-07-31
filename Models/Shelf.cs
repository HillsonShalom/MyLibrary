using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibrary.Models
{
    public class Shelf
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double AvailableWidth { get; set; }
        public int LibraryId { get; set; }
        [ForeignKey(nameof(LibraryId))]
        public Library Library { get; set; }
    }
}
