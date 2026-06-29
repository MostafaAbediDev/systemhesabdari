using _0_FrameWork.Infrastructure;
using BankManagement.Application.Contracts.ChequeBook;
using BankManagement.Domain.Bank.ChequeBookAgg;

namespace BankManagement.Infrastructure.EFCore.Repository
{
    public class ChequeBookRepository : RepositoryBase<long, ChequeBooks>, IChequeBookRepository
    {
        private readonly BankFakeDataContext _context;

        public ChequeBookRepository(BankFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<ChequeBookViewModel> GetChequeBooks()
        {
            return _context.ChequeBooks
                .Where(x => !x.IsDeleted)
                .Select(x => new ChequeBookViewModel
                {
                    Id = x.Id,
                    ChequeCount = x.ChequeCount,
                    FirstChequeCode = x.FirstChequeCode,
                    LastChequeCode = x.LastChequeCode,
                    SerialNumber = x.SerialNumber,
                    ReceiveDate = x.ReceiveDate.ToString("yyyy/MM/dd"),
                    Status = x.Status.ToString(),
                    CompanyBankAccountTitle = x.CompanyBankAccount.AccountNumber,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public EditChequeBook GetDetails(long id)
        {
            return _context.ChequeBooks
               .Where(x => x.Id == id)
               .Select(x => new EditChequeBook
               {
                   Id = x.Id,
                   ChequeCount = x.ChequeCount,
                   FirstChequeCode = x.FirstChequeCode,
                   LastChequeCode = x.LastChequeCode,
                   SerialNumber = x.SerialNumber,
                   ReceiveDate = x.ReceiveDate,
                   CompanyBankAccountId = x.CompanyBankAccountId
               })
               .FirstOrDefault();
        }

        public List<ChequeBookViewModel> Search(ChequeBookSearchModel searchModel)
        {
            var query = _context.ChequeBooks
                .Select(x => new ChequeBookViewModel
                {
                    Id = x.Id,
                    ChequeCount = x.ChequeCount,
                    FirstChequeCode = x.FirstChequeCode,
                    LastChequeCode = x.LastChequeCode,
                    SerialNumber = x.SerialNumber,
                    ReceiveDate = x.ReceiveDate.ToString("yyyy/MM/dd"),
                    Status = x.Status.ToString(),
                    CompanyBankAccountTitle = x.CompanyBankAccount.AccountNumber,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                });

            if (!string.IsNullOrWhiteSpace(searchModel.SerialNumber))
                query = query.Where(x => x.SerialNumber.Contains(searchModel.SerialNumber));

            if (searchModel.CompanyBankAccountId.HasValue &&
                searchModel.CompanyBankAccountId.Value > 0)
                query = query.Where(x => x.Id == searchModel.CompanyBankAccountId.Value);

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
