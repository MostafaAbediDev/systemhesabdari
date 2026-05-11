using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using GeneralInfoManagement.Domain.BaseInfo.PictureAgg;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using GeneralInfoManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GeneralInfoManagement.Infrastructure.EFCore
{
    public class GeneralInfoSystemContext : DbContext
    {
        public DbSet<Branches> Branches { get; set; }
        public DbSet<BranchArchive> BranchArchive { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<FinancialPeriods> FinancialPeriods { get; set; }
        public DbSet<Pictures> Pictures { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Provinces> Provinces { get; set; }


        public GeneralInfoSystemContext(DbContextOptions<GeneralInfoSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(BranchMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
