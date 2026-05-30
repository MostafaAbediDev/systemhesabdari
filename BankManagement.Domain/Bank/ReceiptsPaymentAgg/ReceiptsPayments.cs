using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using BankManagement.Domain.Bank.ChequeAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using BankManagement.Domain.Bank.FundAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;

namespace BankManagement.Domain.Bank.ReceiptsPaymentAgg
{
    public class ReceiptsPayments : EntityBase
    {
        public string Description { get; private set; }
        public int Type { get; private set; }
        public decimal Amount { get; private set; }
        public int PaymentMethod { get; private set; }
        public long BranchId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public long PersonId { get; private set; }
        public long FundId { get; private set; }
        public long CompanyBankAccountId { get; private set; }
        public long ChequeId { get; private set; }
        public long AccountingDocumentId { get; private set; }
        public Branches Branches { get; private set; }
        public FinancialPeriods FinancialPeriods { get; private set; }
        //public Persons Persons { get; private set; }
        public Funds Funds { get; private set; }
        public CompanyBankAccounts CompanyBankAccounts { get; private set; }
        public Cheques Cheques { get; private set; }
        public AccountingDocuments AccountingDocuments { get; private set; }

        public ReceiptsPayments(string description, int type, decimal amount, int paymentMethod)
        {
            Description = description;
            Type = type;
            Amount = amount;
            PaymentMethod = paymentMethod;
        }

        public void Edit(string description, int type, decimal amount, int paymentMethod)
        {
            Description = description;
            Type = type;
            Amount = amount;
            PaymentMethod = paymentMethod;
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
