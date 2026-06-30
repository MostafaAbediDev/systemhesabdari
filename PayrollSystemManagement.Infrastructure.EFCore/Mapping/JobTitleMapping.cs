using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Mapping
{
    public class JobTitleMapping : IEntityTypeConfiguration<JobTitles>
    {
        public void Configure(EntityTypeBuilder<JobTitles> builder)
        {
            builder.ToTable("JobTitles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300).IsRequired(false);

            builder.HasOne(x => x.Departments).WithMany(x => x.JobTitles).HasForeignKey(x => x.DepartmentId);



        }
    }
}
