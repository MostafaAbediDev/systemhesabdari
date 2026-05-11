using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductPriceAgg;

namespace InventoryManagement.Domain.Inventory.UnitAgg
{
    public class Units : EntityBase
    {
        public string Title { get; private set; }
        public string Symbol { get; private set; }
        public List<ProductPrices> ProductPrices { get; private set; }

        protected Units()
        {
            ProductPrices = new List<ProductPrices>();
        }

        public Units(string title, string symbol)
        {
            Title = title;
            Symbol = symbol;
        }

        public void Edit(string title, string symbol)
        {
            Title = title;
            Symbol = symbol;
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
