using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Branches;

namespace GeneralInfoManagement.Domain.BaseInfo.BranchesAgg
{
    public interface IBranchRepository : IRepository<long, Branches>
    {
        EditBranch GetDetails(long id);
        List<BranchViewModel> Search(BranchSearchModel searchModel);
        List<BranchViewModel> GetAllBranches();
    }
}
