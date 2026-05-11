using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAttributeAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductArrributeValueAgg
{
    public class ProductAttributeValues : EntityBase
    {
        public string Value { get; private set; }
        public long ProductId { get; private set; }
        public long AttributeId { get; private set; }
        public Products Products { get; private set; }
        public ProductAttributes Attributes { get; private set; }

        public ProductAttributeValues(string value)
        {
            Value = value;
        }

        public void Edit(string value)
        {
            Value = value;
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
