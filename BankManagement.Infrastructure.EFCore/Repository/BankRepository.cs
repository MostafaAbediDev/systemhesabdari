using _0_FrameWork.Infrastructure;
using BankManagement.Application.Contracts.Bank;
using BankManagement.Domain.Bank.BankAgg;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Infrastructure.EFCore.Repository
{
    public class BankRepository : RepositoryBase<long, Banks>, IBankRepository
    {
        private readonly BankFakeDataContext _context;

        public BankRepository(BankFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<BankViewModel> GetBanks()
        {
            return _context.Banks
                .AsNoTracking()
                .Include(x => x.BankTypes)
                .Select(x => new BankViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Country = x.Country,
                    Description = x.Description,
                    Logo = x.Logo,
                    BankType = x.BankTypes.Title,
                    CreationDate = x.CreationDate.ToString()
                })
                .ToList();
        }

        public EditBank GetDetails(long id)
        {
            return _context.Banks
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new EditBank
                {
                    Id = x.Id,
                    Title = x.Title,
                    Country = x.Country,
                    Description = x.Description,
                    Logo = x.Logo,
                    BankTypeId = x.BankTypeId
                })
                .FirstOrDefault();
        }

        public List<BankViewModel> Search(BankSearchModel searchModel)
        {
            var query = _context.Banks
                .AsNoTracking()
                .Include(x => x.BankTypes)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));

            if (!string.IsNullOrWhiteSpace(searchModel.BankType))
                query = query.Where(x => x.BankTypes.Title.Contains(searchModel.BankType));

            return query
                .Select(x => new BankViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Country = x.Country,
                    Description = x.Description,
                    Logo = x.Logo,
                    BankType = x.BankTypes.Title,
                    CreationDate = x.CreationDate.ToString()
                })
                .ToList();
        }
    }
}
