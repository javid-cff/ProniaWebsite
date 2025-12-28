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
            return View(card);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Card card)
        {
            if (!ModelState.IsValid)
            {
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

            if (card is not { })
                return NotFound();

            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Card card)
        {
            if (!ModelState.IsValid)
            {
                return View(card);
            }

            var existCard = await _context.Cards.FindAsync(card.Id);

            if (existCard is null)
                return BadRequest();

            existCard.Title = card.Title;
            existCard.Description = card.Description;
            existCard.ImageUrl = card.ImageUrl;
            existCard.isOnline = card.isOnline;

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
