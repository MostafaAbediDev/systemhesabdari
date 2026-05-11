using InventoryManagement.Domain.Inventory.Product.ProductSerialAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductSerialMapping : IEntityTypeConfiguration<ProductSerials>
    {
        public void Configure(EntityTypeBuilder<ProductSerials> builder)
        {
            builder.ToTable("ProductSerials");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SerialNumber).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Products).WithMany(x => x.ProductSerials).HasForeignKey(x => x.ProductId);

        }
    }
}
