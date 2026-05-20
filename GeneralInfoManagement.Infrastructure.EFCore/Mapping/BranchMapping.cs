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

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.NationalId)
                .HasMaxLength(20);

            builder.Property(x => x.EconomicCode)
                .HasMaxLength(20);

            builder.Property(x => x.RegisterNumber)
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.PostCode)
                .HasMaxLength(10);

            builder.Property(x => x.IsMain);

            builder.OwnsOne(x => x.Location, m =>
            {
                m.Property(p => p.Latitude)
                    .HasColumnName("Latitude")
                    .HasColumnType("decimal(9,6)");

                m.Property(p => p.Longitude)
                    .HasColumnName("Longitude")
                    .HasColumnType("decimal(9,6)");
            });

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Branch)
                .HasForeignKey(x => x.CompanyId);

            builder.HasMany(x => x.BranchArchive)
                .WithOne(x => x.Branch)
                .HasForeignKey(x => x.BranchId);

            builder.HasMany(x => x.FinancialPeriod)
                .WithOne(x => x.Branch)
                .HasForeignKey(x => x.BranchId);
        }
    }
}
