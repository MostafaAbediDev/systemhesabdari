using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class CompanyMapping : IEntityTypeConfiguration<Companies>
    {
        public void Configure(EntityTypeBuilder<Companies> builder)
        {
            builder.ToTable("Companies");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Logo).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.LegalName).HasMaxLength(200).IsRequired();

            builder.HasMany(s => s.Branch).WithOne(s => s.Company).HasForeignKey(s => s.CompanyId);

        }
    }
}
