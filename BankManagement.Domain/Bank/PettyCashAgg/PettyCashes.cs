using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace BankManagement.Domain.Bank.PettyCashAgg
{
    public class PettyCashes : EntityBase
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public decimal InitialAmount { get; private set; }
        public decimal CurrentBalance { get; private set; }
        public decimal MaxLimit { get; private set; }
        public DateTime LastSettlementDate { get; private set; }
        public long BranchId { get; private set; }
        public long ResponsiblePersonId { get; private set; }
        public long HolderPersonId { get; private set; }
        public long AccountId { get; private set; }
        public long SettlementAccountId { get; private set; }
        public Branches Branches { get; private set; }
        //public Persons Persons { get; private set; }
        public Accounts Accounts { get; private set; }

        public PettyCashes(string title, string description, decimal initialAmount, decimal currentBalance, decimal maxLimit, DateTime lastSettlementDate)
        {
            Title = title;
            Description = description;
            InitialAmount = initialAmount;
            CurrentBalance = currentBalance;
            MaxLimit = maxLimit;
            LastSettlementDate = lastSettlementDate;
        }

        public void Edit(string title, string description, decimal initialAmount, decimal currentBalance, decimal maxLimit, DateTime lastSettlementDate)
        {
            Title = title;
            Description = description;
            InitialAmount = initialAmount;
            CurrentBalance = currentBalance;
            MaxLimit = maxLimit;
            LastSettlementDate = lastSettlementDate;
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
