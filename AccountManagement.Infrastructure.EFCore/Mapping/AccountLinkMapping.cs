using AccountManagement.Domain.Account.AccountLinkAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManagement.Infrastructure.EFCore.Mapping
{
    public class AccountLinkMapping : IEntityTypeConfiguration<AccountLinks>
    {
        public void Configure(EntityTypeBuilder<AccountLinks> builder)
        {
            builder.ToTable("AccountLinks");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Account).WithMany(x => x.AccountLink).HasForeignKey(x => x.AccountId);
        }
    }
}
