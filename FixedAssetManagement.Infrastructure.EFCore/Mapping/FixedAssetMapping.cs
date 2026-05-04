using FixedAssetManagement.Domain.FixedAsset.FixedAssetAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixedAssetManagement.Infrastructure.EFCore.Mapping
{
    public class FixedAssetMapping : IEntityTypeConfiguration<FixedAssets>
    {
        public void Configure(EntityTypeBuilder<FixedAssets> builder)
        {
            builder.ToTable("FixedAssets");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
            builder.Property(x => x.AssetCode).HasMaxLength(50).IsRequired();

            builder.Property(x => x.PutchasePrice).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.SalvageValue).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountAssetId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountDepreciationId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountExpenseId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.AssetDepreciations).WithOne(s => s.FixedAssets).HasForeignKey(s => s.FixedAssetId);
            builder.HasMany(s => s.Disposals).WithOne(s => s.FixedAssets).HasForeignKey(s => s.FixedAssetId);

        }
    }
}
