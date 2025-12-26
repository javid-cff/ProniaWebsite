using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class CardController(ProniaDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var card = await _context.Cards.ToListAsync();
            var cards = await _context.Cards.Include(c => c.Category).ToListAsync();
            return View(cards);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await SendCategoriesViewBag();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Card card)
        {
            if (!ModelState.IsValid) 
            {
                await SendCategoriesViewBag();
                return View(card);
            }


            var isExistsCategory = await _context.Categories.AnyAsync(x => x.Id == card.CategoryId);

            if (!isExistsCategory)
            {
                await SendCategoriesViewBag();

                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil!");
                return View(card);
            }

            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var card = await _context.Cards.FindAsync(id);

            if (card is null)
                return NotFound();

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await SendCategoriesViewBag();

            var card = await _context.Cards.FindAsync(id);

            if (card is not { })
                return NotFound();

            return View(card);
        }

        private async Task SendCategoriesViewBag()
        {
            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;
        }

        [HttpPost]
        public async Task<IActionResult> Update(Card card) 
        {
            if (!ModelState.IsValid)
            {
                await SendCategoriesViewBag();

                return View(card);
            }

            var existCard = await _context.Cards.FindAsync(card.Id);

            if (existCard is null)
                return BadRequest();

            var isExistsCategory = await _context.Categories.AnyAsync(x => x.Id == card.CategoryId);

            if (!isExistsCategory)
            {
                await SendCategoriesViewBag();

                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil!");
                return View(card);
            }

            existCard.Title = card.Title;
            existCard.Description = card.Description;
            existCard.CategoryId = card.CategoryId;
            existCard.ImageUrl = card.ImageUrl;

            _context.Cards.Update(existCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ToggleIsOnline(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound();

            card.isOnline = !card.isOnline;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
