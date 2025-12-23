using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.Contexts
{
    public class ProniaDbContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }

        public DbSet<Slider> Sliders { get; set; }
        public ProniaDbContext(DbContextOptions<ProniaDbContext> options) : base(options)
        {
        }
    }
}
