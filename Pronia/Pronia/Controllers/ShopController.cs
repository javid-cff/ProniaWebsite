using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.ViewModels;

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
    }
}
