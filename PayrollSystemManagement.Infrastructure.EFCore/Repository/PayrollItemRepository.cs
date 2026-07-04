using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Application.Contracts.PayrollItem;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Repository
{
    public class PayrollItemRepository : RepositoryBase<long, PayrollItems>, IPayrollItemRepository
    {

        private readonly PayrollFakeDataContext _context;

        public PayrollItemRepository(PayrollFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditPayrollItem GetDetails(long id)
        {
            return _context.PayrollItems
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditPayrollItem
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsFixed = x.IsFixed,
                    Taxable = x.Taxable,
                    Insuranceable = x.Insuranceable,
                    ItemType = (PayrollItemTypeDTO)x.ItemType,
                    RuleType = (PayrollRuleTypeDTO)x.RuleType,
                    BranchId = x.BranchId
                })
                .FirstOrDefault();
        }

        public List<PayrollItemViewModel> GetPayrollDetails()
        {
            return _context.PayrollItems
                .AsNoTracking()
                .Include(x => x.Branches)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new PayrollItemViewModel
                {
                    Id = x.Id,

                    Title = x.Title,
                    IsFixed = x.IsFixed,
                    Taxable = x.Taxable,
                    Insuranceable = x.Insuranceable,
                    ItemType = x.ItemType.ToString(),
                    RuleType = x.RuleType.ToString(),
                    BranchId = x.Branches.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }

        public List<PayrollItemViewModel> Search(PayrollItemSearchModel searchModel)
        {
            var query = _context.PayrollItems
                .AsNoTracking()
                .Include(x => x.Branches)
                .Where(x => !x.IsDeleted);

            // Title filter
            if (!string.IsNullOrWhiteSpace(searchModel.Title))
            {
                var title = searchModel.Title.Trim();
                query = query.Where(x => x.Title.Contains(title));
            }

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new PayrollItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsFixed = x.IsFixed,
                    Taxable = x.Taxable,
                    Insuranceable = x.Insuranceable,
                    ItemType = x.ItemType.ToString(),
                    RuleType = x.RuleType.ToString(),
                    BranchId = x.Branches.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }
    }
}
