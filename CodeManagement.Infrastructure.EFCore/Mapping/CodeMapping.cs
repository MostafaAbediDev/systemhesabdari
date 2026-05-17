using CodeManagement.Domain.CodeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeManagement.Infrastructure.EFCore.Mapping
{
    public class CodeMapping : IEntityTypeConfiguration<Codes>
    {
        public void Configure(EntityTypeBuilder<Codes> builder)
        {
            builder.ToTable("Codes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.OwnerId)
                .IsRequired();

            builder.Property(x => x.OwnerType)
                .IsRequired();

            builder.HasIndex(x => x.Value)
                .IsUnique();

            builder.HasIndex(x => new { x.OwnerId, x.OwnerType })
                .IsUnique();
        }
    }
}
