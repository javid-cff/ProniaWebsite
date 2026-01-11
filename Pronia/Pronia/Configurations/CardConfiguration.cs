using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pronia.Models;

namespace Pronia.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(c => c.ImageUrl)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.isOnline)
                .HasDefaultValue(false);

            builder.ToTable("Cards");
        }
    }
}
