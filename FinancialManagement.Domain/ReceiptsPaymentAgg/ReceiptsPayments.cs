using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using BankManagement.Domain.Bank.ChequeAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using FinancialManagement.Domain.FundAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace FinancialManagement.Domain.ReceiptsPaymentAgg
{
    public class ReceiptsPayments : EntityBase
    {
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public ReceiptPaymentType Type { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public long BranchId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public long? PersonId { get; private set; }
        public long? FundId { get; private set; }
        public long? CompanyBankAccountId { get; private set; }
        public long? ChequeId { get; private set; }
        public long? AccountingDocumentId { get; private set; }
        public Branches Branches { get; private set; }
        public FinancialPeriods FinancialPeriods { get; private set; }
        public Persons? Persons { get; private set; }
        public Funds? Funds { get; private set; }
        public CompanyBankAccounts? CompanyBankAccounts { get; private set; }
        public Cheques? Cheques { get; private set; }
        public AccountingDocuments? AccountingDocuments { get; private set; }

        protected ReceiptsPayments() { }

        public ReceiptsPayments(string description, decimal amount, ReceiptPaymentType type, PaymentMethod paymentMethod,
            long branchId, long financialPeriodId)
        {
            Description = description;
            Amount = amount;
            Type = type;
            PaymentMethod = paymentMethod;
            BranchId = branchId;
            FinancialPeriodId = financialPeriodId;

            TransactionDate = DateTime.Now;
        }

        public void Edit(string description, decimal amount, ReceiptPaymentType type, PaymentMethod paymentMethod)
        {
            Description = description;
            Amount = amount;
            Type = type;
            PaymentMethod = paymentMethod;
        }
        public void LinkToPerson(long personId)
        {
            PersonId = personId;
        }

        public void LinkToFund(long fundId)
        {
            FundId = fundId;
        }

        public void LinkToBankAccount(long companyBankAccountId)
        {
            CompanyBankAccountId = companyBankAccountId;
        }

        public void LinkToCheque(long chequeId)
        {
            ChequeId = chequeId;
        }

        public void LinkToAccountingDocument(long accountingDocumentId)
        {
            AccountingDocumentId = accountingDocumentId;
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
