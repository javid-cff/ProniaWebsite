using System.ComponentModel.DataAnnotations;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category adı boş ola bilməz!")]
        public string Name { get; set; } = null!;

        public List<ProductGetVM> Products { get; set; } = new();
    }
}
