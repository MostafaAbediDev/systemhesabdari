namespace FinancialManagement.Application.Contracts.ReceiptsPayment
{
    public class CreateReceiptsPayment
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public ReceiptPaymentTypeDTO Type { get; set; }
        public PaymentMethodDTO PaymentMethod { get; set; }
        public long BranchId { get; set; }
        public long FinancialPeriodId { get; set; }
    }
}
