using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonBankAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonBankMapping : IEntityTypeConfiguration<PersonBanks>
    {
        public void Configure(EntityTypeBuilder<PersonBanks> builder)
        {
            builder.ToTable("PersonBanks");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CardNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Shaba).HasMaxLength(50).IsRequired();

            builder.HasOne(x => x.Persons).WithMany(x => x.PersonBanks).HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.BankBranches).WithMany().HasForeignKey(x => x.BankBranchId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
