using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralInfoManagement.Infrastructure.EFCore.Mapping
{
    public class BranchArchiveMapping : IEntityTypeConfiguration<BranchArchive>
    {
        public void Configure(EntityTypeBuilder<BranchArchive> builder)
        {
            builder.ToTable("BranchArchive");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
            builder.Property(x => x.File).HasMaxLength(1000).IsRequired();


            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);


        }
    }
}
