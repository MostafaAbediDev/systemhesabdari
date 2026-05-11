using BankManagement.Domain.Bank.BankAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class BankMapping : IEntityTypeConfiguration<Banks>
    {
        public void Configure(EntityTypeBuilder<Banks> builder)
        {
            builder.ToTable("Banks");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Country).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.HasOne(x => x.Pictures).WithMany().HasForeignKey(x => x.PictureId).OnDelete(DeleteBehavior.NoAction);
<<<<<<< HEAD
=======
            builder.HasOne(x => x.BankTypes).WithMany(x => x.Banks).HasForeignKey(x => x.BankTypeId);
>>>>>>> master

            builder.HasMany(s => s.CompanyBankAccounts).WithOne(s => s.Banks).HasForeignKey(s => s.BankId);

            builder.HasMany(s => s.ChequeBooks).WithOne(s => s.Banks).HasForeignKey(s => s.BankId);

        }
    }
}
