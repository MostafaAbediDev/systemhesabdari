using InventoryManagement.Domain.Inventory.Product.PriceTypeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class PriceTypeMapping : IEntityTypeConfiguration<PriceTypes>
    {
        public void Configure(EntityTypeBuilder<PriceTypes> builder)
        {
            builder.ToTable("PriceTypes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();

            builder.HasOne(x => x.Companies).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.ProductPrices).WithOne(s => s.PriceTypes).HasForeignKey(s => s.PriceTypeId);

        }
    }
}
