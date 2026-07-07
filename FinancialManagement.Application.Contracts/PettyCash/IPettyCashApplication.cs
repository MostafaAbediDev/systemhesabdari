using _0_Framework.Application;

namespace FinancialManagement.Application.Contracts.PettyCash
{
    public interface IPettyCashApplication
    {
        OperationResult Create(CreatePettyCash command);
        OperationResult Edit(EditPettyCash command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        OperationResult ChangeMaxLimit(ChangeMaxLimitDTO command);
        OperationResult ChangeResponsiblePerson(ChangeResponsiblePersonDTO command);
        OperationResult ChangeSettlementAccount(ChangeSettlementAccountDTO command);
        OperationResult IncreaseBalance(IncreaseBalanceDTO command);
        OperationResult DecreaseBalance(DecreaseBalanceDTO command);
        EditPettyCash GetDetails(long id);
        List<PettyCashViewModel> Search(PettyCashSearchModel searchModel);
        List<PettyCashViewModel> GetPettyCashes();
    }
}
