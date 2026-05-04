using InventoryManagement.Domain.Inventory.BarcodeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class BarcodeMapping : IEntityTypeConfiguration<Barcodes>
    {
        public void Configure(EntityTypeBuilder<Barcodes> builder)
        {
            builder.ToTable("Barcodes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Barcode).HasMaxLength(500).IsRequired();
           
            builder.HasOne(x => x.Products).WithMany(x => x.Barcodes).HasForeignKey(x => x.ProductId);


        }
    }
}
