using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;

namespace InventoryManagement.Domain.Inventory.Product.ProductCreateSerieAgg
{
    public class ProductCreateSeries : EntityBase
    {
        public string Title { get; private set; }
        public DateTime StDate { get; private set; }
        public DateTime ExDate { get; private set; }
        public long ProductId { get; private set; }
        public Products Products { get; private set; }
        public List<InventoryTransactions> InventoryTransactions { get; private set; }
        protected ProductCreateSeries()
        {
            InventoryTransactions = new List<InventoryTransactions>();
        }

        public ProductCreateSeries(string title, DateTime stDate, DateTime exDate)
        {
            Title = title;
            StDate = stDate;
            ExDate = exDate;
        }

        public void Edit(string title, DateTime stDate, DateTime exDate)
        {
            Title = title;
            StDate = stDate;
            ExDate = exDate;
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
