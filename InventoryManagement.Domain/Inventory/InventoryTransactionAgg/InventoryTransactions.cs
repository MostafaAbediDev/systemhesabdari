using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using InventoryManagement.Domain.Inventory.FailureTypeAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductCreateSerieAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageLocationAgg;

namespace InventoryManagement.Domain.Inventory.InventoryTransactionAgg
{
    public class InventoryTransactions : EntityBase
    {
        public int ReferenceType { get; private set; }
        public long ReferenceId { get; private set; }
        public int TransactionType { get; private set; }
        public decimal Quantity { get; private set; }
        public DateTime Date { get; private set; }
        public long BranchId { get; private set; }
        public long StorageId { get; private set; }
        public long LocationId { get; private set; }
        public long ProductId { get; private set; }
        public long ProductCreateSeriesId { get; private set; }
        public long FailureTypeId { get; private set; }
        public Branches Branches { get; private set; }
        public Storages Storages { get; private set; }
        public StorageLocations Locations { get; private set; }
        public Products Products { get; private set; }
        public ProductCreateSeries ProductCreateSeries { get; private set; }
        public FailureTypes FailureTypes { get; private set; }

        public InventoryTransactions(int referenceType, long referenceId, int transactionType, decimal quantity, DateTime date)
        {
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            TransactionType = transactionType;
            Quantity = quantity;
            Date = date;
        }

        public void Edit(int referenceType, long referenceId, int transactionType, decimal quantity, DateTime date)
        {
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            TransactionType = transactionType;
            Quantity = quantity;
            Date = date;
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
