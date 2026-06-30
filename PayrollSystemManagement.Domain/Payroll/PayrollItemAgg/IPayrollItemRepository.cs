using _0_FrameWork.Domain;
using PayrollSystemManagement.Application.Contracts.PayrollItem;

namespace PayrollSystemManagement.Domain.Payroll.PayrollItemAgg
{
    public interface IPayrollItemRepository : IRepository<long, PayrollItems>
    {
        EditPayrollItem GetDetails(long id);
        List<PayrollItemViewModel> Search(PayrollItemSearchModel searchModel);
        List<PayrollItemViewModel> GetPayrollDetails();
    }
}
