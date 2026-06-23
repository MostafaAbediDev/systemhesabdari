using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.Cheque;

namespace BankManagement.Domain.Bank.ChequeAgg
{
    public interface IChequeRepository : IRepository<long, Cheques>
    {
        EditCheque GetDetails(long id);
        List<ChequeViewModel> Search(ChequeSearchModel searchModel);
        List<ChequeViewModel> GetCheques();
    }
}
