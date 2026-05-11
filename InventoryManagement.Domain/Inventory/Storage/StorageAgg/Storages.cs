using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageLocationAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageProductAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace InventoryManagement.Domain.Inventory.Storage.StorageAgg
{
    public class Storages : EntityBase
    {
        public string Code { get; private set; }
        public string Title { get; private set; }
        public string Address { get; private set; }
        public string PostalCode { get; private set; }
        public string Phone { get; private set; }
        public string Description { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public long ProvinceId { get; private set; }
        public long CityId { get; private set; }
        public long ManagerPersonId { get; private set; }
        public long BranchId { get; private set; }
        public Provinces Provinces { get; private set; }
        public Cities Cities { get; private set; }
        public Persons ManagerPersons { get; private set; }
        public Branches Branches { get; private set; }
        public List<StorageProducts> StorageProducts { get; private set; }
        public List<InventoryTransactions> InventoryTransactions { get; private set; }

        protected Storages()
        {
            StorageProducts = new List<StorageProducts>();
            InventoryTransactions = new List<InventoryTransactions>();
        }

        public Storages(string code, string title, string address, string postalCode, string phone, string description, DateTime updatedAt)
        {
            Code = code;
            Title = title;
            Address = address;
            PostalCode = postalCode;
            Phone = phone;
            Description = description;
            UpdatedAt = updatedAt;
        }

        public void Edit(string code, string title, string address, string postalCode, string phone, string description, DateTime updatedAt)
        {
            Code = code;
            Title = title;
            Address = address;
            PostalCode = postalCode;
            Phone = phone;
            Description = description;
            UpdatedAt = updatedAt;
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
