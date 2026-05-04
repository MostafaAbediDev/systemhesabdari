using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageAgg;

namespace InventoryManagement.Domain.Inventory.Storage.StorageProductAgg
{
    public class StorageProducts : EntityBase
    {
        public string Description { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal ReservedQuantity { get; private set; }
        public DateTime LastCalculatedAt { get; private set; }
        public long StorageId { get; private set; }
        public long ProductId { get; private set; }
        public Storages Storages { get; private set; }
        public Products Products { get; private set; }

        public StorageProducts(string description, decimal quantity, decimal reservedQuantity, DateTime lastCalculatedAt)
        {
            Description = description;
            Quantity = quantity;
            ReservedQuantity = reservedQuantity;
            LastCalculatedAt = lastCalculatedAt;
        }

        public void Edit(string description, decimal quantity, decimal reservedQuantity, DateTime lastCalculatedAt)
        {
            Description = description;
            Quantity = quantity;
            ReservedQuantity = reservedQuantity;
            LastCalculatedAt = lastCalculatedAt;
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
