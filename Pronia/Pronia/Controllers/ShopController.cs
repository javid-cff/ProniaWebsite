using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.ViewModels;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Controllers
{
    public class ShopController(ProniaDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => !p.isDeleted)
                .Include(p => p.Category)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            ShopVM vm = new ShopVM
            {
                Products = products,
                Categories = categories
            };

            return View(vm);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var cards = await _context.Cards.Take(3).ToListAsync();

            ProductDetailVM vm = new ProductDetailVM
            {
                Products = product,
                Cards = cards
            };

            return View(vm);
        }
    }
}
