using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using Microsoft.EntityFrameworkCore;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class FinancialPeriodRepository : RepositoryBase<long, FinancialPeriods>, IFinancialPeriodRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public FinancialPeriodRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<FinancialPeriodViewModel> GetFinancialPeriods()
        {
            return _context.FinancialPeriods
                .Include(x => x.Branch)
                .Select(x => new FinancialPeriodViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDate = x.StartDate.ToString("yyyy/MM/dd"),
                    EndDate = x.EndDate.ToString("yyyy/MM/dd"),
                    BranchTitle = x.Branch.Title,
                    BranchId = x.BranchId
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public EditFinancialPeriod GetDetails(long id)
        {
            return _context.FinancialPeriods
                            .Select(x => new EditFinancialPeriod
                            {
                                Id = x.Id,
                                Title = x.Title,
                                StartDate = x.StartDate,
                                EndDate = x.EndDate,
                                BranchId = x.BranchId
                            })
                            .FirstOrDefault(x => x.Id == id);
        }

        public List<FinancialPeriodViewModel> Search(FinancialPeriodSearchModel searchModel)
        {
            var query = _context.FinancialPeriods
               .Include(x => x.Branch)
               .Select(x => new FinancialPeriodViewModel
               {
                   Id = x.Id,
                   Title = x.Title,
                   StartDate = x.StartDate.ToString("yyyy/MM/dd"),
                   EndDate = x.EndDate.ToString("yyyy/MM/dd"),
                   BranchTitle = x.Branch.Title,
                   BranchId = x.BranchId
               });

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));

            if (searchModel.BranchId > 0)
                query = query.Where(x => x.BranchId == searchModel.BranchId);

            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}
