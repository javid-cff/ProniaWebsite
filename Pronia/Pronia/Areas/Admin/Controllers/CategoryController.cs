using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController(ProniaDbContext _context) : Controller
    {
        /*public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();

            return View(categories);
        }*/
    }
}
