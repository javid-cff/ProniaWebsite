using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;
using Pronia.ViewModels.BasketViewModels;

namespace Pronia.Controllers
{
    public class BasketController(ProniaDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
