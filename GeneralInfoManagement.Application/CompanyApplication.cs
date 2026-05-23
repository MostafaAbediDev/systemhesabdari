using _0_Framework.Application;
using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Company;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;

namespace GeneralInfoManagement.Application
{
    public class CompanyApplication : ICompanyApplication
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyApplication(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public OperationResult Create(CreateCompanies command)
        {
            var operation = new OperationResult();

            if (_companyRepository.Exists(x => x.Title == command.Title))
                return operation.Failed("نام شرکت تکراری است.");

            var company = new Companies(command.Title, command.Logo, command.LegalName, command.EstablishedDate);

            _companyRepository.Create(company);
            _companyRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();

            var company = _companyRepository.Get(id);
            if (company == null) return new OperationResult().Failed("یافت نشد.");

            company.Active();

            _companyRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var company = _companyRepository.Get(id);
            if (company == null) return new OperationResult().Failed("یافت نشد.");

            company.NotActive();
            _companyRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditCompanies command)
        {
            var operation = new OperationResult();

            var company = _companyRepository.Get(command.Id);
            if (company == null) return new OperationResult().Failed("شرکت یافت نشد.");

            if (_companyRepository.Exists(x => x.Title == command.Title && x.Id != command.Id))
                return new OperationResult().Failed("نام شرکت تکراری است.");

            company.Edit(command.Title, command.Logo, command.LegalName, command.EstablishedDate);

            _companyRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<CompanyViewModel> GetCompanies()
        {
            return _companyRepository.GetCompanies();
        }

        public EditCompanies GetDetails(long id)
        {
            return _companyRepository.GetDetails(id);
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var company = _companyRepository.Get(id);

            if (company == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            company.Remove();
            _companyRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var company = _companyRepository.Get(id);
            if (company == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            company.Restore();

            _companyRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<CompanyViewModel> Search(CompanySearchModel searchModel)
        {
            return _companyRepository.Search(searchModel);
        }
    }
}
