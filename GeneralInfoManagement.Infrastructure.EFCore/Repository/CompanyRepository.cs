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
            return _context.Companies
                .Select(x => new CompanyViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    LegalName = x.LegalName,
                    CreationDate = x.CreationDate.ToString()
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public EditCompanies GetDetails(long id)
        {
            return _context.Companies
                .Select(x => new EditCompanies
                {
                    Id = x.Id,
                    Title = x.Title,
                    LegalName = x.LegalName,
                    Logo = x.Logo,
                    EstablishedDate = x.EstablishedDate
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<CompanyViewModel> Search(CompanySearchModel searchModel)
        {
            var query = _context.Companies
                            .Select(x => new CompanyViewModel
                            {
                                Id = x.Id,
                                Title = x.Title,
                                LegalName = x.LegalName,
                                CreationDate = x.CreationDate.ToString()
                            });

            // فیلترگذاری پویا
            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));

            if (!string.IsNullOrWhiteSpace(searchModel.LegalName))
                query = query.Where(x => x.LegalName.Contains(searchModel.LegalName));

            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}
