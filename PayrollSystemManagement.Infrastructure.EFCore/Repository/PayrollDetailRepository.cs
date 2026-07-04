using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Application.Contracts.PayrollDetail;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Repository
{
    public class PayrollDetailRepository : RepositoryBase<long, PayrollDetails>, IPayrollDetailRepository
    {

        private readonly PayrollFakeDataContext _context;

        public PayrollDetailRepository(PayrollFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditPayrollDetails GetDetails(long id)
        {
            return _context.PayrollDetails
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditPayrollDetails
                {
                    Id = x.Id,
                    Quantity = x.Quantity,
                    Rate = x.Rate,
                    Description = x.Description,
                    PayrollItemId = x.PayrollItemId
                })
                .FirstOrDefault();
        }

        public List<PayrollDetailViewModel> GetPayrollDetails()
        {
            return _context.PayrollDetails
                .AsNoTracking()
                .Include(x => x.Payrolls)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new PayrollDetailViewModel
                {
                    Id = x.Id,
                    Quantity = x.Quantity,
                    Rate = x.Rate,
                    Amount = x.Amount,
                    Description = x.Description,
                    PayrollTitle = x.PayrollItems.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }

        public List<PayrollDetailViewModel> Search(PayrollDetailsSearchModel searchModel)
        {
            var query = _context.PayrollDetails
                .AsNoTracking()
                .Include(x => x.Payrolls)
                .Where(x => !x.IsDeleted);

            if (searchModel.Quantity > 0)
                query = query.Where(x => x.Quantity == searchModel.Quantity);

            if (searchModel.Rate > 0)
                query = query.Where(x => x.Rate == searchModel.Rate);

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new PayrollDetailViewModel
                {
                    Id = x.Id,
                    Quantity = x.Quantity,
                    Rate = x.Rate,
                    Amount = x.Amount,
                    Description = x.Description,
                    PayrollTitle = x.PayrollItems.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }
    }
}
