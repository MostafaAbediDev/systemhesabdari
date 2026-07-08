using _0_FrameWork.Domain;
using FinancialManagement.Application.Contracts.PettyCash;

namespace FinancialManagement.Domain.PettyCashAgg
{
    public interface IPettyCashRepository : IRepository<long, PettyCashes>
    {
        EditPettyCash GetDetails(long id);
        List<PettyCashViewModel> Search(PettyCashSearchModel searchModel);
        List<PettyCashViewModel> GetPettyCashes();
    }
}
