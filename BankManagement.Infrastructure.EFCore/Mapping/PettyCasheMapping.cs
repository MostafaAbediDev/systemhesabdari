using BankManagement.Domain.Bank.PettyCashAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class PettyCasheMapping : IEntityTypeConfiguration<PettyCashes>
    {
        public void Configure(EntityTypeBuilder<PettyCashes> builder)
        {
            builder.ToTable("PettyCashes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.Property(x => x.InitialAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.CurrentBalance).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.MaxLimit).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.SettlementAccountId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Persons).WithMany().HasForeignKey(x => x.HolderPersonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Persons).WithMany().HasForeignKey(x => x.ResponsiblePersonId).OnDelete(DeleteBehavior.NoAction);


        }
    }
}
