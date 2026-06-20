using FinancialManagement.Domain.FundAgg;
using FinancialManagement.Domain.PettyCashAgg;
using FinancialManagement.Domain.ReceiptsPaymentAgg;
using FinancialManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagement.Infrastructure.EFCore
{
    public class FinancialFakeDataContext : DbContext
    {
        public DbSet<Funds> Funds { get; set; }
        public DbSet<PettyCashes> PettyCashes { get; set; }
        public DbSet<ReceiptsPayments> ReceiptsPayments { get; set; }

        public FinancialFakeDataContext(DbContextOptions<FinancialFakeDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<GeneralInfoManagement.Domain.BaseInfo.BranchesAgg.Location>();

            var assembly = typeof(ReceiptsPaymentMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
