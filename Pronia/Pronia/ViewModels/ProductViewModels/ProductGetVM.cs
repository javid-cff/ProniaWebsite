using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string MainImagePath { get; set; } = string.Empty;
        public List<string>? AdditionalImagePath { get; set; }
        public string? CategoryName { get; set; }
        public bool isDeleted { get; set; }
    }
}
