using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
