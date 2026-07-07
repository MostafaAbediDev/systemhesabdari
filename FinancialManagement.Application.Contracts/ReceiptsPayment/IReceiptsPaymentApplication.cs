using _0_Framework.Application;
using FinancialManagement.Application.Contracts.ReceiptsPayment;

public interface IReceiptsPaymentApplication
    {
        OperationResult Create(CreateReceiptsPayment command);
        OperationResult Edit(EditReceiptsPayment command);
        OperationResult LinkToPerson(LinkToPersonDTO command);
        OperationResult LinkToFund(LinkToFundDTO command);
        OperationResult LinkToBankAccount(LinkToBankAccountDTO command);
        OperationResult LinkToCheque(LinkToChequeDTO command);
        OperationResult LinkToAccountingDocument(LinkToAccountingDocumentDTO command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditReceiptsPayment GetDetails(long id);
        List<ReceiptsPaymentViewModel> Search(ReceiptsPaymentSearchModel searchModel);
        List<ReceiptsPaymentViewModel> GetReceiptsPayments();
    }
