using FixedAssetManagement.Domain.FixedAsset.AssetDisposalAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixedAssetManagement.Infrastructure.EFCore.Mapping
{
    public class AssetDisposalMapping : IEntityTypeConfiguration<AssetDisposals>
    {
        public void Configure(EntityTypeBuilder<AssetDisposals> builder)
        {
            builder.ToTable("AssetDisposals");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            builder.Property(x => x.AccumulatedDepreciation).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.BookValue).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.SaleAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.GainOrLoss).HasPrecision(18, 2).IsRequired();


            builder.HasOne(x => x.BuyerPerson).WithMany().HasForeignKey(x => x.BuyerPersonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Accounts).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.FixedAssets).WithMany().HasForeignKey(x => x.FixedAssetId);
            builder.HasOne(x => x.Invoices).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AccountingDocuments).WithMany().HasForeignKey(x => x.AccountingDocumentId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
