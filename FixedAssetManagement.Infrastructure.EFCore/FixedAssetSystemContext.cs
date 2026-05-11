using FixedAssetManagement.Domain.FixedAsset.AssetDepreciationAgg;
using FixedAssetManagement.Domain.FixedAsset.AssetDisposalAgg;
using FixedAssetManagement.Domain.FixedAsset.FixedAssetAgg;
using FixedAssetManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FixedAssetManagement.Infrastructure.EFCore
{
    public class FixedAssetSystemContext : DbContext
    {
        public DbSet<FixedAssets> FixedAssets { get; set; }
        public DbSet<AssetDepreciations> AssetDepreciations { get; set; }
        public DbSet<AssetDisposals> AssetDisposals { get; set; }
        

        public FixedAssetSystemContext(DbContextOptions<FixedAssetSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(FixedAssetMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
