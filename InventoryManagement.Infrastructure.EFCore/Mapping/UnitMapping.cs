using InventoryManagement.Domain.Inventory.UnitAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class UnitMapping : IEntityTypeConfiguration<Units>
    {
        public void Configure(EntityTypeBuilder<Units> builder)
        {
            builder.ToTable("Units");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Symbol).HasMaxLength(20).IsRequired();

            builder.HasMany(s => s.ProductPrices).WithOne(s => s.Units).HasForeignKey(s => s.UnitId);

        }
    }
}
