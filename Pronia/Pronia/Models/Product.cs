using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string MainImagePath { get; set; } = null!;
        public string? AdditionalImagePaths { get; set; }
        public bool isDeleted { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
