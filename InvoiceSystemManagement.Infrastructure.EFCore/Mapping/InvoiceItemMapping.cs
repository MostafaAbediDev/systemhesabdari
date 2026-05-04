using InvoiceSystemManagement.Domain.Invoice.InvoiceItemAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSystemManagement.Infrastructure.EFCore.Mapping
{
    public class InvoiceItemMapping : IEntityTypeConfiguration<InvoiceItems>
    {
        public void Configure(EntityTypeBuilder<InvoiceItems> builder)
        {
            builder.ToTable("InvoiceItems");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.Property(x => x.Quantity).HasPrecision(18, 3).IsRequired();
            builder.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Discount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Tax).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.Invoices).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.InvoiceId);
            builder.HasOne(x => x.Products).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.ProductBatches).WithMany().HasForeignKey(x => x.BatchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Units).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
