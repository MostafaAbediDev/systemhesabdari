using GeneralInfoManagement.Domain.General.ProvinceAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class ProvinceMapping : IEntityTypeConfiguration<Provinces>
    {
        public void Configure(EntityTypeBuilder<Provinces> builder)
        {
            builder.ToTable("Provinces");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();


            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();

            builder.HasMany(s => s.Cities).WithOne(s => s.Provinces).HasForeignKey(s => s.ProvinceId);
            builder.HasMany(s => s.Branches).WithOne(s => s.Provinces).HasForeignKey(s => s.ProvinceId);

        }
    }
}
