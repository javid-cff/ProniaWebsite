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
            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;

            return View(new Card());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Card card)
        {
            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;

            if (!ModelState.IsValid) 
            {
                return View(card);
            }

            var isExistsCategory = await _context.Categories.AnyAsync(x => x.Id == card.CategoryId);

            if (!isExistsCategory) 
            {
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
            var card = await _context.Cards.FindAsync(id);

            if ( card is not { })
                return NotFound();

            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Card card) 
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existCard = await _context.Cards.FindAsync(card.Id);

            if (existCard is null)
                return BadRequest();

            existCard.Title = card.Title;
            existCard.Description = card.Description;
            existCard.ImageUrl = card.ImageUrl;

            _context.Cards.Update(existCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
