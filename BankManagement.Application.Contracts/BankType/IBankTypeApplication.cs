using _0_Framework.Application;

namespace BankManagement.Application.Contracts.BankType
{
    public interface IBankTypeApplication
    {
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        List<BankTypeViewModel> GetBankTypes();
    }
}
