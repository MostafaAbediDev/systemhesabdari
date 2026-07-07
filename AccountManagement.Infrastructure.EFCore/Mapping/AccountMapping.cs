using AccountManagement.Domain.Account.AccountAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManagement.Infrastructure.EFCore.Mapping
{
    public class AccountMapping : IEntityTypeConfiguration<Accounts>
    {
        public void Configure(EntityTypeBuilder<Accounts> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired(false);

            builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(s => s.Children).WithOne(s => s.Parent).HasForeignKey(s => s.ParentId);
            builder.HasMany(s => s.AccountLink).WithOne(s => s.Account).HasForeignKey(s => s.AccountId);
            builder.HasMany(s => s.AccountingEntrie).WithOne(s => s.Account).HasForeignKey(s => s.AccountId);

        }
    }
}
