using _0_Framework.Application;

namespace BankManagement.Application.Contracts.BankBranch
{
    public interface IBankBranchApplication
    {
        OperationResult Create(CreateBankBranch command);
        OperationResult Edit(EditBankBranch command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditBankBranch GetDetails(long id);
        List<BankBranchViewModel> Search(BankBranchSearchModel searchModel);
        List<BankBranchViewModel> GetBankBranches();
    }
}
