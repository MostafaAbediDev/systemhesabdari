using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.BankAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace BankManagement.Domain.Bank.CompanyBankAccountAgg
{
    public class CompanyBankAccounts : EntityBase
    {
        public string AccountTitle { get; private set; }
        public string AccountNumber { get; private set; }
        public string CardNumber { get; private set; }
        public string Shaba { get; private set; }
        public long BranchId { get; private set; }
        public long BankId { get; private set; }
        public Branches Branches { get; private set; }
        public Banks Banks { get; private set; }

        protected CompanyBankAccounts()
        {
        }

        public CompanyBankAccounts(string accountTitle, string accountNumber, string cardNumber, string shaba, long branchId, long bankId)
        {
            AccountTitle = accountTitle;
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            BranchId = branchId;
            BankId = bankId;
        }

        public void Edit(string accountTitle, string accountNumber, string cardNumber, string shaba)
        {
            AccountTitle = accountTitle;
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
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
