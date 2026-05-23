using _0_Framework.Application;

namespace GeneralInfoManagement.Application.Contract.BranchArchice
{
    public interface IBranchArchiveApplication
    {
        OperationResult Create(CreateBranchArchive command);
        OperationResult Edit(EditBranchArchive command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditBranchArchive GetDetails(long id);
        List<BranchArchiveViewModel> Search(BranchArchiveSearchModel searchModel);
        List<BranchArchiveViewModel> GetBranchArchives();
    }
}
