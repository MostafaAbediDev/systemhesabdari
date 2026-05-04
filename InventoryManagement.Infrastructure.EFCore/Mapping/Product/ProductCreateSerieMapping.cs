using InventoryManagement.Domain.Inventory.Product.ProductCreateSerieAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductCreateSerieMapping : IEntityTypeConfiguration<ProductCreateSeries>
    {
        public void Configure(EntityTypeBuilder<ProductCreateSeries> builder)
        {
            builder.ToTable("ProductCreateSeries");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
           
            builder.HasOne(x => x.Products).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.InventoryTransactions).WithOne(s => s.ProductCreateSeries).HasForeignKey(s => s.ProductCreateSeriesId);

        }
    }
}
