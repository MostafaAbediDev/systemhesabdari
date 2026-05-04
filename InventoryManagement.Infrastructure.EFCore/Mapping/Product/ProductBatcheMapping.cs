using InventoryManagement.Domain.Inventory.Product.ProductBatcheAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductBatcheMapping : IEntityTypeConfiguration<ProductBatches>
    {
        public void Configure(EntityTypeBuilder<ProductBatches> builder)
        {
            builder.ToTable("ProductBatches");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BatchNumber).HasMaxLength(100).IsRequired();

            builder.Property(x => x.Quantity).HasPrecision(4, 2).IsRequired();

            builder.HasOne(x => x.Products).WithMany(x => x.ProductBatches).HasForeignKey(x => x.ProductId);

        }
    }
}
