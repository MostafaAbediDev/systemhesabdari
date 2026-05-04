using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;

namespace InventoryManagement.Domain.Inventory.FailureTypeAgg
{
    public class FailureTypes : EntityBase
    {
        public string Title { get; private set; }
        public List<InventoryTransactions> InventoryTransactions { get; private set; }

        protected FailureTypes()
        {
            InventoryTransactions = new List<InventoryTransactions>();
        }

        public FailureTypes(string title)
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
