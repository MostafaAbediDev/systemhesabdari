using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Company;

namespace GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg
{
    public interface ICompanyRepository : IRepository<long, Companies>
    {
        EditCompanies GetDetails(long id);
        List<CompanyViewModel> Search(CompanySearchModel searchModel);
        List<CompanyViewModel> GetCompanies();
    }
}
