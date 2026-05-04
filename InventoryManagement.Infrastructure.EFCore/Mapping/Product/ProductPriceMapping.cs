using InventoryManagement.Domain.Inventory.Product.ProductPriceAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductPriceMapping : IEntityTypeConfiguration<ProductPrices>
    {
        public void Configure(EntityTypeBuilder<ProductPrices> builder)
        {
            builder.ToTable("ProductPrices");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.Units).WithMany(x => x.ProductPrices).HasForeignKey(x => x.UnitId);
            builder.HasOne(x => x.Products).WithMany(x => x.ProductPrices).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.PriceTypes).WithMany(x => x.ProductPrices).HasForeignKey(x => x.PriceTypeId);
           
        }
    }
}
