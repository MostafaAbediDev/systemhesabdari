using _0_FrameWork.Domain;
using FinancialManagement.Application.Contracts.ReceiptsPayment;

namespace FinancialManagement.Domain.ReceiptsPaymentAgg
{
    public interface IReceiptsPaymentRepository : IRepository<long, ReceiptsPayments>
    {
        EditReceiptsPayment GetDetails(long id);
        List<ReceiptsPaymentViewModel> Search(ReceiptsPaymentSearchModel searchModel);
        List<ReceiptsPaymentViewModel> GetReceiptsPayments();
    }
}
