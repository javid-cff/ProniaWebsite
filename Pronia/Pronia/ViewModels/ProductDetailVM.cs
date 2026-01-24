using Pronia.Models;

namespace Pronia.ViewModels
{
    public class ProductDetailVM
    {
        public Product Products { get; set; } = new Product();
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
