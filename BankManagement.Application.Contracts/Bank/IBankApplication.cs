using _0_Framework.Application;

namespace BankManagement.Application.Contracts.Bank
{
    public interface IBankApplication
    {
        OperationResult Create(CreateBank command);
        OperationResult Edit(EditBank command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditBank GetDetails(long id);
        List<BankViewModel> Search(BankSearchModel searchModel);
        List<BankViewModel> GetBanks();
    }
}
