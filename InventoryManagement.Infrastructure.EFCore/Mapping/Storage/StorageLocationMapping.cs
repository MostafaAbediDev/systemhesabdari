using InventoryManagement.Domain.Inventory.Storage.StorageLocationAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Storage
{
    public class StorageLocationMapping : IEntityTypeConfiguration<StorageLocations>
    {
        public void Configure(EntityTypeBuilder<StorageLocations> builder)
        {
            builder.ToTable("StorageLocations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.HasOne(x => x.Storages).WithMany().HasForeignKey(x => x.StorageId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.InventoryTransactions).WithOne(s => s.Locations).HasForeignKey(s => s.LocationId);

        }
    }
}
