using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Card
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Image bos ola bilmez!")]
        public string ImageUrl { get; set; } = null!;
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
