using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;

namespace GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg
{
    public interface IFinancialPeriodRepository : IRepository<long, FinancialPeriods>
    {
        EditFinancialPeriod GetDetails(long id);
        List<FinancialPeriodViewModel> Search(FinancialPeriodSearchModel searchModel);
        List<FinancialPeriodViewModel> GetFinancialPeriods();
    }
}
