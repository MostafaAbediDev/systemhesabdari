using BankManagement.Domain.Bank.ReceiptsPaymentAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class ReceiptsPaymentMapping : IEntityTypeConfiguration<ReceiptsPayments>
    {
        public void Configure(EntityTypeBuilder<ReceiptsPayments> builder)
        {
            builder.ToTable("ReceiptsPayments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.FinancialPeriods).WithMany().HasForeignKey(x => x.FinancialPeriodId).OnDelete(DeleteBehavior.NoAction);
            //builder.HasOne(x => x.Persons).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AccountingDocuments).WithMany().HasForeignKey(x => x.AccountingDocumentId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CompanyBankAccounts).WithMany(x => x.ReceiptsPayments).HasForeignKey(x => x.CompanyBankAccountId);
            builder.HasOne(x => x.Funds).WithMany(x => x.ReceiptsPayments).HasForeignKey(x => x.FundId);
            builder.HasOne(x => x.Cheques).WithMany(x => x.ReceiptsPayments).HasForeignKey(x => x.ChequeId);

        }
    }
}
