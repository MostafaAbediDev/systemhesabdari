using BankManagement.Domain.Bank.ChequeBookAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class ChequeBookMapping : IEntityTypeConfiguration<ChequeBooks>
    {
        public void Configure(EntityTypeBuilder<ChequeBooks> builder)
        {
            builder.ToTable("ChequeBooks");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.CompanyBankAccount).WithMany()
                .HasForeignKey(x => x.CompanyBankAccountId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Cheques).WithOne(s => s.ChequeBooks).HasForeignKey(s => s.ChequeBookId);

        }
    }
}
