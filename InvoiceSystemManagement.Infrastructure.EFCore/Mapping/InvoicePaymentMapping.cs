using InvoiceSystemManagement.Domain.Invoice.InvoicePaymentAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSystemManagement.Infrastructure.EFCore.Mapping
{
    public class InvoicePaymentMapping : IEntityTypeConfiguration<InvoicePayments>
    {
        public void Configure(EntityTypeBuilder<InvoicePayments> builder)
        {
            builder.ToTable("InvoicePayments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.Invoices).WithMany(x => x.InvoicePayments).HasForeignKey(x => x.InvoiceId);
            builder.HasOne(x => x.ReceiptPayments).WithMany().HasForeignKey(x => x.ReceiptPaymentId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
