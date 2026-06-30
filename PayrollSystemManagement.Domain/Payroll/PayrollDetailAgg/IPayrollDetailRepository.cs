using _0_FrameWork.Domain;
using PayrollSystemManagement.Application.Contracts.PayrollDetail;

namespace PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg
{
    public interface IPayrollDetailRepository : IRepository<long, PayrollDetails>
    {
        EditPayrollDetails GetDetails(long id);
        List<PayrollDetailViewModel> Search(PayrollDetailsSearchModel searchModel);
        List<PayrollDetailViewModel> GetPayrollDetails();
    }
}
