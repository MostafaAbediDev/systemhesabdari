using FixedAssetManagement.Domain.FixedAsset.AssetDepreciationAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixedAssetManagement.Infrastructure.EFCore.Mapping
{
    public class AssetDepreciationMapping : IEntityTypeConfiguration<AssetDepreciations>
    {
        public void Configure(EntityTypeBuilder<AssetDepreciations> builder)
        {
            builder.ToTable("AssetDepreciations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DepreciationAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.AccumulatedDepreciation).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.BookValue).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.FixedAssets).WithMany(x => x.AssetDepreciations).HasForeignKey(x => x.FixedAssetId);
            builder.HasOne(x => x.AccountingDocuments).WithMany().HasForeignKey(x => x.AccountingDocumentId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
