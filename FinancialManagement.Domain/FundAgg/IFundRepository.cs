using _0_FrameWork.Domain;
using FinancialManagement.Application.Contracts.Fund;

namespace FinancialManagement.Domain.FundAgg
{
    public interface IFundRepository : IRepository<long, Funds>
    {
        EditFund GetDetails(long id);
        List<FundViewModel> Search(FundSearchModel searchModel);
        List<FundViewModel> GetFunds();
    }
}
