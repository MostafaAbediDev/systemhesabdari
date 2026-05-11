using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductSerialAgg
{
    public class ProductSerials : EntityBase
    {
        public string SerialNumber { get; private set; }
        public bool IsSold { get; private set; }
        public long ProductId { get; private set; }
        public Products Products { get; private set; }

        public ProductSerials(string serialNumber)
        {
            SerialNumber = serialNumber;
            IsSold = false;
        }

        public void Edit(string serialNumber)
        {
            SerialNumber = serialNumber;
            IsSold = false;
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
