using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class CardVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Şəkil seçilməlidir!")]
        public IFormFile ImageFile { get; set; } = null!;

        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool isOnline { get; set; }
    }
}
