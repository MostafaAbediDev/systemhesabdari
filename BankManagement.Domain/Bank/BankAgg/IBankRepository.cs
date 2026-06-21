using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.Bank;

namespace BankManagement.Domain.Bank.BankAgg
{
    public interface IBankRepository : IRepository<long, Banks>
    {
        EditBank GetDetails(long id);
        List<BankViewModel> Search(BankSearchModel searchModel);
        List<BankViewModel> GetBanks();
    }
}
