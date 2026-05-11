using BankManagement.Domain.Bank.BankAgg;
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

            builder.Property(x => x.OwnerName).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Banks).WithMany(x => x.ChequeBooks).HasForeignKey(x => x.BankId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Cheques).WithOne(s => s.ChequeBooks).HasForeignKey(s => s.ChequeBookId);

        }
    }
}
