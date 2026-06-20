using InvoiceSystemManagement.Domain.Invoice.ReceiptPaymentInvoiceAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class ReceiptPaymentInvoiceMapping : IEntityTypeConfiguration<ReceiptPaymentInvoices>
    {
        public void Configure(EntityTypeBuilder<ReceiptPaymentInvoices> builder)
        {
            builder.ToTable("ReceiptPaymentInvoices");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.ReceiptsPayments).WithMany().HasForeignKey(x => x.ReceiptPaymentId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Invoices).WithMany(x => x.ReceiptPaymentInvoices).HasForeignKey(x => x.InvoiceId);

        }
    }
}
