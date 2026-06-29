using _0_Framework.Application;

namespace BankManagement.Application.Contracts.ChequeBook
{
    public interface IChequeBookApplication
    {
        OperationResult Create(CreateChequeBook command);
        OperationResult Edit(EditChequeBook command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        OperationResult ChangeStatus(ChangeChequeBookStatus command);
        EditChequeBook GetDetails(long id);
        List<ChequeBookViewModel> Search(ChequeBookSearchModel searchModel);
        List<ChequeBookViewModel> GetChequeBooks();
    }
}
