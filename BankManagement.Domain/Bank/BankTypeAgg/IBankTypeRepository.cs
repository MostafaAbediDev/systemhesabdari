using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.BankType;

namespace BankManagement.Domain.Bank.BankTypeAgg
{
    public interface IBankTypeRepository : IRepository<long, BankTypes>
    {
        List<BankTypeViewModel> GetBankTypes();
    }
}
