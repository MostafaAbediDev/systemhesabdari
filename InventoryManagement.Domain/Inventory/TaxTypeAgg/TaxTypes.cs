using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.TaxTypeAgg
{
    public class TaxTypes : EntityBase
    {
        public string Title { get; private set; }
        public decimal TaxPercent { get; private set; }
        public string Description { get; private set; }
        public bool IsDefault { get; private set; }
        public long CompanyId { get; private set; }
        public Companies Company { get; private set; }
        public List<Products> Products { get; private set; }

        protected TaxTypes()
        {
            Products = new List<Products>();
        }

        public TaxTypes(string title, decimal taxPercent, string description)
        {
            Title = title;
            TaxPercent = taxPercent;
            Description = description;
            IsDefault = false;
        }

        public void Edit(string title, decimal taxPercent, string description)
        {
            Title = title;
            TaxPercent = taxPercent;
            Description = description;
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
