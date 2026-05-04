using LogManagement.Domain.LogAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogManagement.Infrastructure.EFCore.Mapping
{
    public class LogMapping : IEntityTypeConfiguration<Logs>
    {
        public void Configure(EntityTypeBuilder<Logs> builder)
        {
            builder.ToTable("Logs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Module).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Action).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ActionType).HasMaxLength(20).IsRequired();
            builder.Property(x => x.EntityName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.OldValue).HasMaxLength(10000).IsRequired();
            builder.Property(x => x.NewValue).HasMaxLength(10000).IsRequired();
            builder.Property(x => x.Severity).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
            builder.Property(x => x.ErrorMessage).HasMaxLength(10000).IsRequired();
            builder.Property(x => x.StackTrace).HasMaxLength(10000).IsRequired();
            builder.Property(x => x.IpAddress).HasMaxLength(50).IsRequired();
            builder.Property(x => x.UserAgent).HasMaxLength(300).IsRequired();

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Persons).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
