using AccountManagement.Domain.Account.AccountingDocumentAgg;
using AccountManagement.Domain.Account.AccountingEntrieAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManagement.Infrastructure.EFCore.Mapping
{
    public class AccountingEntrieMapping : IEntityTypeConfiguration<AccountingEntries>
    {
        public void Configure(EntityTypeBuilder<AccountingEntries> builder)
        {
            builder.ToTable("AccountingEntries");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Debit).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Credit).HasPrecision(18, 2).IsRequired();

            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            //builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AccountingDocument).WithMany().HasForeignKey(x => x.AccountingDocumentId);
            builder.HasOne(x => x.Account).WithMany(x => x.AccountingEntrie).HasForeignKey(x => x.AccountId);


        }
    }
}
