using BankManagement.Domain.Bank.BankAgg;
using BankManagement.Domain.Bank.BankBrancheAgg;
using BankManagement.Domain.Bank.BankTypeAgg;
using BankManagement.Domain.Bank.ChequeAgg;
using BankManagement.Domain.Bank.ChequeBookAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using BankManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Infrastructure.EFCore
{
    public class BankFakeDataContext : DbContext
    {
        public DbSet<Banks> Banks { get; set; }
        public DbSet<BankBranches> BankBranches { get; set; }
        public DbSet<BankTypes> BankTypes  { get; set; }
        public DbSet<ChequeBooks> ChequeBooks { get; set; }
        public DbSet<Cheques> Cheques { get; set; }
        public DbSet<CompanyBankAccounts> CompanyBankAccounts { get; set; }
       
        public BankFakeDataContext(DbContextOptions<BankFakeDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<GeneralInfoManagement.Domain.BaseInfo.BranchesAgg.Location>();

            var assembly = typeof(BankMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
