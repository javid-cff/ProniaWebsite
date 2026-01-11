using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category adı boş ola bilməz!")]
        public string Name { get; set; } = null!;

        public List<ProductVM> Products { get; set; } = new List<ProductVM>();
    }
}
