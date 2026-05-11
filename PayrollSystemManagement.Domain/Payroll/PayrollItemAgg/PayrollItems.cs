using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;

namespace PayrollSystemManagement.Domain.Payroll.PayrollItemAgg
{
    public class PayrollItems : EntityBase
    {
        public string Title { get; private set; }
        public int ItemType { get; private set; }
        public bool IsFixed { get; private set; }
        public long BranchId { get; private set; }
        public Branches Branches { get; private set; }
        public List<PayrollDetails> PayrollDetails { get; private set; }

        protected PayrollItems()
        {
            PayrollDetails = new List<PayrollDetails>();
        }

        public PayrollItems(string title, int itemType)
        {
            Title = title;
            ItemType = itemType;
            IsFixed = false;
        }

        public void Edit(string title, int itemType)
        {
            Title = title;
            ItemType = itemType;
            IsFixed = false;
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
