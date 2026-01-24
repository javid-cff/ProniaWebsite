using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;
using Pronia.ViewModels;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(ProniaDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Select(p => new ProductGetVM
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                MainImagePath = p.MainImagePath,
                CategoryName = p.Category.Name,
                isDeleted = p.isDeleted,
                AdditionalImagePath = new List<string> { p.AdditionalImagePaths }
            }).ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }

            string mainFileName = Guid.NewGuid().ToString() + "_" + vm.MainImageFile.FileName;
            string mainPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", mainFileName);
            using (FileStream stream = new FileStream(mainPath, FileMode.Create))
            {
                await vm.MainImageFile.CopyToAsync(stream);
            }

            Product product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                MainImagePath = mainFileName,
                isDeleted = false
            };

            if (vm.AdditionalImageFiles != null)
            {
                List<string> addFiles = new List<string>();
                foreach (var file in vm.AdditionalImageFiles)
                {
                    string addName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string addPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", addName);
                    using (FileStream stream = new FileStream(addPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    addFiles.Add(addName);
                }
                product.AdditionalImagePaths = string.Join(",", addFiles);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.MainImage = product.MainImagePath;
            ViewBag.AdditionalImages = product.AdditionalImagePaths?.Split(',').ToList();

            ProductUpdateVM vm = new ProductUpdateVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM vm)
        {
            var existed = await _context.Products.FindAsync(vm.Id);
            if (existed == null) return NotFound();

            if (vm.MainImageFile == null)
            {
                ModelState.Remove("MainImageFile");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();

                ViewBag.MainImagePath = existed.MainImagePath;
                ViewBag.AdditionalImagePaths = existed.AdditionalImagePaths?.Split(',').ToList();
                return View(vm);
            }

            existed.Name = vm.Name;
            existed.Description = vm.Description;
            existed.Price = vm.Price;
            existed.CategoryId = vm.CategoryId;

            if (vm.MainImageFile != null)
            {
                string oldPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", existed.MainImagePath);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

                string newFileName = Guid.NewGuid().ToString() + "_" + vm.MainImageFile.FileName;
                string newPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", newFileName);

                using (FileStream stream = new FileStream(newPath, FileMode.Create))
                {
                    await vm.MainImageFile.CopyToAsync(stream);
                }
                existed.MainImagePath = newFileName;
            }

            if (vm.AdditionalImageFiles != null && vm.AdditionalImageFiles.Count > 0)
            {
                if (!string.IsNullOrEmpty(existed.AdditionalImagePaths))
                {
                    foreach (var imgName in existed.AdditionalImagePaths.Split(','))
                    {
                        string path = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", imgName);
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }
                }

                List<string> newAddFilenames = new List<string>();
                foreach (var file in vm.AdditionalImageFiles)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string path = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    newAddFilenames.Add(fileName);
                }
                existed.AdditionalImagePaths = string.Join(",", newAddFilenames);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (!string.IsNullOrEmpty(product.MainImagePath))
            {
                string mainPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", product.MainImagePath);
                if (System.IO.File.Exists(mainPath)) System.IO.File.Delete(mainPath);
            }

            if (!string.IsNullOrEmpty(product.AdditionalImagePaths))
            {
                foreach (var imgName in product.AdditionalImagePaths.Split(','))
                {
                    string addPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", imgName);
                    if (System.IO.File.Exists(addPath)) System.IO.File.Delete(addPath);
                }
            }

            _context.Products.Remove(product);
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
    }
}