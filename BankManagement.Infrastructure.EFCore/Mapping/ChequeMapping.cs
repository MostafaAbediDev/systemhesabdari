using BankManagement.Domain.Bank.BankAgg;
using BankManagement.Domain.Bank.ChequeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class ChequeMapping : IEntityTypeConfiguration<Cheques>
    {
        public void Configure(EntityTypeBuilder<Cheques> builder)
        {
            builder.ToTable("Cheques");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChequeNumber).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ChequeBooks).WithMany(x => x.Cheques).HasForeignKey(x => x.ChequeBookId);


        }
    }
}
