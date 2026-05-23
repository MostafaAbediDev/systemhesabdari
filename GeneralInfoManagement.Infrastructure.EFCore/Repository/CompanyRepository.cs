using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.Company;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class CompanyRepository : RepositoryBase<long, Companies>, ICompanyRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public CompanyRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<CompanyViewModel> GetCompanies()
        {
            throw new NotImplementedException();
        }

        public EditCompanies GetDetails(long id)
        {
            throw new NotImplementedException();
        }

        public List<CompanyViewModel> Search(CompanySearchModel searchModel)
        {
            throw new NotImplementedException();
        }
    }
}
