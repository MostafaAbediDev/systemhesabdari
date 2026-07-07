using AccountManagement.Domain.Account.AccountAgg;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using AccountManagement.Domain.Account.AccountingEntrieAgg;
using AccountManagement.Domain.Account.AccountLinkAgg;
using AccountManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AccountManagement.Infrastructure.EFCore
{
    public class AccountSystemContext : DbContext
    {
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<AccountingDocuments> AccountingDocuments { get; set; }
        public DbSet<AccountingEntries> AccountingEntries { get; set; }
        public DbSet<AccountLinks> AccountLinks { get; set; }
        
        public AccountSystemContext(DbContextOptions<AccountSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<GeneralInfoManagement.Domain.BaseInfo.BranchesAgg.Location>();

            var assembly = typeof(AccountMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
