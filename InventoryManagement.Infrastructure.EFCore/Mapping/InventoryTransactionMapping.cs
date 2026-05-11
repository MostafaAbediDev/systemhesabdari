using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class InventoryTransactionMapping : IEntityTypeConfiguration<InventoryTransactions>
    {
        public void Configure(EntityTypeBuilder<InventoryTransactions> builder)
        {
            builder.ToTable("InventoryTransactions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Storages).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.StorageId);
            builder.HasOne(x => x.Locations).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.LocationId);
            builder.HasOne(x => x.Products).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.ProductCreateSeries).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.ProductCreateSeriesId);
            builder.HasOne(x => x.FailureTypes).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.FailureTypeId);

        }
    }
}
