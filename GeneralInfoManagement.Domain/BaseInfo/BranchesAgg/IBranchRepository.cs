using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Branches;

namespace GeneralInfoManagement.Domain.BaseInfo.BranchesAgg
{
    public interface IBranchRepository : IRepository<long, Branches>
    {
        EditBranch GetDetails(long id);
        List<BranchViewModel> Search(BranchSearchModel searchModel);
        List<BranchViewModel> GetAllBranches();
        void ResetAllMainBranches();
        public Branches GetCurrentMainBranch(long companyId);
        public bool ExistsMainBranch(long companyId, long? excludeId = null);
    }
}
