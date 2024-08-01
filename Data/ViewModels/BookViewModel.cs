namespace MyLibrary.Data.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class BookViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(5, 100, ErrorMessage = "Height must be between 5 and 100.")]
        public double Height { get; set; }

        [Required]
        [Range(0.1, 20, ErrorMessage = "Width must be between 0.1 and 20.")]
        public double Width { get; set; }

        [Required]
        public int ShelfId { get; set; }

        [CustomValidation(typeof(BookViewModel), "ValidateHeight")]
        public double BookHeight { get; set; }

        public static ValidationResult ValidateHeight(double height, ValidationContext context)
        {
            var instance = context.ObjectInstance as BookViewModel;
            if (instance != null && instance.BookHeight > instance.ShelfHeight)
            {
                return new ValidationResult("The book height exceeds the shelf height.");
            }
            if (instance != null && instance.BookHeight + 10 <= instance.ShelfHeight)
            {
                return new ValidationResult("The book is significantly lower than the height of the shelf");
            }
            return ValidationResult.Success;
        }

        public double ShelfHeight { get; set; }
    }

}
