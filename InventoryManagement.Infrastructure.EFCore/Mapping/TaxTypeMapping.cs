using InventoryManagement.Domain.Inventory.TaxTypeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class TaxTypeMapping : IEntityTypeConfiguration<TaxTypes>
    {
        public void Configure(EntityTypeBuilder<TaxTypes> builder)
        {
            builder.ToTable("TaxTypes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            builder.Property(x => x.TaxPercent).HasPrecision(5, 2).IsRequired();

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Products).WithOne(s => s.TaxTypes).HasForeignKey(s => s.TaxTypeId);

        }
    }
}
