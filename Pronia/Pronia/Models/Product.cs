using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Product:BaseEntity
    {
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Image bos ola bilmez!")]
        public string ImagePath { get; set; } = null!;
        public bool isDeleted { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
