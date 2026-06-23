using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace FinancialManagement.Domain.PettyCashAgg
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
        public Persons ResponsiblePerson { get; private set; }
        public Persons HolderPerson { get; private set; }
        public Accounts Account { get; private set; }
        public Accounts SettlementAccount { get; private set; }

        public PettyCashes(string title, string description, decimal initialAmount, decimal maxLimit, DateTime lastSettlementDate,
            long branchId, long responsiblePersonId, long holderPersonId, long accountId, long settlementAccountId)
        {
            Title = title;
            Description = description;
            InitialAmount = initialAmount;
            MaxLimit = maxLimit;
            LastSettlementDate = lastSettlementDate;
            BranchId = branchId;
            ResponsiblePersonId = responsiblePersonId;
            HolderPersonId = holderPersonId;
            AccountId = accountId;
            SettlementAccountId = settlementAccountId;
        }

        public void Edit(string title, string description, decimal maxLimit, DateTime lastSettlementDate)
        {
            Title = title;
            Description = description;
            MaxLimit = maxLimit;
            LastSettlementDate = lastSettlementDate;
        }

        public void ChangeMaxLimit(decimal maxLimit)
        {
            MaxLimit = maxLimit;
        }

        public void ChangeResponsiblePerson(long responsiblePersonId)
        {
            ResponsiblePersonId = responsiblePersonId;
        }

        public void ChangeSettlementAccount(long settlementAccountId)
        {
            SettlementAccountId = settlementAccountId;
        }

        public void IncreaseBalance(decimal amount)
        {
            CurrentBalance += amount;
        }

        public void DecreaseBalance(decimal amount)
        {
            CurrentBalance -= amount;
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
