using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using BankManagement.Domain.Bank.ReceiptsPaymentAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace BankManagement.Domain.Bank.FundAgg
{
    public class Funds : EntityBase
    {
        public string Title { get; private set; }
        public long BranchId { get; private set; }
        public long AccountId { get; private set; }
        public Branches Branches { get; private set; }
        public Accounts Accounts { get; private set; }
        public List<ReceiptsPayments> ReceiptsPayments { get; private set; }

        protected Funds()
        {
            ReceiptsPayments = new List<ReceiptsPayments>();
        }

        public Funds(string title)
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
