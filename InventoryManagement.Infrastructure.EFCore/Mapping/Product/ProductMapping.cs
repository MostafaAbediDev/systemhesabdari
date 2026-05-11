using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping.Product
{
    public class ProductMapping : IEntityTypeConfiguration<Products>
    {
        public void Configure(EntityTypeBuilder<Products> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            builder.Property(x => x.ConversionFactor).HasPrecision(4, 2).IsRequired();


            builder.HasOne(x => x.TaxTypes).WithMany(x => x.Products).HasForeignKey(x => x.TaxTypeId);
            builder.HasOne(x => x.Brands).WithMany(x => x.Products).HasForeignKey(x => x.BrandId);
            builder.HasOne(x => x.Categories).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
            builder.HasOne(x => x.Units).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.UnitDetails).WithMany().HasForeignKey(x => x.UnitDetailsId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Companies).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Barcodes).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);
            builder.HasMany(s => s.ProductSerials).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);
            builder.HasMany(s => s.ProductBatches).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);
            builder.HasMany(s => s.ProductAttributeValues).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);
            builder.HasMany(s => s.ProductPrices).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);
            builder.HasMany(s => s.StorageProducts).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);
            builder.HasMany(s => s.InventoryTransactions).WithOne(s => s.Products).HasForeignKey(s => s.ProductId);

        }
    }
}
