using _0_FrameWork.Infrastructure;
using FinancialManagement.Application.Contracts.PettyCash;
using FinancialManagement.Domain.PettyCashAgg;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagement.Infrastructure.EFCore.Repository
{
    public class PettyCashRepository : RepositoryBase<long, PettyCashes>, IPettyCashRepository
    {

        private readonly FinancialFakeDataContext _context;

        public PettyCashRepository(FinancialFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditPettyCash GetDetails(long id)
        {
            return _context.PettyCashes
                .Select(x => new EditPettyCash
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    InitialAmount = x.InitialAmount,
                    MaxLimit = x.MaxLimit,
                    LastSettlementDate = x.LastSettlementDate,
                    BranchId = x.BranchId,
                    ResponsiblePersonId = x.ResponsiblePersonId,
                    HolderPersonId = x.HolderPersonId,
                    AccountId = x.AccountId,
                    SettlementAccountId = x.SettlementAccountId
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<PettyCashViewModel> GetPettyCashes()
        {
            return _context.PettyCashes
               .Include(x => x.Branches)
               .Include(x => x.ResponsiblePerson)
               .Include(x => x.HolderPerson)
               .Select(x => new PettyCashViewModel
               {
                   Id = x.Id,
                   Title = x.Title,
                   Description = x.Description,
                   InitialAmount = x.InitialAmount,
                   CurrentBalance = x.CurrentBalance,
                   MaxLimit = x.MaxLimit,
                   LastSettlementDate = x.LastSettlementDate,
                   BranchName = x.Branches.Title,
                   ResponsiblePersonName = $"{x.ResponsiblePerson.FirstName} {x.ResponsiblePerson.LastName}",
                   HolderPersonName = $"{x.HolderPerson.FirstName} {x.HolderPerson.LastName}",
                   CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
               })
               .OrderBy(x => x.Title)
               .ToList();
        }

        public List<PettyCashViewModel> Search(PettyCashSearchModel searchModel)
        {
            var query = _context.PettyCashes
                .Include(x => x.Branches)
                .Include(x => x.ResponsiblePerson)
                .Include(x => x.HolderPerson)
                .Select(x => new PettyCashViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    InitialAmount = x.InitialAmount,
                    CurrentBalance = x.CurrentBalance,
                    MaxLimit = x.MaxLimit,
                    LastSettlementDate = x.LastSettlementDate,
                    BranchName = x.Branches.Title,
                    ResponsiblePersonName = $"{x.ResponsiblePerson.FirstName} {x.ResponsiblePerson.LastName}",
                    HolderPersonName = $"{x.HolderPerson.FirstName} {x.HolderPerson.LastName}",
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
