using InventoryManagement.Domain.Inventory.Storage.StorageAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Storage
{
    public class StorageMapping : IEntityTypeConfiguration<Storages>
    {
        public void Configure(EntityTypeBuilder<Storages> builder)
        {
            builder.ToTable("Storages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
            builder.Property(x => x.PostalCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            builder.HasOne(x => x.Provinces).WithMany().HasForeignKey(x => x.ProvinceId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Cities).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.ManagerPersons).WithMany().HasForeignKey(x => x.ManagerPersonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.StorageProducts).WithOne(s => s.Storages).HasForeignKey(s => s.StorageId);
            builder.HasMany(s => s.InventoryTransactions).WithOne(s => s.Storages).HasForeignKey(s => s.StorageId);

        }
    }
}
