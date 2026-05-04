using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using FixedAssetManagement.Domain.FixedAsset.FixedAssetAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoiceAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace FixedAssetManagement.Domain.FixedAsset.AssetDisposalAgg
{
    public class AssetDisposals : EntityBase
    {
        public string Description { get; private set; }
        public int DisposalType { get; private set; }
        public decimal AccumulatedDepreciation { get; private set; }
        public decimal BookValue { get; private set; }
        public decimal SaleAmount { get; private set; }
        public decimal GainOrLoss { get; private set; }
        public DateTime DisposalDate { get; private set; }
        public long FixedAssetId { get; private set; }
        public long BuyerPersonId { get; private set; }
        public long AccountId { get; private set; }
        public long InvoiceId { get; private set; }
        public long AccountingDocumentId { get; private set; }
        public FixedAssets FixedAssets { get; private set; }
        public Persons BuyerPerson { get; private set;}
        public Accounts Accounts { get; private set; }
        public Invoices Invoices { get; private set; }
        public AccountingDocuments AccountingDocuments { get; private set; }

        public AssetDisposals(string description, int disposalType, decimal accumulatedDepreciation, decimal bookValue, 
            decimal saleAmount, decimal gainOrLoss, DateTime disposalDate)
        {
            Description = description;
            DisposalType = disposalType;
            AccumulatedDepreciation = accumulatedDepreciation;
            BookValue = bookValue;
            SaleAmount = saleAmount;
            GainOrLoss = gainOrLoss;
            DisposalDate = disposalDate;
        }

        public void Edit(string description, int disposalType, decimal accumulatedDepreciation, decimal bookValue,
            decimal saleAmount, decimal gainOrLoss, DateTime disposalDate)
        {
            Description = description;
            DisposalType = disposalType;
            AccumulatedDepreciation = accumulatedDepreciation;
            BookValue = bookValue;
            SaleAmount = saleAmount;
            GainOrLoss = gainOrLoss;
            DisposalDate = disposalDate;
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
