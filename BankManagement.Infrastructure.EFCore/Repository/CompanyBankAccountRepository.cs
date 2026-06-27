using _0_FrameWork.Infrastructure;
using BankManagement.Application.Contracts.CompanyBankAccount;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;

namespace BankManagement.Infrastructure.EFCore.Repository
{
    public class CompanyBankAccountRepository : RepositoryBase<long, CompanyBankAccounts>, ICompanyBankAccountRepository
    {
        private readonly BankFakeDataContext _context;

        public CompanyBankAccountRepository(BankFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<CompanyBankAccountViewModel> GetCompanyBankAccounts()
        {
            return _context.CompanyBankAccounts
               .Where(x => !x.IsDeleted)
               .Select(x => new CompanyBankAccountViewModel
               {
                   Id = x.Id,
                   AccountTitle = x.AccountTitle,
                   AccountNumber = x.AccountNumber,
                   CardNumber = x.CardNumber,
                   Shaba = x.Shaba,
                   BankName = x.Banks.Title,
                   BranchName = x.Branches.Title,
                   CreationDate = x.CreationDate.ToString(),
                   IsActive = x.IsActive,
                   IsDeleted = x.IsDeleted
               })
               .OrderByDescending(x => x.Id)
               .ToList();
        }

        public EditCompanyBankAccount GetDetails(long id)
        {
            return _context.CompanyBankAccounts
               .Where(x => x.Id == id)
               .Select(x => new EditCompanyBankAccount
               {
                   Id = x.Id,
                   AccountTitle = x.AccountTitle,
                   AccountNumber = x.AccountNumber,
                   CardNumber = x.CardNumber,
                   Shaba = x.Shaba,
                   BranchId = x.BranchId,
                   BankId = x.BankId
               })
               .FirstOrDefault();
        }

        public List<CompanyBankAccountViewModel> Search(CompanyBankAccountSearchModel searchModel)
        {
            var query = _context.CompanyBankAccounts
                .Select(x => new CompanyBankAccountViewModel
                {
                    Id = x.Id,
                    AccountTitle = x.AccountTitle,
                    AccountNumber = x.AccountNumber,
                    CardNumber = x.CardNumber,
                    Shaba = x.Shaba,
                    BankName = x.Banks.Title,
                    BranchName = x.Branches.Title,
                    CreationDate = x.CreationDate.ToString(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                });

            if (!string.IsNullOrWhiteSpace(searchModel.AccountNumber))
                query = query.Where(x => x.AccountNumber.Contains(searchModel.AccountNumber));

            if (!string.IsNullOrWhiteSpace(searchModel.AccountTitle))
                query = query.Where(x => x.AccountTitle.Contains(searchModel.AccountTitle));

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
