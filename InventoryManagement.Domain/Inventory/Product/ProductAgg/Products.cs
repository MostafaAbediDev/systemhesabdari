using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using InventoryManagement.Domain.Inventory.BarcodeAgg;
using InventoryManagement.Domain.Inventory.BrandAgg;
using InventoryManagement.Domain.Inventory.CategoryAgg;
using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;
using InventoryManagement.Domain.Inventory.Product.ProductArrributeValueAgg;
using InventoryManagement.Domain.Inventory.Product.ProductBatcheAgg;
using InventoryManagement.Domain.Inventory.Product.ProductCreateSerieAgg;
using InventoryManagement.Domain.Inventory.Product.ProductPriceAgg;
using InventoryManagement.Domain.Inventory.Product.ProductSerialAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageProductAgg;
using InventoryManagement.Domain.Inventory.TaxTypeAgg;
using InventoryManagement.Domain.Inventory.UnitAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductAgg
{
    public class Products : EntityBase
    {
        public string Code { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public decimal? ConversionFactor { get; private set; }
        public bool IsService { get; private set; }
        public bool IsStockable { get; private set; }
        public bool HasSerial { get; private set; }
        public bool HasExpirationDate { get; private set; }
        public long CompanyId { get; private set; }
        public long UnitId { get; private set; }
        public long? UnitDetailsId { get; private set; }
        public long CategoryId { get; private set; }
        public long BrandId { get; private set; }
        public long TaxTypeId { get; private set; }
        public TaxTypes TaxTypes { get; private set; }
        public Brands Brands { get; private set; }
        public Categories Categories { get; private set; }
        public Units UnitDetails { get; private set; }
        public Units Units { get; private set; }
        public Companies Companies { get; private set; }
        public List<Barcodes> Barcodes { get; private set; }
        public List<ProductSerials> ProductSerials { get; private set; }
        public List<ProductBatches> ProductBatches { get; private set; }
        public List<ProductAttributeValues> ProductAttributeValues { get; private set; }
        public List<ProductPrices> ProductPrices { get; private set; }
        public List<StorageProducts> StorageProducts { get; private set; }
        public List<InventoryTransactions> InventoryTransactions { get; private set; }

        protected Products()
        {
            Barcodes = new List<Barcodes>();
            ProductSerials = new List<ProductSerials>();
            ProductBatches = new List<ProductBatches>();
            ProductAttributeValues = new List<ProductAttributeValues>();
            ProductPrices = new List<ProductPrices>();
            StorageProducts = new List<StorageProducts>();
            InventoryTransactions = new List<InventoryTransactions>();
        }

        public Products(string code, string title, string description, decimal? conversionFactor)
        {
            Code = code;
            Title = title;
            Description = description;
            ConversionFactor = conversionFactor;
            IsService = false;
            IsStockable = false;
            HasSerial = false;
            HasExpirationDate = false;
        }
        public void Edit(string code, string title, string description, decimal? conversionFactor)
        {
            Code = code;
            Title = title;
            Description = description;
            ConversionFactor = conversionFactor;
            IsService = false;
            IsStockable = false;
            HasSerial = false;
            HasExpirationDate = false;
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
