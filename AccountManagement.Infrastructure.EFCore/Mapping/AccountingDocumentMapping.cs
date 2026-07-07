using AccountManagement.Domain.Account.AccountingDocumentAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManagement.Infrastructure.EFCore.Mapping
{
    public class AccountingDocumentMapping : IEntityTypeConfiguration<AccountingDocuments>
    {
        public void Configure(EntityTypeBuilder<AccountingDocuments> builder)
        {
            builder.ToTable("AccountingDocuments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.FinancialPeriod).WithMany().HasForeignKey(x => x.FinancialPeriodId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.AccountingEntrie).WithOne(s => s.AccountingDocument).HasForeignKey(s => s.AccountingDocumentId);

        }
    }
}
