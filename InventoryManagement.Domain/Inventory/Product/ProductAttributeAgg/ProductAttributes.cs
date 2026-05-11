using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductArrributeValueAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductAttributeAgg
{
    public class ProductAttributes : EntityBase
    {
        public string Title { get; private set; }
        public List<ProductAttributeValues> ProductAttributeValues { get; private set; }

        protected ProductAttributes()
        {
            ProductAttributeValues = new List<ProductAttributeValues>();
        }
        public ProductAttributes(string title)
        {
            Title = title;
        }

        public void Edit(string title)
        {
            Title = title;
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
