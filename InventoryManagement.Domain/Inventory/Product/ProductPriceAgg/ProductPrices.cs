using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.PriceTypeAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.UnitAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductPriceAgg
{
    public class ProductPrices : EntityBase
    {
        public decimal Price { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }
        public bool IsDefault { get; private set; }
        public long UnitId { get; private set; }
        public long ProductId { get; private set; }
        public long PriceTypeId { get; private set; }
        public Units Units { get; private set; }
        public Products Products { get; private set; }
        public PriceTypes PriceTypes { get; private set; }

        public ProductPrices(decimal price, DateTime fromDate, DateTime? toDate)
        {
            Price = price;
            FromDate = fromDate;
            ToDate = toDate;
            IsDefault = false;
        }

        public void Edit(decimal price, DateTime fromDate, DateTime? toDate)
        {
            Price = price;
            FromDate = fromDate;
            ToDate = toDate;
            IsDefault = false;
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
