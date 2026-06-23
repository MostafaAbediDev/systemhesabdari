using _0_FrameWork.Infrastructure;
using BankManagement.Application.Contracts.BankBranch;
using BankManagement.Domain.Bank.BankBrancheAgg;

namespace BankManagement.Infrastructure.EFCore.Repository
{
    public class BankBranchRepository : RepositoryBase<long, BankBranches>, IBankBranchRepository
    {
        private readonly BankFakeDataContext _context;

        public BankBranchRepository(BankFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<BankBranchViewModel> GetBankBranches()
        {
            return _context.BankBranches
                .Select(x => new BankBranchViewModel
                {
                    Id = x.Id,
                    BranchName = x.BranchName,
                    BranchCode = x.BranchCode,
                    Address = x.Address,
                    Telephone = x.Telephone,
                    BankId = x.Banks.Title,
                    ProvinceId = x.Provinces.Title,
                    CityId = x.Cities.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .OrderBy(x => x.BranchName)
                .ToList();
        }

        public EditBankBranch GetDetails(long id)
        {
            return _context.BankBranches
                .Select(x => new EditBankBranch
                {
                    Id = x.Id,
                    BranchName = x.BranchName,
                    BranchCode = x.BranchCode,
                    Address = x.Address,
                    Telephone = x.Telephone,
                    BankId = x.BankId,
                    ProvinceId = x.ProvinceId,
                    CityId = x.CityId
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<BankBranchViewModel> Search(BankBranchSearchModel searchModel)
        {
            var query = _context.BankBranches
                .Select(x => new BankBranchViewModel
                {
                    Id = x.Id,
                    BranchName = x.BranchName,
                    BranchCode = x.BranchCode,
                    Address = x.Address,
                    Telephone = x.Telephone,
                    BankId = x.Banks.Title,
                    ProvinceId = x.Provinces.Title,
                    CityId = x.Cities.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                });

            if (!string.IsNullOrWhiteSpace(searchModel.BranchName))
                query = query.Where(x => x.BranchName.Contains(searchModel.BranchName));

            if (!string.IsNullOrWhiteSpace(searchModel.BranchCode))
                query = query.Where(x => x.BranchCode.Contains(searchModel.BranchCode));

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
