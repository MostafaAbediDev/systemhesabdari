using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;
using PayrollSystemManagement.Infrastructure.EFCore.Mapping;
using System.Collections.Generic;

namespace PayrollSystemManagement.Infrastructure.EFCore
{
    public class PayrollSystemContext : DbContext
    {
        public DbSet<Payrolls> Payrolls { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<PayrollDetails> PayrollDetails { get; set; }
        public DbSet<PayrollItems> PayrollItems { get; set; }


        public PayrollSystemContext(DbContextOptions<PayrollSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(PayrollMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
