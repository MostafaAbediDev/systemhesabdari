using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageAgg;

namespace InventoryManagement.Domain.Inventory.Storage.StorageLocationAgg
{
    public class StorageLocations : EntityBase
    {
        public string Title { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public int Level { get; private set; }
        public long StorageId { get; private set; }
        public long ParentId { get; private set; }
        public Storages Storages { get; private set; }
        public StorageLocations Parent { get; private set; }
        public List<InventoryTransactions> InventoryTransactions { get; private set; }

        protected StorageLocations()
        {
            InventoryTransactions = new List<InventoryTransactions>();
        }

        public StorageLocations(string title, string code, string description, int level)
        {
            Title = title;
            Code = code;
            Description = description;
            Level = level;
        }


        public void Edit(string title, string code, string description, int level)
        {
            Title = title;
            Code = code;
            Description = description;
            Level = level;
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
