using InvoiceSystemManagement.Domain.Invoice.InvoiceAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSystemManagement.Infrastructure.EFCore.Mapping
{
    public class InvoiceMapping : IEntityTypeConfiguration<Invoices>
    {
        public void Configure(EntityTypeBuilder<Invoices> builder)
        {
            builder.ToTable("Invoices");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
            builder.Property(x => x.ReturnReason).HasMaxLength(300).IsRequired();

            builder.HasOne(x => x.Persons).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.FinancialPeriods).WithMany().HasForeignKey(x => x.FinancialPeriodId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Storages).WithMany().HasForeignKey(x => x.StorageId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AccountingDocuments).WithMany().HasForeignKey(x => x.AccountingDoucumentId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.InvoiceItems).WithOne(s => s.Invoices).HasForeignKey(s => s.InvoiceId);
            builder.HasMany(s => s.InvoicePayments).WithOne(s => s.Invoices).HasForeignKey(s => s.InvoiceId);
            
        }
    }
}
