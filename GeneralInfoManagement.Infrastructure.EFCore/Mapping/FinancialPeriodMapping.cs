using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class FinancialPeriodMapping : IEntityTypeConfiguration<FinancialPeriods>
    {
        public void Configure(EntityTypeBuilder<FinancialPeriods> builder)
        {
            builder.ToTable("FinancialPeriods");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();

            builder.HasOne(x => x.Branch).WithMany(x => x.FinancialPeriod).HasForeignKey(x => x.BranchId);

        }
    }
}
