using _0_Framework.Application;

namespace GeneralInfoManagement.Application.Contract.Company
{
    public interface ICompanyApplication
    {
        OperationResult Create(CreateCompanies command);
        OperationResult Edit(EditCompanies command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditCompanies GetDetails(long id);
        List<CompanyViewModel> Search(CompanySearchModel searchModel);
        List<CompanyViewModel> GetCompanies();
    }
}
