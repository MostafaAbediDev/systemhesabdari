using _0_Framework.Application;

namespace BankManagement.Application.Contracts.Cheque
{
    public interface IChequeApplication
    {
        OperationResult Create(CreateCheque command);
        OperationResult Edit(EditCheque command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        OperationResult ChangeStatus(ChangeChequeStatus command);
        EditCheque GetDetails(long id);
        List<ChequeViewModel> Search(ChequeSearchModel searchModel);
        List<ChequeViewModel> GetCheques();
    }
}
