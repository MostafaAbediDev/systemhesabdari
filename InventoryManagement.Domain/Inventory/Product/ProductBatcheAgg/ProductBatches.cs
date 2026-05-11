using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductBatcheAgg
{
    public class ProductBatches : EntityBase
    {
        public string BatchNumber { get; private set; }
        public decimal Quantity { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public long ProductId { get; private set; }
        public Products Products { get; private set; }
        

        public ProductBatches(string batchNumber, decimal quantity, DateTime expirationDate)
        {
            BatchNumber = batchNumber;
            Quantity = quantity;
            ExpirationDate = expirationDate;
        }

        public void Edit(string batchNumber, decimal quantity, DateTime expirationDate)
        {
            BatchNumber = batchNumber;
            Quantity = quantity;
            ExpirationDate = expirationDate;
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
