using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pronia.Models;

namespace Pronia.Configurations
{
    public class ProductConfiguration:IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.ImagePath).IsRequired();
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.ToTable(p => p.HasCheckConstraint("CK_Product_Price", "[Price] > 0"));

            builder.HasOne(x => x.Category).WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
