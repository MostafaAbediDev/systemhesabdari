using _0_Framework.Application;

namespace GeneralInfoManagement.Application.Contract.Branches
{
    public interface IBranchApplication
    {
        OperationResult Create(CreateBranches command);
        OperationResult Edit(EditBranch command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        OperationResult IsMain(long id);
        OperationResult IsNotMain(long id);
        EditBranch GetDetails(long id);
        List<BranchViewModel> Search(BranchSearchModel searchModel);
        List<BranchViewModel> GetBranches();
    }
}
