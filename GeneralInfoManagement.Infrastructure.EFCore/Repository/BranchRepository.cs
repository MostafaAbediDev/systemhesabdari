using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class BranchRepository : RepositoryBase<long, Branches>, IBranchRepository
    {
        private readonly GeneralInfoFakeDataContext _fakeDataContext;

        public BranchRepository(GeneralInfoFakeDataContext fakeDataContext) : base(fakeDataContext)
        {
            _fakeDataContext = fakeDataContext;
        }

        public List<BranchViewModel> GetAllPersons()
        {
            throw new NotImplementedException();
        }

        public EditBranch GetDetails(long id)
        {
            throw new NotImplementedException();
        }

        public List<BranchViewModel> Search(BranchSearchModel searchModel)
        {
            throw new NotImplementedException();
        }
    }
}
