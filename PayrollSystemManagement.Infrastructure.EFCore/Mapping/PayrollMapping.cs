using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Mapping
{
    public class PayrollMapping : IEntityTypeConfiguration<Payrolls>
    {
        public void Configure(EntityTypeBuilder<Payrolls> builder)
        {
            builder.ToTable("Payrolls");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalBenefits).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.TotalDeduction).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.NetPay).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.Employees).WithMany(x => x.Payrolls).HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.FinancialPeriods).WithMany().HasForeignKey(x => x.FinancialPeriodId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AccountingDocuments).WithMany().HasForeignKey(x => x.AccountingDocumentId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.PayrollDetails).WithOne(s => s.Payrolls).HasForeignKey(s => s.PayrollId);

        }
    }
}
