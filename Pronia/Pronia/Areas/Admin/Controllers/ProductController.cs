using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class ProductController(ProniaDbContext _context, IWebHostEnvironment _env) : Controller
    {
        private string ImagePath => Path.Combine(_env.WebRootPath, "assets/images/website-images");

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(x => x.Category)
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            await LoadCategories();

            if (!ModelState.IsValid)
                return View(vm);

            if (!await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Belə category mövcud deyil!");
                return View(vm);
            }

            string mainFileName = Guid.NewGuid() + Path.GetExtension(vm.MainImageFile.FileName);
            string mainFilePath = Path.Combine(ImagePath, mainFileName);

            using (FileStream fs = new(mainFilePath, FileMode.Create))
                await vm.MainImageFile.CopyToAsync(fs);

            List<string> additionalFileNames = new();

            if (vm.AdditionalImageFiles != null)
            {
                foreach (var file in vm.AdditionalImageFiles)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(ImagePath, fileName);

                    using FileStream fs = new(filePath, FileMode.Create);
                    await file.CopyToAsync(fs);

                    additionalFileNames.Add(fileName);
                }
            }

            Product product = new()
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                isDeleted = vm.isDeleted,
                MainImagePath = mainFileName,
                AdditionalImagePaths = additionalFileNames.Any()
                    ? string.Join(",", additionalFileNames)
                    : null
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await LoadCategories();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ProductVM vm = new()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                isDeleted = product.isDeleted
            };

            ViewBag.MainImage = product.MainImagePath;
            ViewBag.AdditionalImages = product.AdditionalImagePaths?.Split(',').ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductVM vm)
        {
            await LoadCategories();

            if (!ModelState.IsValid)
                return View(vm);

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (!await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Belə category mövcud deyil!");
                return View(vm);
            }

            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.CategoryId = vm.CategoryId;
            product.isDeleted = vm.isDeleted;

            if (vm.MainImageFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(vm.MainImageFile.FileName);
                string filePath = Path.Combine(ImagePath, fileName);

                using FileStream fs = new(filePath, FileMode.Create);
                await vm.MainImageFile.CopyToAsync(fs);

                product.MainImagePath = fileName;
            }

            if (vm.AdditionalImageFiles != null && vm.AdditionalImageFiles.Any())
            {
                List<string> images = product.AdditionalImagePaths?
                    .Split(',')
                    .ToList() ?? new();

                foreach (var file in vm.AdditionalImageFiles)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(ImagePath, fileName);

                    using FileStream fs = new(filePath, FileMode.Create);
                    await file.CopyToAsync(fs);

                    images.Add(fileName);
                }

                product.AdditionalImagePaths = string.Join(",", images);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Toggle(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.isDeleted = !product.isDeleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
        }
    }
}
