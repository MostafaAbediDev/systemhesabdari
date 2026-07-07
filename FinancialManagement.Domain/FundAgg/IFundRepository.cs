using _0_FrameWork.Domain;

namespace FinancialManagement.Domain.FundAgg
{
    public interface IFundRepository : IRepository<long, Funds>
    {
        EditFunds GetDetails(long id);
        List<FundViewModel> Search(FundSearchModel searchModel);
        List<FundViewModel> GetFunds();
    }
}
