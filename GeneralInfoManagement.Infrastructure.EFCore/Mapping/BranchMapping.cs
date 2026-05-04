using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class BranchMapping : IEntityTypeConfiguration<Branches>
    {
        public void Configure(EntityTypeBuilder<Branches> builder)
        {
            builder.ToTable("Branches");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Lat_Log).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
            builder.Property(x => x.PostCode).HasMaxLength(10).IsRequired();

            builder.HasOne(x => x.Company).WithMany(x => x.Branch).HasForeignKey(x => x.CompanyId);

            builder.HasMany(s => s.BranchArchive).WithOne(s => s.Branch).HasForeignKey(s => s.BranchId);
            builder.HasMany(s => s.FinancialPeriod).WithOne(s => s.Branch).HasForeignKey(s => s.BranchId);

        }
    }
}
