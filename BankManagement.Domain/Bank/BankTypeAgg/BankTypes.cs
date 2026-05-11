using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.BankAgg;

namespace BankManagement.Domain.Bank.BankTypeAgg
{
    public class BankTypes : EntityBase
    {
        public int Code { get; private set; }
        public string Title { get; private set; }
        public List<Banks> Banks { get; private set; }

        protected BankTypes()
        {
            Banks = new List<Banks>();
        }

        public BankTypes(int code, string title)
        {
            Code = code;
            Title = title;
        }

        public void Edit(int code, string title)
        {
            Code = code;
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
