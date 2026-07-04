using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Application.Contracts.Payroll;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Repository
{
    public class PayrollRepository : RepositoryBase<long, Payrolls>, IPayrollRepository
    {

        private readonly PayrollFakeDataContext _context;

        public PayrollRepository(PayrollFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<PayrollViewModel> GetAllPayrolls()
        {
            return _context.Payrolls
                .AsNoTracking()
                .Include(x => x.Employees)
                .Include(x => x.Branches)
                .Include(x => x.FinancialPeriods)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new PayrollViewModel
                {
                    Id = x.Id,
                    Employee = x.Employees.EmployeeCode,
                    Branch = x.Branches.Title,
                    FinancialPeriod = x.FinancialPeriods.Title,
                    Status = x.Status.ToString(),
                    TotalBenefits = x.TotalBenefits,
                    TotalDeduction = x.TotalDeduction,
                    NetPay = x.NetPay,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }

        public EditPayroll GetDetails(long id)
        {
            return _context.Payrolls
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditPayroll
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    BranchId = x.BranchId,
                    FinancialPeriodId = x.FinancialPeriodId
                })
                .FirstOrDefault();
        }

        public Payrolls GetWithDetails(long id)
        {
            return _context.Payrolls
                .Include(x => x.PayrollDetails)
                    .ThenInclude(x => x.PayrollItems)
                .Include(x => x.Employees)
                .Include(x => x.Branches)
                .Include(x => x.FinancialPeriods)
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<PayrollViewModel> Search(PayrollSearchModel searchModel)
        {
            var query = _context.Payrolls
                .AsNoTracking()
                .Include(x => x.Employees)
                .Include(x => x.Branches)
                .Include(x => x.FinancialPeriods)
                .Where(x => !x.IsDeleted);

            if (searchModel.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == searchModel.EmployeeId.Value);

            if (searchModel.BranchId.HasValue)
                query = query.Where(x => x.BranchId == searchModel.BranchId.Value);

            if (searchModel.FinancialPeriodId.HasValue)
                query = query.Where(x => x.FinancialPeriodId == searchModel.FinancialPeriodId.Value);

            if (searchModel.Status.HasValue)
            {
                var status = (PayrollStatus)searchModel.Status.Value;
                query = query.Where(x => x.Status == status);
            }

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new PayrollViewModel
                {
                    Id = x.Id,

                    Employee = x.Employees.EmployeeCode,
                    Branch = x.Branches.Title,
                    FinancialPeriod = x.FinancialPeriods.Title,

                    Status = x.Status.ToString(),

                    TotalBenefits = x.TotalBenefits,
                    TotalDeduction = x.TotalDeduction,
                    NetPay = x.NetPay,

                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }
    }
}
