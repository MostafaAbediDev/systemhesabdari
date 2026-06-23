using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.BankBranch;

namespace BankManagement.Domain.Bank.BankBrancheAgg
{
    public interface IBankBranchRepository : IRepository<long, BankBranches>
    {
        EditBankBranch GetDetails(long id);
        List<BankBranchViewModel> Search(BankBranchSearchModel searchModel);
        List<BankBranchViewModel> GetBankBranches();
    }
}
