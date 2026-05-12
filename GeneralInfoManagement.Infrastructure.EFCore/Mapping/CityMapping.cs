using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.General.CityAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class CityMapping : IEntityTypeConfiguration<Cities>
    {
        public void Configure(EntityTypeBuilder<Cities> builder)
        {
            builder.ToTable("Cities");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();


            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();

            builder.HasOne(x => x.Provinces).WithMany(x => x.Cities).HasForeignKey(x => x.ProvinceId);

        }
    }
}
