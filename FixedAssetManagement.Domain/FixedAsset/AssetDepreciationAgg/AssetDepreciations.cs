using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using FixedAssetManagement.Domain.FixedAsset.FixedAssetAgg;

namespace FixedAssetManagement.Domain.FixedAsset.AssetDepreciationAgg
{
    public class AssetDepreciations : EntityBase
    {
        public int PeriodYear { get; private set; }
        public int PeriodMonth { get; private set; }
        public decimal DepreciationAmount { get; private set; }
        public decimal AccumulatedDepreciation { get; private set; }
        public decimal BookValue { get; private set; }
        public long FixedAssetId { get; private set; }
        public long AccountingDocumentId { get; private set; }
        public FixedAssets FixedAssets { get; private set; }
        public AccountingDocuments AccountingDocuments { get; private set; }

        public AssetDepreciations(int periodYear, int periodMonth, decimal depreciationAmount, decimal accumulatedDepreciation, decimal bookValue)
        {
            PeriodYear = periodYear;
            PeriodMonth = periodMonth;
            DepreciationAmount = depreciationAmount;
            AccumulatedDepreciation = accumulatedDepreciation;
            BookValue = bookValue;
        }

        public void Edit(int periodYear, int periodMonth, decimal depreciationAmount, decimal accumulatedDepreciation, decimal bookValue)
        {
            PeriodYear = periodYear;
            PeriodMonth = periodMonth;
            DepreciationAmount = depreciationAmount;
            AccumulatedDepreciation = accumulatedDepreciation;
            BookValue = bookValue;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
