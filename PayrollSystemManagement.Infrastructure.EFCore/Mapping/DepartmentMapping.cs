using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Mapping
{
    public class DepartmentMapping : IEntityTypeConfiguration<Departments>
    {
        public void Configure(EntityTypeBuilder<Departments> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300).IsRequired(false);


            builder.HasMany(s => s.JobTitles).WithOne(s => s.Departments).HasForeignKey(s => s.DepartmentId);

        }
    }
}
