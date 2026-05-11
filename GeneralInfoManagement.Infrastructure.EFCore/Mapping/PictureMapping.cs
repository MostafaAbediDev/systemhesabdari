using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.PictureAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class PictureMapping : IEntityTypeConfiguration<Pictures>
    {
        public void Configure(EntityTypeBuilder<Pictures> builder)
        {
            builder.ToTable("Pictures");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
        }
    }
}
