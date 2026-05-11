using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.BarcodeAgg
{
    public class Barcodes : EntityBase
    {
        public string Barcode { get; private set; }
        public long ProductId { get; private set; }
        public Products Products { get; private set; }

        public Barcodes(string barcode)
        {
            Barcode = barcode;
        }

        public void Edit(string barcode)
        {
            Barcode = barcode;
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
