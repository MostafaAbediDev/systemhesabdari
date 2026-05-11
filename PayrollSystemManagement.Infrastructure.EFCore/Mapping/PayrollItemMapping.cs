using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Mapping
{
    public class PayrollItemMapping : IEntityTypeConfiguration<PayrollItems>
    {
        public void Configure(EntityTypeBuilder<PayrollItems> builder)
        {
            builder.ToTable("PayrollItems");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
          
            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.PayrollDetails).WithOne(s => s.PayrollItems).HasForeignKey(s => s.PayrollItemId);

        }
    }
}
