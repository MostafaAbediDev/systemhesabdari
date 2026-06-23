using _0_Framework.Application;
using BankManagement.Application.Contracts.Cheque;
using BankManagement.Domain.Bank.ChequeAgg;

namespace BankManagement.Application
{
    public class ChequeApplication : IChequeApplication
    {
        private readonly IChequeRepository _chequeRepository;

        public ChequeApplication(IChequeRepository chequeRepository)
        {
            _chequeRepository = chequeRepository;
        }

        public OperationResult ChangeStatus(ChangeChequeStatus command)
        {
            var operation = new OperationResult();

            var cheque = _chequeRepository.Get(command.Id);

            if (cheque == null)
                return operation.Failed("چک یافت نشد.");

            cheque.ChangeStatus((ChequeStatus)command.Status);

            _chequeRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Create(CreateCheque command)
        {
            var operation = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.ChequeNumber))
                return operation.Failed("شماره چک الزامی است.");

            if (command.Amount <= 0)
                return operation.Failed("مبلغ باید بیشتر از صفر باشد.");

            var cheque = new Cheques(
                (ChequeType)command.ChequeType,
                command.ChequeNumber.Trim(),
                command.Amount,
                command.DueDate,
                (ChequeReferenceType)command.ReferenceType,
                command.ReferenceId,
                command.ChequeBookId,
                command.BranchId
            );

            _chequeRepository.Create(cheque);
            _chequeRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditCheque command)
        {
            var operation = new OperationResult();

            var cheque = _chequeRepository.Get(command.Id);

            if (cheque == null)
                return operation.Failed("چک یافت نشد.");

            if (string.IsNullOrWhiteSpace(command.ChequeNumber))
                return operation.Failed("شماره چک الزامی است.");

            if (command.Amount <= 0)
                return operation.Failed("مبلغ باید بیشتر از صفر باشد.");

            cheque.Edit(
                (ChequeType)command.ChequeType,
                command.ChequeNumber.Trim(),
                command.Amount,
                command.DueDate,
                (ChequeReferenceType)command.ReferenceType,
                command.ReferenceId,
                command.ChequeBookId,
                command.BranchId
            );

            _chequeRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<ChequeViewModel> GetCheques()
        {
            return _chequeRepository.GetCheques();
        }

        public EditCheque GetDetails(long id)
        {
            return _chequeRepository.GetDetails(id);
        }

        public List<ChequeViewModel> Search(ChequeSearchModel searchModel)
        {
            return _chequeRepository.Search(searchModel);
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var person = _chequeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Remove();
            _chequeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var person = _chequeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Restore();
            _chequeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var person = _chequeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Active();
            _chequeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var person = _chequeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.NotActive();
            _chequeRepository.SaveChanges();
            return operation.Succedded();
        }
    }
}
