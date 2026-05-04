using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using FixedAssetManagement.Domain.FixedAsset.AssetDepreciationAgg;
using FixedAssetManagement.Domain.FixedAsset.AssetDisposalAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace FixedAssetManagement.Domain.FixedAsset.FixedAssetAgg
{
    public class FixedAssets : EntityBase
    {
        public string AssetCode { get; private set; }
        public string Title { get; private set; }
        public string Category { get; private set; }
        public DateTime PutchaseDate { get; private set; }
        public decimal PutchasePrice { get; private set; }
        public decimal SalvageValue { get; private set; }
        public int UsefulLife { get; private set; }
        public int DepreciationMethod { get; private set; }
        public long BranchId { get; private set; }
        public long AccountAssetId { get; private set; }
        public long AccountDepreciationId { get; private set; }
        public long AccountExpenseId { get; private set; }
        public Branches Branches { get; private set; }
        public Accounts Accounts { get; private set; }
        public List<AssetDepreciations> AssetDepreciations { get; private set; }
        public List<AssetDisposals> Disposals { get; private set; }

        protected FixedAssets()
        {
            AssetDepreciations = new List<AssetDepreciations>();
            Disposals = new List<AssetDisposals>();
        }

        public FixedAssets(string assetCode, string title, string category, DateTime putchaseDate, decimal putchasePrice, decimal salvageValue, int usefulLife, int depreciationMethod)
        {
            AssetCode = assetCode;
            Title = title;
            Category = category;
            PutchaseDate = putchaseDate;
            PutchasePrice = putchasePrice;
            SalvageValue = salvageValue;
            UsefulLife = usefulLife;
            DepreciationMethod = depreciationMethod;
        }
        public void Edit(string assetCode, string title, string category, DateTime putchaseDate, 
            decimal putchasePrice, decimal salvageValue, int usefulLife, int depreciationMethod)
        {
            AssetCode = assetCode;
            Title = title;
            Category = category;
            PutchaseDate = putchaseDate;
            PutchasePrice = putchasePrice;
            SalvageValue = salvageValue;
            UsefulLife = usefulLife;
            DepreciationMethod = depreciationMethod;
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
