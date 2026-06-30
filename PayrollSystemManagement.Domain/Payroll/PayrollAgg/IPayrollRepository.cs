using _0_FrameWork.Domain;
using PayrollSystemManagement.Application.Contracts.Payroll;

namespace PayrollSystemManagement.Domain.Payroll.PayrollAgg
{
    public interface IPayrollRepository : IRepository<long, Payrolls>
    {
        EditPayroll GetDetails(long id);
        List<PayrollViewModel> Search(PayrollSearchModel searchModel);
        List<PayrollViewModel> GetAll();
        Payrolls GetWithDetails(long id);
    }
}
