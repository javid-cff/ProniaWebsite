using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product adı boş ola bilməz!")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Qiymət daxil edilməlidir!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Qiymət 0-dan böyük olmalıdır!")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Şəkil seçilməlidir!")]
        public IFormFile MainImageFile { get; set; } = null!;
        public List<IFormFile>? AdditionalImageFiles { get; set; }

        public bool isDeleted { get; set; }

        [Required(ErrorMessage = "Category seçilməlidir!")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}
