namespace FinancialManagement.Application.Contracts.ReceiptsPayment
{
    public class ReceiptsPaymentViewModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }
        public string BranchName { get; set; }
        public long FinancialPeriodId { get; set; }
        public string PersonName { get; set; }
        public string FundTitle { get; set; }
        public string BankAccountTitle { get; set; }
        public long? ChequeId { get; set; }
        public long? AccountingDocumentId { get; set; }
        public string CreationDate { get; set; }

    }
}
