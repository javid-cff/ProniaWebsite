using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class ProductController(ProniaDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var product = await _context.Products.ToListAsync();
            var products = await _context.Products.Include(c => c.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await SendCategoriesViewBag();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                await SendCategoriesViewBag();
                return View(product);
            }


            var isExistsCategory = await _context.Categories.AnyAsync(x => x.Id == product.CategoryId);

            if (!isExistsCategory)
            {
                await SendCategoriesViewBag();

                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil!");
                return View(product);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await SendCategoriesViewBag();

            var product = await _context.Products.FindAsync(id);

            if (product is not { })
                return NotFound();

            return View(product);
        }

        private async Task SendCategoriesViewBag()
        {
            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            if (!ModelState.IsValid)
            {
                await SendCategoriesViewBag();

                return View(product);
            }

            var existProduct = await _context.Products.FindAsync(product.Id);

            if (existProduct is null)
                return BadRequest();

            var isExistsCategory = await _context.Categories.AnyAsync(x => x.Id == product.CategoryId);

            if (!isExistsCategory)
            {
                await SendCategoriesViewBag();

                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil!");
                return View(product);
            }

            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.CategoryId = product.CategoryId;
            existProduct.ImagePath = product.ImagePath;
            existProduct.Price = product.Price;
            existProduct.isDeleted = product.isDeleted;

            _context.Products.Update(existProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Toggle(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.isDeleted = !product.isDeleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
