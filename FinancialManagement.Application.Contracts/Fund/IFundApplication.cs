using _0_Framework.Application;

namespace FinancialManagement.Application.Contracts.Fund
{
    public interface IFundApplication
    {
        OperationResult Create(CreateFunds command);
        OperationResult Edit(EditFund command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditFund GetDetails(long id);
        List<FundViewModel> Search(FundSearchModel searchModel);
        List<FundViewModel> GetFunds();
    }
}
