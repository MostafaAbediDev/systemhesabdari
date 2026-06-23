using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.ChequeBook;

namespace BankManagement.Domain.Bank.ChequeBookAgg
{
    public interface IChequeBookRepository : IRepository<long, ChequeBooks>
    {
        EditChequeBook GetDetails(long id);
        List<ChequeBookViewModel> Search(ChequeBookSearchModel searchModel);
        List<ChequeBookViewModel> GetChequeBooks();
    }
}
