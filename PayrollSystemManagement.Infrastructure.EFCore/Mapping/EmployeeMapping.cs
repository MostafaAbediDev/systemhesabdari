using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Mapping
{
    public class EmployeeMapping : IEntityTypeConfiguration<Employees>
    {
        public void Configure(EntityTypeBuilder<Employees> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EmployeeCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Department).HasMaxLength(100).IsRequired();
            builder.Property(x => x.JobTitle).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ContractType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.InsuranceNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.NationalCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.BankAccountNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.IBAN).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300).IsRequired();

            builder.Property(x => x.BaseSalary).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Persons).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.NoAction);


            builder.HasMany(s => s.Payrolls).WithOne(s => s.Employees).HasForeignKey(s => s.EmployeeId);

        }
    }
}
