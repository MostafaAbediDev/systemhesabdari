using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using BankManagement.Domain.Bank.BankAgg;
using BankManagement.Domain.Bank.ReceiptsPaymentAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace BankManagement.Domain.Bank.CompanyBankAccountAgg
{
    public class CompanyBankAccounts : EntityBase
    {
        public string AccountTitle { get; private set; }
        public string AccountNumber { get; private set; }
        public string CardNumber { get; private set; }
        public string Shaba { get; private set; }
        public string Code { get; private set; }
        public string Phone { get; private set; }
        public string Website { get; private set; }
        public long BranchId { get; private set; }
        public long BankId { get; private set; }
        public long AccountId { get; private set; }
        public Branches Branches { get; private set; }
        public Banks Banks { get; private set; }
        public Accounts Accounts { get; private set; }
        public List<ReceiptsPayments> ReceiptsPayments { get; private set; }

        protected CompanyBankAccounts()
        {
            ReceiptsPayments = new List<ReceiptsPayments>();
        }

        public CompanyBankAccounts(string accountTitle, string accountNumber, string cardNumber, string shaba, string code, string phone, string website)
        {
            AccountTitle = accountTitle;
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            Code = code;
            Phone = phone;
            Website = website;
        }

        public void Edit(string accountTitle, string accountNumber, string cardNumber, string shaba, string code, string phone, string website)
        {
            AccountTitle = accountTitle;
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            Code = code;
            Phone = phone;
            Website = website;
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
