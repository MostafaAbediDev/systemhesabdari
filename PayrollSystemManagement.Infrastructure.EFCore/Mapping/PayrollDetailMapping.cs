using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Mapping
{
    public class PayrollDetailMapping : IEntityTypeConfiguration<PayrollDetails>
    {
        public void Configure(EntityTypeBuilder<PayrollDetails> builder)
        {
            builder.ToTable("PayrollDetails");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Rate).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.Payrolls).WithMany(x => x.PayrollDetails).HasForeignKey(x => x.PayrollId);
            builder.HasOne(x => x.PayrollItems).WithMany().HasForeignKey(x => x.PayrollItemId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
