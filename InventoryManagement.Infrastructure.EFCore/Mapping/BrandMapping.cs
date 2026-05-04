using InventoryManagement.Domain.Inventory.BrandAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class BrandMapping : IEntityTypeConfiguration<Brands>
    {
        public void Configure(EntityTypeBuilder<Brands> builder)
        {
            builder.ToTable("Brands");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

            builder.HasMany(s => s.Products).WithOne(s => s.Brands).HasForeignKey(s => s.BrandId);

        }
    }
}
