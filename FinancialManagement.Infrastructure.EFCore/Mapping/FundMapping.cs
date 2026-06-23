using FinancialManagement.Domain.FundAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialManagement.Infrastructure.EFCore.Mapping
{
    public class FundMapping : IEntityTypeConfiguration<Funds>
    {
        public void Configure(EntityTypeBuilder<Funds> builder)
        {
            builder.ToTable("Funds");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
