using _0_Framework.Application;
using BankManagement.Application.Contracts.BankType;
using BankManagement.Domain.Bank.BankTypeAgg;

namespace BankManagement.Application
{
    public class BankTypeApplication : IBankTypeApplication
    {
        private readonly IBankTypeRepository _bankTypeRepository;

        public BankTypeApplication(IBankTypeRepository bankTypeRepository)
        {
            _bankTypeRepository = bankTypeRepository;
        }

        public List<BankTypeViewModel> GetBankTypes()
        {
            return _bankTypeRepository.GetBankTypes();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var person = _bankTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Remove();
            _bankTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var person = _bankTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Restore();
            _bankTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var person = _bankTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Active();
            _bankTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var person = _bankTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.NotActive();
            _bankTypeRepository.SaveChanges();
            return operation.Succedded();
        }
    }
}
