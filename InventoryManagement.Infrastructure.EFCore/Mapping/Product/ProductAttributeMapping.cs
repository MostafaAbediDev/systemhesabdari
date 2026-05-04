using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAttributeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductAttributeMapping : IEntityTypeConfiguration<ProductAttributes>
    {
        public void Configure(EntityTypeBuilder<ProductAttributes> builder)
        {
            builder.ToTable("ProductAttributes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
           
            builder.HasMany(s => s.ProductAttributeValues).WithOne(s => s.Attributes).HasForeignKey(s => s.AttributeId);

        }
    }
}
