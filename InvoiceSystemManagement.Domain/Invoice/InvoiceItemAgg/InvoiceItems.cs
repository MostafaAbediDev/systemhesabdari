using _0_FrameWork.Domain;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductBatcheAgg;
using InventoryManagement.Domain.Inventory.UnitAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoiceAgg;

namespace InvoiceSystemManagement.Domain.Invoice.InvoiceItemAgg
{
    public class InvoiceItems : EntityBase
    {
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal Tax { get; private set; }
        public string Description { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int LineNumber { get; private set; }
        public long InvoiceId { get; private set; }
        public long ProductId { get; private set; }
        public long BatchId { get; private set; }
        public long UnitId { get; private set; }
        public Invoices Invoices { get; private set; }
        public Products Products { get; private set; }
        public ProductBatches ProductBatches { get; private set; }
        public Units Units { get; private set; }

        public InvoiceItems(decimal quantity, decimal unitPrice, decimal discount, decimal tax, string description, int lineNumber, DateTime updatedAt)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
            Tax = tax;
            Description = description;
            LineNumber = lineNumber;
            UpdatedAt = updatedAt;
        }

        public void Edit(decimal quantity, decimal unitPrice, decimal discount, decimal tax, string description, int lineNumber, DateTime updatedAt)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
            Tax = tax;
            Description = description;
            LineNumber = lineNumber;
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
