using _0_FrameWork.Infrastructure;
using FinancialManagement.Application.Contracts.Fund;
using FinancialManagement.Domain.FundAgg;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagement.Infrastructure.EFCore.Repository
{
    public class FundRepository : RepositoryBase<long, Funds>, IFundRepository
    {
        private readonly FinancialFakeDataContext _context;

        public FundRepository(FinancialFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditFund GetDetails(long id)
        {
            return _context.Funds
                .Select(x => new EditFund
                {
                    Id = x.Id,
                    Title = x.Title,
                    BranchId = x.BranchId,
                    AccountId = x.AccountId
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<FundViewModel> GetFunds()
        {
            return _context.Funds
               .Select(x => new FundViewModel
               {
                   Id = x.Id,
                   Title = x.Title,
                   BranchTitle = x.Branches.Title,
                   CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
               })
               .OrderBy(x => x.Title)
               .ToList();
        }

        public List<FundViewModel> Search(FundSearchModel searchModel)
        {
            var query = _context.Funds
                .Include(x => x.Branches)
                .Select(x => new FundViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    BranchTitle = x.Branches.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                });

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
