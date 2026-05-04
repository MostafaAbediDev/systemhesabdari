using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoiceItemAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoicePaymentAgg;
using InvoiceSystemManagement.Domain.Invoice.ReceiptPaymentInvoiceAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace InvoiceSystemManagement.Domain.Invoice.InvoiceAgg
{
    public class Invoices : EntityBase
    {
        public string InvoiceNumber { get; private set; }
        public int InvoiceType { get; private set; }
        public int Status { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string Description { get; private set; }
        public string ReturnReason { get; private set; }
        public long PersonId { get; private set; }
        public long CreatedBy { get; private set; }
        public long BranchId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public long StorageId { get; private set; }
        public long AccountingDoucumentId { get; private set; }
        public Persons Persons { get; private set; }
        public Branches Branches { get; private set; }
        public FinancialPeriods FinancialPeriods { get; private set; }
        public Storages Storages { get; private set; }
        public AccountingDocuments AccountingDocuments { get; private set; }
        public List<InvoiceItems> InvoiceItems { get; private set; }
        public List<InvoicePayments> InvoicePayments { get; private set; }
        public List<ReceiptPaymentInvoices> ReceiptPaymentInvoices { get; private set; }

        protected Invoices()
        {
            InvoiceItems = new List<InvoiceItems>();
            InvoicePayments = new List<InvoicePayments>();
            ReceiptPaymentInvoices = new List<ReceiptPaymentInvoices>();
        }

        public Invoices(string invoiceNumber, int invoiceType, int status, DateTime invoiceDate, 
            DateTime dueDate, string description, string returnReason, DateTime updatedAt)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceType = invoiceType;
            Status = status;
            InvoiceDate = invoiceDate;
            DueDate = dueDate;
            Description = description;
            ReturnReason = returnReason;
            UpdatedAt = updatedAt;
        }

        public void Edit(string invoiceNumber, int invoiceType, int status, DateTime invoiceDate,
            DateTime dueDate, string description, string returnReason, DateTime updatedAt)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceType = invoiceType;
            Status = status;
            InvoiceDate = invoiceDate;
            DueDate = dueDate;
            Description = description;
            ReturnReason = returnReason;
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
