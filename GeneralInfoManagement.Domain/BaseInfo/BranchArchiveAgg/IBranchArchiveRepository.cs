using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.BranchArchice;

namespace GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg
{
    public interface IBranchArchiveRepository : IRepository<long, BranchArchive>
    {
        EditBranchArchive GetDetails(long id);
        List<BranchArchiveViewModel> Search(BranchArchiveSearchModel searchModel);
        List<BranchArchiveViewModel> GetBranchArchives();
    }
}
