using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.Contexts
{
    public class ProniaDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public ProniaDbContext(DbContextOptions<ProniaDbContext> options) : base(options)
        {
        }
    }
}
