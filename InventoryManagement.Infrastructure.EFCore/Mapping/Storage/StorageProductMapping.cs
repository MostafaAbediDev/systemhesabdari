using InventoryManagement.Domain.Inventory.Storage.StorageProductAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Storage
{
    public class StorageProductMapping : IEntityTypeConfiguration<StorageProducts>
    {
        public void Configure(EntityTypeBuilder<StorageProducts> builder)
        {
            builder.ToTable("StorageProducts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.Property(x => x.Quantity).HasPrecision(4, 2).IsRequired();
            builder.Property(x => x.ReservedQuantity).HasPrecision(4, 2).IsRequired();

            builder.HasOne(x => x.Storages).WithMany(x => x.StorageProducts).HasForeignKey(x => x.StorageId);
            builder.HasOne(x => x.Products).WithMany(x => x.StorageProducts).HasForeignKey(x => x.ProductId);

        }
    }
}
