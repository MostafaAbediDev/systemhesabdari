using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;
using PayrollSystemManagement.Infrastructure.EFCore.Mapping;

namespace PayrollSystemManagement.Infrastructure.EFCore
{
    public class PayrollFakeDataContext : DbContext
    {
        public DbSet<Payrolls> Payrolls { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<PayrollDetails> PayrollDetails { get; set; }
        public DbSet<PayrollItems> PayrollItems { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<JobTitles> JobTitles { get; set; }


        public PayrollFakeDataContext(DbContextOptions<PayrollFakeDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<GeneralInfoManagement.Domain.BaseInfo.BranchesAgg.Location>();

            var assembly = typeof(PayrollMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
