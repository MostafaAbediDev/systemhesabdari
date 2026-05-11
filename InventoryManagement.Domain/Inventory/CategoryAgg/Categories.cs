using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.CategoryAgg
{
    public class Categories : EntityBase
    {
        public string Title { get; private set; }
        public long? ParentId { get; private set; }
        public long CompanyId { get; private set; }
        public Categories Parent { get; private set; }
        public Companies Companies { get; private set; }
        public List<Categories> Children { get; private set; }
        public List<Products> Products { get; private set; }

        protected Categories()
        {
            Children = new List<Categories>();
        }

        public Categories(string title)
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
