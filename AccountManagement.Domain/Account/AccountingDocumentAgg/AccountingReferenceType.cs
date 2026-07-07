namespace AccountManagement.Domain.Account.AccountingDocumentAgg
{
    public enum AccountingReferenceType
    {
        None = 0,
        SalesInvoice = 1,
        PurchaseInvoice = 2,
        Receipt = 3,
        Payment = 4,
        BankTransaction = 5,
        InventoryIssue = 6,
        InventoryReceipt = 7,
        Payroll = 8,
        Manual = 9
    }
}