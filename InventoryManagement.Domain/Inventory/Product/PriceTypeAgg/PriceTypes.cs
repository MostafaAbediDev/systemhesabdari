using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using InventoryManagement.Domain.Inventory.Product.ProductPriceAgg;

namespace InventoryManagement.Domain.Inventory.Product.PriceTypeAgg
{
    public class PriceTypes : EntityBase
    {
        public string Title { get; private set; }
        public long CompanyId { get; private set; }
        public Companies Companies { get; private set; }
        public List<ProductPrices> ProductPrices { get; private set; }

        protected PriceTypes()
        {
            ProductPrices = new List<ProductPrices>();
        }

        public PriceTypes(string title)
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
