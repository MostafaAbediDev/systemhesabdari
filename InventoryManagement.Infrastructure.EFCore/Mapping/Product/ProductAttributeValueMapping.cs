using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductArrributeValueAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductAttributeValueMapping : IEntityTypeConfiguration<ProductAttributeValues>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValues> builder)
        {
            builder.ToTable("ProductAttributeValues");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Products).WithMany(x => x.ProductAttributeValues).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.Attributes).WithMany(x => x.ProductAttributeValues).HasForeignKey(x => x.AttributeId);


        }
    }
}
