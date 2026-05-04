using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class CompanyBankAccountMapping : IEntityTypeConfiguration<CompanyBankAccounts>
    {
        public void Configure(EntityTypeBuilder<CompanyBankAccounts> builder)
        {
            builder.ToTable("CompanyBankAccounts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AccountTitle).HasMaxLength(200).IsRequired();
            builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CardNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Shaba).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Website).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Banks).WithMany(x => x.CompanyBankAccounts).HasForeignKey(x => x.BankId);
            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.ReceiptsPayments).WithOne(s => s.CompanyBankAccounts).HasForeignKey(s => s.CompanyBankAccountId);

        }
    }
}
