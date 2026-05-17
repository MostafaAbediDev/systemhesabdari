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

            builder.Property(x => x.OwnerId)
                   .IsRequired();

            builder.Property(x => x.OwnerType)
                   .IsRequired();

            builder.Property(x => x.Url)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(x => x.IsMain)
                   .IsRequired();

            builder.HasIndex(x => new { x.OwnerId, x.OwnerType });
        }
    }

}
