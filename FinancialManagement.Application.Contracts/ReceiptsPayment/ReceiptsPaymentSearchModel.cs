namespace FinancialManagement.Application.Contracts.ReceiptsPayment
{
    public class ReceiptsPaymentSearchModel
    {
        public decimal Amount { get; set; }
        public ReceiptPaymentTypeDTO Type { get; set; }
    }
}
