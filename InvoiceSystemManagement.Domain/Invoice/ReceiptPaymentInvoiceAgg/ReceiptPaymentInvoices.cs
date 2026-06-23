using _0_FrameWork.Domain;
using FinancialManagement.Domain.ReceiptsPaymentAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoiceAgg;

namespace InvoiceSystemManagement.Domain.Invoice.ReceiptPaymentInvoiceAgg
{
    public class ReceiptPaymentInvoices : EntityBase
    {
        public decimal Amount { get; private set; }
        public long ReceiptPaymentId { get; private set; }
        public long InvoiceId { get; private set; }
        public ReceiptsPayments ReceiptsPayments { get; private set; }
        public Invoices Invoices { get; private set; }
        public ReceiptPaymentInvoices(decimal amount)
        {
            Amount = amount;
        }
        public void Edit(decimal amount)
        {
            Amount = amount;
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
