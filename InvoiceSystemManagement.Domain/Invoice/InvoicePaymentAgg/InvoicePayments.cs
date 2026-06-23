using _0_FrameWork.Domain;
using FinancialManagement.Domain.ReceiptsPaymentAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoiceAgg;

namespace InvoiceSystemManagement.Domain.Invoice.InvoicePaymentAgg
{
    public class InvoicePayments : EntityBase
    {
        public decimal Amount { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public long InvoiceId { get; private set; }
        public long ReceiptPaymentId { get; private set; }
        public Invoices Invoices { get; private set; }
        public ReceiptsPayments ReceiptPayments { get; private set; }

        public InvoicePayments(decimal amount, DateTime updatedAt)
        {
            Amount = amount;
            UpdatedAt = updatedAt;
        }

        public void Edit(decimal amount, DateTime updatedAt)
        {
            Amount = amount;
            UpdatedAt = updatedAt;
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
