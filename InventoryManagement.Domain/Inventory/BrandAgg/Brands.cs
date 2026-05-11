using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.BrandAgg
{
    public class Brands : EntityBase
    {
        public string Title { get; private set; }
        public List<Products> Products { get; private set; }

        protected Brands()
        {
            Products = new List<Products>();
        }

        public Brands(string title)
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
